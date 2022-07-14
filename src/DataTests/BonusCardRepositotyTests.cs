using Xunit;
using Microsoft.EntityFrameworkCore;

using WineSales.Data;
using WineSales.Domain.Models;
using WineSales.Domain.Exceptions;
using WineSales.Data.Repositories;

namespace DataTests
{
    public class BonusCardRepositotyTests
    {
        private readonly DbContextOptions<DataBaseContext> _contextOptions;

        public BonusCardRepositotyTests()
        {
            _contextOptions = new DbContextOptionsBuilder<DataBaseContext>()
                .UseInMemoryDatabase("BonusCardRepositoryTests")
                .Options;

            using var context = new DataBaseContext(_contextOptions);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.BonusCards.AddRange(
                new BonusCard { Bonuses = 150, Phone = "88005553535"},
                new BonusCard { Bonuses = 100, Phone = "89005553535"});

            context.SaveChanges();
        }

        private DataBaseContext CreateContext() => new DataBaseContext(_contextOptions);

        [Fact]
        public void CreateTest()
        {
            using var context = CreateContext();
            var repository = new BonusCardRepository(context);

            var bonusCard = new BonusCard { Bonuses = 0, Phone = "87005553535" };

            repository.Create(bonusCard);

            Assert.Equal(3, context.BonusCards.Count());

            var createdBonusCard = context.BonusCards
                .Single(card => card.Phone == bonusCard.Phone);

            Assert.NotNull(createdBonusCard);
            Assert.Equal(3, createdBonusCard.ID);
            Assert.Equal(bonusCard.Bonuses, createdBonusCard.Bonuses);
            Assert.Equal(bonusCard.Phone, createdBonusCard.Phone);
        }

        [Fact]
        public void AddByPhoneTest()
        {
            var context = new DataBaseContext(_contextOptions);
            var repository = new BonusCardRepository(context);

            var phone = "87005553535";

            repository.AddByPhone(phone);

            Assert.Equal(3, context.BonusCards.Count());

            var addedBonusCard = context.BonusCards
                .Single(card => card.Phone == phone);

            Assert.NotNull(addedBonusCard);
            Assert.Equal(3, addedBonusCard.ID);
            Assert.Equal(0, addedBonusCard.Bonuses);
            Assert.Equal(phone, addedBonusCard.Phone);
        }

        [Fact]
        public void GetAllTest()
        {
            using var context = CreateContext();
            var repository = new BonusCardRepository(context);

            var bonusCards = repository.GetAll();

            Assert.Equal(2, bonusCards.Count);

            Assert.Collection(
                bonusCards,
                card =>
                {
                    Assert.Equal(1, card.ID);
                    Assert.Equal(150, card.Bonuses);
                    Assert.Equal("88005553535", card.Phone);
                },
                card =>
                {
                    Assert.Equal(2, card.ID);
                    Assert.Equal(100, card.Bonuses);
                    Assert.Equal("89005553535", card.Phone);
                });
        }

        [Fact]
        public void GetByIDTest()
        {
            using var context = CreateContext();
            var repository = new BonusCardRepository(context);

            var bonusCard = repository.GetByID(2);

            Assert.NotNull(bonusCard);
            Assert.Equal(100, bonusCard.Bonuses);
            Assert.Equal("89005553535", bonusCard.Phone);
        }

        [Fact]
        public void GetByBonusesTest()
        {
            using var context = CreateContext();
            var repository = new BonusCardRepository(context);

            var bonusCards = repository.GetByBonuses(150);

            Assert.Equal(1, bonusCards.Count);

            Assert.Collection(
                bonusCards,
                card =>
                {
                    Assert.Equal(1, card.ID);
                    Assert.Equal(150, card.Bonuses);
                    Assert.Equal("88005553535", card.Phone);
                });
        }

        [Fact]
        public void GetByPhoneTest()
        {
            using var context = CreateContext();
            var repository = new BonusCardRepository(context);

            var bonusCard = repository.GetByPhone("89005553535");

            Assert.NotNull(bonusCard);
            Assert.Equal(2, bonusCard.ID);
            Assert.Equal(100, bonusCard.Bonuses);
        }

        [Fact]
        public void GetBonusesTest()
        {
            using var context = CreateContext();
            var repository = new BonusCardRepository(context);

            var bonuses = repository.GetBonuses("88005553535");

            Assert.Equal(150, bonuses);
        }

        [Fact]
        public void UpdateTest()
        {
            using var context = CreateContext();
            var repository = new BonusCardRepository(context);

            var bonusCard = new BonusCard { ID = 2, Bonuses = 120, Phone = "89005553535" };

            repository.Update(bonusCard);

            Assert.Equal(2, context.BonusCards.Count());

            var updatedBonusCard = context.BonusCards
                .Single(card => card.ID == bonusCard.ID);

            Assert.NotNull(updatedBonusCard);
            Assert.Equal(2, updatedBonusCard.ID);
            Assert.Equal(bonusCard.Bonuses, updatedBonusCard.Bonuses);
            Assert.Equal(bonusCard.Phone, updatedBonusCard.Phone);
        }

        [Fact]
        public void AddBonusesTest()
        {
            using var context = CreateContext();
            var repository = new BonusCardRepository(context);

            repository.AddBonuses("89005553535", 10);

            Assert.Equal(2, context.BonusCards.Count());

            var updatedBonusCard = context.BonusCards
                .Single(card => card.Phone == "89005553535");

            Assert.NotNull(updatedBonusCard);
            Assert.Equal(2, updatedBonusCard.ID);
            Assert.Equal(110, updatedBonusCard.Bonuses);
            Assert.Equal("89005553535", updatedBonusCard.Phone);
        }

        [Fact]
        public void WriteOffBonusesTest()
        {
            using var context = CreateContext();
            var repository = new BonusCardRepository(context);

            repository.WriteOffBonuses("89005553535", 10);

            Assert.Equal(2, context.BonusCards.Count());

            var updatedBonusCard = context.BonusCards
                .Single(card => card.Phone == "89005553535");

            Assert.NotNull(updatedBonusCard);
            Assert.Equal(2, updatedBonusCard.ID);
            Assert.Equal(90, updatedBonusCard.Bonuses);
            Assert.Equal("89005553535", updatedBonusCard.Phone);
        }

        [Fact]
        public void DeleteTest()
        {
            using var context = CreateContext();
            var repository = new BonusCardRepository(context);

            var bonusCard = new BonusCard { ID = 1, Bonuses = 150, Phone = "88005553535" };

            repository.Delete(bonusCard);

            Assert.Equal(1, context.BonusCards.Count());
        }

        [Fact]
        public void DeleteByPhoneTest()
        {
            using var context = CreateContext();
            var repository = new BonusCardRepository(context);

            repository.DeleteByPhone("88005553535");

            Assert.Equal(1, context.BonusCards.Count());
        }
    }
}
