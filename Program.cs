using Microsoft.EntityFrameworkCore;
using System;
using Project_AG.Data;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? 
    builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllersWithViews();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var esCulture = new CultureInfo("es-CO");

    esCulture.NumberFormat.CurrencySymbol = "$";
    esCulture.NumberFormat.NumberDecimalSeparator = ",";
    esCulture.NumberFormat.NumberGroupSeparator = "";
    esCulture.NumberFormat.CurrencyGroupSizes = new int[] { 3 };
    esCulture.NumberFormat.CurrencyDecimalDigits = 0;

    var supportedCultures = new[] { esCulture };

    options.DefaultRequestCulture = new RequestCulture(culture: esCulture, uiCulture: esCulture);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});


builder.Services.AddSession();
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseRequestLocalization();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();