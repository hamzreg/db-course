using Xunit;
using Microsoft.EntityFrameworkCore;

using WineSales.Data;
using WineSales.Domain.Models;
using WineSales.Data.Repositories;

namespace DataTests
{
    public class SupplierWineRepositoryTests
    {
        private readonly DbContextOptions<DataBaseContext> _contextOptions;

        public SupplierWineRepositoryTests()
        {
            _contextOptions = new DbContextOptionsBuilder<DataBaseContext>()
                .UseInMemoryDatabase("SupplierWineRepositoryTests")
                .Options;

            using var context = new DataBaseContext(_contextOptions);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Suppliers.AddRange(
                new Supplier
                {
                    ID = 1,
                    Name = "Fanagoria"
                },
                new Supplier
                {
                    ID = 2,
                    Name = "Agora"
                });

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

            context.SupplierWines.AddRange(
                new SupplierWine
                {
                    ID = 1,
                    SupplierID = 1,
                    WineID = 2,
                    Price = 700,
                    Percent = 50,
                    Rating = 5
                },
                new SupplierWine
                {
                    ID = 2,
                    SupplierID = 2,
                    WineID = 1,
                    Price = 500,
                    Percent = 50,
                    Rating = 8
                });

            context.SaveChanges();
        }

        private DataBaseContext CreateContext() => new DataBaseContext(_contextOptions);

        [Fact]
        public void CreateTest()
        {
            using var context = CreateContext();
            var repository = new SupplierWineRepository(context);

            var supplierWine = new SupplierWine
            {
                SupplierID = 2,
                WineID = 2,
                Price = 1000,
                Percent = 50,
                Rating = 6
            };

            repository.Create(supplierWine);

            Assert.Equal(3, context.SupplierWines.Count());

            var createdSupplierWine = context.SupplierWines.Find(3);

            Assert.NotNull(createdSupplierWine);
            Assert.Equal(supplierWine.SupplierID, createdSupplierWine.SupplierID);
            Assert.Equal(supplierWine.WineID, createdSupplierWine.WineID);
            Assert.Equal(supplierWine.Price, createdSupplierWine.Price);
            Assert.Equal(supplierWine.Percent, createdSupplierWine.Percent);
            Assert.Equal(supplierWine.Rating, createdSupplierWine.Rating);
        }

        [Fact]
        public void GetAllTest()
        {
            using var context = CreateContext();
            var repository = new SupplierWineRepository(context);

            var supplierWines = repository.GetAll();

            Assert.Equal(2, supplierWines.Count);

            Assert.Collection(
                supplierWines,
                supplierWine =>
                {
                    Assert.Equal(1, supplierWine.ID);
                    Assert.Equal(1, supplierWine.SupplierID);
                    Assert.Equal(2, supplierWine.WineID);
                    Assert.Equal(700, supplierWine.Price);
                    Assert.Equal(50, supplierWine.Percent);
                    Assert.Equal(5, supplierWine.Rating);
                },
                supplierWine =>
                {
                    Assert.Equal(2, supplierWine.ID);
                    Assert.Equal(2, supplierWine.SupplierID);
                    Assert.Equal(1, supplierWine.WineID);
                    Assert.Equal(500, supplierWine.Price);
                    Assert.Equal(50, supplierWine.Percent);
                    Assert.Equal(8, supplierWine.Rating);
                });
        }

        [Fact]
        public void GetByID()
        {
            using var context = CreateContext();
            var repository = new SupplierWineRepository(context);

            var supplierWine = repository.GetByID(1);

            Assert.NotNull(supplierWine);
            Assert.Equal(1, supplierWine.SupplierID);
            Assert.Equal(2, supplierWine.WineID);
            Assert.Equal(700, supplierWine.Price);
            Assert.Equal(50, supplierWine.Percent);
            Assert.Equal(5, supplierWine.Rating);
        }

        [Fact]
        public void GetByWineIDTest()
        {
            using var context = CreateContext();
            var repository = new SupplierWineRepository(context);

            var supplierWines = repository.GetByWineID(2);

            Assert.Equal(1, supplierWines.Count);

            Assert.Collection(
                supplierWines,
                supplierWine =>
                {
                    Assert.Equal(1, supplierWine.ID);
                    Assert.Equal(1, supplierWine.SupplierID);
                    Assert.Equal(2, supplierWine.WineID);
                    Assert.Equal(700, supplierWine.Price);
                    Assert.Equal(50, supplierWine.Percent);
                    Assert.Equal(5, supplierWine.Rating);
                });
        }

        [Fact]
        public void GetByPriceTest()
        {
            using var context = CreateContext();
            var repository = new SupplierWineRepository(context);

            var supplierWines = repository.GetByPrice(500);

            Assert.Equal(1, supplierWines.Count);

            Assert.Collection(
                supplierWines,
                supplierWine =>
                {
                    Assert.Equal(2, supplierWine.ID);
                    Assert.Equal(2, supplierWine.SupplierID);
                    Assert.Equal(1, supplierWine.WineID);
                    Assert.Equal(500, supplierWine.Price);
                    Assert.Equal(50, supplierWine.Percent);
                    Assert.Equal(8, supplierWine.Rating);
                });
        }

        [Fact]
        public void GetByPercentTest()
        {
            using var context = CreateContext();
            var repository = new SupplierWineRepository(context);

            var supplierWines = repository.GetByPercent(50);

            Assert.Equal(2, supplierWines.Count);

            Assert.Collection(
                supplierWines,
                supplierWine =>
                {
                    Assert.Equal(1, supplierWine.ID);
                    Assert.Equal(1, supplierWine.SupplierID);
                    Assert.Equal(2, supplierWine.WineID);
                    Assert.Equal(700, supplierWine.Price);
                    Assert.Equal(50, supplierWine.Percent);
                    Assert.Equal(5, supplierWine.Rating);
                },
                supplierWine =>
                {
                    Assert.Equal(2, supplierWine.ID);
                    Assert.Equal(2, supplierWine.SupplierID);
                    Assert.Equal(1, supplierWine.WineID);
                    Assert.Equal(500, supplierWine.Price);
                    Assert.Equal(50, supplierWine.Percent);
                    Assert.Equal(8, supplierWine.Rating);
                });
        }

        [Fact]
        public void GetBySupplierIDTest()
        {
            using var context = CreateContext();
            var repository = new SupplierWineRepository(context);

            var (wines, prices) = repository.GetBySupplierID(1);

            Assert.Equal(1, wines.Count);
            Assert.Equal(1, prices.Count);

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
                });

            Assert.Collection(
                prices,
                price => Assert.Equal(700, price));
        }

        [Fact]
        public void GetAllWineTest()
        {
            using var context = CreateContext();
            var repository = new SupplierWineRepository(context);

            var (wines, prices) = repository.GetAllWine();

            Assert.Equal(2, wines.Count);
            Assert.Equal(2, prices.Count);

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
                prices,
                price => Assert.Equal(1050, price),
                price => Assert.Equal(750, price));
        }

        [Fact]
        public void GetByAdminTest()
        {
            using var context = CreateContext();
            var repository = new SupplierWineRepository(context);

            var (wines, suppliers, prices) = repository.GetByAdmin();

            Assert.Equal(2, wines.Count);
            Assert.Equal(2, suppliers.Count);
            Assert.Equal(2, prices.Count);

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
                supplier => Assert.Equal("Agora", supplier));

            Assert.Collection(
                prices,
                price => Assert.Equal(1050, price),
                price => Assert.Equal(750, price));
        }

        [Fact]
        public void GetRatingTest()
        {
            using var context = CreateContext();
            var repository = new SupplierWineRepository(context);

            var (wines, points) = repository.GetRating();

            Assert.Equal(2, wines.Count);
            Assert.Equal(2, points.Count);

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
                points,
                value => Assert.Equal(8, value),
                value => Assert.Equal(5, value));
        }

        [Fact]
        public void UpdateTest()
        {
            using var context = CreateContext();
            var repository = new SupplierWineRepository(context);

            var supplierWine = new SupplierWine
            {
                ID = 2,
                SupplierID = 2,
                WineID = 1,
                Price = 600,
                Percent = 50,
                Rating = 6
            };

            repository.Update(supplierWine);

            Assert.Equal(2, context.SupplierWines.Count());

            var updatedSupplierWine = context.SupplierWines.Find(2);

            Assert.NotNull(updatedSupplierWine);
            Assert.Equal(supplierWine.SupplierID, updatedSupplierWine.SupplierID);
            Assert.Equal(supplierWine.WineID, updatedSupplierWine.WineID);
            Assert.Equal(supplierWine.Price, updatedSupplierWine.Price);
            Assert.Equal(supplierWine.Percent, updatedSupplierWine.Percent);
            Assert.Equal(supplierWine.Rating, updatedSupplierWine.Rating);
        }

        [Fact]
        public void DeleteTest()
        {
            using var context = CreateContext();
            var repository = new SupplierWineRepository(context);

            var supplierWine = new SupplierWine
            {
                ID = 2,
                SupplierID = 2,
                WineID = 1,
                Price = 500,
                Percent = 50,
                Rating = 8
            };

            repository.Delete(supplierWine);

            Assert.Equal(1, context.SupplierWines.Count());

            var deletedSupplierWine = context.SupplierWines.Find(2);
            Assert.Null(deletedSupplierWine);
        }
    }
}
