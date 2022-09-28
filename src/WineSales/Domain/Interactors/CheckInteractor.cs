using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Exceptions;
using WineSales.Config;

namespace WineSales.Domain.Interactors
{
    public interface ICheckInteractor
    {
        void CreateCheck(Check check);
        Check GetBySale(int saleID);
        Check GetByPurchase(int purchaseID);
        void DeleteCheck(Check check);
    }

    public class CheckInteractor : ICheckInteractor
    {
        private readonly ICheckRepository checkRepository;

        public CheckInteractor(ICheckRepository checkRepository)
        {
            this.checkRepository = checkRepository;
        }

        public void CreateCheck(Check check)
        {
            if (Exist(check.SaleID))
                throw new CheckException("This check already exists.");

            check.ShiftNumber = CheckConfig.MinShiftNumber;

            checkRepository.Create(check);
        }

        public Check GetBySale(int saleID)
        {
            return checkRepository.GetBySaleID(saleID);
        }

        public Check GetByPurchase(int purchaseID)
        {
            return checkRepository.GetByPurchase(purchaseID);
        }

        public void DeleteCheck(Check check)
        {
            if (!Exist(check.SaleID))
                throw new CheckException("This check does not exist.");

            checkRepository.Delete(check);
        }

        private bool Exist(int saleID)
        {
            return checkRepository.GetBySaleID(saleID) != null;
        }
    }
}
