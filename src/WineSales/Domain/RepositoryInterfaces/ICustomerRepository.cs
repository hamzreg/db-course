using WineSales.Domain.Models;

namespace WineSales.Domain.RepositoryInterfaces
{
    public interface ICustomerRepository : ICrudRepository<Customer>
    {
        Customer GetByNameSurname(string name, string surname);
        List<Customer> GetByName(string name);
        List<Customer> GetBySurname(string surname);
    }
}
