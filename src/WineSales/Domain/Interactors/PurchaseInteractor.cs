using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Exceptions;
using WineSales.Config;

namespace WineSales.Domain.Interactors
{
    public interface IPurchaseInteractor
    {
        void Create(Purchase purchase);
        (List<Wine>, List<double>) GetByCustomer(Customer customer);
        void ChangeStatus(Purchase purchase);
        void Delete(Purchase purchase);
    }

    public class PurchaseInteractor : IPurchaseInteractor
    {
        private readonly IPurchaseRepository purchaseRepository;

        public PurchaseInteractor(IPurchaseRepository purchaseRepository)
        {
            this.purchaseRepository = purchaseRepository;
        }

        public void Create(Purchase purchase)
        {
            if (!CheckStatus(purchase.Status))
                throw new PurchaseException("Invalid input of status.");
            else if (Exist(purchase.ID))
                throw new PurchaseException("This purchase already exists.");

            purchaseRepository.Create(purchase);
        }

        public (List<Wine>, List<double>) GetByCustomer(Customer customer)
        {
            return purchaseRepository.GetByCustomer(customer.ID);
        }

        public void ChangeStatus(Purchase purchase)
        {
            if (!CheckStatus(purchase.Status))
                throw new PurchaseException("Invalid input of status.");
            if (!Exist(purchase.ID))
                throw new PurchaseException("This purchase doesn't exist.");

            purchaseRepository.Update(purchase);
        }

        public void Delete(Purchase purchase)
        {
            if (!Exist(purchase.ID))
                throw new PurchaseException("This purchase doesn't exist.");

            purchaseRepository.Delete(purchase);
        }

        private bool Exist(int id)
        {
            return purchaseRepository.GetByID(id) != null;
        }

        private bool CheckStatus(int status)
        {
            return (status == (int)PurchaseConfig.Statuses.Active ||
                    status == (int)PurchaseConfig.Statuses.Canceled);
        }
    }
}
