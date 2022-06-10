using WineSales.Domain.RepositoryInterfaces;

namespace WineSales.Domain.Interactors
{
    public class SaleInteractor
    {
        ISaleRepository saleRepository;
        public SaleInteractor(ISaleRepository saleRepository)
        {
            this.saleRepository = saleRepository;
        }
    }
}
