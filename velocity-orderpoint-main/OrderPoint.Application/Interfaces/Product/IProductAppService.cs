using OrderPoint.Domain.Common;
using OrderPoint.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Application.Interfaces.Product
{
    public interface IProductAppService
    {
        (APIResponse, IQueryable<ProductsViewModel>) GetAllProducts(bool? isCustomer);
        (APIResponse, IQueryable<ProductsViewModel>) GetAllProductsBywholesaler(Int32 wholesalerID, Int64? categoryID);
        (APIResponse, IQueryable<ProductsViewModel>) GetAllCustomerProductsBywholesaler(Int32 wholesalerID, Int64? categoryID, Int64? customerID);
        (APIResponse, IQueryable<ProductsViewModel>) GetAllCategoryBywholesaler(Int32 wholesalerID);
        (APIResponse, IQueryable<ProductsViewModel>) GetAllCategoryByuserID(Int32 userID);
 
    }
}
