//זה החלק שהעולם החיצון מדבר איתו

using BLL;
using BLL.Services;
using BLL.Interfaces;
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
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IOwnersRepository, OwnersRepository>();
builder.Services.AddScoped<IParkingSpotRepository, ParkingSpotRepository>();
builder.Services.AddScoped<IReservationsRepository, ReservationsRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IReviewsRepository, ReviewsRepository>();
builder.Services.AddScoped<ISpotAvailabilityRepository, SpotAvailabilityRepository>();
builder.Services.AddScoped<IDynamicPricingRulesRepository, DynamicPricingRulesRepository>();

// רישום ה-Services ל-DI מול הממשקים (Interfaces)
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IOwnerService, OwnerService>();
builder.Services.AddScoped<IParkingSpotService, ParkingSpotService>();
builder.Services.AddScoped<IReservationsService, ReservationsService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ISpotAvailabilityService, SpotAvailabilityService>();
builder.Services.AddScoped<IDynamicPricingService, DynamicPricingService>();

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
