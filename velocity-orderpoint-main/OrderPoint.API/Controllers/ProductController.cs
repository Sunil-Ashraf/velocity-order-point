using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OrderPoint.Application.Interfaces.Customers;
using OrderPoint.Application.Interfaces.Product;
using OrderPoint.Application.Services.Customers;
using OrderPoint.Domain.Common;
using OrderPoint.Domain.ViewModel;

namespace OrderPoint.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductAppService _productAppService;
        private readonly IWholesalerAppService _wholesalertAppService;
        public ProductController(IProductAppService productAppService, IWholesalerAppService wholesalertAppService)
        {
            _productAppService = productAppService;
            _wholesalertAppService = wholesalertAppService;
        }
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string sortColumn = "ProductName",
        [FromQuery] string sortOrder = "desc",
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] bool? isCustomer = false
        )
        {
            try
            {
                var response = _productAppService.GetAllProducts(isCustomer);
                if (!response.Item1.Success)
                {
                    return BadRequest(response.Item1);
                }
                var query = response.Item2;

                // Apply searching
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(
                    //s => (s.Id > 0 && s.Id.ToString().Contains(search))
                    s => (s.ProductName != null && s.ProductName.ToLower().Contains(search.ToLower()))
                    || (s.Description != null && s.Description.ToLower().Contains(search.ToLower()))
                    //|| (s.CategoryName != null && s.CategoryName.ToLower().Contains(search.ToLower()))
                    //|| (s.QuantityTypeName != null && s.QuantityTypeName.ToLower().Contains(search.ToLower()))
                    );
                }
                // Filter by date range if provided
                //if (startDate.HasValue)
                //{
                //    query = query.Where(d => d.CreationTime >= startDate.Value);
                //}

                //if (endDate.HasValue)
                //{
                //    query = query.Where(d => d.CreationTime <= endDate.Value);
                //}
                // Apply sorting dynamically
                query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescendingDynamic(sortColumn)
                    : query.OrderByDynamic(sortColumn);

                // Get total count before pagination
                var totalRecords = await query.CountAsync();

                // Apply pagination
                var products = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                // Return structured paginated response
                return Ok(new
                {
                    TotalRecords = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Data = products
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

    
        [HttpGet("GetAllProductsBywholesaler")]
        public async Task<IActionResult> GetAllProductsBywholesaler(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string sortColumn = "ProductName",
        [FromQuery] string sortOrder = "desc",
        [FromQuery] Int32 wholesalerID=1 ,
        [FromQuery] Int64? categoryID=null  
        )
        {
            try
            {
                var response = _productAppService.GetAllProductsBywholesaler(wholesalerID, categoryID);
                if (!response.Item1.Success)
                {
                    return BadRequest(response.Item1);
                }
                var query = response.Item2;

                // Apply searching
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(
                    //s => (s.Id > 0 && s.Id.ToString().Contains(search))
                    s => (s.ProductName != null && s.ProductName.ToLower().Contains(search.ToLower()))
                    || (s.Description != null && s.Description.ToLower().Contains(search.ToLower()))
                    //|| (s.CategoryName != null && s.CategoryName.ToLower().Contains(search.ToLower()))
                    //|| (s.QuantityTypeName != null && s.QuantityTypeName.ToLower().Contains(search.ToLower()))
                    );
                }
                // Filter by date range if provided
                //if (startDate.HasValue)
                //{
                //    query = query.Where(d => d.CreationTime >= startDate.Value);
                //}

                //if (endDate.HasValue)
                //{
                //    query = query.Where(d => d.CreationTime <= endDate.Value);
                //}
                // Apply sorting dynamically
                query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescendingDynamic(sortColumn)
                    : query.OrderByDynamic(sortColumn);

                // Get total count before pagination
                var totalRecords = await query.CountAsync();

                // Apply pagination
                var products = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                // Return structured paginated response
                return Ok(new
                {
                    TotalRecords = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Data = products
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetAllCategoryBywholesaler")]
        public async Task<IActionResult> GetAllCategoryBywholesaler(
 [FromQuery] int pageNumber = 1,
 [FromQuery] int pageSize = 10,
 [FromQuery] string? search = null,
 [FromQuery] string sortColumn = "CategoryName",
 [FromQuery] string sortOrder = "desc",
 [FromQuery] Int32 wholesalerID = 1,
 [FromQuery] bool isGetAllCategory = false
            )
        {
            try
            {
                var response = _productAppService.GetAllCategoryBywholesaler(wholesalerID);
                if (!response.Item1.Success)
                {
                    return BadRequest(response.Item1);
                }
               
                var query = response.Item2;

                if (isGetAllCategory == true) // Get all Recod with out pagination
                {
                    var wholesalerData = await _wholesalertAppService.GetWholesalerDetailByID(wholesalerID);

                

                    


                    var productss = await query.ToListAsync();
                    // Return structured paginated response
                    return Ok(new
                    {
                        TotalRecords = productss.Count(),
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        Data = productss,
                        WholesalerDetail= wholesalerData
                    });
                }
                // Apply searching
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(
                    //s => (s.Id > 0 && s.Id.ToString().Contains(search))
                    s => (s.CategoryName != null && s.CategoryName.ToLower().Contains(search.ToLower()))
                    //|| (s.Description != null && s.Id.ToString().ToLower().Contains(search.ToLower()))
                    //|| (s.CategoryName != null && s.CategoryName.ToLower().Contains(search.ToLower()))
                    //|| (s.QuantityTypeName != null && s.QuantityTypeName.ToLower().Contains(search.ToLower()))
                    );
                }
                // Filter by date range if provided
                //if (startDate.HasValue)
                //{
                //    query = query.Where(d => d.CreationTime >= startDate.Value);
                //}

                //if (endDate.HasValue)
                //{
                //    query = query.Where(d => d.CreationTime <= endDate.Value);
                //}
                // Apply sorting dynamically
                query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescendingDynamic(sortColumn)
                    : query.OrderByDynamic(sortColumn);

                // Get total count before pagination
                var totalRecords = await query.CountAsync();

                

              
                 // Apply pagination
                
                    var products = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                    // Return structured paginated response
                    return Ok(new
                    {
                        TotalRecords = totalRecords,
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        Data = products
                    });
                 
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetAllCategoryByUserID")]
        public async Task<IActionResult> GetAllCategoryByUserID(
[FromQuery] int pageNumber = 1,
[FromQuery] int pageSize = 10,
[FromQuery] string? search = null,
[FromQuery] string sortColumn = "CategoryName",
[FromQuery] string sortOrder = "desc",
[FromQuery] Int32 userID = 0,
[FromQuery] bool isGetAllCategory = false
         )
        {
            try
            {
                var response = _productAppService.GetAllCategoryByuserID(userID);
                if (!response.Item1.Success)
                {
                    return BadRequest(response.Item1);
                }

                var query = response.Item2;

                
                // Apply searching
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(
                    //s => (s.Id > 0 && s.Id.ToString().Contains(search))
                    s => (s.CategoryName != null && s.CategoryName.ToLower().Contains(search.ToLower()))
                    //|| (s.Description != null && s.Id.ToString().ToLower().Contains(search.ToLower()))
                    //|| (s.CategoryName != null && s.CategoryName.ToLower().Contains(search.ToLower()))
                    //|| (s.QuantityTypeName != null && s.QuantityTypeName.ToLower().Contains(search.ToLower()))
                    );
                }
                // Filter by date range if provided
                //if (startDate.HasValue)
                //{
                //    query = query.Where(d => d.CreationTime >= startDate.Value);
                //}

                //if (endDate.HasValue)
                //{
                //    query = query.Where(d => d.CreationTime <= endDate.Value);
                //}
                // Apply sorting dynamically
                query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescendingDynamic(sortColumn)
                    : query.OrderByDynamic(sortColumn);

                // Get total count before pagination
                var totalRecords = await query.CountAsync();




                // Apply pagination

                var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
                // Return structured paginated response
                return Ok(new
                {
                    TotalRecords = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Data = products
                });

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetAllCustomerProductsBywholesaler")]
        public async Task<IActionResult> GetAllCustomerProductsBywholesaler(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string sortColumn = "ProductName",
        [FromQuery] string sortOrder = "desc",
        [FromQuery] Int32 wholesalerID = 1,
        [FromQuery] Int64? categoryID = null,
        [FromQuery] Int64? customerID = null
        )
        {
            try
            {
                var response = _productAppService.GetAllCustomerProductsBywholesaler(wholesalerID, categoryID, customerID);
                if (!response.Item1.Success)
                {
                    return BadRequest(response.Item1);
                }
                var query = response.Item2;

                // Apply searching
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(
                    //s => (s.Id > 0 && s.Id.ToString().Contains(search))
                    s => (s.ProductName != null && s.ProductName.ToLower().Contains(search.ToLower()))
                    || (s.Description != null && s.Description.ToLower().Contains(search.ToLower()))
                    //|| (s.CategoryName != null && s.CategoryName.ToLower().Contains(search.ToLower()))
                    //|| (s.QuantityTypeName != null && s.QuantityTypeName.ToLower().Contains(search.ToLower()))
                    );
                }
                // Filter by date range if provided
                //if (startDate.HasValue)
                //{
                //    query = query.Where(d => d.CreationTime >= startDate.Value);
                //}

                //if (endDate.HasValue)
                //{
                //    query = query.Where(d => d.CreationTime <= endDate.Value);
                //}
                // Apply sorting dynamically
                query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescendingDynamic(sortColumn)
                    : query.OrderByDynamic(sortColumn);

                // Get total count before pagination
                var totalRecords = await query.CountAsync();

                if ((pageNumber - 1) * pageSize >= totalRecords)
               {
                    pageNumber = 1; // reset to first page OR show empty
                }
                // Apply pagination
                var products = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                // Return structured paginated response
                return Ok(new
                {
                    TotalRecords = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Data = products
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
