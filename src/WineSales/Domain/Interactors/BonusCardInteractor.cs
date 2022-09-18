using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Exceptions;
using WineSales.Config;

namespace WineSales.Domain.Interactors
{
    public interface IBonusCardInteractor
    {
        void CreateBonusCard(string phone);
        int GetBonuses(string phone);
        BonusCard GetByID(int id);
        BonusCard GetByPhone(string phone);
        void AddBonuses(string phone, int bonuses);
        void WriteOffBonuses(string phone, int bonuses);
        void DeleteBonusCard(string phone);
    }

    public class BonusCardInteractor : IBonusCardInteractor
    {
        private readonly IBonusCardRepository bonusCardRepository;

        public BonusCardInteractor(IBonusCardRepository bonusCardRepository)
        {
            this.bonusCardRepository = bonusCardRepository;
        }

        public BonusCard GetByID(int id)
        {
            return bonusCardRepository.GetByID(id);
        }

        public BonusCard GetByPhone(string phone)
        {
            return bonusCardRepository.GetByPhone(phone);
        }

        public void Create(BonusCard bonusCard)
        {
            if (!CheckPhone(bonusCard.Phone))
                throw new BonusCardException("Invalid input of phone.");
            else if (Exist(bonusCard.Phone))
                throw new BonusCardException("The bonus card is already linked to this phone.");

            bonusCardRepository.Create(bonusCard);
        }

        public void CreateBonusCard(string phone)
        {
            if (!CheckPhone(phone))
                throw new BonusCardException("Invalid input of phone.");
            else if (Exist(phone))
                throw new BonusCardException("The bonus card is already linked to this phone.");

            bonusCardRepository.AddByPhone(phone);
        }

        public int GetBonuses(string phone)
        {
            if (!CheckPhone(phone))
                throw new BonusCardException("Invalid input of phone.");
            else if (!Exist(phone))
                throw new BonusCardException("This bonus card doesn't exist.");

            return bonusCardRepository.GetBonuses(phone);
        }

        public void AddBonuses(string phone, int bonuses)
        {
            if (bonuses < 0)
                throw new BonusCardException("Wrong number of bonuses.");
            else if (!CheckPhone(phone))
                throw new BonusCardException("Invalid input of phone.");
            else if (!Exist(phone))
                throw new BonusCardException("This bonus card doesn't exist.");

            bonusCardRepository.AddBonuses(phone, bonuses);
        }

        public void WriteOffBonuses(string phone, int bonuses)
        {
            if (bonuses < 0)
                throw new BonusCardException("Wrong number of bonuses.");
            else if (!CheckPhone(phone))
                throw new BonusCardException("Invalid input of phone.");
            else if (!Exist(phone))
                throw new BonusCardException("This bonus card doesn't exist.");

            double state = GetBonuses(phone);

            if (state < bonuses)
                throw new BonusCardException("Not enough bonuses to write off.");

            bonusCardRepository.WriteOffBonuses(phone, bonuses);
        }

        public void DeleteBonusCard(string phone)
        {
            if (!CheckPhone(phone))
                throw new BonusCardException("Invalid input of phone.");
            else if (!Exist(phone))
                throw new BonusCardException("This bonus card doesn't exist.");

            bonusCardRepository.DeleteByPhone(phone);
        }

        private bool Exist(string phone)
        {
            return bonusCardRepository.GetByPhone(phone) != null;
        }

        private bool CheckPhone(string phone)
        {
            if (phone == null)
                return false;
            else if (phone.Length != BonusCardConfig.PhoneLen)
                return false;
            else if (!int.TryParse(phone.Substring(0, BonusCardConfig.PhoneLen / 2),
                                   out int _) ||
                     !int.TryParse(phone.Substring(BonusCardConfig.PhoneLen / 2),
                                   out int _))
                return false;
            return true;
        }
    }
}
