using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using WineSales.Data;
using WineSales.Data.Repositories;
using WineSales.Domain.Models;
using WineSales.Domain.Interactors;
using WineSales.Domain.RepositoryInterfaces;

using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

using Serilog;
using Serilog.Core;

void ConfigureServices(IServiceCollection services)
{
    services.AddTransient<IBonusCardInteractor, BonusCardInteractor>();
    services.AddTransient<ICustomerInteractor, CustomerInteractor>();
    services.AddTransient<IPurchaseInteractor, PurchaseInteractor>();
    services.AddTransient<ISaleInteractor, SaleInteractor>();
    services.AddTransient<ISupplierInteractor, SupplierInteractor>();
    services.AddTransient<ISupplierWineInteractor, SupplierWineInteractor>();
    services.AddSingleton<IUserInteractor, UserInteractor>();
    services.AddTransient<IWineInteractor, WineInteractor>();

    services.AddTransient<IBonusCardRepository, BonusCardRepository>();
    services.AddTransient<ICustomerRepository, CustomerRepository>();
    services.AddTransient<IPurchaseRepository, PurchaseRepository>();
    services.AddTransient<ISaleRepository, SaleRepository>();
    services.AddTransient<ISupplierRepository, SupplierRepository>();
    services.AddTransient<ISupplierWineRepository, SupplierWineRepository>();
    services.AddSingleton<IUserRepository, UserRepository>();
    services.AddTransient<IWineRepository, WineRepository>();
}

Log.Logger = new LoggerConfiguration().
                 Enrich.FromLogContext().
                 WriteTo.File(@"log.txt").
                 CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrapProviders()
    .AddFontAwesomeIcons();

ConfigureServices(builder.Services);
builder.Configuration.AddJsonFile("dbsettings.json");
builder.Services.AddDbContext<DataBaseContext>(options => options.UseNpgsql(
      builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Singleton);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
