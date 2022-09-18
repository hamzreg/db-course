using WineSales.Domain.Models;

namespace WineSales.Domain.RepositoryInterfaces
{
    public interface IBonusCardRepository : ICrudRepository<BonusCard>
    {
        void AddByPhone(string phone);
        List<BonusCard> GetByBonuses(int bonuses);
        BonusCard? GetByPhone(string phone);
        int GetBonuses(string phone);
        void AddBonuses(string phone, int bonuses);
        void WriteOffBonuses(string phone, int bonuses);
        void DeleteByPhone(string phone);
    }
}
