using Xunit;
using Microsoft.EntityFrameworkCore;

using WineSales.Data;
using WineSales.Domain.Models;
using WineSales.Data.Repositories;

namespace DataTests
{
    public class WineRepositoryTests
    {
        private readonly DbContextOptions<DataBaseContext> _contextOptions;

        public WineRepositoryTests()
        {
            _contextOptions = new DbContextOptionsBuilder<DataBaseContext>()
                .UseInMemoryDatabase("WineRepositoryTests")
                .Options;

            using var context = new DataBaseContext(_contextOptions);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Wines.AddRange(
                new Wine
                {
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
            var repository = new WineRepository(context);

            var wine = new Wine
            {
                Kind = "lambrusco",
                Color = "rose",
                Sugar = "sweet",
                Volume = 0.75,
                Alcohol = 7.5,
                Aging = 2,
                Number = 1
            };

            repository.Create(wine);

            Assert.Equal(3, context.Wines.Count());

            var createdWine = context.Wines.Find(3);

            Assert.NotNull(createdWine);
            Assert.Equal(wine.Kind, createdWine.Kind);
            Assert.Equal(wine.Color, createdWine.Color);
            Assert.Equal(wine.Sugar, createdWine.Sugar);
            Assert.Equal(wine.Volume, createdWine.Volume);
            Assert.Equal(wine.Alcohol, createdWine.Alcohol);
            Assert.Equal(wine.Aging, createdWine.Aging);
            Assert.Equal(wine.Number, createdWine.Number);
        }

        [Fact]
        public void GetAllTest()
        {
            using var context = CreateContext();
            var repository = new WineRepository(context);

            var wines = repository.GetAll();

            Assert.Equal(2, wines.Count);

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
        }

        [Fact]
        public void GetByIDTest()
        {
            using var context = CreateContext();
            var repository = new WineRepository(context);

            var wine = repository.GetByID(2);

            Assert.NotNull(wine);
            Assert.Equal("lambrusco", wine.Kind);
            Assert.Equal("white", wine.Color);
            Assert.Equal("semi-sweet", wine.Sugar);
            Assert.Equal(0.75, wine.Volume);
            Assert.Equal(7.5, wine.Alcohol);
            Assert.Equal(2, wine.Aging);
            Assert.Equal(2, wine.Number);
        }

        [Fact]
        public void GetByKindTest()
        {
            using var context = CreateContext();
            var repository = new WineRepository(context);

            var wines = repository.GetByKind("lambrusco");

            Assert.Equal(2, wines.Count);

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
        }

        [Fact]
        public void GetByColorTest()
        {
            using var context = CreateContext();
            var repository = new WineRepository(context);

            var wines = repository.GetByColor("white");

            Assert.Equal(1, wines.Count);

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
        }

        [Fact]
        public void GetBySugarTest()
        {
            using var context = CreateContext();
            var repository = new WineRepository(context);

            var wines = repository.GetBySugar("dry");

            Assert.Equal(1, wines.Count);

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
                });
        }

        [Fact]
        public void GetByVolumeTest()
        {
            using var context = CreateContext();
            var repository = new WineRepository(context);

            var wines = repository.GetByVolume(0.75);

            Assert.Equal(2, wines.Count);

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
        }

        [Fact]
        public void GetByAlcoholTest()
        {
            using var context = CreateContext();
            var repository = new WineRepository(context);

            var wines = repository.GetByAlcohol(7.5);

            Assert.Equal(2, wines.Count);

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
        }

        [Fact]
        public void GetByAgingTest()
        {
            using var context = CreateContext();
            var repository = new WineRepository(context);

            var wines = repository.GetByAging(2);

            Assert.Equal(2, wines.Count);

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
        }

        [Fact]
        public void GetByNumberTest()
        {
            using var context = CreateContext();
            var repository = new WineRepository(context);

            var wines = repository.GetByNumber(2);

            Assert.Equal(1, wines.Count);

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
        }

        [Fact]
        public void UpdateTest()
        {
            using var context = CreateContext();
            var repository = new WineRepository(context);

            var wine = new Wine
            {
                ID = 1,
                Kind = "lambrusco",
                Color = "red",
                Sugar = "dry",
                Volume = 0.75,
                Alcohol = 7.5,
                Aging = 2,
                Number = 2
            };

            repository.Update(wine);

            Assert.Equal(2, context.Wines.Count());

            var updatedWine = context.Wines.Find(wine.ID);

            Assert.NotNull(updatedWine);
            Assert.Equal("lambrusco", wine.Kind);
            Assert.Equal("red", wine.Color);
            Assert.Equal("dry", wine.Sugar);
            Assert.Equal(0.75, wine.Volume);
            Assert.Equal(7.5, wine.Alcohol);
            Assert.Equal(2, wine.Aging);
            Assert.Equal(2, wine.Number);
        }

        [Fact]
        public void DeleteTest()
        {
            using var context = CreateContext();
            var repository = new WineRepository(context);

            var wine = new Wine
            {
                ID = 1,
                Kind = "lambrusco",
                Color = "red",
                Sugar = "dry",
                Volume = 0.75,
                Alcohol = 7.5,
                Aging = 2,
                Number = 1
            };

            repository.Delete(wine);

            Assert.Equal(1, context.Wines.Count());
        }
    }
}
