using Xunit;
using Microsoft.EntityFrameworkCore;

using WineSales.Data;
using WineSales.Domain.Models;
using WineSales.Data.Repositories;

namespace DataTests
{
    public class CustomerRepositoryTests
    {
        private readonly DbContextOptions<DataBaseContext> _contextOptions;

        public CustomerRepositoryTests()
        {
            _contextOptions = new DbContextOptionsBuilder<DataBaseContext>()
                .UseInMemoryDatabase("CustomerRepositoryTests")
                .Options;

            using var context = new DataBaseContext(_contextOptions);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Customers.AddRange(
                new Customer { Name = "Regina", Surname = "Khamzina" },
                new Customer { Name = "Anton", Surname = "Mikhalev" });

            context.SaveChanges();
        }

        private DataBaseContext CreateContext() => new DataBaseContext(_contextOptions);

        [Fact]
        public void CreateTest()
        {
            using var context = CreateContext();
            var repository = new CustomerRepository(context);

            var customer = new Customer { Name = "Anna", Surname = "Timoshenko" };

            repository.Create(customer);

            Assert.Equal(3, context.Customers.Count());

            var createdCustomer = context.Customers
                .Single(cstmr => cstmr.ID == 3);

            Assert.NotNull(createdCustomer);
            Assert.Equal(customer.Name, createdCustomer.Name);
            Assert.Equal(customer.Surname, createdCustomer.Surname);
        }

        [Fact]
        public void GetAllTest()
        {
            using var context = CreateContext();
            var repository = new CustomerRepository(context);

            var customers = repository.GetAll();

            Assert.Equal(2, customers.Count);

            Assert.Collection(
                customers,
                cstmr =>
                {
                    Assert.Equal(1, cstmr.ID);
                    Assert.Equal("Regina", cstmr.Name);
                    Assert.Equal("Khamzina", cstmr.Surname);
                },
                cstmr =>
                {
                    Assert.Equal(2, cstmr.ID);
                    Assert.Equal("Anton", cstmr.Name);
                    Assert.Equal("Mikhalev", cstmr.Surname);
                });
        }

        [Fact]
        public void GetByIDTest()
        {
            using var context = CreateContext();
            var repository = new CustomerRepository(context);

            var customer = repository.GetByID(2);

            Assert.NotNull(customer);
            Assert.Equal("Anton", customer.Name);
            Assert.Equal("Mikhalev", customer.Surname);
        }

        [Fact]
        public void GetByNameTest()
        {
            using var context = CreateContext();
            var repository = new CustomerRepository(context);

            var customers = repository.GetByName("Regina");

            Assert.Equal(1, customers.Count);

            Assert.Collection(
                customers,
                cstmr =>
                {
                    Assert.Equal(1, cstmr.ID);
                    Assert.Equal("Regina", cstmr.Name);
                    Assert.Equal("Khamzina", cstmr.Surname);
                });
        }

        [Fact]
        public void GetBySurnameTest()
        {
            using var context = CreateContext();
            var repository = new CustomerRepository(context);

            var customers = repository.GetBySurname("Mikhalev");

            Assert.Equal(1, customers.Count);

            Assert.Collection(
                customers,
                cstmr =>
                {
                    Assert.Equal(2, cstmr.ID);
                    Assert.Equal("Anton", cstmr.Name);
                    Assert.Equal("Mikhalev", cstmr.Surname);
                });
        }

        [Fact]
        public void UpdateTest()
        {
            using var context = CreateContext();
            var repository = new CustomerRepository(context);

            var customer = new Customer { ID = 1, Name = "Regina", Surname = "Mikhaleva" };

            repository.Update(customer);

            Assert.Equal(2, context.Customers.Count());

            var updatedCustomer = context.Customers
                .Single(cstmr => cstmr.ID == customer.ID);

            Assert.NotNull(updatedCustomer);
            Assert.Equal(1, updatedCustomer.ID);
            Assert.Equal(customer.Name, updatedCustomer.Name);
            Assert.Equal(customer.Surname, updatedCustomer.Surname);
        }

        [Fact]
        public void DeleteTest()
        {
            using var context = CreateContext();
            var repository = new CustomerRepository(context);

            var customer = new Customer { ID = 2, Name = "Anton", Surname = "Mikhalev" };

            repository.Delete(customer);

            Assert.Equal(1, context.Customers.Count());
        }
    }
}
