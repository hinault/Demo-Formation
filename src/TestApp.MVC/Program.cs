using Microsoft.EntityFrameworkCore;
using TestApp.ApplicationCore.Interfaces;
using TestApp.ApplicationCore.Services;
using TestApp.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddScoped(typeof(IAsyncRepository<>), typeof(AsyncRepository<>));
builder.Services.AddScoped<IEtudiantsService, EtudiantsService>();

builder.Services.AddDbContext<TestAppContext>(options =>
  options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//Enregistrement de redis dans le conteneur d'IoC
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnectionString");
});

//Enregistrement des HealthChecks
builder.Services.AddHealthChecks()
       .AddDbContextCheck<TestAppContext>();


builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPINSIGHTS_CONNECTIONSTRING"]);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHealthChecks("/healthz");

app.Run();
