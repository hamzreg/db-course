using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Exceptions;

namespace WineSales.Domain.Interactors
{
    public interface ISupplierInteractor
    {
        void Create(Supplier supplier);
        Supplier GetByWine(int wineID);
        void Update(Supplier supplier);
        void Delete(Supplier supplier);
    }

    public class SupplierInteractor : ISupplierInteractor
    {
        private readonly ISupplierRepository supplierRepository;

        public SupplierInteractor(ISupplierRepository supplierRepository)
        {
            this.supplierRepository = supplierRepository;
        }

        public void Create(Supplier supplier)
        {
            if (Exist(supplier.Name))
                throw new SupplierException("This supplier already exists.");

            supplierRepository.Create(supplier);
        }

        public Supplier GetByWine(int wineID)
        {
            return supplierRepository.GetByWine(wineID);
        }

        public void Update(Supplier supplier)
        {
            if (!Exist(supplier.Name))
                throw new SupplierException("This supplier doesn't exist.");

            supplierRepository.Update(supplier);
        }

        public void Delete(Supplier supplier)
        {
            if (!Exist(supplier.Name))
                throw new SupplierException("This supplier doesn't exist.");

            supplierRepository.Delete(supplier);
        }

        private bool Exist(string name)
        {
            return supplierRepository.GetByName(name) != null;
        }
    }
}
