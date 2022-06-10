using WineSales.Domain.Models;

namespace WineSales.Domain.RepositoryInterfaces
{
    public interface IBonusCardRepository : ICrudRepository<BonusCard>
    {
        List<BonusCard> GetByBonuses(double bonuses);
        bool ExistsPhone(string phone);
        void AddByPhone(string phone);
        BonusCard GetByPhone(string phone);
        double GetBonuses(string phone);
        void CreditBonuses(double bonuses, string phone);
        void WriteOffBonuses(double bonuses, string phone);
        bool Exists(BonusCard bonusCard);
    }
}
