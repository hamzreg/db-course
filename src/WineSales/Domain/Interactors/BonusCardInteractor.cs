using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Config;

namespace WineSales.Domain.Interactors
{
    public class BonusCardInteractor
    {
        IBonusCardRepository bonusCardRepository;

        public BonusCardInteractor(IBonusCardRepository bonusCardRepository)
        {
            this.bonusCardRepository = bonusCardRepository;
        }
        public bool CheckPhone(string phone)
        {
            if (phone == null)
                return false;
            else if (!int.TryParse(phone, out int num))
                return false;
            else if (phone.Length != Constants.MaxPhoneLen)
                return false;
            return true;
        }
        public bool ExistsPhone(string phone)
        {
            return bonusCardRepository.ExistsPhone(phone);
        }
        public int Create(string phone)
        {
            if (CheckPhone(phone) && !ExistsPhone(phone))
            {
                bonusCardRepository.AddByPhone(phone);
                return (int)ReturnCodes.Codes.OK;
            }

            return (int)ReturnCodes.Codes.InvalidInput;
        }
        public int CreditBonuses(double bonuses, string phone)
        {
            if (bonuses < 0)
                return (int)ReturnCodes.Codes.InvalidInput;

            if (!CheckPhone(phone))
                return (int)ReturnCodes.Codes.InvalidInput;

            bonusCardRepository.CreditBonuses(bonuses, phone);

            return (int)ReturnCodes.Codes.OK;
        }
        public (int, double) GetBonuses(string phone)
        {
            if (!CheckPhone(phone))
                return ((int)ReturnCodes.Codes.InvalidInput, Constants.ErrorValue);

            double bonuses = bonusCardRepository.GetBonuses(phone);

            return ((int)ReturnCodes.Codes.OK, bonuses);
        }
        public int WriteOffBonuses(double bonuses, string phone)
        {
            if (bonuses < 0)
                return (int)ReturnCodes.Codes.InvalidInput;

            if (!CheckPhone(phone))
                return (int)ReturnCodes.Codes.InvalidInput;

            (int code, double value) state = GetBonuses(phone);

            if (state.code != (int)ReturnCodes.Codes.OK)
                return state.code;
            else if (state.value <= Constants.MinBonusCard)
                return (int)ReturnCodes.Codes.Empty;

            bonusCardRepository.WriteOffBonuses(bonuses, phone);

            return (int)ReturnCodes.Codes.OK;
        }
        public bool Exists(BonusCard bonusCard)
        {
            return bonusCardRepository.Exists(bonusCard);
        }
        public int Delete(BonusCard bonusCard)
        {
            if (Exists(bonusCard))
            {
                bonusCardRepository.Delete(bonusCard);
                return (int)ReturnCodes.Codes.OK;
            }

            return (int)ReturnCodes.Codes.NotExists;
        }
    }
}
