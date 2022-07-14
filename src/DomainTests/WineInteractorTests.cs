using Xunit;
using Moq;

using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Interactors;
using WineSales.Domain.Exceptions;
using WineSales.Config;

namespace DomainTests
{
    public class WineInteractorTests
    {
        private readonly IWineInteractor _interactor;
        private readonly IWineRepository _mockRepository;

        private readonly List<Wine> mockWines;

        public WineInteractorTests()
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
                    Number = WineConfig.MinNumber + 1
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

            var mockRepository = new Mock<IWineRepository>();
            mockRepository.Setup(obj => obj.GetByID(It.IsAny<int>())).Returns(
                (int id) => mockWines.Find(x => x.ID == id));
            mockRepository.Setup(obj => obj.GetAll()).Returns(mockWines);
            mockRepository.Setup(obj => obj.Create(It.IsAny<Wine>())).Callback(
                (Wine wine) =>
                {
                    wine.ID = mockWines.Count + 1;
                    mockWines.Add(wine);
                }
                );
            mockRepository.Setup(obj => obj.Delete(It.IsAny<Wine>())).Callback(
                (Wine wine) =>
                {
                    mockWines.RemoveAll(x => x.ID == wine.ID);
                }
                );

            _mockRepository = mockRepository.Object;
            _interactor = new WineInteractor(_mockRepository);
        }

        [Fact]
        public void CreateWineTest()
        {
            var expectedCount = mockWines.Count + 1;

            var wine = new Wine
            {
                Kind = "lambrusco",
                Color = "white",
                Sugar = "semi-sweet",
                Volume = 1.5,
                Alcohol = 7.5,
                Aging = 2
            };

            _interactor.CreateWine(wine);
            Assert.Equal(expectedCount, mockWines.Count);

            var winesList = mockWines;
            Assert.All(winesList, obj => Assert.InRange(obj.ID, low: 1, high: expectedCount));

            var newWine = _mockRepository.GetByID(mockWines.Count);
            Assert.Equal(WineConfig.MinNumber, newWine.Number);
        }

        [Fact]
        public void AlreadyExistsCreateWineTest()
        {
            var expectedCount = mockWines.Count;

            var wine = new Wine
            {
                ID = 2,
                Kind = "lambrusco",
                Color = "white",
                Sugar = "semi-sweet",
                Volume = 0.75,
                Alcohol = 7.5,
                Aging = 2
            };

            _interactor.CreateWine(wine);
            Assert.Equal(expectedCount, mockWines.Count);

            var newWine = _mockRepository.GetByID(wine.ID);
            Assert.Equal(WineConfig.MinNumber + 2, newWine.Number);
        }

        [Fact]
        public void InvalidInputCreateWineTest()
        {
            var wine = new Wine
            {
                Kind = "lambrusco",
                Color = "black",
                Sugar = "semi-sweet",
                Volume = 0.75,
                Alcohol = 7.5,
                Aging = 2
            };

            void action() => _interactor.CreateWine(wine);
            Assert.Throws<WineException>(action);

            var exception = Assert.Throws<WineException>(action);
            Assert.Equal("Wine: Invalid input of wine.", exception.Message);
        }

        [Fact]
        public void OneExistsDeleteWineTest()
        {
            var expectedCount = mockWines.Count - 1;

            var wine = new Wine
            {
                ID = 2,
                Kind = "lambrusco",
                Color = "white",
                Sugar = "semi-sweet",
                Volume = 0.75,
                Alcohol = 7.5,
                Aging = 2
            };

            _interactor.DeleteWine(wine);
            Assert.Equal(expectedCount, mockWines.Count);

            Assert.Null(mockWines.Find(x => x.ID == wine.ID));
        }

        [Fact]
        public void SeveralExistDeleteWineTest()
        {
            var expectedCount = mockWines.Count;

            var wine = new Wine
            {
                ID = 2,
                Kind = "lambrusco",
                Color = "white",
                Sugar = "semi-sweet",
                Volume = 0.75,
                Alcohol = 7.5,
                Aging = 2,
                Number = WineConfig.MinNumber + 1
            };

            _interactor.DeleteWine(wine);
            Assert.Equal(expectedCount, mockWines.Count);

            var newWine = _mockRepository.GetByID(wine.ID);
            Assert.Equal(WineConfig.MinNumber, newWine.Number);
        }

        [Fact]
        public void NotExistDeleteWineTest()
        {
            var expectedCount = mockWines.Count;

            var wine = new Wine
            {
                ID = 5,
                Kind = "lambrusco",
                Color = "white",
                Sugar = "semi-sweet",
                Volume = 0.75,
                Alcohol = 7.5,
                Aging = 2
            };

            void action() => _interactor.DeleteWine(wine);
            Assert.Throws<WineException>(action);

            var exception = Assert.Throws<WineException>(action);
            Assert.Equal("Wine: This wine doesn't exist.", exception.Message);
        }

        [Fact]
        public void InvalidInputDeleteWineTest()
        {
            var wine = new Wine
            {
                ID = 2,
                Kind = "lambrusco",
                Color = "white",
                Sugar = "semi-sweet",
                Volume = 0.1,
                Alcohol = 7.5,
                Aging = 2
            };

            void action() => _interactor.DeleteWine(wine);
            Assert.Throws<WineException>(action);

            var exception = Assert.Throws<WineException>(action);
            Assert.Equal("Wine: Invalid input of wine.", exception.Message);
        }
    }
}
