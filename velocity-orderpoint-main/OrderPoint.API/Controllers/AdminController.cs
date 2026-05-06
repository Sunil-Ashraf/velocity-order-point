using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderPoint.Domain.ViewModel;
using OrderPoint.Application.Interfaces.User;
using OrderPoint.Application.Services.User;
using OrderPoint.Domain.Entities;
using OrderPoint.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Azure;
using Microsoft.AspNetCore.SignalR;
using OrderPoint.API.Hubs;
namespace OrderPoint.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserAppService _userAppService;
        private readonly IHubContext<UserHub> _hub;
        public AdminController(IUserAppService userAppService, IHubContext<UserHub> hub)
        {
            _userAppService = userAppService;
            _hub = hub;
        }
        [HttpGet("GetAdminList")]
        public async Task<IActionResult> GetAdminsAsync([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string sortColumn = "ProductName",
        [FromQuery] string sortOrder = "desc",
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
        {
            
            var response = await _userAppService.GetAdminsAsync();
            if (!response.Item1.Success)
            {
                return BadRequest(response.Item1);
            }

            var query = response.Item2;

            // Apply searching
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(
                    s => (s.FirstName != null && s.FirstName.ToLower().Contains(search.ToLower()))
                      || (s.LastName != null && s.LastName.ToLower().Contains(search.ToLower()))
                      || (s.Email != null && s.Email.ToLower().Contains(search.ToLower()))
                );
            }

            // Apply sorting dynamically
            query = sortOrder.ToLower() == "desc"
                ? query.OrderByDescendingDynamic(sortColumn)
                : query.OrderByDynamic(sortColumn);

            // Get total count before pagination
            var totalRecords = query.Count();

            // Apply pagination
            var admins = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Return structured paginated response
            return Ok(new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = admins
            });
        }
        
        [HttpGet("GetAdminByID")]
        public async Task<IActionResult> GetAdminsAsync(String  userID)
        {
            var vmodel = await _userAppService.GetuserbyId(userID);
            return Ok(vmodel);
        }

        [HttpPut("Updateuser")]
        public async Task<IActionResult> Updateuser(EditUseViewModel model)
        {
            var vmodel = await _userAppService.UpdateUser(model);
            return Ok(vmodel);
        }
        [HttpGet("Removeuser")]
        public async Task<IActionResult> Removeuser(String userID, long? customerID, long? wholersalerID)
        {
            if (customerID.HasValue|| wholersalerID.HasValue)
            {
                var vmodel = await _userAppService.RemoveUser(userID, customerID, wholersalerID);

                if (vmodel.Success)
                {
                 //  await _hub.Clients.User(userID).SendAsync("ForceLogout");
                    await _hub.Clients.All.SendAsync("ForceLogout", new { type = "User", ID= userID });
                   // await _hub.Clients.All.SendAsync("ForceLogout");
                }
                return Ok(vmodel);
            }
            else
            {

                var vmodel = await _userAppService.RemoveUser(userID);
                 
                if (vmodel.Success)
                {
                    await _hub.Clients.All.SendAsync("ForceLogout", new { type = "User", ID = userID });
                }
                return Ok(vmodel);
            }
        }
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string sortColumn = "FirstName",
            [FromQuery] string sortOrder = "asc",
            [FromQuery] string? userType = null

            )
        {
            try
            {
                var response =  await _userAppService.GetAllUser(userType);

                if (!response.Item1.Success)
                {
                    return BadRequest(response.Item1);
                }
                var query = response.Item2;

                // Apply searching
                if (!string.IsNullOrEmpty(search))
                {
                    var searchTerms = search.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    foreach (var term in searchTerms)
                    {
                        query = query.Where(s =>
                            (s.FirstName != null && s.FirstName.ToLower().Contains(term)) ||
                            (s.LastName != null && s.LastName.ToLower().Contains(term)) ||
                            (s.Email != null && s.Email.ToLower().Contains(term)) ||
                            (s.UserType != null && s.UserType.ToLower().Contains(term))
                            ||
                            (s.DateCreated != null && Convert.ToString(s.UserType).Contains(term))
                             ||
                            (s.LastLogin != null && Convert.ToString(s.LastLogin).Contains(term))
                        );
                    }
                }


                // Apply sorting dynamically
                query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescendingDynamic(sortColumn)
                    : query.OrderByDynamic(sortColumn);

                // Get total count before pagination
                var totalRecords = query.Count();//await query.CountAsync();

                //// Apply pagination
                //var products = await query
                //    .Skip((pageNumber - 1) * pageSize)
                //    .Take(pageSize)
                //    .ToListAsync();

                //// Return structured paginated response
                //return Ok(new
                //{
                //    TotalRecords = totalRecords,
                //    PageNumber = pageNumber,
                //    PageSize = pageSize,
                //    Data = products
                //});
                var paginated = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

                return Ok(new
                {
                    TotalRecords = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Data = paginated
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet("GetUserLinksByRoles")]
        public async Task<IActionResult> GetUserLinksByRoles(String userID, String UserType)
        {
            var vmodel = await _userAppService.GetUserLinksByRoles(userID, UserType);
            return Ok(vmodel);
        }

        [HttpGet("GetuserListbyUserType")]
        public async Task<IActionResult> GetuserListbyUserType(String UserType)
        {
            var vmodel = await _userAppService.GetuserListbyUserType(UserType);
            return Ok(vmodel);
        }
      


    }
}
