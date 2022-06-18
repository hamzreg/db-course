using WineSales.Domain.Models;

namespace WineSales.Domain.RepositoryInterfaces
{
    public interface IBonusCardRepository : ICrudRepository<BonusCard>
    {
        List<BonusCard> GetByBonuses(double bonuses);
        void AddByPhone(string phone);
        BonusCard GetByPhone(string phone);
        double GetBonuses(string phone);
        void AddBonuses(string phone, double bonuses);
        void WriteOffBonuses(string phone, double bonuses);
        void DeleteByPhone(string phone);
    }
}
