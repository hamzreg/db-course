using Xunit;
using Moq;

using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Interactors;
using WineSales.Domain.Exceptions;

namespace DomainTests
{
    public class BonusCardInteractorTests
    {
        private readonly IBonusCardInteractor _interactor;
        private readonly IBonusCardRepository _mockRepository;

        private readonly List<BonusCard> mockBonusCards;

        public BonusCardInteractorTests()
        {
            mockBonusCards = new List<BonusCard>
            {
                new BonusCard
                {
                    ID = 1,
                    Bonuses = 0,
                    Phone = "88005553535"
                },
                new BonusCard
                {
                    ID = 2,
                    Bonuses = 100,
                    Phone = "89005553535"
                },
                new BonusCard
                {
                    ID = 3,
                    Bonuses = 50,
                    Phone = "81005553535"
                }
            };

            var mockRepository = new Mock<IBonusCardRepository>();
            mockRepository.Setup(obj => obj.GetByPhone(It.IsAny<string>())).Returns(
                (string phone) => mockBonusCards.Find(x => x.Phone == phone));
            mockRepository.Setup(obj => obj.AddByPhone(It.IsAny<string>())).Callback(
                (string phone) =>
                {
                    var bonusCard = new BonusCard
                    {
                        ID = mockBonusCards.Count + 1,
                        Bonuses = 0,
                        Phone = phone
                    };

                    mockBonusCards.Add(bonusCard);
                }
                );
            mockRepository.Setup(obj => obj.GetBonuses(It.IsAny<string>())).Returns(
                (string phone) =>
                {
                    var bonusCard = mockBonusCards.Find(x => x.Phone == phone);
                    return bonusCard.Bonuses;
                }
                );
            mockRepository.Setup(obj => obj.AddBonuses(It.IsAny<string>(), 
                                                       It.IsAny<int>())).Callback(
                (string phone, int bonuses) =>
                {
                    mockBonusCards.FindAll(x => x.Phone == phone)
                                  .ForEach(x => x.Bonuses += bonuses);
                }
                );
            mockRepository.Setup(obj => obj.WriteOffBonuses(It.IsAny<string>(),
                                                            It.IsAny<int>())).Callback(
                (string phone, int bonuses) =>
                {
                    mockBonusCards.FindAll(x => x.Phone == phone)
                                  .ForEach(x => x.Bonuses -= bonuses);
                }
                );
            mockRepository.Setup(obj => obj.DeleteByPhone(It.IsAny<string>())).Callback(
                (string phone) =>
                {
                    mockBonusCards.RemoveAll(x => x.Phone == phone);
                }
                );

            _mockRepository = mockRepository.Object;
            _interactor = new BonusCardInteractor(_mockRepository);
        }

        [Fact]
        public void CreateBonusCardTest()
        {
            var expectedCount = mockBonusCards.Count + 1;

            _interactor.CreateBonusCard("85005553535");
            Assert.Equal(expectedCount, mockBonusCards.Count);

            var bonusCardsList = mockBonusCards;
            Assert.All(bonusCardsList, obj => Assert.InRange(obj.ID, low: 1, high: expectedCount));
        }

        [Fact]
        public void InvalidPhoneCreateBonusCardTest()
        {
            void action() => _interactor.CreateBonusCard("8800");
            Assert.Throws<BonusCardException>(action);

            var exception = Assert.Throws<BonusCardException>(action);
            Assert.Equal("BonusCard: Invalid input of phone.", exception.Message);
        }

        [Fact]
        public void AlreadyExistsCreateBonusCardTest()
        {
            void action() => _interactor.CreateBonusCard(mockBonusCards[0].Phone);
            Assert.Throws<BonusCardException>(action);

            var exception = Assert.Throws<BonusCardException>(action);
            Assert.Equal("BonusCard: The bonus card is already linked to this phone.", 
                         exception.Message);
        }

        [Fact]
        public void GetBonusesTest()
        {
            var expectedBonuses = mockBonusCards[2].Bonuses;
            var bonuses = _interactor.GetBonuses(mockBonusCards[2].Phone);
            Assert.Equal(expectedBonuses, bonuses);
        }

        [Fact]
        public void InvalidPhoneGetBonusesTest()
        {
            void action() => _interactor.GetBonuses("880055535a5");
            Assert.Throws<BonusCardException>(action);

            var exception = Assert.Throws<BonusCardException>(action);
            Assert.Equal("BonusCard: Invalid input of phone.", exception.Message);
        }

        [Fact]
        public void NotExistsGetBonusesTest()
        {
            void action() => _interactor.GetBonuses("12345678999");
            Assert.Throws<BonusCardException>(action);

            var exception = Assert.Throws<BonusCardException>(action);
            Assert.Equal("BonusCard: This bonus card doesn't exist.",
                         exception.Message);
        }

        [Fact]
        public void AddBonusesTest()
        {
            int bonuses = 10;
            var beforeBonuses = _interactor.GetBonuses(mockBonusCards[1].Phone);
            _interactor.AddBonuses(mockBonusCards[1].Phone, bonuses);
            var afterBonuses = _interactor.GetBonuses(mockBonusCards[1].Phone);

            Assert.Equal(afterBonuses - beforeBonuses, bonuses);
        }

        [Fact]
        public void WrongBonusesNumAddBonusesTest()
        {
            void action() => _interactor.AddBonuses(mockBonusCards[1].Phone, -10);
            Assert.Throws<BonusCardException>(action);

            var exception = Assert.Throws<BonusCardException>(action);
            Assert.Equal("BonusCard: Wrong number of bonuses.", exception.Message);
        }

        [Fact]
        public void InvalidPhoneAddBonusesTest()
        {
            void action() => _interactor.AddBonuses("8b005553535", 10);
            Assert.Throws<BonusCardException>(action);

            var exception = Assert.Throws<BonusCardException>(action);
            Assert.Equal("BonusCard: Invalid input of phone.", exception.Message);
        }

        [Fact]
        public void NotExistsAddBonusesTest()
        {
            void action() => _interactor.AddBonuses("12345678999", 10);
            Assert.Throws<BonusCardException>(action);

            var exception = Assert.Throws<BonusCardException>(action);
            Assert.Equal("BonusCard: This bonus card doesn't exist.",
                         exception.Message);
        }

        [Fact]
        public void WriteOffBonusesTest()
        {
            int bonuses = 10;
            var beforeBonuses = _interactor.GetBonuses(mockBonusCards[1].Phone);
            _interactor.WriteOffBonuses(mockBonusCards[1].Phone, bonuses);
            var afterBonuses = _interactor.GetBonuses(mockBonusCards[1].Phone);

            Assert.Equal(beforeBonuses - afterBonuses, bonuses);
        }

        [Fact]
        public void WrongBonusesNumWriteOffBonusesTest()
        {
            void action() => _interactor.WriteOffBonuses(mockBonusCards[1].Phone, -10);
            Assert.Throws<BonusCardException>(action);

            var exception = Assert.Throws<BonusCardException>(action);
            Assert.Equal("BonusCard: Wrong number of bonuses.", exception.Message);
        }

        [Fact]
        public void InvalidPhoneWriteOffBonusesTest()
        {
            void action() => _interactor.WriteOffBonuses("880055535355", 10);
            Assert.Throws<BonusCardException>(action);

            var exception = Assert.Throws<BonusCardException>(action);
            Assert.Equal("BonusCard: Invalid input of phone.", exception.Message);
        }

        [Fact]
        public void NotExistsWriteOffBonusesTest()
        {
            void action() => _interactor.WriteOffBonuses("12345678999", 10);
            Assert.Throws<BonusCardException>(action);

            var exception = Assert.Throws<BonusCardException>(action);
            Assert.Equal("BonusCard: This bonus card doesn't exist.",
                         exception.Message);
        }

        [Fact]
        public void NotEnoughBonusesWriteOffBonusesTest()
        {
            void action() => _interactor.WriteOffBonuses(mockBonusCards[2].Phone, 60);
            Assert.Throws<BonusCardException>(action);

            var exception = Assert.Throws<BonusCardException>(action);
            Assert.Equal("BonusCard: Not enough bonuses to write off.", exception.Message);
        }

        [Fact]
        public void DeleteBonusCardTest()
        {
            var expectedCount = mockBonusCards.Count - 1;
            string? phone = mockBonusCards[0].Phone;

            _interactor.DeleteBonusCard(phone);
            Assert.Equal(expectedCount, mockBonusCards.Count);
 
            Assert.Null(mockBonusCards.Find(x => x.Phone == phone));
        }

        [Fact]
        public void InvalidPhoneDeleteBonusCardTest()
        {
            void action() => _interactor.DeleteBonusCard("8800a");
            Assert.Throws<BonusCardException>(action);

            var exception = Assert.Throws<BonusCardException>(action);
            Assert.Equal("BonusCard: Invalid input of phone.", exception.Message);
        }

        [Fact]
        public void NotExistsDeleteBonusCardTest()
        {
            void action() => _interactor.DeleteBonusCard("12345678999");
            Assert.Throws<BonusCardException>(action);

            var exception = Assert.Throws<BonusCardException>(action);
            Assert.Equal("BonusCard: This bonus card doesn't exist.",
                         exception.Message);
        }
    }
}