using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Models;

namespace WineSales.Domain.Interactors
{
    public class SupplierInteractor
    {
        ISupplierRepository supplierRepository;

        public SupplierInteractor(ISupplierRepository supplierRepository)
        {
            this.supplierRepository = supplierRepository;
        }

        public void Create(Supplier supplier)
        {
            supplierRepository.Create(supplier);
        }

        public List<Supplier> GetByFilter(int filter)
        {
            return supplierRepository.GetByFilter(filter);
        }
        public List<Supplier> GetByWine(Wine wine)
        {
            return supplierRepository.GetByWine(wine);
        }
        public void Update(Supplier supplier)
        {
            supplierRepository.Update(supplier);
        }
        public void Delete(Supplier supplier)
        {
            supplierRepository.Delete(supplier);
        }
        public List<Supplier> SortByFilter(int filter)
        {
            return supplierRepository.SortByFilter(filter);
        }
    }
}
