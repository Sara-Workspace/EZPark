using BLL;
using BLL.Services;
using DAL;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//מאפשר גישה לפרויקט כשצריך לפנות למסד הנתונים
// רישום ה-DbContext
builder.Services.AddDbContext<ParkingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
