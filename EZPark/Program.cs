//זה החלק שהעולם החיצון מדבר איתו

using BLL;
using BLL.Services;
using DAL;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;//בשביל הכלי שפותח את הארגז לדפדפן

var builder = WebApplication.CreateBuilder(args);

// הנתיב המלא לתיקיית התמונות
var imagesFolder = Path.GetFullPath(Path.Combine(
    builder.Environment.ContentRootPath,
    builder.Configuration["ImagesFolder"]!));
Directory.CreateDirectory(imagesFolder); // יוצר את התיקייה אם אינה קיימת

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// קריאת מחרוזת החיבור למסד הנתונים
// הערך לא כתוב בקוד, הוא נשמר במשתנה סביבה בשם ConnectionStrings__DefaultConnection
// ASP.NET Core מתרגם את הקו התחתון הכפול __ לנקודתיים, ולכן הוא נקרא כאן בשם DefaultConnection
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");

// בדיקה: אם משתנה הסביבה לא הוגדר במחשב (או שהערך ריק)
// האפליקציה נעצרת מיד עם הודעה ברורה, במקום להיכשל אחר כך בשגיאה לא מובנת מול מסד הנתונים
if (string.IsNullOrWhiteSpace(connStr))
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' is missing. Set it as an environment variable.");

// רישום ה-DbContext עם מחרוזת החיבור שקראנו ממשתנה הסביבה
builder.Services.AddDbContext<ParkingDbContext>(options =>
    options.UseSqlServer(connStr));


// רישום AutoMapper - טוען את ההוראות מ-MappingProfile
builder.Services.AddAutoMapper(typeof(MappingProfile));

// רישום ה-Repositories ל-DI (הזרקת תלויות)
// רישום ה-Repositories ל-DI (הזרקת תלויות)
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IOwnersRepository, OwnersRepository>();
builder.Services.AddScoped<IParkingSpotRepository, ParkingSpotRepository>();
builder.Services.AddScoped<IReservationsRepository, ReservationsRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IReviewsRepository, ReviewsRepository>();
builder.Services.AddScoped<ISpotAvailabilityRepository, SpotAvailabilityRepository>();
builder.Services.AddScoped<IDynamicPricingRulesRepository, DynamicPricingRulesRepository>();

// רישום ה-Services ל-DI
builder.Services.AddScoped<DriverService>();
builder.Services.AddScoped<OwnerService>();
builder.Services.AddScoped<ParkingSpotService>();
builder.Services.AddScoped<ReservationsService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<SpotAvailabilityService>();
builder.Services.AddSingleton<IImageService>(new ImageService(imagesFolder));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// פותח את תיקיית התמונות לדפדפן בכתובת /images
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesFolder),
    RequestPath = "/images"
});

app.UseAuthorization();

app.MapControllers();

app.Run();
