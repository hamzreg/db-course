using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Exceptions;
using WineSales.Config;

namespace WineSales.Domain.Interactors
{
    public interface ICustomerInteractor
    {
        void Create(Customer customer);
        void Update(Customer customer);
        void Delete(Customer customer);
    }

    public class CustomerInteractor : ICustomerInteractor
    {
        private readonly ICustomerRepository customerRepository;

        CustomerInteractor(ICustomerRepository customerRepository)
        {
            this.customerRepository = customerRepository;
        }

        public void Create(Customer customer)
        {
            if (!CheckCustomer(customer))
                throw new CustomerException("Invalid input.");
            else if (Exist(customer.Phone))
                throw new CustomerException("This customer already exists.");

            customerRepository.Create(customer);
        }

        public void Update(Customer customer)
        {
            if (!CheckCustomer(customer))
                throw new CustomerException("Invalid input.");
            else if (!Exist(customer.Phone))
                throw new CustomerException("This customer doesn't exist.");

            customerRepository.Update(customer);
        }

        public void Delete(Customer customer)
        {
            if (!CheckCustomer(customer))
                throw new CustomerException("Invalid input.");
            else if (!Exist(customer.Phone))
                throw new CustomerException("This customer doesn't exist.");

            customerRepository.Delete(customer);
        }

        private bool Exist(string phone)
        {
            return customerRepository.GetByPhone(phone) != null;
        }

        private bool CheckPhone(string phone)
        {
            if (phone == null)
                return false;
            else if (!int.TryParse(phone, out int num))
                return false;
            else if (phone.Length != BonusCardConfig.PhoneLen)
                return false;
            return true;
        }

        private bool CheckCustomer(Customer customer)
        {
            if (customer.Name == null)
                return false;
            else if (customer.Surname == null)
                return false;
            else if (!CheckPhone(customer.Phone))
                return false;
            return true;
        }
    }
}
