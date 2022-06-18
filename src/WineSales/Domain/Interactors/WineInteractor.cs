using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Exceptions;
using WineSales.Config;

namespace WineSales.Domain.Interactors
{
    public interface IWineInteractor
    {
        void Create(Wine wine, double purchasePrice, int percent);
        (List<Wine>, List<double>) GetBySupplier(int supplierID);
        List<Wine> GetAll();
        (List<Wine>, List<double>) GetCart(int customerID);
        (List<Wine>, List<string>, List<double>) GetByAdmin();
        (List<Wine>, List<double>) GetRating();
        void Update(Wine wine);
        void Delete(Wine wine);
    }

    public class WineInteractor
    {
        private readonly IWineRepository wineRepository;

        public WineInteractor(IWineRepository wineRepository)
        {
            this.wineRepository = wineRepository;
        }

        public void Create(Wine wine, double purchasePrice, int percent)
        {
            if (percent < SaleConfig.MinPercent)
                throw new WineException("Invalid input of percent.");
            else if (purchasePrice < SaleConfig.MinPurchasePrice)
                throw new WineException("Invalid input of purchase price.");
            else if (!CheckWine(wine))
                throw new WineException("Invalid input of wine.");
            else if (Exist(wine.ID))
                throw new WineException("This wine already exists.");

            wineRepository.Add(wine, purchasePrice, percent);
        }

        public (List<Wine>, List<double>) GetBySupplier(int supplierID)
        {
            return wineRepository.GetBySupplier(supplierID);
        }

        public List<Wine> GetAll()
        {
            return wineRepository.GetAll();
        }

        public (List<Wine>, List<double>) GetCart(int customerID)
        {
            return wineRepository.GetCart(customerID);
        }

        public (List<Wine>, List<string>, List<double>) GetByAdmin()
        {
            return wineRepository.GetByAdmin();
        }

        public (List<Wine>, List<double>) GetRating()
        {
            return wineRepository.GetRating();
        }

        public void Update(Wine wine)
        {
            if (!Exist(wine.ID))
                throw new WineException("This wine doesn't exist.");
            else if (!CheckWine(wine))
                throw new WineException("Invalid input of wine.");

            wineRepository.Update(wine);
        }

        public void Delete(Wine wine)
        {
            if (!Exist(wine.ID))
                throw new WineException("This wine doesn't exist.");
            else if (!CheckWine(wine))
                throw new WineException("Invalid input of wine.");

            wineRepository.Delete(wine);
        }

        private bool Exist(int id)
        {
            return wineRepository.GetByID(id) != null;
        }

        private bool CheckWine(Wine wine)
        {
            if (!WineConfig.Colors.Contains(wine.Color))
                return false;
            else if (!WineConfig.Sugar.Contains(wine.Sugar))
                return false;
            else if (wine.Volume < WineConfig.MinVolume ||
                     wine.Volume > WineConfig.MaxVolume)
                return false;
            else if (wine.Alcohol < WineConfig.MinAlcohol ||
                     wine.Alcohol > WineConfig.MaxAlcohol)
                return false;
            else if (wine.Aging < WineConfig.MinAging ||
                     wine.Aging > WineConfig.MaxAging)
                return false;
            return true;
        }
    }
}
