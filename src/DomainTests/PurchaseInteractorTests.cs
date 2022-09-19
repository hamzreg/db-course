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
        private readonly List<Sale> mockSales;
        private readonly List<Wine> mockWines;
        private readonly List<SupplierWine> mockSupplierWine;

        public PurchaseInteractorTests()
        {
            mockPurchases = new List<Purchase>
            {
                new Purchase
                {
                    ID = 1,
                    Price = 500,
                    Status = (int)PurchaseConfig.Statuses.Active,
                    CustomerID = 2
                },
                new Purchase
                {
                    ID = 2,
                    Price = 1000,
                    Status = (int)PurchaseConfig.Statuses.Canceled,
                    CustomerID = 10
                },
                new Purchase
                {
                    ID = 3,
                    Price = 700,
                    Status = (int)PurchaseConfig.Statuses.Active,
                    CustomerID = 10
                },
                new Purchase
                {
                    ID = 4,
                    Price = 300,
                    Status = (int)PurchaseConfig.Statuses.Active,
                    CustomerID = 10
                }
            };

            mockSales = new List<Sale>
            {
                new Sale
                {
                    ID = 1,
                    SellingPrice = 500,
                    PurchaseID = 1,
                    SupplierWineID = 2
                },
                new Sale
                {
                    ID = 2,
                    SellingPrice = 700,
                    PurchaseID = 3,
                    SupplierWineID = 1
                },
                new Sale
                {
                    ID = 3,
                    SellingPrice = 300,
                    PurchaseID = 4,
                    SupplierWineID = 3
                }
            };

            mockWines = new List<Wine>
            {
                new Wine
                {
                    ID = 1,
                    Kind = "lambrusco",
                    Color = "red",
                    Sugar = "dry",
                    Volume = 0.75,
                    Alcohol = 7.5,
                    Aging = 2
                },
                new Wine
                {
                    ID = 2,
                    Kind = "lambrusco",
                    Color = "white",
                    Sugar = "semi-sweet",
                    Volume = 0.75,
                    Alcohol = 7.5,
                    Aging = 2
                },
                new Wine
                {
                    ID = 3,
                    Kind = "lambrusco",
                    Color = "rose",
                    Sugar = "sweet",
                    Volume = 0.75,
                    Alcohol = 7.5,
                    Aging = 2
                }
            };

            mockSupplierWine = new List<SupplierWine>
            {
                new SupplierWine
                {
                    ID = 1,
                    SupplierID = 1,
                    WineID = 1,
                    Percent = 50
                },
                new SupplierWine
                {
                    ID = 2,
                    SupplierID = 2,
                    WineID = 1,
                    Percent = 50
                },
                new SupplierWine
                {
                    ID = 3,
                    SupplierID = 1,
                    WineID = 2,
                    Percent = 50
                }
            };

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
            mockRepository.Setup(obj => obj.GetByCustomerID(It.IsAny<int>())).Returns(
                (int id) =>
                {
                    var purchasesList = mockPurchases.FindAll(x => 
                                        (x.CustomerID == id &&
                                         x.Status == (int)PurchaseConfig.Statuses.Active));

                    var pricesList = new List<double>();
                    var winesList = new List<Wine>();

                    foreach(Purchase purchase in purchasesList)
                    {
                        pricesList.Add(purchase.Price);

                        var sale = mockSales.Find(x => x.PurchaseID == purchase.ID &&
                                                       x.SellingPrice == purchase.Price);
                        var supplierWine = mockSupplierWine.Find(x => x.ID == sale.SupplierWineID);
                        winesList.Add(mockWines.Find(x => x.ID == supplierWine.WineID));
                    }

                    return (winesList, pricesList);
                }
                );
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
                Status = (int)PurchaseConfig.Statuses.Active,
                CustomerID = 4
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
                Status = 3,
                CustomerID = 4
            };

            void action() => _interactor.CreatePurchase(purchase);
            Assert.Throws<PurchaseException>(action);

            var exception = Assert.Throws<PurchaseException>(action);
            Assert.Equal("Purchase: Invalid input of status.", exception.Message);
        }

        [Fact]
        public void AlreadyExistsCreatePurchaseTest()
        {
            var purchase = new Purchase
            {
                ID = 1,
                Price = 500,
                Status = (int)PurchaseConfig.Statuses.Active,
                CustomerID = 2
            };

            void action() => _interactor.CreatePurchase(purchase);
            Assert.Throws<PurchaseException>(action);

            var exception = Assert.Throws<PurchaseException>(action);
            Assert.Equal("Purchase: This purchase already exists.",
                         exception.Message);
        }

        [Fact]
        public void GetByCustomerTest()
        {
            var customer = new Customer
            {
                ID = 10
            };

            var expectedPrices = new List<double> {700, 300};

            var expectedWines = new List<Wine>
            {
                new Wine
                {
                    ID = 1,
                    Kind = "lambrusco",
                    Color = "red",
                    Sugar = "dry",
                    Volume = 0.75,
                    Alcohol = 7.5,
                    Aging = 2
                },
                new Wine
                {
                    ID = 2,
                    Kind = "lambrusco",
                    Color = "white",
                    Sugar = "semi-sweet",
                    Volume = 0.75,
                    Alcohol = 7.5,
                    Aging = 2
                }
            };

            var (ids, dates, wines, prices) = _interactor.GetByCustomer(customer.ID);

            Assert.Equal(expectedWines.Count, wines.Count);
            Assert.Equal(expectedPrices.Count, prices.Count);

            for (int i = 0; i < expectedWines.Count; i++)
            {
                Assert.Equal(expectedWines[i].ID, wines[i].ID);
                Assert.Equal(expectedWines[i].Kind, wines[i].Kind);
                Assert.Equal(expectedWines[i].Color, wines[i].Color);
                Assert.Equal(expectedWines[i].Sugar, wines[i].Sugar);
                Assert.Equal(expectedWines[i].Volume, wines[i].Volume);
                Assert.Equal(expectedWines[i].Alcohol, wines[i].Alcohol);
                Assert.Equal(expectedWines[i].Aging, wines[i].Aging);

                Assert.Equal(expectedPrices[i], prices[i]);
            }
        }

        [Fact]
        public void ChangeStatusTest()
        {
            var expectedCount = mockPurchases.Count;

            var purchase = new Purchase
            {
                ID = 3,
                Price = 700,
                Status = (int)PurchaseConfig.Statuses.Canceled,
                CustomerID = 10
            };

            _interactor.ChangeStatus(purchase);
            Assert.Equal(expectedCount, mockPurchases.Count);

            var purchasesList = mockPurchases;
            Assert.All(purchasesList, obj => Assert.InRange(obj.ID, low: 1, high: expectedCount));

            var updatedPurchase = mockPurchases.Find(x => x.ID == purchase.ID);
            Assert.NotNull(updatedPurchase);
            Assert.Equal(purchase.ID, updatedPurchase?.ID);
            Assert.Equal(purchase.Price, updatedPurchase?.Price);
            Assert.Equal(purchase.Status, updatedPurchase?.Status);
        }

        [Fact]
        public void InvalidStatusChangeStatusTest()
        {
            var purchase = new Purchase
            {
                ID = 3,
                Price = 700,
                Status = 3,
                CustomerID = 10
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
                ID = 5,
                Price = 1000,
                Status = (int)PurchaseConfig.Statuses.Active,
                CustomerID = 4
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
                Price = 1000,
                Status = (int)PurchaseConfig.Statuses.Canceled,
                CustomerID = 10
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
                ID = 5,
                Price = 1000,
                Status = (int)PurchaseConfig.Statuses.Active,
                CustomerID = 4
            };

            void action() => _interactor.DeletePurchase(purchase);
            Assert.Throws<PurchaseException>(action);

            var exception = Assert.Throws<PurchaseException>(action);
            Assert.Equal("Purchase: This purchase doesn't exist.",
                         exception.Message);
        }
    }
}
