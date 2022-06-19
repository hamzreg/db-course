﻿using WineSales.Domain.Models;
using WineSales.Domain.RepositoryInterfaces;
using WineSales.Domain.Exceptions;
using WineSales.Config;

namespace WineSales.Domain.Interactors
{
    public interface ICustomerInteractor
    {
        void CreateCustomer(Customer customer);
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

        public void CreateCustomer(Customer customer)
        {
            if (Exist(customer.ID))
                throw new CustomerException("This customer already exists.");

            customerRepository.Create(customer);
        }

        public void UpdateCustomer(Customer customer)
        {
            if (!Exist(customer.ID))
                throw new CustomerException("This customer doesn't exist.");

            customerRepository.Update(customer);
        }

        public void DeleteCustomer(Customer customer)
        {
            if (!Exist(customer.ID))
                throw new CustomerException("This customer doesn't exist.");

            customerRepository.Delete(customer);
        }

        private bool Exist(int id)
        {
            return customerRepository.GetByID(id) != null;
        }
    }
}
