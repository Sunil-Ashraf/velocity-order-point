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
    public class WholesalerAppService : IWholesalerAppService
    {
        private readonly IWholesalerRepository _wholesalerRepository;

        public WholesalerAppService(IWholesalerRepository wholesalerRepository) 
        {
            _wholesalerRepository = wholesalerRepository; 
        }
        public (APIResponse, IQueryable<WholesalerViewModel>) GetAllWholesalerList()
        {
            return _wholesalerRepository.GetAllWholesalerList();
        }
        public Task<APIResponse> AddEditWholesaler(AddEditWholesalerModel model)
        {

            return _wholesalerRepository.AddEditWholesaler(model);
        }
        public Task<APIResponse> SetWholesalerDeliverytime(DeliveryOptionVM model)
        {

            return _wholesalerRepository.SetWholesalerDeliverytime(model);
        }



        public Task<APIResponse> GetWholesalerByID(long id)
        {
            return _wholesalerRepository.GetWholesalerByID(id);
        } 
        public   Task<APIResponse> GetWholesalerByName(String name)
        {
            return _wholesalerRepository.GetWholesalerByName(name);
        }

        public Task<APIResponse> GetWholesaleUserListBysalerID(long id)
        {
            return _wholesalerRepository.GetWholesaleUserListBysalerID(id);
        }
        public Task<APIResponse> GetWholesalerUserByUserID(long userID)
        {
            return _wholesalerRepository.GetWholesalerUserByUserID(userID);
        }

        public Task<APIResponse> AddWholesalerLinks(Int32 userID, Int32 wholesalerID)
        {
            return _wholesalerRepository.AddWholesalerLinks(userID, wholesalerID);
        }
        public Task<APIResponse> UpdateWholesalerStatus(Int32 id)
        {
            return _wholesalerRepository.UpdateWholesalerStatus(id);
        }
        public (APIResponse, IQueryable<WholesalerViewModel>) GetWholesalerbyUserID()
        {
            return _wholesalerRepository.GetWholesalerbyUserID();
           
        }

        //public Task<APIResponse> UpdateCustomer(AddEditCustomerModel model)
        //{
        //    return _wholesalerRepository.UpdateCustomer(model);
        //}
        //public Task<APIResponse> GetAllWholesalerListss()
        //{
        //    return _wholesalerRepository.GetAllWholesalerList();
        //}

        //public Task<APIResponse> GetCustomerUserByCustomerID(long id)
        //{
        //    return _wholesalerRepository.GetCustomerUserByCustomerID(id);
        //}

        //public Task<APIResponse> GetCustomerUserByUserID(long id)
        //{
        //    return _wholesalerRepository.GetCustomerUserByUserID(id);
        //}

        public Task<AddEditWholesalerModel> GetWholesalerDetailByID(long id)
        {
            return _wholesalerRepository.GetWholesalerDetailByID(id);
        }  
        public Task<APIResponse> GetdeliverySchedulerByWholesaler(Int64 id)
        {
            return _wholesalerRepository.GetdeliverySchedulerByWholesaler(id);
        }
        public Task<Response<List<UserDetail>>> GetWholersalerCustomerUserListbysalerID(long id)
        
        {
            return _wholesalerRepository.GetWholersalerCustomerUserListbysalerID(id);
        }


    }
}
