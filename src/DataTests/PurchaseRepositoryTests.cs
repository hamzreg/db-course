using Xunit;
using Microsoft.EntityFrameworkCore;

using WineSales.Data;
using WineSales.Domain.Models;
using WineSales.Data.Repositories;
using WineSales.Config;

namespace DataTests
{
    public class PurchaseRepositoryTests
    {
        private readonly DbContextOptions<DataBaseContext> _contextOptions;

        public PurchaseRepositoryTests()
        {
            _contextOptions = new DbContextOptionsBuilder<DataBaseContext>()
                .UseInMemoryDatabase("PurchaseRepositoryTests")
                .Options;

            using var context = new DataBaseContext(_contextOptions);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Purchases.AddRange(
                new Purchase 
                { 
                    Price = 500, 
                    Status = (int)PurchaseConfig.Statuses.Active, 
                    CustomerID = 2 
                },
                new Purchase 
                {
                    Price = 700,
                    Status = (int)PurchaseConfig.Statuses.Canceled,
                    CustomerID = 2
                });

            context.Sales.AddRange(
                new Sale
                {
                    PurchaseID = 2,
                    SupplierWineID = 1,
                    Date = new DateOnly(2022,9,9)
                },
                new Sale
                {
                    PurchaseID = 1,
                    SupplierWineID = 2,
                    Date = new DateOnly(2022,8,15)
                });

            context.SupplierWines.AddRange(new SupplierWine { WineID = 1 },
                                           new SupplierWine { WineID = 1});

            context.Wines.Add(
               new Wine
               {
                   Kind = "lambrusco",
                   Color = "white",
                   Sugar = "semi-sweet",
                   Volume = 0.75,
                   Alcohol = 7.5,
                   Aging = 2,
                   Number = 2
               });

            context.SaveChanges();
        }

        private DataBaseContext CreateContext() => new DataBaseContext(_contextOptions);

        [Fact]
        public void CreateTest()
        {
            using var context = CreateContext();
            var repository = new PurchaseRepository(context);

            var purchase = new Purchase
            { 
              Price = 1000,
              Status = (int)PurchaseConfig.Statuses.Active,
              CustomerID = 1
            };

            repository.Create(purchase);

            Assert.Equal(3, context.Purchases.Count());

            var createdPurchase = context.Purchases
                .Single(prchs => prchs.ID == 3);

            Assert.NotNull(createdPurchase);
            Assert.Equal(3, createdPurchase.ID);
            Assert.Equal(purchase.Price, createdPurchase.Price);
            Assert.Equal(purchase.Status, createdPurchase.Status);
            Assert.Equal(purchase.CustomerID, createdPurchase.CustomerID);
        }

        [Fact]
        public void GetAllTest()
        {
            using var context = CreateContext();
            var repository = new PurchaseRepository(context);

            var purchases = repository.GetAll();

            Assert.Equal(2, purchases.Count);

            Assert.Collection(
                purchases,
                purchase =>
                {
                    Assert.Equal(1, purchase.ID);
                    Assert.Equal(500, purchase.Price);
                    Assert.Equal((int)PurchaseConfig.Statuses.Active, purchase.Status);
                    Assert.Equal(2, purchase.CustomerID);
                },
                purchase =>
                {
                    Assert.Equal(2, purchase.ID);
                    Assert.Equal(700, purchase.Price);
                    Assert.Equal((int)PurchaseConfig.Statuses.Canceled, purchase.Status);
                    Assert.Equal(2, purchase.CustomerID);
                });
        }

        [Fact]
        public void GetByIDTest()
        {
            using var context = CreateContext();
            var repository = new PurchaseRepository(context);

            var purchase = repository.GetByID(2);

            Assert.NotNull(purchase);
            Assert.Equal(700, purchase.Price);
            Assert.Equal((int)PurchaseConfig.Statuses.Canceled, purchase.Status);
            Assert.Equal(2, purchase.CustomerID);
        }

        [Fact]
        public void GetByPriceTest()
        {
            using var context = CreateContext();
            var repository = new PurchaseRepository(context);

            var purchases = repository.GetByPrice(500);

            Assert.Equal(1, purchases.Count);

            Assert.Collection(
                purchases,
                purchase =>
                {
                    Assert.Equal(1, purchase.ID);
                    Assert.Equal(500, purchase.Price);
                    Assert.Equal((int)PurchaseConfig.Statuses.Active, purchase.Status);
                    Assert.Equal(2, purchase.CustomerID);
                });
        }

        [Fact]
        public void GetByStatusTest()
        {
            using var context = CreateContext();
            var repository = new PurchaseRepository(context);

            var purchases = repository.GetByStatus((int)PurchaseConfig.Statuses.Active);

            Assert.Equal(1, purchases.Count);

            Assert.Collection(
                purchases,
                purchase =>
                {
                    Assert.Equal(1, purchase.ID);
                    Assert.Equal(500, purchase.Price);
                    Assert.Equal((int)PurchaseConfig.Statuses.Active, purchase.Status);
                    Assert.Equal(2, purchase.CustomerID);
                });
        }

        [Fact]
        public void GetByCustomerIDTest()
        {
            using var context = CreateContext();
            var repository = new PurchaseRepository(context);

            var (ids, dates, wines, prices) = repository.GetByCustomerID(2);

            Assert.Collection(
                wines,
                wine =>
                {
                    Assert.Equal(1, wine.ID);
                    Assert.Equal("lambrusco", wine.Kind);
                    Assert.Equal("white", wine.Color);
                    Assert.Equal("semi-sweet", wine.Sugar);
                    Assert.Equal(0.75, wine.Volume);
                    Assert.Equal(7.5, wine.Alcohol);
                    Assert.Equal(2, wine.Aging);
                    Assert.Equal(2, wine.Number);
                },
                wine =>
                {
                    Assert.Equal(1, wine.ID);
                    Assert.Equal("lambrusco", wine.Kind);
                    Assert.Equal("white", wine.Color);
                    Assert.Equal("semi-sweet", wine.Sugar);
                    Assert.Equal(0.75, wine.Volume);
                    Assert.Equal(7.5, wine.Alcohol);
                    Assert.Equal(2, wine.Aging);
                    Assert.Equal(2, wine.Number);
                });

            Assert.Collection(
                prices,
                price => Assert.Equal(500, price),
                price => Assert.Equal(700, price));
        }

        [Fact]
        public void UpdateTest()
        {
            using var context = CreateContext();
            var repository = new PurchaseRepository(context);

            var purchase = new Purchase
            {
                ID = 1,
                Price = 500,
                Status = (int)PurchaseConfig.Statuses.Canceled,
                CustomerID = 2
            };

            repository.Update(purchase);

            Assert.Equal(2, context.Purchases.Count());

            var updatedPurchase = context.Purchases
                .Single(prchs => prchs.ID == purchase.ID);

            Assert.NotNull(updatedPurchase);
            Assert.Equal(purchase.ID, updatedPurchase.ID);
            Assert.Equal(purchase.Price, updatedPurchase.Price);
            Assert.Equal(purchase.Status, updatedPurchase.Status);
            Assert.Equal(purchase.CustomerID, updatedPurchase.CustomerID);
        }

        [Fact]
        public void DeleteTest()
        {
            using var context = CreateContext();
            var repository = new PurchaseRepository(context);

            var purchase = new Purchase
            {
                ID = 1,
                Price = 500,
                Status = (int)PurchaseConfig.Statuses.Active,
                CustomerID = 2
            };

            repository.Delete(purchase);

            Assert.Equal(1, context.Purchases.Count());
        }
    }
}
