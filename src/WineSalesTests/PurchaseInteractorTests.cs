using Xunit;
using Moq;

using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Interactors;
using WineSales.Domain.Exceptions;
using WineSales.Config;

namespace DomainTests
{
    public class PurchaseInteractorTests
    {
        private readonly IPurchaseInteractor _interactor;
        private readonly IPurchaseRepository _mockRepository;

        private readonly List<Purchase> mockPurchases;
        private readonly List<Customer> mockCustomers;

        public PurchaseInteractorTests()
        {
            mockPurchases = new List<Purchase>
            {
                new Purchase
                {
                    ID = 1,
                    Price = 500,
                    Status = (int)PurchaseConfig.Statuses.Active
                },
                new Purchase
                {
                    ID = 2,
                    Price = 1000,
                    Status = (int)PurchaseConfig.Statuses.Canceled
                },
                new Purchase
                {
                    ID = 3,
                    Price = 700,
                    Status = (int)PurchaseConfig.Statuses.Active
                }
            };

/*            mockCustomers = new List<Customer>
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
            };*/

            var mockRepository = new Mock<IPurchaseRepository>();
            mockRepository.Setup(obj => obj.GetByID(It.IsAny<int>())).Returns(
                (int id) => mockPurchases.Find(x => x.ID == id));
            mockRepository.Setup(obj => obj.Create(It.IsAny<Purchase>())).Callback(
                (Purchase purchase) =>
                {
                    purchase.ID = mockPurchases.Count + 1;
                    mockPurchases.Add(purchase);
                }
                );
/*            mockRepository.Setup(obj => obj.GetByCustomer(It.IsAny<int>())).Returns(
                (int id) =>
                {
                    purchase.ID = mockPurchases.Count + 1;
                    mockPurchases.Add(purchase);
                }
                );*/
            mockRepository.Setup(obj => obj.Update(It.IsAny<Purchase>())).Callback(
                (Purchase purchase) =>
                {
                    mockPurchases.FindAll(x => x.ID == purchase.ID)
                                 .ForEach(x => x.Status = purchase.Status);
                }
                );
            mockRepository.Setup(obj => obj.Delete(It.IsAny<Purchase>())).Callback(
                (Purchase purchase) =>
                {
                    mockPurchases.RemoveAll(x => x.ID == purchase.ID);
                }
                );

            _mockRepository = mockRepository.Object;
            _interactor = new PurchaseInteractor(_mockRepository);
        }

        [Fact]
        public void CreatePurchaseTest()
        {
            var expectedCount = mockPurchases.Count + 1;

            var purchase = new Purchase
            {
                Price = 600,
                Status = (int)PurchaseConfig.Statuses.Active
            };

            _interactor.CreatePurchase(purchase);
            Assert.Equal(expectedCount, mockPurchases.Count);

            var purchasesList = mockPurchases;
            Assert.All(purchasesList, obj => Assert.InRange(obj.ID, low: 1, high: expectedCount));
        }

        [Fact]
        public void InvalidStatusCreatePurchaseTest()
        {
            var purchase = new Purchase
            {
                Price = 600,
                Status = 3
            };

            void action() => _interactor.CreatePurchase(purchase);
            Assert.Throws<PurchaseException>(action);

            var exception = Assert.Throws<PurchaseException>(action);
            Assert.Equal("Purchase: Invalid input of status.", exception.Message);
        }

        [Fact]
        public void AlreadyExistsCreatePurchaseTest()
        {
            void action() => _interactor.CreatePurchase(mockPurchases[0]);
            Assert.Throws<PurchaseException>(action);

            var exception = Assert.Throws<PurchaseException>(action);
            Assert.Equal("Purchase: This purchase already exists.",
                         exception.Message);
        }

        [Fact]
        public void ChangeStatusTest()
        {
            var expectedCount = mockPurchases.Count;

            var purchase = new Purchase
            {
                ID = 3,
                Price = 700,
                Status = (int)PurchaseConfig.Statuses.Canceled
            };

            _interactor.ChangeStatus(purchase);
            Assert.Equal(expectedCount, mockPurchases.Count);

            var purchasesList = mockPurchases;
            Assert.All(purchasesList, obj => Assert.InRange(obj.ID, low: 1, high: expectedCount));

            var updatedPurchase = mockPurchases.Find(x => x.ID == purchase.ID);
            Assert.NotNull(updatedPurchase);
            Assert.Equal(updatedPurchase?.ID, purchase.ID);
            Assert.Equal(updatedPurchase?.Price, purchase.Price);
            Assert.Equal(updatedPurchase?.Status, purchase.Status);
        }

        [Fact]
        public void InvalidStatusChangeStatusTest()
        {
            var purchase = new Purchase
            {
                Price = 600,
                Status = 3
            };

            void action() => _interactor.ChangeStatus(purchase);
            Assert.Throws<PurchaseException>(action);

            var exception = Assert.Throws<PurchaseException>(action);
            Assert.Equal("Purchase: Invalid input of status.", exception.Message);
        }

        [Fact]
        public void NotExistsChangeStatusTest()
        {
            var purchase = new Purchase
            {
                ID = 4,
                Price = 1000,
                Status = (int)PurchaseConfig.Statuses.Active
            };

            void action() => _interactor.ChangeStatus(purchase);
            Assert.Throws<PurchaseException>(action);

            var exception = Assert.Throws<PurchaseException>(action);
            Assert.Equal("Purchase: This purchase doesn't exist.",
                         exception.Message);
        }

        [Fact]
        public void DeletePurchaseTest()
        {
            var expectedCount = mockPurchases.Count - 1;

            var purchase = new Purchase
            {
                ID = 2,
                Price = 600,
                Status = 3
            };

            _interactor.DeletePurchase(purchase);
            Assert.Equal(expectedCount, mockPurchases.Count);

            Assert.Null(mockPurchases.Find(x => x.ID == purchase.ID));
        }

        [Fact]
        public void NotExistsDeletePurchaseTest()
        {
            var purchase = new Purchase
            {
                ID = 4,
                Price = 1000,
                Status = (int)PurchaseConfig.Statuses.Active
            };

            void action() => _interactor.DeletePurchase(purchase);
            Assert.Throws<PurchaseException>(action);

            var exception = Assert.Throws<PurchaseException>(action);
            Assert.Equal("Purchase: This purchase doesn't exist.",
                         exception.Message);
        }
    }
}
