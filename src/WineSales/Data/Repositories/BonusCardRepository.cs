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

        public void Create(BonusCard bonusCard)
        {
            try
            {
                _context.BonusCards.Add(bonusCard);
                _context.SaveChanges();
            }
            catch
            {
                throw new BonusCardException("Failed to create bonus card.");
            }
        }

        public void AddByPhone(string phone)
        {
            var bonusCard = new BonusCard();

            bonusCard.Bonuses = 0;
            bonusCard.Phone = phone;

            try
            {
                _context.BonusCards.Add(bonusCard);
                _context.SaveChanges();
            }
            catch
            {
                throw new BonusCardException("Failed to add by phone.");
            }
        }

        public List<BonusCard> GetAll()
        {
            return _context.BonusCards.ToList();
        }

        public BonusCard? GetByID(int id)
        {
            return _context.BonusCards.Find(id);
        }

        public List<BonusCard> GetByBonuses(int bonuses)
        {
            return _context.BonusCards.Where(bonusCard => bonusCard.Bonuses == bonuses)
                                 .ToList();
        }

        public BonusCard? GetByPhone(string phone)
        {
            return _context.BonusCards.FirstOrDefault(bonusCard => bonusCard.Phone == phone);
        }

        public int GetBonuses(string phone)
        {
            var bonusCard = GetByPhone(phone);
            return bonusCard.Bonuses;
        }

        public void Update(BonusCard bonusCard)
        {
            try
            {
                _context.BonusCards.Update(bonusCard);
                _context.SaveChanges();
            }
            catch
            {
                throw new BonusCardException("Failed to update bonus card.");
            }
        }

        public void AddBonuses(string phone, int bonuses)
        {
            var bonusCard = GetByPhone(phone);
            bonusCard.Bonuses += bonuses;

            try
            {
                _context.BonusCards.Update(bonusCard);
                _context.SaveChanges();
            }
            catch
            {
                throw new BonusCardException("Failed to add bonuses.");
            }
        }

        public void WriteOffBonuses(string phone, int bonuses)
        {
            var bonusCard = GetByPhone(phone);
            bonusCard.Bonuses -= bonuses;

            try
            {
                _context.BonusCards.Update(bonusCard);
                _context.SaveChanges();
            }
            catch
            {
                throw new BonusCardException("Failed to write off bonuses.");
            }
        }

        public void Delete(BonusCard bonusCard)
        {
            var foundBonusCard = GetByID(bonusCard.ID);

            if (foundBonusCard == null)
                throw new BonusCardException("Failed to get card by id.");

            try
            {
                _context.BonusCards.Remove(foundBonusCard);
                _context.SaveChanges();
            }
            catch
            {
                throw new BonusCardException("Failed to delete bonus card.");
            }
        }

        public void DeleteByPhone(string phone)
        {
            var bonusCard = GetByPhone(phone);

            if (bonusCard == null)
                throw new BonusCardException("Failed to get card by phone.");

            try
            {
                _context.BonusCards.Remove(bonusCard);
                _context.SaveChanges();
            }
            catch
            {
                throw new BonusCardException("Failed to delete by phone.");
            }
        }
    }
}
