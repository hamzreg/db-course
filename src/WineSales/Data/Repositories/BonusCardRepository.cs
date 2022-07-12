using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Exceptions;

namespace WineSales.Data.Repositories
{
    public class BonusCardRepository : IBonusCardRepository
    {
        private readonly DataBaseContext _context;

        public BonusCardRepository(DataBaseContext context)
        {
            _context = context;
        }

        public List<BonusCard> GetByBonuses(int bonuses)
        {
            return _context.Cards.Where(card => card.Bonuses == bonuses)
                                 .ToList();
        }

        public void AddByPhone(string phone)
        {
            var bonusCard = new BonusCard();

            bonusCard.ID = _context.Cards.Count() + 1;
            bonusCard.Bonuses = 0;
            bonusCard.Phone = phone;

            try
            {
                _context.Cards.Add(bonusCard);
                _context.SaveChanges();
            }
            catch
            {
                throw new BonusCardException("Failed to add by phone.");
            }
        }

        public BonusCard? GetByPhone(string phone)
        {
            return _context.Cards.FirstOrDefault(card => card.Phone == phone);
        }

        public int GetBonuses(string phone)
        {
            var card = GetByPhone(phone);
            return card.Bonuses;
        }

        public void AddBonuses(string phone, int bonuses)
        {
            var card = GetByPhone(phone);

            try
            {
                card.Bonuses += bonuses;
                _context.Cards.Update(card);
                _context.SaveChanges();
            }
            catch
            {
                throw new BonusCardException("Failed to add bonuses.");
            }
        }

        public void WriteOffBonuses(string phone, int bonuses)
        {
            var card = GetByPhone(phone);

            try
            {
                card.Bonuses -= bonuses;
                _context.Cards.Update(card);
                _context.SaveChanges();
            }
            catch
            {
                throw new BonusCardException("Failed to write off bonuses.");
            }
        }

        public void DeleteByPhone(string phone)
        {
            var card = GetByPhone(phone);

            if (card == null)
                throw new BonusCardException("Failed to get card by phone.");

            try
            {
                _context.Cards.Remove(card);
                _context.SaveChanges();
            }
            catch
            {
                throw new BonusCardException("Failed to delete by phone.");
            }
        }
    }
}
