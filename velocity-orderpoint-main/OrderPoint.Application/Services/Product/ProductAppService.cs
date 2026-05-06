using OrderPoint.Application.Interfaces.Product;
using OrderPoint.Domain.Common;
using OrderPoint.Domain.Interfaces.Product;
using OrderPoint.Domain.Interfaces.User;
using OrderPoint.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Application.Services.Product
{
    public class ProductAppService : IProductAppService
    {
        private readonly IProductRepository _productRepository;

        public ProductAppService(IProductRepository productRepository) { _productRepository = productRepository; }
        public (APIResponse, IQueryable<ProductsViewModel>) GetAllProducts(bool? isCustomer)
        {
            if (isCustomer == true)
            {
                return _productRepository.GetAllCustomerProducts();
            }
            else
            {

            return _productRepository.GetAllProducts();
            }
        }
        public (APIResponse, IQueryable<ProductsViewModel>) GetAllProductsBywholesaler(Int32 wholesalerID, Int64? categoryID)
        {
            return _productRepository.GetAllProductsBywholesaler(wholesalerID, categoryID);
        }
        
        public (APIResponse, IQueryable<ProductsViewModel>) GetAllCustomerProductsBywholesaler(Int32 wholesalerID, Int64? categoryID, Int64? customerID)
        {
            return _productRepository.GetAllCustomerProductsBywholesaler(wholesalerID, categoryID, customerID);
        }
        public (APIResponse, IQueryable<ProductsViewModel>) GetAllCategoryBywholesaler(Int32 wholesalerID)
        {
            return _productRepository.GetAllCategoryBywholesaler(wholesalerID);
        }
        public (APIResponse, IQueryable<ProductsViewModel>) GetAllCategoryByuserID(Int32 userID)
        {
            return _productRepository.GetAllCategoryByuserID(userID);
        }
    }
}
