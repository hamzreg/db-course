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

void ConfigureServices(IServiceCollection services)
{
    services.AddScoped<IBonusCardInteractor, BonusCardInteractor>();
    services.AddScoped<ICustomerInteractor, CustomerInteractor>();
    services.AddScoped<IPurchaseInteractor, PurchaseInteractor>();
    services.AddScoped<ISaleInteractor, SaleInteractor>();
    services.AddScoped<ISupplierInteractor, SupplierInteractor>();
    services.AddScoped<ISupplierWineInteractor, SupplierWineInteractor>();
    services.AddSingleton<IUserInteractor, UserInteractor>();
    services.AddScoped<IWineInteractor, WineInteractor>();

    services.AddScoped<IBonusCardRepository, BonusCardRepository>();
    services.AddScoped<ICustomerRepository, CustomerRepository>();
    services.AddScoped<IPurchaseRepository, PurchaseRepository>();
    services.AddScoped<ISaleRepository, SaleRepository>();
    services.AddScoped<ISupplierRepository, SupplierRepository>();
    services.AddScoped<ISupplierWineRepository, SupplierWineRepository>();
    services.AddSingleton<IUserRepository, UserRepository>();
    services.AddScoped<IWineRepository, WineRepository>();

    // Bad way
/*    services.AddDbContext<DataBaseContext>(
        opts =>
        {
            opts.UseNpgsql(@"Server=localhost;Port=5432;Database=wine_sales;
                User ID=postgres;Password=postgres");
        });*/
}

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

// Test db
/*using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;

    var myRepository = services.GetRequiredService<IBonusCardRepository>();

    var card = myRepository.GetByID(1);
    Console.WriteLine(card.Phone);
}    */


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
