using BLL;
using BLL.Services;
using BLL.Interfaces;
using BLL.Geocoding;
using DAL;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Parking.Filters;
using BLL.Service;
using Microsoft.Extensions.AI;
using BLL.Agent;
using BLL.PlateRecognition;

var builder = WebApplication.CreateBuilder(args);

// 1. הגדרת נתיב התמונות
var imagesFolder = Path.GetFullPath(Path.Combine(
    builder.Environment.ContentRootPath,
    builder.Configuration["ImagesFolder"]!));
Directory.CreateDirectory(imagesFolder);

// 2. הוספת קונטרולרים + רישום השומר המרכזי לשגיאות
// כל שגיאה שלא נתפסה באחד הקונטרולרים תגיע ל-GlobalExceptionFilter,
// והוא יחזיר תשובה מסודרת (404 / 409 / 400 / 500) במקום קריסה
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

// 3. קריאת מחרוזת חיבור למסד הנתונים
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connStr))
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' is missing. Set it as an environment variable.");

builder.Services.AddDbContext<ParkingDbContext>(options =>
    options.UseSqlServer(connStr));

// 4. רישום AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// 5. רישום ה-Repositories ל-DI
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IOwnersRepository, OwnersRepository>();
builder.Services.AddScoped<IParkingSpotRepository, ParkingSpotRepository>();
builder.Services.AddScoped<IReservationsRepository, ReservationsRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IReviewsRepository, ReviewsRepository>();
builder.Services.AddScoped<ISpotAvailabilityRepository, SpotAvailabilityRepository>();
builder.Services.AddScoped<IDynamicPricingRulesRepository, DynamicPricingRulesRepository>();

// 6. רישום ה-Services ל-DI דרך הממשקים (Interfaces)
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IOwnerService, OwnerService>();
builder.Services.AddScoped<IParkingSpotService, ParkingSpotService>();
builder.Services.AddScoped<IReservationsService, ReservationsService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ISpotAvailabilityService, SpotAvailabilityService>();
builder.Services.AddScoped<IDynamicPricingService, DynamicPricingService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddSingleton<IImageService>(new ImageService(imagesFolder));

builder.Services.AddScoped<DynamicPricingService>();
builder.Services.AddScoped<SpotSearchService>();
builder.Services.AddSingleton<SpotHoldRegistry>();
builder.Services.AddScoped<SpotHoldService>();

// 6א. API חיצוני: המרת כתובת למיקום (Geocoding) של OpenStreetMap
//     AddHttpClient רושם את השירות ב-DI ומזריק לו HttpClient מוכן (לא יוצרים new HttpClient בעצמנו)
builder.Services.AddHttpClient<IGeocodingService, GeocodingService>(client =>
{
    client.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");   // הכתובת הראשית של ה-API
    client.Timeout = TimeSpan.FromSeconds(10);                              // לא מחכים יותר מ-10 שניות
    // חובה לפי הכללים של OpenStreetMap: להציג מי אנחנו. בלי זה הבקשה נחסמת
    client.DefaultRequestHeaders.UserAgent.ParseAdd("EZPark-StudentProject/1.0 (+https://github.com/Sara-Workspace/EZPark)");
});

// 6ב. AI Agent: חיבור ל-Gemini של גוגל
//     המפתח נקרא ממשתנה הסביבה Gemini__ApiKey (שני קווים תחתונים הופכים לנקודתיים)
var geminiApiKey = builder.Configuration["Gemini:ApiKey"];
if (string.IsNullOrWhiteSpace(geminiApiKey))
    throw new InvalidOperationException("Gemini:ApiKey is missing. Set the environment variable Gemini__ApiKey.");

//     משתמשים בחבילה הרשמית של גוגל (Google.GenAI), ולא בחבילה של OpenAI כמו במדריך:
//     מודלי Gemini 3 ומעלה דורשים "חתימת מחשבה" (thought signature) בכל קריאה לכלי,
//     והחבילה של OpenAI זורקת אותה בדרך. החבילה של גוגל שומרת עליה.
//     שאר הקוד לא משתנה, כי הוא מכיר רק את IChatClient.
//     המודל: Flash-Lite = הכי קל וזול, כדי שהמכסה החינמית לא תיגמר מהר
IChatClient geminiChatClient = new Google.GenAI.Client(apiKey: geminiApiKey)
    .AsIChatClient("gemini-3.5-flash-lite");

//     רישום ב-DI. UseFunctionInvocation = הרכיב שמריץ את הכלים שהמודל מבקש (ה"מזכירה")
builder.Services.AddChatClient(geminiChatClient)
    .UseFunctionInvocation();

//     ה"מוח" של הצ'אט והכלים שלו (BLL/Agent)
builder.Services.AddScoped<ParkingTools>();
builder.Services.AddScoped<IAgentService, AgentService>();

// 6ג. זיהוי לוחיות רישוי מתמונה (BLL/PlateRecognition)
//     משתמש באותו Gemini שרשמנו למעלה (IChatClient), רק בלי כלים: שולחים תמונה ומקבלים מספר
builder.Services.AddScoped<IPlateRecognitionService, PlateRecognitionService>();

// 7. רישום שירותי אבטחה ב-DI
builder.Services.AddScoped<IPasswordHasher<Owners>, PasswordHasher<Owners>>();
builder.Services.AddScoped<IPasswordHasher<Driver>, PasswordHasher<Driver>>();

// 8. הגדרת אימות JWT
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("Jwt:Key is missing. Set the environment variable Jwt__Key.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// 9. הגדרת Swagger עם כפתור Authorize ל-JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "EZPark API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "הזן את הטוקן בפורמט: Bearer {your_token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 10. CORS בשביל React
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // הכתובת של Vite
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// חייב להופיע לפני UseAuthentication ו-UseAuthorization
app.UseCors("AllowReactApp");

app.UseHttpsRedirection();

// 11. הגשת תמונות סטטיות
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesFolder),
    RequestPath = "/images"
});

// 12. הפעלת אימות והרשאות
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();