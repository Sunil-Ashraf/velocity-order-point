using OrderPoint.Domain.Common;
using OrderPoint.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.Interfaces.Product
{
    public interface IProductRepository
    {
        (APIResponse, IQueryable<ProductsViewModel>) GetAllProducts();
        (APIResponse, IQueryable<ProductsViewModel>) GetAllCustomerProducts();
        (APIResponse, IQueryable<ProductsViewModel>) GetAllProductsBywholesaler(Int32 wholesalerID, Int64? categoryID);
        (APIResponse, IQueryable<ProductsViewModel>) GetAllCustomerProductsBywholesaler(Int32 wholesalerID, Int64? categoryID, Int64? customerID);
        (APIResponse, IQueryable<ProductsViewModel>) GetAllCategoryBywholesaler(Int32 wholesalerID);
        (APIResponse, IQueryable<ProductsViewModel>) GetAllCategoryByuserID(Int32 userID);
    }
}
