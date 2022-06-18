﻿using WineSales.Domain.Models;

namespace WineSales.Domain.RepositoryInterfaces
{
    public interface ISaleRepository : ICrudRepository<Sale>
    {
        List<Sale> GetByPurschasePrice(double purchasePrice);
        List<Sale> GetBySellingPrice(double sellingPrice);
        List<Sale> GetByMargin(double margin);
        List<Sale> GetByCosts(double costs);
        List<Sale> GetByProfit(double profit);
        List<Sale> GetByWineNumer(int wineNumber);
        List<Sale> GetByDate(DateOnly date);
        (List<Wine>, List<double>) GetBySupplier(int supplierID);
        (List<Wine>, List<string>, List<Sale>) GetByAdmin(int adminID);
    }
}