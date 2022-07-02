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
        private readonly List<SupplierWine> mockSupplierWine;

        public SaleInteractorTests()
        {
            mockSales = new List<Sale>
            {
                new Sale
                {
                    ID = 1,
                    PurchasePrice = 500,
                    SupplierWineID = 2
                },
                new Sale
                {
                    ID = 2,
                    PurchasePrice = 700,
                    SupplierWineID = 1
                },
                new Sale
                {
                    ID = 3,
                    PurchasePrice = 300,
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

                    foreach(SupplierWine supplierWine in mockSupplierWine)
                    {
                        if (supplierWine.SupplierID == id)
                        {
                            var wine = mockWines.Find(x => x.ID == supplierWine.WineID);
                            winesList.Add(wine);
                            
                            var sale = mockSales.Find(x => x.SupplierWineID == supplierWine.ID);
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

                    foreach (Sale sale in mockSales)
                    {
                        var supplierWine = mockSupplierWine.Find(x => x.ID == sale.SupplierWineID);

                        winesList.Add(mockWines.Find(x => x.ID == supplierWine.WineID));

                        var supplier = mockSuppliers.Find(x => x.ID == supplierWine.SupplierID);
                        supplierList.Add(supplier.Name);
                    }

                    return (winesList, supplierList, mockSales);
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
                SupplierWineID = 5
            };

            int percent = SaleConfig.MinPercent + 7;

            _interactor.CreateSale(sale, percent);
            Assert.Equal(expectedCount, mockSales.Count);

            var salesList = mockSales;
            Assert.All(salesList, obj => Assert.InRange(obj.ID, low: 1, high: expectedCount));

            var newSale = _mockRepository.GetByID(sale.ID);

            double expectedSellingPrice = sale.PurchasePrice * (1 + percent / 100);
            Assert.Equal(expectedSellingPrice, newSale.SellingPrice);

            double expectedMargin = expectedSellingPrice - sale.PurchasePrice;
            Assert.Equal(expectedMargin, newSale.Margin);

            double expectedProfit = (expectedMargin - sale.Costs) * sale.WineNumber;
            Assert.Equal(expectedProfit, newSale.Profit);
        }

        [Fact]
        public void InvalidPercentCreateSaleTest()
        {
            var sale = new Sale
            {
                PurchasePrice = SaleConfig.MinPurchasePrice + 100,
                Costs = SaleConfig.MinCosts + 2,
                WineNumber = SaleConfig.MinWineNumer,
                SupplierWineID = 5
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
                SupplierWineID = 5
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
                SupplierWineID = 5
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
                SupplierWineID = 5
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
                PurchasePrice = 500,
                Costs = SaleConfig.MinCosts + 2,
                WineNumber = SaleConfig.MinWineNumer,
                SupplierWineID = 2
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
            int supplierID = 1;

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

            var expectedCount = expectedPrices.Count();

            var (wines, prices) = _interactor.GetBySupplierID(supplierID);

            Assert.Equal(expectedCount, wines.Count);
            Assert.Equal(expectedCount, prices.Count);

            for (int i = 0; i < expectedWines.Count; i++)
            {
                Assert.Equal(wines[i].ID, expectedWines[i].ID);
                Assert.Equal(wines[i].Kind, expectedWines[i].Kind);
                Assert.Equal(wines[i].Color, expectedWines[i].Color);
                Assert.Equal(wines[i].Sugar, expectedWines[i].Sugar);
                Assert.Equal(wines[i].Volume, expectedWines[i].Volume);
                Assert.Equal(wines[i].Alcohol, expectedWines[i].Alcohol);
                Assert.Equal(wines[i].Aging, expectedWines[i].Aging);
            }

            for (int i = 0; i < expectedPrices.Count; i++)
                Assert.Equal(prices[i], expectedPrices[i]);
        }

        [Fact]
        public void GetByAdminIDTest()
        {
            int adminID = 1;

            var expectedCount = mockSales.Count;

            var expectedSales = mockSales;
            var expectedSuppliers = new List<string> {"Agora", "Fanagoria", "Fanagoria"};
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

            var (wines, suppliers, sales) = _interactor.GetByAdminID(adminID);

            Assert.Equal(expectedCount, wines.Count);
            Assert.Equal(expectedCount, suppliers.Count);
            Assert.Equal(expectedCount, sales.Count);

            for (int i = 0; i < expectedWines.Count; i++)
            {
                Assert.Equal(wines[i].ID, expectedWines[i].ID);
                Assert.Equal(wines[i].Kind, expectedWines[i].Kind);
                Assert.Equal(wines[i].Color, expectedWines[i].Color);
                Assert.Equal(wines[i].Sugar, expectedWines[i].Sugar);
                Assert.Equal(wines[i].Volume, expectedWines[i].Volume);
                Assert.Equal(wines[i].Alcohol, expectedWines[i].Alcohol);
                Assert.Equal(wines[i].Aging, expectedWines[i].Aging);
            }

            for (int i = 0; i < expectedSuppliers.Count; i++)
                Assert.Equal(suppliers[i], expectedSuppliers[i]);

            for (int i = 0; i < expectedSales.Count; i++)
                Assert.Equal(sales[i], expectedSales[i]);
        }

        [Fact]
        public void DeleteSaleTest()
        {
            var expectedCount = mockSales.Count - 1;

            var sale = new Sale
            {
                ID = 1,
                PurchasePrice = 500,
                SupplierWineID = 2
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
                SupplierWineID = 2
            };

            void action() => _interactor.DeleteSale(sale);
            Assert.Throws<SaleException>(action);

            var exception = Assert.Throws<SaleException>(action);
            Assert.Equal("Sale: This sale doesn't exist.",
                         exception.Message);
        }
    }
}
