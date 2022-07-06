﻿using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Exceptions;
using WineSales.Config;

namespace WineSales.Domain.Interactors
{
    public interface ISupplierWineInteractor
    {
        void CreateSupplierWine(SupplierWine supplierWine);
        (List<Wine>, List<double>) GetBySupplierID(int supplierID);
        (List<Wine>, List<double>) GetAllWine();
        (List<Wine>, List<double>) GetCart(int customerID);
        (List<Wine>, List<string>, List<double>) GetByAdminID(int adminID);
        (List<Wine>, List<double>) GetRating();
        void UpdateSupplierWine(SupplierWine supplierWine);
        void DeleteSupplierWine(SupplierWine supplierWine);
    }
    public class SupplierWineInteractor : ISupplierWineInteractor
    {
        private ISupplierWineRepository supplierWineRepository;

        public SupplierWineInteractor(ISupplierWineRepository supplierWineRepository)
        {
            this.supplierWineRepository = supplierWineRepository;
        }

        public void CreateSupplierWine(SupplierWine supplierWine)
        {
            if (!CheckSupplierWine(supplierWine))
                throw new SupplierWineException("Invalid input of supplierWine.");
            else if (Exist(supplierWine))
                throw new SupplierWineException("This supplier already has this wine.");

            supplierWine.Rating = WineConfig.MinRating;
            supplierWineRepository.Create(supplierWine);
        }

        public (List<Wine>, List<double>) GetBySupplierID(int supplierID)
        {
            return supplierWineRepository.GetBySupplierID(supplierID);
        }

        public (List<Wine>, List<double>) GetAllWine()
        {
            return supplierWineRepository.GetAllWine();
        }

        public (List<Wine>, List<double>) GetCart(int customerID)
        {
            return supplierWineRepository.GetCart(customerID);
        }

        public (List<Wine>, List<string>, List<double>) GetByAdminID(int adminID)
        {
            return supplierWineRepository.GetByAdminID(adminID);
        }

        public (List<Wine>, List<double>) GetRating()
        {
            return supplierWineRepository.GetRating();
        }

        public void UpdateSupplierWine(SupplierWine supplierWine)
        {
            if (!CheckSupplierWine(supplierWine))
                throw new SupplierWineException("Invalid input of supplierWine.");
            else if (NotExist(supplierWine.ID))
                throw new SupplierWineException("This supplier doesn't have this wine.");

            supplierWineRepository.Update(supplierWine);
        }

        public void DeleteSupplierWine(SupplierWine supplierWine)
        {
            if (!CheckSupplierWine(supplierWine))
                throw new SupplierWineException("Invalid input of supplierWine.");
            else if (NotExist(supplierWine.ID))
                throw new SupplierWineException("This supplier doesn't have this wine.");

            supplierWineRepository.Delete(supplierWine);
        }

        private bool Exist(SupplierWine supplierWine)
        {
            return supplierWineRepository.GetAll().Any(obj =>
                                                       obj.SupplierID == supplierWine.SupplierID &&
                                                       obj.WineID == supplierWine.WineID);
        }

        private bool NotExist(int id)
        {
            return supplierWineRepository.GetByID(id) == null;
        }

        private bool CheckSupplierWine(SupplierWine supplierWine)
        {
            if (supplierWine.Percent < SaleConfig.MinPercent)
                return false;
            else if (supplierWine.Price < SaleConfig.MinPurchasePrice)
                return false;
            return true;
        }
    }
}