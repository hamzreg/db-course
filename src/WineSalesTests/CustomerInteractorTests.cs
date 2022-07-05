using Xunit;
using Moq;

using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Interactors;
using WineSales.Domain.Exceptions;

namespace DomainTests
{
    public class CustomerInteractorTests
    {
        private readonly ICustomerInteractor _interactor;
        private readonly ICustomerRepository _mockRepository;

        private readonly List<Customer> mockCustomers;

        public CustomerInteractorTests()
        {
            mockCustomers = new List<Customer>
            {
                new Customer
                {
                    ID = 1,
                    Name = "Regina",
                    Surname = "Khamzina"
                },
                new Customer
                {
                    ID = 2,
                    Name = "Anna",
                    Surname = "Timoshenko"
                },
                new Customer
                {
                    ID = 3,
                    Name = "Marina",
                    Surname = "Maslova"
                }
            };

            var mockRepository = new Mock<ICustomerRepository>();
            mockRepository.Setup(obj => obj.GetByID(It.IsAny<int>())).Returns(
                (int id) => mockCustomers.Find(x => x.ID == id));
            mockRepository.Setup(obj => obj.Create(It.IsAny<Customer>())).Callback(
                (Customer customer) =>
                {
                    customer.ID = mockCustomers.Count + 1;
                    mockCustomers.Add(customer);
                }
                );
            mockRepository.Setup(obj => obj.Update(It.IsAny<Customer>())).Callback(
                (Customer customer) =>
                {
                    mockCustomers.FindAll(x => x.ID == customer.ID)
                                 .ForEach(x =>
                                 {
                                     x.Name = customer.Name;
                                     x.Surname = customer.Surname;
                                 });
                }
                );
            mockRepository.Setup(obj => obj.Delete(It.IsAny<Customer>())).Callback(
                (Customer customer) =>
                {
                    mockCustomers.RemoveAll(x => x.ID == customer.ID);
                }
                );

            _mockRepository = mockRepository.Object;
            _interactor = new CustomerInteractor(_mockRepository);
        }

        [Fact]
        public void CreateCustomerTest()
        {
            var expectedCount = mockCustomers.Count + 1;

            var customer = new Customer
            {
                Name = "Natalya",
                Surname = "Skvortsova"
            };

            _interactor.CreateCustomer(customer);
            Assert.Equal(expectedCount, mockCustomers.Count);

            var customersList = mockCustomers;
            Assert.All(customersList, obj => Assert.InRange(obj.ID, low: 1, high: expectedCount));
        }

        [Fact]
        public void AlreadyExistsCreateCustomerTest()
        {
            var customer = new Customer
            {
                ID = 1,
                Name = "Regina",
                Surname = "Khamzina"
            };

            void action() => _interactor.CreateCustomer(customer);
            Assert.Throws<CustomerException>(action);

            var exception = Assert.Throws<CustomerException>(action);
            Assert.Equal("Customer: This customer already exists.",
                         exception.Message);
        }

        [Fact]
        public void UpdateCustomerTest()
        {
            var expectedCount = mockCustomers.Count;

            var customer = new Customer
            {
                ID = 1,
                Name = "Regina",
                Surname = "Mikhaleva"
            };

            _interactor.UpdateCustomer(customer);
            Assert.Equal(expectedCount, mockCustomers.Count);

            var customersList = mockCustomers;
            Assert.All(customersList, obj => Assert.InRange(obj.ID, low: 1, high: expectedCount));

            var updatedCustomer = mockCustomers.Find(x => x.ID == customer.ID);
            Assert.NotNull(updatedCustomer);
            Assert.Equal(customer.ID, updatedCustomer?.ID);
            Assert.Equal(customer.Name, updatedCustomer?.Name);
            Assert.Equal(customer.Surname, updatedCustomer?.Surname);
        }

        [Fact]
        public void NotExistsUpdateCustomerTest()
        {
            var customer = new Customer
            {
                ID = 4,
                Name = "Natalya",
                Surname = "Skvortsova"
            };

            void action() => _interactor.UpdateCustomer(customer);
            Assert.Throws<CustomerException>(action);

            var exception = Assert.Throws<CustomerException>(action);
            Assert.Equal("Customer: This customer doesn't exist.",
                         exception.Message);
        }

        [Fact]
        public void DeleteCustomerTest()
        {
            var expectedCount = mockCustomers.Count - 1;

            var customer = new Customer
            {
                ID = 2,
                Name = "Anna",
                Surname = "Timoshenko"
            };

            _interactor.DeleteCustomer(customer);
            Assert.Equal(expectedCount, mockCustomers.Count);

            Assert.Null(mockCustomers.Find(x => x.ID == customer.ID));
        }

        [Fact]
        public void NotExistsDeleteCustomerTest()
        {
            var customer = new Customer
            {
                ID = 4,
                Name = "Natalya",
                Surname = "Skvortsova"
            };

            void action() => _interactor.DeleteCustomer(customer);
            Assert.Throws<CustomerException>(action);

            var exception = Assert.Throws<CustomerException>(action);
            Assert.Equal("Customer: This customer doesn't exist.",
                         exception.Message);
        }
    }
}
