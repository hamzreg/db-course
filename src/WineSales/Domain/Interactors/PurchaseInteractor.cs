using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Exceptions;
using WineSales.Config;

namespace WineSales.Domain.Interactors
{
    public interface IPurchaseInteractor
    {
        void CreatePurchase(Purchase purchase);
        (List<Wine>, List<double>) GetByCustomer(int customerID);
        Purchase GetActive(int customerID, double price);
        void ChangeStatus(Purchase purchase);
        void DeletePurchase(Purchase purchase);
    }

    public class PurchaseInteractor : IPurchaseInteractor
    {
        private readonly IPurchaseRepository purchaseRepository;

        public PurchaseInteractor(IPurchaseRepository purchaseRepository)
        {
            this.purchaseRepository = purchaseRepository;
        }

        public void CreatePurchase(Purchase purchase)
        {
            if (!CheckStatus(purchase.Status))
                throw new PurchaseException("Invalid input of status.");
            else if (Exist(purchase))
                throw new PurchaseException("This purchase already exists.");

            purchaseRepository.Create(purchase);
        }

        public (List<Wine>, List<double>) GetByCustomer(int customerID)
        {
            return purchaseRepository.GetByCustomerID(customerID);
        }

        public Purchase GetActive(int customerID, double price)
        {
            return purchaseRepository.GetActive(customerID, price);
        }

        public void ChangeStatus(Purchase purchase)
        {
            if (!CheckStatus(purchase.Status))
                throw new PurchaseException("Invalid input of status.");
            if (!Exist(purchase))
                throw new PurchaseException("This purchase doesn't exist.");

            purchaseRepository.Update(purchase);
        }

        public void DeletePurchase(Purchase purchase)
        {
            if (!Exist(purchase))
                throw new PurchaseException("This purchase doesn't exist.");

            purchaseRepository.Delete(purchase);
        }

        private bool Exist(Purchase purchase)
        {
            return purchaseRepository.GetAll().Any(obj =>
                                                   obj.Price == purchase.Price &&
                                                   obj.Status == purchase.Status &&
                                                   obj.CustomerID == purchase.CustomerID);
        }

        private bool CheckStatus(int status)
        {
            return (status == (int)PurchaseConfig.Statuses.Active ||
                    status == (int)PurchaseConfig.Statuses.Canceled);
        }
    }
}
