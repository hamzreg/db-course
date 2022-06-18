using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Exceptions;
using WineSales.Config;

namespace WineSales.Domain.Interactors
{
    public interface ISaleInteractor
    {
        void Create(Sale sale, int percent);
        (List<Wine>, List<double>) GetBySupplier(int supplierID);
        (List<Wine>, List<string>, List<Sale>) GetByAdmin(int adminID);
    }

    public class SaleInteractor : ISaleInteractor
    {
        private readonly ISaleRepository saleRepository;
        public SaleInteractor(ISaleRepository saleRepository)
        {
            this.saleRepository = saleRepository;
        }

        public void Create(Sale sale, int percent)
        {
            if (percent < SaleConfig.MinPercent)
                throw new SaleException("Invalid input of percent.");
            else if (sale.PurchasePrice < SaleConfig.MinPurchasePrice)
                throw new SaleException("Invalid input of purchase price.");
            else if (sale.Costs < SaleConfig.MinCosts)
                throw new SaleException("Ivalid input of costs.");
            else if (sale.WineNumber < SaleConfig.MinWineNumer)
                throw new SaleException("Invalid input of wine number.");

            sale.SellingPrice = sale.PurchasePrice * (1 + percent / 100);
            sale.Margin = sale.SellingPrice - sale.PurchasePrice;
            sale.Profit = (sale.Margin - sale.Costs) * sale.WineNumber;

            saleRepository.Create(sale);
        }

        public (List<Wine>, List<double>) GetBySupplier(int supplierID)
        {
            return saleRepository.GetBySupplier(supplierID);
        }

        public (List<Wine>, List<string>, List<Sale>) GetByAdmin(int adminID)
        {
            return saleRepository.GetByAdmin(adminID);
        }
    }
}
