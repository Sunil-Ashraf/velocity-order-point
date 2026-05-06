using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderPoint.Application.Interfaces.Customers;
using OrderPoint.Application.Services.Customers;
using OrderPoint.Domain.Common;
using OrderPoint.Domain.Entities;
using OrderPoint.Domain.ViewModel;

namespace OrderPoint.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerAppService _customerAppService;
        public CustomerController(ICustomerAppService productAppService)
        {
            _customerAppService = productAppService;
        }
        [HttpGet("GetAllCustomers")]
        public async Task<IActionResult> GetAllCustomers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string sortColumn = "ID",
        [FromQuery] string sortOrder = "desc" ,
        [FromQuery] long? wholesalerID = null
        
        )
        {
            try
            {
                var response = _customerAppService.GetAllCustomers(wholesalerID);
                if (!response.Item1.Success)
                {
                    return BadRequest(response.Item1);
                }
                var query = response.Item2; // IQueryable<CustomerModel>

                if (!string.IsNullOrWhiteSpace(search))
                {
                    string lowerSearch = search.ToLower();

                    query = query.Where(s =>
                        s.Id.ToString().Contains(lowerSearch)
                        || (!string.IsNullOrEmpty(s.CustomerName) && s.CustomerName.ToLower().Contains(lowerSearch))
                        || (!string.IsNullOrEmpty(s.WholesalerName) && s.WholesalerName.ToLower().Contains(lowerSearch))
                        || (s.WholesalerID.HasValue && s.WholesalerID.Value.ToString().Contains(lowerSearch))
                        || (!string.IsNullOrEmpty(s.WholesalerReference) && s.WholesalerReference.ToLower().Contains(lowerSearch))
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


        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> CreateCustomer(AddEditCustomerModel model)
        {
            if (model.Id > 0)
            {
                var vmodel = _customerAppService.UpdateCustomer(model);
                return Ok(vmodel);
            }
            else
            {
                var vmodel = _customerAppService.CreateCustomer(model);
                return Ok(vmodel);
                
            }
            
            
        }

       
        [HttpGet("GetCustomerByID")]
        public async Task<IActionResult> GetCustomerByID(Int32 Id)
        {
            var vmodel = await _customerAppService.GetCustomerbyId(Id);
            return Ok(vmodel);
        }

        [HttpPut("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer(AddEditCustomerModel model)
        {
            var vmodel = _customerAppService.UpdateCustomer(model);
            return Ok(vmodel);
        }
        [HttpGet("GetAllWholesalerList")]
        public async Task<IActionResult> GetAllWholesalerList()
        {
            var vmodel = await _customerAppService.GetAllWholesalerList();
            return Ok(vmodel);
        }
        [HttpGet("GetCustomerUserByCustomerID")]
        public async Task<IActionResult> GetCustomerUserByCustomerID(Int32 customerID)
        {
            var vmodel = await _customerAppService.GetCustomerUserByCustomerID(customerID);
            return Ok(vmodel);
        }
        [HttpGet("GetCustomerUserByUserID")]
        public async Task<IActionResult> GetCustomerUserByUserID(Int32 userID)
        {
            var vmodel = await _customerAppService.GetCustomerUserByUserID(userID);
            return Ok(vmodel);
        }
        [HttpGet("AddCustomerLinks")]
        public async Task<IActionResult> AddCustomerLinks(Int32 userID, Int32 customerID)
        {
            var vmodel = await _customerAppService.AddCustomerLinks(userID,customerID);
            return Ok(vmodel);
        }

        [HttpGet("GetWholersalerCustomer")]
        public async Task<IActionResult> GetWholersalerCustomer(
     [FromQuery] int pageNumber = 1,
     [FromQuery] int pageSize = 10,
     [FromQuery] string? search = null,
     [FromQuery] string sortColumn = "CustomerName",
     [FromQuery] string sortOrder = "asc"
     

     )
        {
            try
            {
                var response = _customerAppService.GetWholesalerCustomers();
                if (!response.Item1.Success)
                {
                    return BadRequest(response.Item1);
                }
                var query = response.Item2; // IQueryable<CustomerModel>

                if (!string.IsNullOrWhiteSpace(search))
                {
                    string lowerSearch = search.ToLower();

                    query = query.Where(s =>
                        s.Id.ToString().Contains(lowerSearch)
                        || (!string.IsNullOrEmpty(s.CustomerName) && s.CustomerName.ToLower().Contains(lowerSearch))
                        || (!string.IsNullOrEmpty(s.WholesalerName) && s.WholesalerName.ToLower().Contains(lowerSearch))
                        || (s.WholesalerID.HasValue && s.WholesalerID.Value.ToString().Contains(lowerSearch))
                        || (!string.IsNullOrEmpty(s.WholesalerReference) && s.WholesalerReference.ToLower().Contains(lowerSearch))
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
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder(OrderPlacementModel model)
        {
            var vmodel =  await _customerAppService.CreateOrder(model);

            //var vitemmodel =   _customerAppService.AddOrderItems(model);
            return Ok(vmodel);
        }
        [HttpPost("SendOrdrEmail")]
        public async Task<IActionResult> SendOrdrEmail(OrderPlacementModel model)
        {
           

            var vitemmodel = await _customerAppService.AddOrderItems(model);
            return Ok(vitemmodel);
        }
    }
}
