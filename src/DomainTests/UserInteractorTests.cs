using Xunit;
using Moq;

using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Interactors;
using WineSales.Domain.Exceptions;

namespace DomainTests
{
    public class UserInteractorTests
    {
        private readonly IUserInteractor _interactor;
        private readonly IUserRepository _mockRepository;

        private readonly List<User> mockUsers;

        public UserInteractorTests()
        {
            mockUsers = new List<User>
            {
                new User("hamzreg", "password", "admin")
                {
                    ID = 1
                },
                new User("r1mok", "password", "supplier")
                {
                    ID = 2
                },
                new User("kovkir", "password", "customer")
                {
                    ID = 3
                }
            };

            var mockRepository = new Mock<IUserRepository>();
            mockRepository.Setup(obj => obj.GetByID(It.IsAny<int>())).Returns(
                (int id) => mockUsers.Find(x => x.ID == id));
            mockRepository.Setup(obj => obj.GetByLogin(It.IsAny<string>())).Returns(
                (string login) => mockUsers.Find(x => x.Login == login));
            mockRepository.Setup(obj => obj.Create(It.IsAny<User>())).Callback(
                (User user) =>
                {
                    user.ID = mockUsers.Count + 1;
                    mockUsers.Add(user);
                }
                );
            mockRepository.Setup(obj => obj.Update(It.IsAny<User>())).Callback(
                (User user) =>
                {
                    mockUsers.FindAll(x => x.ID == user.ID)
                                 .ForEach(x =>
                                 {
                                     x.Login = user.Login;
                                     x.Password = user.Password;
                                 });
                }
                );
            mockRepository.Setup(obj => obj.Delete(It.IsAny<User>())).Callback(
                (User user) =>
                {
                    mockUsers.RemoveAll(x => x.ID == user.ID);
                }
                );
            mockRepository.Setup(obj => obj.Register(It.IsAny<User>())).Callback(
                (User user) =>
                {
                    var newUser = new User(user.Login, user.Password, user.Role)
                    {
                        ID = mockUsers.Count + 1
                    };

                    mockUsers.Add(newUser);
                }
                );
            mockRepository.Setup(obj => obj.ConnectToDataStore(It.IsAny<User>())).Returns(
                (User user) => true);
            _mockRepository = mockRepository.Object;
            _interactor = new UserInteractor(_mockRepository);
        }

        [Fact]
        public void CreateUserTest()
        {
            var expectedCount = mockUsers.Count + 1;

            var user = new User("MyMiDi", "password", "customer");

            _interactor.CreateUser(user);
            Assert.Equal(expectedCount, mockUsers.Count);

            var usersList = mockUsers;
            Assert.All(usersList, obj => Assert.InRange(obj.ID, low: 1, high: expectedCount));
        }

        [Fact]
        public void AlreadyExistsCreateUserTest()
        {
            var user = new User("hamzreg", "password", "customer");

            void action() => _interactor.CreateUser(user);
            Assert.Throws<UserException>(action);

            var exception = Assert.Throws<UserException>(action);
            Assert.Equal("User: This user already exists.",
                         exception.Message);
        }

        [Fact]
        public void InvalidPasswordCreateUserTest()
        {
            var user = new User("MyMiDi", "pass", "customer");

            void action() => _interactor.CreateUser(user);
            Assert.Throws<UserException>(action);

            var exception = Assert.Throws<UserException>(action);
            Assert.Equal("User: Invalid input of password.",
                         exception.Message);
        }

        [Fact]
        public void UpdateCustomerTest()
        {
            var expectedCount = mockUsers.Count;

            var user = new User("gerzmah", "password", "admin")
            {
                ID = 1
            };

            _interactor.UpdateUser(user);
            Assert.Equal(expectedCount, mockUsers.Count);

            var usersList = mockUsers;
            Assert.All(usersList, obj => Assert.InRange(obj.ID, low: 1, high: expectedCount));

            var updatedUser = mockUsers.Find(x => x.ID == user.ID);
            Assert.NotNull(updatedUser);
            Assert.Equal(user.ID, updatedUser?.ID);
            Assert.Equal(user.Login, updatedUser?.Login);
            Assert.Equal(user.Password, updatedUser?.Password);
            Assert.Equal(user.Role, updatedUser?.Role);
        }

        [Fact]
        public void NotExistsUpdateUserTest()
        {
            var user = new User("gerzmah", "password", "admin")
            {
                ID = 5
            };

            void action() => _interactor.UpdateUser(user);
            Assert.Throws<UserException>(action);

            var exception = Assert.Throws<UserException>(action);
            Assert.Equal("User: This user doesn't exist.",
                         exception.Message);
        }

        [Fact]
        public void UsedLoginUpdateUserTest()
        {
            var user = new User("r1mok", "password", "admin")
            {
                ID = 1
            };

            void action() => _interactor.UpdateUser(user);
            Assert.Throws<UserException>(action);

            var exception = Assert.Throws<UserException>(action);
            Assert.Equal("User: This login is already in use.",
                         exception.Message);
        }

        [Fact]
        public void InvalidPasswordUpdateUserTest()
        {
            var user = new User("gerzmah", "pass", "admin")
            {
                ID = 1
            };

            void action() => _interactor.UpdateUser(user);
            Assert.Throws<UserException>(action);

            var exception = Assert.Throws<UserException>(action);
            Assert.Equal("User: Invalid input of password.",
                         exception.Message);
        }

        [Fact]
        public void DeleteUserTest()
        {
            var expectedCount = mockUsers.Count - 1;

            var user = new User("kovkir", "password", "customer")
            {
                ID = 3
            };

            _interactor.DeleteUser(user);
            Assert.Equal(expectedCount, mockUsers.Count);

            Assert.Null(mockUsers.Find(x => x.ID == user.ID));
        }

        [Fact]
        public void NotExistsDeleteUserTest()
        {
            var user = new User("hamzreg", "password", "admin")
            {
                ID = 5
            };

            void action() => _interactor.DeleteUser(user);
            Assert.Throws<UserException>(action);

            var exception = Assert.Throws<UserException>(action);
            Assert.Equal("User: This user doesn't exist.",
                         exception.Message);
        }

        [Fact]
        public void RegisterTest()
        {
            var expectedCount = mockUsers.Count + 1;

            var info = new LoginDetails
            {
                Login = "MyMiDi",
                Password = "password"
            };

            _interactor.Register(info, "customer");
            Assert.Equal(expectedCount, mockUsers.Count);

            var usersList = mockUsers;
            Assert.All(usersList, obj => Assert.InRange(obj.ID, low: 1, high: expectedCount));
        }

        [Fact]
        public void AlreadyExistsRegisterTest()
        {
            var info = new LoginDetails
            {
                Login = "kovkir",
                Password = "password"
            };

            void action() => _interactor.Register(info, "customer");
            Assert.Throws<UserException>(action);

            var exception = Assert.Throws<UserException>(action);
            Assert.Equal("User: This user already exists.",
                         exception.Message);
        }

        [Fact]
        public void InvalidPasswordRegisterTest()
        {
            var info = new LoginDetails
            {
                Login = "MyMiDi",
                Password = "pass"
            };

            void action() => _interactor.Register(info, "customer");
            Assert.Throws<UserException>(action);

            var exception = Assert.Throws<UserException>(action);
            Assert.Equal("User: Invalid input of password.",
                         exception.Message);
        }

        [Fact]
        public void SignInTest()
        {
            var info = new LoginDetails
            {
                Login = "hamzreg",
                Password = "password"
            };

            _interactor.SignIn(info);

            User nowUser = _interactor.GetNowUser();
            Assert.Equal(info.Login, nowUser.Login);
            Assert.Equal(info.Password, nowUser.Password);
        }

        [Fact]
        public void NotExistsSignInTest()
        {
            var info = new LoginDetails
            {
                Login = "gerzmah",
                Password = "password"
            };

            void action() => _interactor.SignIn(info);
            Assert.Throws<UserException>(action);

            var exception = Assert.Throws<UserException>(action);
            Assert.Equal("User: This user doesn't exist.",
                         exception.Message);
        }

        [Fact]
        public void InvalidPasswordSignInTest()
        {
            var info = new LoginDetails
            {
                Login = "hamzreg",
                Password = "pass"
            };

            void action() => _interactor.SignIn(info);
            Assert.Throws<UserException>(action);

            var exception = Assert.Throws<UserException>(action);
            Assert.Equal("User: Invalid input of password.",
                         exception.Message);
        }

        [Fact]
        public void WrongPasswordSignInTest()
        {
            var info = new LoginDetails
            {
                Login = "hamzreg",
                Password = "helloworld"
            };

            void action() => _interactor.SignIn(info);
            Assert.Throws<UserException>(action);

            var exception = Assert.Throws<UserException>(action);
            Assert.Equal("User: Invalid password.",
                         exception.Message);
        }
    }
}
