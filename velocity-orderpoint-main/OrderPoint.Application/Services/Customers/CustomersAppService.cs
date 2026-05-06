using OrderPoint.Application.Interfaces.Customers;
using OrderPoint.Application.Interfaces.Product;
using OrderPoint.Domain.Common;
using OrderPoint.Domain.Interfaces.Customers;
 
using OrderPoint.Domain.Interfaces.User;
using OrderPoint.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Application.Services.Customers
{
    public class CustomersAppService : ICustomerAppService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomersAppService(ICustomerRepository customerRepository) { _customerRepository = customerRepository; }
        public (APIResponse, IQueryable<CustomerModel>) GetAllCustomers(long? wholesalerID)
        {
            return _customerRepository.GetAllCustomers(wholesalerID);
        }
        public (APIResponse, IQueryable<CustomerModel>) GetWholesalerCustomers()
        {
            return _customerRepository.GetWholesalerCustomers();
        }
        public Task<APIResponse> CreateCustomer(AddEditCustomerModel model)
        {
            
            return _customerRepository.CreateCustomer(model);
        } 
        public Task<APIResponse> CreateOrder(OrderPlacementModel model)
        {
            
            return _customerRepository.CreateOrder(model);
        }
        public Task<APIResponse> AddOrderItems(OrderPlacementModel model)
        {
            
            return _customerRepository.AddOrderItems(model);
        }



        public Task<APIResponse> GetCustomerbyId(long id)
        {
            return _customerRepository.GetCustomerbyId(id);
        }

        public Task<APIResponse> UpdateCustomer(AddEditCustomerModel model)
        {
            return _customerRepository.UpdateCustomer(model);
        }
        public Task<APIResponse> GetAllWholesalerList()
        {
            return _customerRepository.GetAllWholesalerList();
        }

        public Task<APIResponse> GetCustomerUserByCustomerID(long id)
        {
            return _customerRepository.GetCustomerUserByCustomerID(id);
        }

        public Task<APIResponse> GetCustomerUserByUserID(long id)
        {
            return _customerRepository.GetCustomerUserByUserID(id);
        }
        public Task<APIResponse> AddCustomerLinks(Int32 userID, Int32 customerID)
        {
            return _customerRepository.AddCustomerLinks(userID ,customerID);
        }

    }
}
