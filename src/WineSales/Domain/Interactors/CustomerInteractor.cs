using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Exceptions;
using WineSales.Config;

namespace WineSales.Domain.Interactors
{
    public interface ICustomerInteractor
    {
        void CreateCustomer(Customer customer);
        Customer GetByID(int id);
        Customer GetByNameSurname(string name, string surname);
        void UpdateCustomer(Customer customer);
        void DeleteCustomer(Customer customer);
    }

    public class CustomerInteractor : ICustomerInteractor
    {
        private readonly ICustomerRepository customerRepository;

        public CustomerInteractor(ICustomerRepository customerRepository)
        {
            this.customerRepository = customerRepository;
        }

        public Customer GetByID(int id)
        {
            return customerRepository.GetByID(id);
        }

        public Customer GetByNameSurname(string name, string surname)
        {
            return customerRepository.GetByNameSurname(name, surname);
        }

        public void CreateCustomer(Customer customer)
        {
            if (Exist(customer))
                throw new CustomerException("This customer already exists.");

            customerRepository.Create(customer);
        }

        public void UpdateCustomer(Customer customer)
        {
            if (!Exist(customer))
                throw new CustomerException("This customer doesn't exist.");

            customerRepository.Update(customer);
        }

        public void DeleteCustomer(Customer customer)
        {
            if (!Exist(customer))
                throw new CustomerException("This customer doesn't exist.");

            customerRepository.Delete(customer);
        }

        private bool Exist(Customer customer)
        {
            return customerRepository.GetAll().Any(obj => 
                                                   obj.Name == customer.Name &&
                                                   obj.Surname == customer.Surname);
        }
    }
}
