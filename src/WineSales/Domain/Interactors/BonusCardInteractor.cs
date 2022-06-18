using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Config;
using WineSales.Domain.Exceptions;

namespace WineSales.Domain.Interactors
{
    public interface IBonusCardInteractor
    {
        void Create(string phone);
        double GetBonuses(string phone);
        void AddBonuses(string phone, double bonuses);
        void WriteOffBonuses(string phone, double bonuses);
        void Delete(string phone);
    }

    public class BonusCardInteractor : IBonusCardInteractor
    {
        private readonly IBonusCardRepository bonusCardRepository;

        public BonusCardInteractor(IBonusCardRepository bonusCardRepository)
        {
            this.bonusCardRepository = bonusCardRepository;
        }

        public void Create(string phone)
        {
            if (!CheckPhone(phone))
                throw new BonusCardException("Invalid input of phone.");
            else if (Exist(phone))
                throw new BonusCardException("The bonus card is already linked to this phone.");
                
            bonusCardRepository.AddByPhone(phone);
        }

        public double GetBonuses(string phone)
        {
            if (!CheckPhone(phone))
                throw new BonusCardException("Invalid input of phone.");
            else if (!Exist(phone))
                throw new BonusCardException("This bonus card doesn't exist.");

            return bonusCardRepository.GetBonuses(phone);
        }

        public void AddBonuses(string phone, double bonuses)
        {
            if (bonuses < 0)
                throw new BonusCardException("Wrong number of bonuses.");
            else if (!CheckPhone(phone))
                throw new BonusCardException("Invalid input of phone.");
            else if (!Exist(phone))
                throw new BonusCardException("This bonus card doesn't exist.");

            bonusCardRepository.AddBonuses(phone, bonuses);
        }

        public void WriteOffBonuses(string phone, double bonuses)
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

        public void Delete(string phone)
        {
            if (!CheckPhone(phone))
                throw new BonusCardException("Invalid input of phone.");
            else if (!Exist(phone))
                throw new BonusCardException("This bonus card doesn't exist.");
            
            bonusCardRepository.DeleteByPhone(phone);
        }

        public bool Exist(string phone)
        {
            return bonusCardRepository.GetByPhone(phone) != null;
        }

        private bool CheckPhone(string phone)
        {
            if (phone == null)
                return false;
            else if (!int.TryParse(phone, out int num))
                return false;
            else if (phone.Length != CustomerConfig.PhoneLen)
                return false;
            return true;
        }
    }
}
