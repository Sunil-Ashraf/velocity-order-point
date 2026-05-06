using Microsoft.AspNetCore.Identity;
using OrderPoint.Domain.Common;
using OrderPoint.Domain.DbContexts.Repositories;
using OrderPoint.Domain.Entities;
using OrderPoint.Domain.Helper;
using OrderPoint.Domain.Interfaces.Product;
using OrderPoint.Domain.ModifyIdentity;
using OrderPoint.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OrderPoint.Persistence.Repositories.Product
{
    public class ProductRepository : IProductRepository
    {
        private readonly IRepository<OrderPoint.Domain.Entities.Product> _productRepository;
        private readonly IRepository<OrderPoint.Domain.Entities.CustomerProducts> _CustomerProductsRepository;
        private readonly IRepository<OrderPoint.Domain.Entities.WholesalerUser> _wholesalerUserRepository;
        private readonly IRepository<OrderPoint.Domain.Entities.CustomerUser> _customerUserRepository;
        private readonly IRepository<OrderPoint.Domain.Entities.Customer> _customerRepository;
        private readonly IRepository<OrderPoint.Domain.Entities.Category> _categoryRepository;
        private readonly UserHelper _userHelper;
        public ProductRepository(UserHelper userHelper, IRepository<OrderPoint.Domain.Entities.Product> productRepository, IRepository<WholesalerUser> wholesalerUserRepository , IRepository<OrderPoint.Domain.Entities.Category> categoryRepository, IRepository<CustomerUser> customerUserRepository, IRepository<Customer> customerRepository,
            IRepository<OrderPoint.Domain.Entities.CustomerProducts> customerProductsRepository
            )
        {
            _userHelper = userHelper;
            _productRepository = productRepository;
            _wholesalerUserRepository = wholesalerUserRepository;
            _categoryRepository = categoryRepository;
            _customerUserRepository = customerUserRepository;
            _customerRepository = customerRepository;
            _CustomerProductsRepository= customerProductsRepository;

        }
        public (APIResponse, IQueryable<ProductsViewModel>) GetAllProducts()
        {
            try
            {
                var userId = _userHelper.GetUserId();
               
                if (!long.TryParse(userId, out var parsedUserId))
                    return (APIResponse.Create(false, "Invalid or missing user ID."), new List<ProductsViewModel>().AsQueryable());
                var categories = _categoryRepository.GetAll();
                var products = from ws in _wholesalerUserRepository.GetAll()
                               join p in _productRepository.GetAll()
                                   on ws.WholesalerID equals Convert.ToInt64(p.WholesalerID)
                               join cat in categories
                              on new { p.CategoryID, WholesalerID = Convert.ToInt64(p.WholesalerID) }
                              equals new { CategoryID = cat.ID, WholesalerID = Convert.ToInt64(cat.WholesalerID) }
                              into catJoin
                               from cat in catJoin.DefaultIfEmpty()
                               where ws.UserID == parsedUserId
                               select new ProductsViewModel
                               {
                                   Id = p.ID,
                                   ProductName = p.Name,
                                   Description = p.Description,
                                   CategoryId = p.CategoryID,
                                   CategoryName = cat != null ? cat.Name : null,
                                   QuantityTypeName = p.QuantityType == 1 ? "Per Item" : "Kg",
                                   QuantityTypeId = p.QuantityType,
                               };


                return (APIResponse.Create(true), products);
            }
            catch (Exception ex)
            {
                return (APIResponse.Create(false, $"Error: {ex.Message}"), new List<ProductsViewModel>().AsQueryable());
            }
        } 
        public (APIResponse, IQueryable<ProductsViewModel>) GetAllCustomerProducts()
        {
            try
            {
                var userId = _userHelper.GetUserId();
                
                if (!long.TryParse(userId, out var parsedUserId))
                    return (APIResponse.Create(false, "Invalid or missing user ID."), new List<ProductsViewModel>().AsQueryable());

                var products = from cUser in _customerUserRepository.GetAll()
                               join c in _customerRepository.GetAll() on cUser.CustomerID equals  c.ID
                               join product in _productRepository.GetAll() on c.WholesalerID equals Convert.ToInt64(product.WholesalerID)
                               //join category in _categoryRepository.GetAll()
                               //   on product.CategoryID equals category.ID into categoryJoin
                               //from cat in categoryJoin.DefaultIfEmpty()
                               where cUser.UserID == parsedUserId
                               select new ProductsViewModel
                               {
                                   Id = product.ID,
                                   ProductName = product.Name,
                                   Description = product.Description,
                                   CategoryId = product.CategoryID,
                                   QuantityTypeId = product.QuantityType,
                                   QuantityTypeName = product.QuantityType == 1 ? "Per Item" : "Kg",
                                   Price = product.Price 
                                   //CategoryName = c != null ? c.Name : null,

                               };

                return (APIResponse.Create(true), products);
            }
            catch (Exception ex)
            {
                return (APIResponse.Create(false, $"Error: {ex.Message}"), new List<ProductsViewModel>().AsQueryable());
            }
        }
        public (APIResponse, IQueryable<ProductsViewModel>) GetAllProductsBywholesaler(Int32 wholesalerID, Int64? categoryID)
        {
            try
            {
                var categories = _categoryRepository.GetAll();
                var productlist = _productRepository.GetAll();

                var products = from product in productlist
                               join cat in categories
                               on new { product.CategoryID, WholesalerID =  Convert.ToInt64(product.WholesalerID) }
                               equals new { CategoryID = cat.ID, WholesalerID = Convert.ToInt64(cat.WholesalerID) }
                               into catJoin
                               from cat in catJoin.DefaultIfEmpty()
                               where Convert.ToInt64(product.WholesalerID) == wholesalerID
                               && (!categoryID.HasValue || product.CategoryID == categoryID.Value)
                               select new ProductsViewModel
                               {
                                   Id = product.ID,
                                   ProductName = product.Name,
                                   Description = product.Description,
                                   CategoryId = product.CategoryID,
                                   QuantityTypeId = product.QuantityType,
                                   QuantityTypeName  = product.QuantityType==1 ?"Per Item": "Kg",
                                   CategoryName = cat != null ? cat.Name : null,
                                   Price = product.Price
                               };



                 
                return (APIResponse.Create(true), products);
            }
            catch (Exception ex)
            {
                return (APIResponse.Create(false, $"Error: {ex.Message}"), new List<ProductsViewModel>().AsQueryable());
            }
        }
     
        public (APIResponse, IQueryable<ProductsViewModel>) GetAllCustomerProductsBywholesaler(Int32 wholesalerID, Int64? categoryID, Int64? customerID)
        {
            try
            {
                
               

                var products = from cp in _CustomerProductsRepository.GetAll()
                            join p in _productRepository.GetAll()
                                on new { ProductID = (long)cp.ProductID, WholesalerID = cp.WholesalerID.ToString() }
                                equals new { ProductID = p.ID, p.WholesalerID }
                            join c in _categoryRepository.GetAll()
                                on new { CategoryID = p.CategoryID, WholesalerID = p.WholesalerID }
                                equals new { CategoryID = c.ID, WholesalerID = c.WholesalerID.ToString() }
                                into catJoin
                            from c in catJoin.DefaultIfEmpty()

                               where Convert.ToInt64(p.WholesalerID) == wholesalerID && cp.CustomerID == customerID
                                 && (!categoryID.HasValue || p.CategoryID == categoryID.Value)
                               select new ProductsViewModel
                            {
                                Id = p.ID,
                                ProductName = p.Name,
                                Description = p.Description,
                                CategoryId = p.CategoryID,
                                QuantityTypeId = p.QuantityType,
                                QuantityTypeName = p.QuantityType == 1 ? "Per Item" : "Kg",
                                CategoryName = c != null ? c.Name : null,
                                Price = cp.Price
                            };
                








                return (APIResponse.Create(true), products);
            }
            catch (Exception ex)
            {
                return (APIResponse.Create(false, $"Error: {ex.Message}"), new List<ProductsViewModel>().AsQueryable());
            }
        }


        public (APIResponse, IQueryable<ProductsViewModel>) GetAllCategoryBywholesaler(Int32 wholesalerID)
        {
            try
            {
                var categoryList = _categoryRepository.GetAll()
                    .Where(cat => cat.WholesalerID == wholesalerID)
                    .Select(cat => new ProductsViewModel
                    {
                        Id = cat.ID,
                        CategoryName = cat.Name
                    });

                return (APIResponse.Create(true), categoryList);
            }
            catch (Exception ex)
            {
                return (APIResponse.Create(false, $"Error: {ex.Message}"), new List<ProductsViewModel>().AsQueryable());
            }
        }

        public (APIResponse, IQueryable<ProductsViewModel>) GetAllCategoryByuserID(Int32 userID)
        {
            try
            {
                var categoryList = from wholeSalerUser in _wholesalerUserRepository.GetAll()
                               join cat in _categoryRepository.GetAll() on wholeSalerUser.WholesalerID equals Convert.ToInt64(cat.WholesalerID)
                               where wholeSalerUser.UserID == userID
                               select new ProductsViewModel
                               {
                                   Id = cat.ID,
                                   CategoryName = cat.Name
                               };

                 

                return (APIResponse.Create(true), categoryList);
            }
            catch (Exception ex)
            {
                return (APIResponse.Create(false, $"Error: {ex.Message}"), new List<ProductsViewModel>().AsQueryable());
            }
        }

        

    }
}
