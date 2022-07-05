using Xunit;
using Moq;

using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Interactors;
using WineSales.Domain.Exceptions;
using WineSales.Config;

namespace DomainTests
{
    public class SupplierWineInteractorTests
    {
        private readonly ISupplierWineInteractor _interactor;
        private readonly ISupplierWineRepository _mockRepository;

        private readonly List<SupplierWine> mockSupplierWine;
        private readonly List<Wine> mockWines;
        private readonly List<Supplier> mockSuppliers;

        public SupplierWineInteractorTests()
        {
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
                    Aging = 2,
                    Number = 1
                },
                new Wine
                {
                    ID = 2,
                    Kind = "lambrusco",
                    Color = "white",
                    Sugar = "semi-sweet",
                    Volume = 0.75,
                    Alcohol = 7.5,
                    Aging = 2,
                    Number = 1
                },
                new Wine
                {
                    ID = 3,
                    Kind = "lambrusco",
                    Color = "rose",
                    Sugar = "sweet",
                    Volume = 0.75,
                    Alcohol = 7.5,
                    Aging = 2,
                    Number = 1
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
                    Price = 700,
                    Percent = 50,
                    Rating = 5
                },
                new SupplierWine
                {
                    ID = 2,
                    SupplierID = 2,
                    WineID = 1,
                    Price = 500,
                    Percent = 50,
                    Rating = 8
                },
                new SupplierWine
                {
                    ID = 3,
                    SupplierID = 1,
                    WineID = 2,
                    Price = 300,
                    Percent = 50,
                    Rating = 6
                }
            };

            var mockRepository = new Mock<ISupplierWineRepository>();
            mockRepository.Setup(obj => obj.GetByID(It.IsAny<int>())).Returns(
                (int id) => mockSupplierWine.Find(x => x.ID == id));
            mockRepository.Setup(obj => obj.GetAll()).Returns(mockSupplierWine);
            mockRepository.Setup(obj => obj.Create(It.IsAny<SupplierWine>())).Callback(
                (SupplierWine supplierWine) =>
                {
                    supplierWine.ID = mockSupplierWine.Count + 1;
                    mockSupplierWine.Add(supplierWine);
                }
                );
            mockRepository.Setup(obj => obj.GetBySupplierID(It.IsAny<int>())).Returns(
                (int id) =>
                {
                    var winesList = new List<Wine>();
                    var pricesList = new List<double>();

                    foreach (SupplierWine wine in mockSupplierWine)
                    {
                        if (wine.SupplierID == id)
                        {
                            winesList.Add(mockWines.Find(x => x.ID == wine.WineID));
                            pricesList.Add(wine.Price);
                        }
                    }

                    return (winesList, pricesList);
                }
                );
            mockRepository.Setup(obj => obj.GetAllWine()).Returns(
                () =>
                {
                    var winesList = new List<Wine>();
                    var pricesList = new List<double>();

                    foreach (SupplierWine wine in mockSupplierWine)
                    {
                        winesList.Add(mockWines.Find(x => x.ID == wine.WineID));
                        pricesList.Add(wine.Price * (1 + wine.Percent / 100.0));
                    }

                    return (winesList, pricesList);
                }
                );
            mockRepository.Setup(obj => obj.GetByAdminID(It.IsAny<int>())).Returns(
                (int id) =>
                {
                    var winesList = new List<Wine>();
                    var suppliersList = new List<string>();
                    var pricesList = new List<double>();

                    foreach (SupplierWine wine in mockSupplierWine)
                    {
                        winesList.Add(mockWines.Find(x => x.ID == wine.WineID));

                        var supplier = mockSuppliers.Find(x => x.ID == wine.SupplierID);
                        suppliersList.Add(supplier.Name);

                        pricesList.Add(wine.Price * (1 + wine.Percent / 100.0));
                    }

                    return (winesList, suppliersList, pricesList);
                }
                );
            mockRepository.Setup(obj => obj.GetRating()).Returns(
                () =>
                {
                    var winesList = new List<Wine>();
                    var pointsList = new List<double>();

                    mockSupplierWine.Sort((a, b) => a.Rating.CompareTo(b.Rating));
                    mockSupplierWine.Reverse();

                    foreach (SupplierWine wine in mockSupplierWine)
                    {
                        winesList.Add(mockWines.Find(x => x.ID == wine.WineID));
                        pointsList.Add(wine.Rating);
                    }

                    return (winesList, pointsList);
                }
                );
            mockRepository.Setup(obj => obj.Update(It.IsAny<SupplierWine>())).Callback(
                (SupplierWine supplierWine) =>
                {
                    mockSupplierWine.FindAll(x => x.ID == supplierWine.ID)
                                    .ForEach(x =>
                                    {
                                        x.Price = supplierWine.Price;
                                        x.Percent = supplierWine.Percent;
                                        x.Rating = supplierWine.Rating;
                                    });
                }
                );
            mockRepository.Setup(obj => obj.Delete(It.IsAny<SupplierWine>())).Callback(
                (SupplierWine supplierWine) =>
                {
                    mockSupplierWine.RemoveAll(x => x.ID == supplierWine.ID);
                }
                );

            _mockRepository = mockRepository.Object;
            _interactor = new SupplierWineInteractor(_mockRepository);
        }

        [Fact]
        public void CreateSupplierWineTest()
        {
            var expectedCount = mockSupplierWine.Count + 1;

            var supplierWine = new SupplierWine
            {
                SupplierID = 2,
                WineID = 2,
                Price = 300,
                Percent = 50,
                Rating = 8
            };

            var supplierWineList = mockSupplierWine;
            Assert.All(supplierWineList, obj => Assert.InRange(obj.ID, low: 1, high: expectedCount));

            _interactor.CreateSupplierWine(supplierWine);
            Assert.Equal(expectedCount, mockSupplierWine.Count);

            var newWine = _mockRepository.GetByID(expectedCount);
            Assert.Equal(WineConfig.MinRating, newWine.Rating);
        }

        [Fact]
        public void AlreadyExistsCreateSupplierWineTest()
        {
            var supplierWine = new SupplierWine
            {
                SupplierID = 2,
                WineID = 1,
                Price = 500,
                Percent = 50,
                Rating = 8
            };

            void action() => _interactor.CreateSupplierWine(supplierWine);
            Assert.Throws<SupplierWineException>(action);

            var exception = Assert.Throws<SupplierWineException>(action);
            Assert.Equal("SupplierWine: This supplier already has this wine.", exception.Message);
        }

        [Fact]
        public void InvalidInputCreateSupplierWineTest()
        {
            var supplierWine = new SupplierWine
            {
                SupplierID = 2,
                WineID = 2,
                Price = 300,
                Percent = SaleConfig.MinPercent - 2,
                Rating = 6
            };

            void action() => _interactor.CreateSupplierWine(supplierWine);
            Assert.Throws<SupplierWineException>(action);

            var exception = Assert.Throws<SupplierWineException>(action);
            Assert.Equal("SupplierWine: Invalid input of supplierWine.", exception.Message);
        }

        [Fact]
        public void GetBySupplierIDTest()
        {
            int supplierID = 1;

            var expectedPrices = new List<double> { 700, 300 };
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

            var (wines, prices) = _interactor.GetBySupplierID(supplierID);

            var expectedCount = expectedPrices.Count;
            Assert.Equal(expectedCount, wines.Count);
            Assert.Equal(expectedCount, prices.Count);

            for (int i = 0; i < expectedCount; i++)
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
        public void GetRatingTest()
        {
            var expectedPoints = new List<double> { 8, 6, 5 };
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
                }
            };

            var (wines, points) = _interactor.GetRating();

            var expectedCount = expectedPoints.Count;
            Assert.Equal(expectedCount, wines.Count);
            Assert.Equal(expectedCount, points.Count);

            for (int i = 0; i < expectedCount; i++)
            {
                Assert.Equal(expectedWines[i].ID, wines[i].ID);
                Assert.Equal(expectedWines[i].Kind, wines[i].Kind);
                Assert.Equal(expectedWines[i].Color, wines[i].Color);
                Assert.Equal(expectedWines[i].Sugar, wines[i].Sugar);
                Assert.Equal(expectedWines[i].Volume, wines[i].Volume);
                Assert.Equal(expectedWines[i].Alcohol, wines[i].Alcohol);
                Assert.Equal(expectedWines[i].Aging, wines[i].Aging);

                Assert.Equal(expectedPoints[i], points[i]);
            }
        }

        [Fact]
        public void GetByAdminIDTest()
        {
            int adminID = 1;

            var expectedPrices = new List<double> { 1050, 750, 450 };
            var expectedSuppliers = new List<string> { "Fanagoria", "Agora", "Fanagoria" };
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

            var (wines, suppliers, prices) = _interactor.GetByAdminID(adminID);

            var expectedCount = expectedPrices.Count;
            Assert.Equal(expectedCount, wines.Count);
            Assert.Equal(expectedCount, suppliers.Count);
            Assert.Equal(expectedCount, prices.Count);

            for (int i = 0; i < expectedCount; i++)
            {
                Assert.Equal(expectedWines[i].ID, wines[i].ID);
                Assert.Equal(expectedWines[i].Kind, wines[i].Kind);
                Assert.Equal(expectedWines[i].Color, wines[i].Color);
                Assert.Equal(expectedWines[i].Sugar, wines[i].Sugar);
                Assert.Equal(expectedWines[i].Volume, wines[i].Volume);
                Assert.Equal(expectedWines[i].Alcohol, wines[i].Alcohol);
                Assert.Equal(expectedWines[i].Aging, wines[i].Aging);

                Assert.Equal(expectedSuppliers[i], suppliers[i]);

                Assert.Equal(expectedPrices[i], prices[i]);
            }
        }

        [Fact]
        public void GetAllWineTest()
        {
            var expectedPrices = new List<double> { 1050, 750, 450 };
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

            var (wines, prices) = _interactor.GetAllWine();

            var expectedCount = expectedPrices.Count;
            Assert.Equal(expectedCount, wines.Count);
            Assert.Equal(expectedCount, prices.Count);

            for (int i = 0; i < expectedCount; i++)
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
        public void UpdateSupplierWineTest()
        {
            var expectedCount = mockSupplierWine.Count;

            var supplierWine = new SupplierWine
            {
                ID = 2,
                SupplierID = 2,
                WineID = 1,
                Price = 600,
                Percent = 45,
                Rating = 6
            };

            _interactor.UpdateSupplierWine(supplierWine);
            Assert.Equal(expectedCount, mockSupplierWine.Count);

            var updatedWine = mockSupplierWine.Find(x => x.ID == supplierWine.ID);
            Assert.NotNull(updatedWine);
            Assert.Equal(supplierWine.Price, updatedWine?.Price);
            Assert.Equal(supplierWine.Percent, updatedWine?.Percent);
            Assert.Equal(supplierWine.Rating, updatedWine?.Rating);
        }

        [Fact]
        public void NotExistUpdateSupplierWineTest()
        {
            var supplierWine = new SupplierWine
            {
                ID = 4,
                SupplierID = 2,
                WineID = 1,
                Price = 500,
                Percent = 50,
                Rating = 8
            };

            void action() => _interactor.UpdateSupplierWine(supplierWine);
            Assert.Throws<SupplierWineException>(action);

            var exception = Assert.Throws<SupplierWineException>(action);
            Assert.Equal("SupplierWine: This supplier doesn't have this wine.", exception.Message);
        }

        [Fact]
        public void InvalidInputUpdateSupplierWineTest()
        {
            var supplierWine = new SupplierWine
            {
                ID = 2,
                SupplierID = 2,
                WineID = 1,
                Price = 500,
                Percent = SaleConfig.MinPercent - 2,
                Rating = 8
            };

            void action() => _interactor.UpdateSupplierWine(supplierWine);
            Assert.Throws<SupplierWineException>(action);

            var exception = Assert.Throws<SupplierWineException>(action);
            Assert.Equal("SupplierWine: Invalid input of supplierWine.", exception.Message);
        }

        [Fact]
        public void DeleteSupplierWineTest()
        {
            var expectedCount = mockSupplierWine.Count - 1;

            var supplierWine = new SupplierWine
            {
                ID = 2,
                SupplierID = 2,
                WineID = 1,
                Price = 500,
                Percent = 50,
                Rating = 8
            };

            _interactor.DeleteSupplierWine(supplierWine);
            Assert.Equal(expectedCount, mockSupplierWine.Count);

            Assert.Null(mockSupplierWine.Find(x => x.ID == supplierWine.ID));
        }

        [Fact]
        public void NotExistDeleteSupplierWineTest()
        {
            var supplierWine = new SupplierWine
            {
                ID = 4,
                SupplierID = 2,
                WineID = 1,
                Price = 500,
                Percent = 50,
                Rating = 8
            };

            void action() => _interactor.DeleteSupplierWine(supplierWine);
            Assert.Throws<SupplierWineException>(action);

            var exception = Assert.Throws<SupplierWineException>(action);
            Assert.Equal("SupplierWine: This supplier doesn't have this wine.", exception.Message);
        }

        [Fact]
        public void InvalidInputDeleteSupplierWineTest()
        {
            var supplierWine = new SupplierWine
            {
                ID = 2,
                SupplierID = 2,
                WineID = 1,
                Price = SaleConfig.MinPurchasePrice - 2,
                Percent = 50,
                Rating = 8
            };

            void action() => _interactor.DeleteSupplierWine(supplierWine);
            Assert.Throws<SupplierWineException>(action);

            var exception = Assert.Throws<SupplierWineException>(action);
            Assert.Equal("SupplierWine: Invalid input of supplierWine.", exception.Message);
        }
    }
}
