using Xunit;
using Microsoft.EntityFrameworkCore;

using WineSales.Data;
using WineSales.Domain.Models;
using WineSales.Data.Repositories;

namespace DataTests
{
    public class SupplierRepositoryTests
    {
        private readonly DbContextOptions<DataBaseContext> _contextOptions;

        public SupplierRepositoryTests()
        {
            _contextOptions = new DbContextOptionsBuilder<DataBaseContext>()
                .UseInMemoryDatabase("SupplierRepositoryTests")
                .Options;

            using var context = new DataBaseContext(_contextOptions);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Suppliers.AddRange(
                new Supplier
                {
                    Name = "Fanagoria",
                    Country = "Russia",
                    Experience = 65,
                    License = true
                },
                new Supplier
                {
                    Name = "Agora",
                    Country = "Russia",
                    Experience = 25,
                    License = true
                });

            context.SupplierWines.AddRange(
                new SupplierWine { SupplierID = 2 },
                new SupplierWine { SupplierID = 1 });

            context.SaveChanges();
        }

        private DataBaseContext CreateContext() => new DataBaseContext(_contextOptions);

        [Fact]
        public void CreateTest()
        {
            using var context = CreateContext();
            var repository = new SupplierRepository(context);

            var supplier = new Supplier
            {
                Name = "Alianta",
                Country = "Russia",
                Experience = 29,
                License = true
            };

            repository.Create(supplier);

            Assert.Equal(3, context.Suppliers.Count());

            var createdSupplier = context.Suppliers.Find(3);

            Assert.NotNull(createdSupplier);
            Assert.Equal(supplier.Name, createdSupplier.Name);
            Assert.Equal(supplier.Country, createdSupplier.Country);
            Assert.Equal(supplier.Experience, createdSupplier.Experience);
            Assert.Equal(supplier.License, createdSupplier.License);
        }

        [Fact]
        public void GetAllTest()
        {
            using var context = CreateContext();
            var repository = new SupplierRepository(context);

            var suppliers = repository.GetAll();

            Assert.Collection(
                suppliers,
                supplier =>
                {
                    Assert.Equal(1, supplier.ID);
                    Assert.Equal("Fanagoria", supplier.Name);
                    Assert.Equal("Russia", supplier.Country);
                    Assert.Equal(65, supplier.Experience);
                    Assert.True(supplier.License);
                },
                supplier =>
                {
                    Assert.Equal(2, supplier.ID);
                    Assert.Equal("Agora", supplier.Name);
                    Assert.Equal("Russia", supplier.Country);
                    Assert.Equal(25, supplier.Experience);
                    Assert.True(supplier.License);
                });
        }

        [Fact]
        public void GetByIDTest()
        {
            using var context = CreateContext();
            var repository = new SupplierRepository(context);

            var supplier = repository.GetByID(1);

            Assert.NotNull(supplier);
            Assert.Equal("Fanagoria", supplier.Name);
            Assert.Equal("Russia", supplier.Country);
            Assert.Equal(65, supplier.Experience);
            Assert.True(supplier.License);
        }

        [Fact]
        public void GetByNameTest()
        {
            using var context = CreateContext();
            var repository = new SupplierRepository(context);

            var supplier = repository.GetByName("Agora");

            Assert.NotNull(supplier);
            Assert.Equal(2, supplier.ID);
            Assert.Equal("Russia", supplier.Country);
            Assert.Equal(25, supplier.Experience);
            Assert.True(supplier.License);
        }

        [Fact]
        public void GetByCountryTest()
        {
            using var context = CreateContext();
            var repository = new SupplierRepository(context);

            var suppliers = repository.GetByCountry("Russia");

            Assert.Collection(
                suppliers,
                supplier =>
                {
                    Assert.Equal(1, supplier.ID);
                    Assert.Equal("Fanagoria", supplier.Name);
                    Assert.Equal("Russia", supplier.Country);
                    Assert.Equal(65, supplier.Experience);
                    Assert.True(supplier.License);
                },
                supplier =>
                {
                    Assert.Equal(2, supplier.ID);
                    Assert.Equal("Agora", supplier.Name);
                    Assert.Equal("Russia", supplier.Country);
                    Assert.Equal(25, supplier.Experience);
                    Assert.True(supplier.License);
                });
        }

        [Fact]
        public void GetByExperienceMoreTest()
        {
            using var context = CreateContext();
            var repository = new SupplierRepository(context);

            var suppliers = repository.GetByExperience(60, true);

            Assert.Collection(
                suppliers,
                supplier =>
                {
                    Assert.Equal(1, supplier.ID);
                    Assert.Equal("Fanagoria", supplier.Name);
                    Assert.Equal("Russia", supplier.Country);
                    Assert.Equal(65, supplier.Experience);
                    Assert.True(supplier.License);
                });
        }

        [Fact]
        public void GetByExperienceLessTest()
        {
            using var context = CreateContext();
            var repository = new SupplierRepository(context);

            var suppliers = repository.GetByExperience(30, false);

            Assert.Collection(
                suppliers,
                supplier =>
                {
                    Assert.Equal(2, supplier.ID);
                    Assert.Equal("Agora", supplier.Name);
                    Assert.Equal("Russia", supplier.Country);
                    Assert.Equal(25, supplier.Experience);
                    Assert.True(supplier.License);
                });
        }

        [Fact]
        public void GetByLicenseTest()
        {
            using var context = CreateContext();
            var repository = new SupplierRepository(context);

            var suppliers = repository.GetByLicense(true);

            Assert.Collection(
                suppliers,
                supplier =>
                {
                    Assert.Equal(1, supplier.ID);
                    Assert.Equal("Fanagoria", supplier.Name);
                    Assert.Equal("Russia", supplier.Country);
                    Assert.Equal(65, supplier.Experience);
                    Assert.True(supplier.License);
                },
                supplier =>
                {
                    Assert.Equal(2, supplier.ID);
                    Assert.Equal("Agora", supplier.Name);
                    Assert.Equal("Russia", supplier.Country);
                    Assert.Equal(25, supplier.Experience);
                    Assert.True(supplier.License);
                });
        }

        [Fact]
        public void GetBySupplierWineIDTest()
        {
            using var context = CreateContext();
            var repository = new SupplierRepository(context);

            var supplier = repository.GetBySupplierWineID(1);

            Assert.NotNull(supplier);
            Assert.Equal(2, supplier.ID);
            Assert.Equal("Agora", supplier.Name);
            Assert.Equal("Russia", supplier.Country);
            Assert.Equal(25, supplier.Experience);
            Assert.True(supplier.License);
        }

        [Fact]
        public void UpdateTest()
        {
            using var context = CreateContext();
            var repository = new SupplierRepository(context);

            var supplier = new Supplier
            {
                ID = 1,
                Name = "Alianta",
                Country = "Russia",
                Experience = 29,
                License = true
            };

            repository.Update(supplier);

            Assert.Equal(2, context.Suppliers.Count());

            var updatedSupplier = context.Suppliers.Find(1);

            Assert.NotNull(updatedSupplier);
            Assert.Equal(supplier.Name, updatedSupplier.Name);
            Assert.Equal(supplier.Country, updatedSupplier.Country);
            Assert.Equal(supplier.Experience, updatedSupplier.Experience);
            Assert.Equal(supplier.License, updatedSupplier.License);
        }

        [Fact]
        public void DeleteTest()
        {
            using var context = CreateContext();
            var repository = new SupplierRepository(context);

            var supplier = new Supplier
            {
                ID = 2,
                Name = "Agora",
                Country = "Russia",
                Experience = 25,
                License = true
            };

            repository.Delete(supplier);

            Assert.Equal(1, context.Suppliers.Count());

            var deletedSupplier = context.Suppliers.Find(2);
            Assert.Null(deletedSupplier);
        }
    }
}
