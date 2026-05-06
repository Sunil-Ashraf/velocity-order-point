using OrderPoint.Domain.Common;
using OrderPoint.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.Interfaces.Customers
{
    public interface ICustomerRepository
    {
        (APIResponse, IQueryable<CustomerModel>) GetAllCustomers(long? wholesalerID);
        (APIResponse, IQueryable<CustomerModel>) GetWholesalerCustomers();
        Task<APIResponse> UpdateCustomer(AddEditCustomerModel model);
        Task<APIResponse> CreateCustomer(AddEditCustomerModel model);
        Task<APIResponse> CreateOrder(OrderPlacementModel model);
        Task<APIResponse> AddOrderItems(OrderPlacementModel model);
        Task<APIResponse> GetCustomerbyId(long id);
        Task<APIResponse> GetAllWholesalerList();

        Task<APIResponse> GetCustomerUserByCustomerID(long id);
          Task<APIResponse> GetCustomerUserByUserID(long id);
        Task<APIResponse> AddCustomerLinks(Int32 userID, Int32 customerID);

    }
}
