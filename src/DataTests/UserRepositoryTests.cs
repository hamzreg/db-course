using Xunit;
using Microsoft.EntityFrameworkCore;

using WineSales.Data;
using WineSales.Domain.Models;
using WineSales.Data.Repositories;

namespace DataTests
{
    public class UserRepositoryTests
    {
        private readonly DbContextOptions<DataBaseContext> _contextOptions;

        public UserRepositoryTests()
        {
            _contextOptions = new DbContextOptionsBuilder<DataBaseContext>()
                .UseInMemoryDatabase("UserRepositoryTests")
                .Options;

            using var context = new DataBaseContext(_contextOptions);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Users.AddRange(
                new User("hamzreg", "password", "admin"),
                new User("r1mok", "password", "supplier"),
                new User("kovkir", "password", "customer"));

            context.SaveChanges();
        }

        private DataBaseContext CreateContext() => new DataBaseContext(_contextOptions);

        [Fact]
        public void CreateTest()
        {
            using var context = CreateContext();
            var repository = new UserRepository(context);

            var user = new User("MyMiDi", "password", "supplier");

            repository.Create(user);

            Assert.Equal(4, context.Users.Count());

            var createdUser = context.Users.Find(4);

            Assert.NotNull(createdUser);
            Assert.Equal(createdUser.Login, user.Login);
            Assert.Equal(createdUser.Password, user.Password);
            Assert.Equal(createdUser.Role, user.Role);
        }

        [Fact]
        public void GetAllTest()
        {
            using var context = CreateContext();
            var repository = new UserRepository(context);

            var users = repository.GetAll();

            Assert.Equal(3, users.Count);

            Assert.Collection(
                users,
                user =>
                {
                    Assert.Equal(1, user.ID);
                    Assert.Equal("hamzreg", user.Login);
                    Assert.Equal("password", user.Password);
                    Assert.Equal("admin", user.Role);
                },
                user =>
                {
                    Assert.Equal(2, user.ID);
                    Assert.Equal("r1mok", user.Login);
                    Assert.Equal("password", user.Password);
                    Assert.Equal("supplier", user.Role);
                },
                user =>
                {
                    Assert.Equal(3, user.ID);
                    Assert.Equal("kovkir", user.Login);
                    Assert.Equal("password", user.Password);
                    Assert.Equal("customer", user.Role);
                });
        }

        [Fact]
        public void GetByIDTest()
        {
            using var context = CreateContext();
            var repository = new UserRepository(context);

            var user = repository.GetByID(2);

            Assert.NotNull(user);
            Assert.Equal("r1mok", user.Login);
            Assert.Equal("password", user.Password);
            Assert.Equal("supplier", user.Role);
        }

        [Fact]
        public void GetByLoginTest()
        {
            using var context = CreateContext();
            var repository = new UserRepository(context);

            var user = repository.GetByLogin("kovkir");

            Assert.NotNull(user);
            Assert.Equal("kovkir", user.Login);
            Assert.Equal("password", user.Password);
            Assert.Equal("customer", user.Role);
        }

        [Fact]
        public void GetByRoleTest()
        {
            using var context = CreateContext();
            var repository = new UserRepository(context);

            var users = repository.GetByRole("admin");

            Assert.Equal(1, users.Count);

            Assert.Collection(
                users,
                user =>
                {
                    Assert.Equal(1, user.ID);
                    Assert.Equal("hamzreg", user.Login);
                    Assert.Equal("password", user.Password);
                    Assert.Equal("admin", user.Role);
                });
        }

        [Fact]
        public void RegisterTest()
        {
            using var context = CreateContext();
            var repository = new UserRepository(context);

            var user = new User("MyMiDi", "password", "supplier");

            repository.Register(user);

            Assert.Equal(4, context.Users.Count());

            var registeredUser = context.Users.Find(4);

            Assert.NotNull(registeredUser);
            Assert.Equal(registeredUser.Login, user.Login);
            Assert.Equal(registeredUser.Password, user.Password);
            Assert.Equal(registeredUser.Role, user.Role);
        }

        [Fact]
        public void UpdateTest()
        {
            using var context = CreateContext();
            var repository = new UserRepository(context);

            var user = new User("gerzmah", "password", "supplier")
            { ID = 1};

            repository.Update(user);

            Assert.Equal(3, context.Users.Count());

            var updatedUser = context.Users.Find(1);

            Assert.NotNull(updatedUser);
            Assert.Equal(updatedUser.Login, user.Login);
            Assert.Equal(updatedUser.Password, user.Password);
            Assert.Equal(updatedUser.Role, user.Role);
        }

        [Fact]
        public void DeleteTest()
        {
            using var context = CreateContext();
            var repository = new UserRepository(context);

            var user = new User("kovkir", "password", "customer")
            { ID = 3 };

            repository.Delete(user);

            Assert.Equal(2, context.Users.Count());

            var deletedUser = context.Users.Find(3);

            Assert.Null(deletedUser);
        }
    }
}
