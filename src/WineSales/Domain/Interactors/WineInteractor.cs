using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Config;

namespace WineSales.Domain.Interactors
{
    public class WineInteractor
    {
        IWineRepository wineRepository;

        public WineInteractor(IWineRepository wineRepository)
        {
            this.wineRepository = wineRepository;
        }
        public bool CheckWine(Wine wine)
        {
            if (wine == null)
                return false;
            else if (!Constants.Colors.Contains(wine.Color))
                return false;
            else if (!Constants.Sugar.Contains(wine.Sugar))
                return false;
            else if (wine.Volume < Constants.MinVolume ||
                     wine.Volume > Constants.MaxVolume)
                return false;
            else if (wine.Alcohol < Constants.MinAlcohol ||
                     wine.Alcohol > Constants.MaxAlcohol)
                return false;
            else if (wine.Aging < Constants.MinAging ||
                     wine.Aging > Constants.MaxAging)
                return false;
            return true;
        }
        public int Create(Wine wine)
        {
            if (CheckWine(wine) && !Exists(wine))
            {
                wineRepository.Create(wine);
                return (int)ReturnCodes.Codes.OK;
            }

            return (int)ReturnCodes.Codes.InvalidInput;
        }
        public (List<Wine>, List<double>) GetBySupplier(int supplierID)
        {
            return wineRepository.GetBySupplier(supplierID);
        }
        public List<Wine> GetByCustomer()
        {
            return wineRepository.GetAll();
        }
        public (List<Wine>, List<string>, List<int>) GetByAdmin()
        {
            return wineRepository.GetByAdmin();
        }
        public (List<Wine>, List<double>) GetRating()
        {
            return wineRepository.GetRating();
        }
        public bool Exists(Wine wine)
        {
            return wineRepository.Exists(wine);
        }
        public int Update(Wine wine)
        {
            if (Exists(wine))
            {
                wineRepository.Update(wine);
                return (int)ReturnCodes.Codes.OK;
            }

            return (int)ReturnCodes.Codes.NotExists;
        }
        public int Delete(Wine wine)
        {
            if (Exists(wine))
            {
                wineRepository.Delete(wine);
                return (int)ReturnCodes.Codes.OK;
            }

            return (int)ReturnCodes.Codes.NotExists;
        }
    }
}
