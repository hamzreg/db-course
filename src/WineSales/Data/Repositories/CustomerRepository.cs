using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace WineSales.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DataBaseContext _context;

        public CustomerRepository(DataBaseContext context)
        {
            _context = context;
        }

        public void Create(Customer customer)
        {
            try
            {
                _context.Customers.Add(customer);
                _context.SaveChanges();
            }
            catch
            {
                throw new CustomerException("Failed to create customer.");
            }
        }

        public List<Customer> GetAll()
        {
            return _context.Customers.ToList();
        }

        public Customer GetByID(int id)
        {
            return _context.Customers.Find(id);
        }

        public Customer GetByNameSurname(string name, string surname)
        {
            return _context.Customers.FirstOrDefault(customer =>
                                                     customer.Name == name &&
                                                     customer.Surname == surname);
        }

        public List<Customer> GetByName(string name)
        {
            return _context.Customers.Where(customer => customer.Name == name)
                .ToList();
        }

        public List<Customer> GetBySurname(string surname)
        {
            return _context.Customers.Where(customer => customer.Surname == surname)
                .ToList();
        }

        public void Update(Customer customer)
        {
            try
            {
                _context.Customers.Update(customer);
                _context.SaveChanges();
            }
            catch
            {
                throw new CustomerException("Failed to update customer.");
            }
        }

        public void UpdateBonusCard(int customerID, int bonusCardID)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("call change_customer_bonus_card({0}, {1});",
                                                      customerID, bonusCardID);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex.Message);
                    throw new CustomerException("Failed to update bonus card.");
                }
            }
        }

        public void Delete(Customer customer)
        {
            var foundCustomer = GetByID(customer.ID);

            if (foundCustomer == null)
                throw new CustomerException("Failed to get customer by id.");

            try
            {
                _context.Customers.Remove(foundCustomer);
                _context.SaveChanges();
            }
            catch
            {
                throw new CustomerException("Failed to delete customer.");
            }
        }
    }
}
