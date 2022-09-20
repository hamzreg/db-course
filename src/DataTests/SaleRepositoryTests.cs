using Xunit;
using Microsoft.EntityFrameworkCore;

using WineSales.Data;
using WineSales.Domain.Models;
using WineSales.Data.Repositories;

namespace DataTests
{
    public class SaleRepositoryTests
    {
        private readonly DbContextOptions<DataBaseContext> _contextOptions;

        public SaleRepositoryTests()
        {
            _contextOptions = new DbContextOptionsBuilder<DataBaseContext>()
                .UseInMemoryDatabase("SaleRepositoryTests")
                .Options;

            using var context = new DataBaseContext(_contextOptions);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Sales.AddRange(
                new Sale
                {
                    PurchasePrice = 500,
                    SellingPrice = 750,
                    Margin = 250,
                    Costs = 100,
                    Profit = 150,
                    WineNumber = 1,
                    Date = new DateOnly(2001, 9, 9),
                    PurchaseID = 1,
                    SupplierWineID = 2
                },
                new Sale
                {
                    PurchasePrice = 700,
                    SellingPrice = 1050,
                    Margin = 350,
                    Costs = 200,
                    Profit = 150,
                    WineNumber = 1,
                    Date = new DateOnly(2012, 3, 24),
                    PurchaseID = 1,
                    SupplierWineID = 1
                });

            context.SupplierWines.AddRange(
                new SupplierWine
                {
                    SupplierID = 1,
                    WineID = 1,
                    Price = 700
                },
                new SupplierWine
                {
                    SupplierID = 1,
                    WineID = 2,
                    Price = 500
                });

            context.Suppliers.Add(new Supplier { ID = 1, Name = "Fanagoria" });

            context.Wines.AddRange(
                new Wine
                {
                    ID = 1,
                    Kind = "lambrusco",
                    Color = "red",
                    Sugar = "dry",
                    Volume = 0.75,
                    Alcohol = 7.5,
                    Aging = 2,
                    Number = 1
                },
                new Wine
                {
                    ID = 2,
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
            var repository = new SaleRepository(context);

            var sale = new Sale
            {
                PurchasePrice = 600,
                SellingPrice = 900,
                Margin = 300,
                Costs = 150,
                Profit = 150,
                WineNumber = 1,
                Date = new DateOnly(2001, 8, 15),
                PurchaseID = 3,
                SupplierWineID = 2
            };

            repository.Create(sale);

            Assert.Equal(3, context.Sales.Count());

            var createdSale = context.Sales.Find(3);

            Assert.NotNull(createdSale);
            Assert.Equal(sale.PurchasePrice, createdSale.PurchasePrice);
            Assert.Equal(sale.SellingPrice, createdSale.SellingPrice);
            Assert.Equal(sale.Margin, createdSale.Margin);
            Assert.Equal(sale.Costs, createdSale.Costs);
            Assert.Equal(sale.Profit, createdSale.Profit);
            Assert.Equal(sale.WineNumber, createdSale.WineNumber);
            Assert.Equal(sale.Date, createdSale.Date);
            Assert.Equal(sale.PurchaseID, createdSale.PurchaseID);
            Assert.Equal(sale.SupplierWineID, createdSale.SupplierWineID);
        }

        [Fact]
        public void GetAllTest()
        {
            using var context = CreateContext();
            var repository = new SaleRepository(context);

            var sales = repository.GetAll();

            Assert.Equal(2, sales.Count);

            Assert.Collection(
                sales,
                sale =>
                {
                    Assert.Equal(1, sale.ID);
                    Assert.Equal(500, sale.PurchasePrice);
                    Assert.Equal(750, sale.SellingPrice);
                    Assert.Equal(250, sale.Margin);
                    Assert.Equal(100, sale.Costs);
                    Assert.Equal(150, sale.Profit);
                    Assert.Equal(1, sale.WineNumber);
                    Assert.Equal(new DateOnly(2001, 9, 9), sale.Date);
                    Assert.Equal(1, sale.PurchaseID);
                    Assert.Equal(2, sale.SupplierWineID);
                },
                sale =>
                {
                    Assert.Equal(2, sale.ID);
                    Assert.Equal(700, sale.PurchasePrice);
                    Assert.Equal(1050, sale.SellingPrice);
                    Assert.Equal(350, sale.Margin);
                    Assert.Equal(200, sale.Costs);
                    Assert.Equal(150, sale.Profit);
                    Assert.Equal(1, sale.WineNumber);
                    Assert.Equal(new DateOnly(2012, 3, 24), sale.Date);
                    Assert.Equal(1, sale.PurchaseID);
                    Assert.Equal(1, sale.SupplierWineID);
                });
        }

        [Fact]
        public void GetByIDTest()
        {
            using var context = CreateContext();
            var repository = new SaleRepository(context);

            var sale = repository.GetByID(2);

            Assert.NotNull(sale);
            Assert.Equal(700, sale.PurchasePrice);
            Assert.Equal(1050, sale.SellingPrice);
            Assert.Equal(350, sale.Margin);
            Assert.Equal(200, sale.Costs);
            Assert.Equal(150, sale.Profit);
            Assert.Equal(1, sale.WineNumber);
            Assert.Equal(new DateOnly(2012, 3, 24), sale.Date);
            Assert.Equal(1, sale.PurchaseID);
            Assert.Equal(1, sale.SupplierWineID);
        }

        [Fact]
        public void GetByPurchasePriceTest()
        {
            using var context = CreateContext();
            var repository = new SaleRepository(context);

            var sales = repository.GetByPurchasePrice(500);

            Assert.Equal(1, sales.Count);

            Assert.Collection(
                sales,
                sale =>
                {
                    Assert.Equal(1, sale.ID);
                    Assert.Equal(500, sale.PurchasePrice);
                    Assert.Equal(750, sale.SellingPrice);
                    Assert.Equal(250, sale.Margin);
                    Assert.Equal(100, sale.Costs);
                    Assert.Equal(150, sale.Profit);
                    Assert.Equal(1, sale.WineNumber);
                    Assert.Equal(new DateOnly(2001, 9, 9), sale.Date);
                    Assert.Equal(1, sale.PurchaseID);
                    Assert.Equal(2, sale.SupplierWineID);
                });
        }

        [Fact]
        public void GetBySellingPriceTest()
        {
            using var context = CreateContext();
            var repository = new SaleRepository(context);

            var sales = repository.GetBySellingPrice(1050);

            Assert.Equal(1, sales.Count);

            Assert.Collection(
                sales,
                sale =>
                {
                    Assert.Equal(2, sale.ID);
                    Assert.Equal(700, sale.PurchasePrice);
                    Assert.Equal(1050, sale.SellingPrice);
                    Assert.Equal(350, sale.Margin);
                    Assert.Equal(200, sale.Costs);
                    Assert.Equal(150, sale.Profit);
                    Assert.Equal(1, sale.WineNumber);
                    Assert.Equal(new DateOnly(2012, 3, 24), sale.Date);
                    Assert.Equal(1, sale.PurchaseID);
                    Assert.Equal(1, sale.SupplierWineID);
                });
        }

        [Fact]
        public void GetByMarginTest()
        {
            using var context = CreateContext();
            var repository = new SaleRepository(context);

            var sales = repository.GetByMargin(250);

            Assert.Equal(1, sales.Count);

            Assert.Collection(
                sales,
                sale =>
                {
                    Assert.Equal(1, sale.ID);
                    Assert.Equal(500, sale.PurchasePrice);
                    Assert.Equal(750, sale.SellingPrice);
                    Assert.Equal(250, sale.Margin);
                    Assert.Equal(100, sale.Costs);
                    Assert.Equal(150, sale.Profit);
                    Assert.Equal(1, sale.WineNumber);
                    Assert.Equal(new DateOnly(2001, 9, 9), sale.Date);
                    Assert.Equal(1, sale.PurchaseID);
                    Assert.Equal(2, sale.SupplierWineID);
                });
        }

        [Fact]
        public void GetByCostsTest()
        {
            using var context = CreateContext();
            var repository = new SaleRepository(context);

            var sales = repository.GetByCosts(200);

            Assert.Equal(1, sales.Count);

            Assert.Collection(
                sales,
                sale =>
                {
                    Assert.Equal(2, sale.ID);
                    Assert.Equal(700, sale.PurchasePrice);
                    Assert.Equal(1050, sale.SellingPrice);
                    Assert.Equal(350, sale.Margin);
                    Assert.Equal(200, sale.Costs);
                    Assert.Equal(150, sale.Profit);
                    Assert.Equal(1, sale.WineNumber);
                    Assert.Equal(new DateOnly(2012, 3, 24), sale.Date);
                    Assert.Equal(1, sale.PurchaseID);
                    Assert.Equal(1, sale.SupplierWineID);
                });
        }

        [Fact]
        public void GetByProfitTest()
        {
            using var context = CreateContext();
            var repository = new SaleRepository(context);

            var sales = repository.GetByProfit(150);

            Assert.Equal(2, sales.Count);

            Assert.Collection(
                sales,
                sale =>
                {
                    Assert.Equal(1, sale.ID);
                    Assert.Equal(500, sale.PurchasePrice);
                    Assert.Equal(750, sale.SellingPrice);
                    Assert.Equal(250, sale.Margin);
                    Assert.Equal(100, sale.Costs);
                    Assert.Equal(150, sale.Profit);
                    Assert.Equal(1, sale.WineNumber);
                    Assert.Equal(new DateOnly(2001, 9, 9), sale.Date);
                    Assert.Equal(1, sale.PurchaseID);
                    Assert.Equal(2, sale.SupplierWineID);
                },
                sale =>
                {
                    Assert.Equal(2, sale.ID);
                    Assert.Equal(700, sale.PurchasePrice);
                    Assert.Equal(1050, sale.SellingPrice);
                    Assert.Equal(350, sale.Margin);
                    Assert.Equal(200, sale.Costs);
                    Assert.Equal(150, sale.Profit);
                    Assert.Equal(1, sale.WineNumber);
                    Assert.Equal(new DateOnly(2012, 3, 24), sale.Date);
                    Assert.Equal(1, sale.PurchaseID);
                    Assert.Equal(1, sale.SupplierWineID);
                });
        }

        [Fact]
        public void GetByWineNumberTest()
        {
            using var context = CreateContext();
            var repository = new SaleRepository(context);

            var sales = repository.GetByWineNumber(1);

            Assert.Equal(2, sales.Count);

            Assert.Collection(
                sales,
                sale =>
                {
                    Assert.Equal(1, sale.ID);
                    Assert.Equal(500, sale.PurchasePrice);
                    Assert.Equal(750, sale.SellingPrice);
                    Assert.Equal(250, sale.Margin);
                    Assert.Equal(100, sale.Costs);
                    Assert.Equal(150, sale.Profit);
                    Assert.Equal(1, sale.WineNumber);
                    Assert.Equal(new DateOnly(2001, 9, 9), sale.Date);
                    Assert.Equal(1, sale.PurchaseID);
                    Assert.Equal(2, sale.SupplierWineID);
                },
                sale =>
                {
                    Assert.Equal(2, sale.ID);
                    Assert.Equal(700, sale.PurchasePrice);
                    Assert.Equal(1050, sale.SellingPrice);
                    Assert.Equal(350, sale.Margin);
                    Assert.Equal(200, sale.Costs);
                    Assert.Equal(150, sale.Profit);
                    Assert.Equal(1, sale.WineNumber);
                    Assert.Equal(new DateOnly(2012, 3, 24), sale.Date);
                    Assert.Equal(1, sale.PurchaseID);
                    Assert.Equal(1, sale.SupplierWineID);
                });
        }

        [Fact]
        public void GetByDateTest()
        {
            using var context = CreateContext();
            var repository = new SaleRepository(context);

            var sales = repository.GetByDate(new DateOnly(2001, 9, 9));

            Assert.Equal(1, sales.Count);

            Assert.Collection(
                sales,
                sale =>
                {
                    Assert.Equal(1, sale.ID);
                    Assert.Equal(500, sale.PurchasePrice);
                    Assert.Equal(750, sale.SellingPrice);
                    Assert.Equal(250, sale.Margin);
                    Assert.Equal(100, sale.Costs);
                    Assert.Equal(150, sale.Profit);
                    Assert.Equal(1, sale.WineNumber);
                    Assert.Equal(new DateOnly(2001, 9, 9), sale.Date);
                    Assert.Equal(1, sale.PurchaseID);
                    Assert.Equal(2, sale.SupplierWineID);
                });
        }

        [Fact]
        public void GetBySupplierIDTest()
        {
            using var context = CreateContext();
            var repository = new SaleRepository(context);

            var (wines, dates, prices) = repository.GetBySupplierID(1);

            Assert.Equal(2, wines.Count);
            Assert.Equal(2, prices.Count);

            Assert.Collection(
                wines,
                wine =>
                {
                    Assert.Equal(1, wine.ID);
                    Assert.Equal("lambrusco", wine.Kind);
                    Assert.Equal("red", wine.Color);
                    Assert.Equal("dry", wine.Sugar);
                    Assert.Equal(0.75, wine.Volume);
                    Assert.Equal(7.5, wine.Alcohol);
                    Assert.Equal(2, wine.Aging);
                    Assert.Equal(1, wine.Number);
                },
                wine =>
                {
                    Assert.Equal(2, wine.ID);
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
                price => Assert.Equal(700, price),
                price => Assert.Equal(500, price));
        }

        [Fact]
        public void GetByAdminTest()
        {
            using var context = CreateContext();
            var repository = new SaleRepository(context);

            var (wines, suppliers, sales) = repository.GetByAdmin();

            Assert.Equal(2, wines.Count);
            Assert.Equal(2, suppliers.Count);
            Assert.Equal(2, sales.Count);

            Assert.Collection(
                wines,
                wine =>
                {
                    Assert.Equal(2, wine.ID);
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
                    Assert.Equal("red", wine.Color);
                    Assert.Equal("dry", wine.Sugar);
                    Assert.Equal(0.75, wine.Volume);
                    Assert.Equal(7.5, wine.Alcohol);
                    Assert.Equal(2, wine.Aging);
                    Assert.Equal(1, wine.Number);
                });

            Assert.Collection(
                suppliers,
                supplier => Assert.Equal("Fanagoria", supplier),
                supplier => Assert.Equal("Fanagoria", supplier));

            Assert.Collection(
                sales,
                sale =>
                {
                    Assert.Equal(1, sale.ID);
                    Assert.Equal(500, sale.PurchasePrice);
                    Assert.Equal(750, sale.SellingPrice);
                    Assert.Equal(250, sale.Margin);
                    Assert.Equal(100, sale.Costs);
                    Assert.Equal(150, sale.Profit);
                    Assert.Equal(1, sale.WineNumber);
                    Assert.Equal(new DateOnly(2001, 9, 9), sale.Date);
                    Assert.Equal(1, sale.PurchaseID);
                    Assert.Equal(2, sale.SupplierWineID);
                },
                sale =>
                {
                    Assert.Equal(2, sale.ID);
                    Assert.Equal(700, sale.PurchasePrice);
                    Assert.Equal(1050, sale.SellingPrice);
                    Assert.Equal(350, sale.Margin);
                    Assert.Equal(200, sale.Costs);
                    Assert.Equal(150, sale.Profit);
                    Assert.Equal(1, sale.WineNumber);
                    Assert.Equal(new DateOnly(2012, 3, 24), sale.Date);
                    Assert.Equal(1, sale.PurchaseID);
                    Assert.Equal(1, sale.SupplierWineID);
                });
        }

        [Fact]
        public void UpdateTest()
        {
            using var context = CreateContext();
            var repository = new SaleRepository(context);

            var sale = new Sale
            {
                ID = 1,
                PurchasePrice = 600,
                SellingPrice = 900,
                Margin = 300,
                Costs = 150,
                Profit = 150,
                WineNumber = 1,
                Date = new DateOnly(2001, 9, 9),
                PurchaseID = 1,
                SupplierWineID = 2
            };

            repository.Update(sale);

            Assert.Equal(2, context.Sales.Count());

            var updatedSale = context.Sales.Find(sale.ID);

            Assert.NotNull(updatedSale);
            Assert.Equal(sale.PurchasePrice, updatedSale.PurchasePrice);
            Assert.Equal(sale.SellingPrice, updatedSale.SellingPrice);
            Assert.Equal(sale.Margin, updatedSale.Margin);
            Assert.Equal(sale.Costs, updatedSale.Costs);
            Assert.Equal(sale.Profit, updatedSale.Profit);
            Assert.Equal(sale.WineNumber, updatedSale.WineNumber);
            Assert.Equal(sale.Date, updatedSale.Date);
            Assert.Equal(sale.PurchaseID, updatedSale.PurchaseID);
            Assert.Equal(sale.SupplierWineID, updatedSale.SupplierWineID);
        }

        [Fact]
        public void DeleteTest()
        {
            using var context = CreateContext();
            var repository = new SaleRepository(context);

            var sale = new Sale
            {
                ID = 2,
                PurchasePrice = 700,
                SellingPrice = 1050,
                Margin = 350,
                Costs = 200,
                Profit = 150,
                WineNumber = 1,
                Date = new DateOnly(2012, 3, 24),
                PurchaseID = 1,
                SupplierWineID = 1
            };

            repository.Delete(sale);

            Assert.Equal(1, context.Sales.Count());

            var deletedSale = context.Sales.Find(3);

            Assert.Null(deletedSale);
        }
    }
}
