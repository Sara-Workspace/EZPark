using BLL;
using BLL.Services;
using DAL;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Parking.Extensions; // ודאי שנוצרה תיקיית Extensions ובה הקובץ ClaimsPrincipalExtensions.cs

var builder = WebApplication.CreateBuilder(args);

// 1. הגדרת נתיב התמונות
var imagesFolder = Path.GetFullPath(Path.Combine(
    builder.Environment.ContentRootPath,
    builder.Configuration["ImagesFolder"]!));
Directory.CreateDirectory(imagesFolder);

// 2. הוספת קונטרולרים ו-OpenAPI
builder.Services.AddControllers();
//builder.Services.AddOpenApi();

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

// 6. רישום ה-Services ל-DI
builder.Services.AddScoped<DriverService>();
builder.Services.AddScoped<OwnerService>();
builder.Services.AddScoped<ParkingSpotService>();
builder.Services.AddScoped<ReservationsService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<SpotAvailabilityService>();
builder.Services.AddSingleton<IImageService>(new ImageService(imagesFolder));

// 7. הגדרת אימות JWT
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

// 8. רישום שירותי אבטחה ב-DI
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher<Owners>, PasswordHasher<Owners>>();
builder.Services.AddScoped<IPasswordHasher<Driver>, PasswordHasher<Driver>>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
   // app.MapOpenApi();
    app.UseSwagger();   // מייצר את קובץ ה-JSON של Swagger
    app.UseSwaggerUI(); // מפעיל את הממשק הויזואלי בנתיב /swagger
}

app.UseHttpsRedirection();

// 9. הגשת תמונות סטטיות
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesFolder),
    RequestPath = "/images"
});

// 10. הפעלת מנגנון האימות וההרשאות בסדר הנכון (קודם Authentication ואחר כך Authorization)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();