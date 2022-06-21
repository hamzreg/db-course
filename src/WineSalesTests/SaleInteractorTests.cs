using Xunit;
using Moq;

using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Interactors;
using WineSales.Domain.Exceptions;
using WineSales.Config;

namespace DomainTests
{
    public class SaleInteractorTests
    {
        private readonly ISaleInteractor _interactor;
        private readonly ISaleRepository _mockRepository;

        private readonly List<Sale> mockSales;
        private readonly List<Wine> mockWines;
        private readonly List<Supplier> mockSuppliers;
        private readonly List<Tuple<int, int>> mockSupplierWine;

        public SaleInteractorTests()
        {
            mockSales = new List<Sale>
            {
                new Sale
                {
                    ID = 1,
                    PurchasePrice = 500,
                    WineID = 2
                },
                new Sale
                {
                    ID = 2,
                    PurchasePrice = 700,
                    WineID = 1
                },
                new Sale
                {
                    ID = 3,
                    PurchasePrice = 300,
                    WineID = 3
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

            mockSuppliers = new List<Supplier>
            {
                new Supplier
                {
                    ID = 1,
                    Name = "Fanagoria"
                },
                new Supplier
                {
                    ID = 2,
                    Name = "Agora"
                }
            };

            mockSupplierWine = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(1, 2),
                new Tuple<int, int>(1, 3),
                new Tuple<int, int>(2, 1)
            };

            var mockRepository = new Mock<ISaleRepository>();
            mockRepository.Setup(obj => obj.GetByID(It.IsAny<int>())).Returns(
                (int id) => mockSales.Find(x => x.ID == id));
            mockRepository.Setup(obj => obj.Create(It.IsAny<Sale>())).Callback(
                (Sale sale) =>
                {
                    sale.ID = mockSales.Count + 1;
                    mockSales.Add(sale);
                }
                );
            mockRepository.Setup(obj => obj.GetBySupplierID(It.IsAny<int>())).Returns(
                (int id) =>
                {
                    var winesList = new List<Wine>();
                    var pricesList = new List<double>();

                    for (int i = 0; i < mockSupplierWine.Count; i++)
                    {
                        if (mockSupplierWine[i].Item1 == id)
                        {
                            var wine = mockWines.Find(x => x.ID == mockSupplierWine[i].Item2);
                            winesList.Add(wine);
                            
                            var sale = mockSales.Find(x => x.WineID == wine.ID);
                            pricesList.Add(sale.PurchasePrice);
                        }

                    }

                    return (winesList, pricesList);
                }
                );
            mockRepository.Setup(obj => obj.GetByAdminID(It.IsAny<int>())).Returns(
                (int id) =>
                { 
                    var winesList = new List<Wine>();
                    var supplierList = new List<string>();
                    var salesList = mockSales;

                    for (int i = 0; i < mockSales.Count; i++)
                    {
                        var wine = mockWines.Find(x => x.ID == mockSales[i].WineID);
                        winesList.Add(wine);

                        var supplierWine = mockSupplierWine.Find(x => x.Item2 == wine.ID);
                        var supplier = mockSuppliers.Find(x => x.ID == supplierWine.Item1);
                        supplierList.Add(supplier.Name);
                    }

                    return (winesList, supplierList, salesList);
                }
                );
            mockRepository.Setup(obj => obj.Delete(It.IsAny<Sale>())).Callback(
                (Sale sale) =>
                {
                    mockSales.RemoveAll(x => x.ID == sale.ID);
                }
                );

            _mockRepository = mockRepository.Object;
            _interactor = new SaleInteractor(_mockRepository);
        }

        [Fact]
        public void CreateSaleTest()
        {
            var expectedCount = mockSales.Count + 1;

            var sale = new Sale
            {
                PurchasePrice = SaleConfig.MinPurchasePrice + 100,
                Costs = SaleConfig.MinCosts + 2,
                WineNumber = SaleConfig.MinWineNumer,
                WineID = 5
            };

            _interactor.CreateSale(sale, SaleConfig.MinPercent + 2);
            Assert.Equal(expectedCount, mockSales.Count);

            var salesList = mockSales;
            Assert.All(salesList, obj => Assert.InRange(obj.ID, low: 1, high: expectedCount));
        }

        [Fact]
        public void InvalidPercentCreateSaleTest()
        {
            var sale = new Sale
            {
                PurchasePrice = SaleConfig.MinPurchasePrice + 100,
                Costs = SaleConfig.MinCosts + 2,
                WineNumber = SaleConfig.MinWineNumer,
                WineID = 5
            };

            void action() => _interactor.CreateSale(sale, SaleConfig.MinPercent - 2);
            Assert.Throws<SaleException>(action);

            var exception = Assert.Throws<SaleException>(action);
            Assert.Equal("Sale: Invalid input of percent.", exception.Message);
        }

        [Fact]
        public void InvalidPurchasePriceCreateSaleTest()
        {
            var sale = new Sale
            {
                PurchasePrice = SaleConfig.MinPurchasePrice - 2,
                Costs = SaleConfig.MinCosts + 2,
                WineNumber = SaleConfig.MinWineNumer,
                WineID = 5
            };

            void action() => _interactor.CreateSale(sale, SaleConfig.MinPercent + 2);
            Assert.Throws<SaleException>(action);

            var exception = Assert.Throws<SaleException>(action);
            Assert.Equal("Sale: Invalid input of purchase price.", exception.Message);
        }

        [Fact]
        public void InvalidCostsCreateSaleTest()
        {
            var sale = new Sale
            {
                PurchasePrice = SaleConfig.MinPurchasePrice + 2,
                Costs = SaleConfig.MinCosts - 2,
                WineNumber = SaleConfig.MinWineNumer,
                WineID = 5
            };

            void action() => _interactor.CreateSale(sale, SaleConfig.MinPercent + 2);
            Assert.Throws<SaleException>(action);

            var exception = Assert.Throws<SaleException>(action);
            Assert.Equal("Sale: Invalid input of costs.", exception.Message);
        }

        [Fact]
        public void InvalidWineNumberCreateSaleTest()
        {
            var sale = new Sale
            {
                PurchasePrice = SaleConfig.MinPurchasePrice + 2,
                Costs = SaleConfig.MinCosts + 2,
                WineNumber = SaleConfig.MinWineNumer - 1,
                WineID = 5
            };

            void action() => _interactor.CreateSale(sale, SaleConfig.MinPercent + 2);
            Assert.Throws<SaleException>(action);

            var exception = Assert.Throws<SaleException>(action);
            Assert.Equal("Sale: Invalid input of wine number.", exception.Message);
        }

        [Fact]
        public void AlreadyExistsCreateSaleTest()
        {
            var sale = new Sale
            {
                ID = 1,
                PurchasePrice = SaleConfig.MinPurchasePrice + 100,
                Costs = SaleConfig.MinCosts + 2,
                WineNumber = SaleConfig.MinWineNumer,
                WineID = 1
            };

            void action() => _interactor.CreateSale(sale, SaleConfig.MinPercent);
            Assert.Throws<SaleException>(action);

            var exception = Assert.Throws<SaleException>(action);
            Assert.Equal("Sale: This sale already exists.",
                         exception.Message);
        }


        [Fact]
        public void GetBySupplierIDTest()
        {
            var supplier = new Supplier
            {
                ID = mockSuppliers[0].ID
            };

            var supplierWine = mockSupplierWine.FindAll(x => x.Item1 == supplier.ID);
            var expectedCount = supplierWine.Count;

            var expectedPrices = new List<double>();
            var expectedWines = new List<Wine>();

            for (int i = 0; i < supplierWine.Count; i++)
            {
                var wine = mockWines.Find(x => x.ID == mockSupplierWine[i].Item2);
                expectedWines.Add(wine);

                var sale = mockSales.Find(x => x.WineID == wine.ID);
                expectedPrices.Add(sale.PurchasePrice);
            }

            var (wines, prices) = _interactor.GetBySupplierID(supplier.ID);

            Assert.Equal(expectedCount, wines.Count);
            Assert.Equal(expectedCount, prices.Count);

            Assert.Equal(expectedWines, wines);
            Assert.Equal(expectedPrices, prices);
        }

        [Fact]
        public void GetByAdminIDTest()
        {
            int adminID = 1;

            var expectedCount = mockSales.Count;

            var expectedSales = mockSales;
            var expectedSuppliers = new List<string>();
            var expectedWines = new List<Wine>();

            for (int i = 0; i < mockSales.Count; i++)
            {
                var wine = mockWines.Find(x => x.ID == mockSales[i].WineID);
                expectedWines.Add(wine);

                var supplierWine = mockSupplierWine.Find(x => x.Item2 == wine.ID);
                var supplier = mockSuppliers.Find(x => x.ID == supplierWine.Item1);
                expectedSuppliers.Add(supplier.Name);
            }

            var (wines, suppliers, sales) = _interactor.GetByAdminID(adminID);

            Assert.Equal(expectedCount, wines.Count);
            Assert.Equal(expectedCount, suppliers.Count);
            Assert.Equal(expectedCount, sales.Count);

            Assert.Equal(expectedWines, wines);
            Assert.Equal(expectedSuppliers, suppliers);
            Assert.Equal(expectedSales, sales);
        }

        [Fact]
        public void DeleteSaleTest()
        {
            var expectedCount = mockSales.Count - 1;

            var sale = new Sale
            {
                ID = 1,
                PurchasePrice = 500,
                Costs = SaleConfig.MinCosts + 2,
                WineNumber = SaleConfig.MinWineNumer,
                WineID = 2
            };

            _interactor.DeleteSale(sale);
            Assert.Equal(expectedCount, mockSales.Count);

            Assert.Null(mockSales.Find(x => x.ID == sale.ID));
        }

        [Fact]
        public void NotExistsDeleteSaleTest()
        {
            var sale = new Sale
            {
                ID = 6,
                PurchasePrice = 500,
                Costs = SaleConfig.MinCosts + 2,
                WineNumber = SaleConfig.MinWineNumer,
                WineID = 2
            };

            void action() => _interactor.DeleteSale(sale);
            Assert.Throws<SaleException>(action);

            var exception = Assert.Throws<SaleException>(action);
            Assert.Equal("Sale: This sale doesn't exist.",
                         exception.Message);
        }
    }
}
