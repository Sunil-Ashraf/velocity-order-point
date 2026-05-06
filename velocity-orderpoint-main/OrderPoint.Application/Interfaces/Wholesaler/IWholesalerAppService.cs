using OrderPoint.Domain.Common;
using OrderPoint.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Application.Interfaces.Customers
{
    public interface IWholesalerAppService
    {
         

        (APIResponse, IQueryable<WholesalerViewModel>) GetAllWholesalerList();
  
        //Task<APIResponse> UpdateCustomer(AddEditCustomerModel model);
        Task<APIResponse> AddEditWholesaler(AddEditWholesalerModel model);
        Task<APIResponse> SetWholesalerDeliverytime(DeliveryOptionVM model);
        Task<APIResponse> GetWholesalerByID(long id);
        Task<APIResponse> GetWholesalerByName(String name);
        Task<AddEditWholesalerModel> GetWholesalerDetailByID(long id);

        Task<APIResponse> GetWholesaleUserListBysalerID(long id);
        Task<APIResponse> GetWholesalerUserByUserID(long userID);
        Task<APIResponse> AddWholesalerLinks(Int32 userID, Int32 wholesalerID);
        Task<APIResponse> UpdateWholesalerStatus(Int32 id);
        (APIResponse, IQueryable<WholesalerViewModel>) GetWholesalerbyUserID();
        Task<APIResponse> GetdeliverySchedulerByWholesaler(Int64 id);

        Task<Response<List<UserDetail>>> GetWholersalerCustomerUserListbysalerID(long id);
        //Task<APIResponse> GetAllWholesalerListss();
        //Task<APIResponse> GetCustomerUserByCustomerID(long id);
        //Task<APIResponse> GetCustomerUserByUserID(long id);
        //Task<APIResponse> AddCustomerLinks(Int32 userID, Int32 customerID);

    }
}
