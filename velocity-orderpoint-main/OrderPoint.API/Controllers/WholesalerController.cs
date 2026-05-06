using Azure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OrderPoint.API.Hubs;
using OrderPoint.Application.Interfaces.Customers;
using OrderPoint.Application.Services.Customers;
using OrderPoint.Domain.Common;
using OrderPoint.Domain.Entities;
using OrderPoint.Domain.ViewModel;
using System.Collections.Generic;

namespace OrderPoint.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class WholesalerController : ControllerBase
    {

        private readonly IWholesalerAppService _wholesalerAppService;
        private readonly IHubContext<UserHub> _hub;
        public WholesalerController(IWholesalerAppService productAppService, IHubContext<UserHub> hub)
        {
            _wholesalerAppService = productAppService;
            _hub = hub;
        }
        [HttpGet("GetAllWholesalerList")]
        public async Task<IActionResult> GetAllWholesalerList(
       [FromQuery] int pageNumber = 1,
       [FromQuery] int pageSize = 10,
       [FromQuery] string? search = null,
       [FromQuery] string sortColumn = "ID",
       [FromQuery] string sortOrder = "desc"
       )
        {
            try
            {
                var response = _wholesalerAppService.GetAllWholesalerList();
                if (!response.Item1.Success)
                {
                    return BadRequest(response.Item1);
                }
                var query = response.Item2; // IQueryable<CustomerModel>

                if (!string.IsNullOrWhiteSpace(search))
                {
                    string lowerSearch = search.ToLower();

                    query = query.Where(s =>
                        s.ID.ToString().Contains(lowerSearch)
                        || (!string.IsNullOrEmpty(s.Name) && s.Name.ToLower().Contains(lowerSearch))
                        || (!string.IsNullOrEmpty(s.Status) && s.Status.ToLower().Contains(lowerSearch))


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

        [HttpPost("AddEditWholesaler")]
        public async Task<IActionResult> AddEditWholesaler([FromForm] AddEditWholesalerModel model)
        {
            if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
            {
                // Get the wwwroot path
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "WholesalePictures");
                // Ensure the directory exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                // Generate a unique file name
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfilePicture.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);
                model.ImagePath = fileName;

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePicture.CopyToAsync(stream);
                }
            }
            if (model.BannerPicture != null && model.BannerPicture.Length > 0)
            {
                // Get the wwwroot path
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "WholesalePictures");
                // Ensure the directory exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                // Generate a unique file name
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.BannerPicture.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);
                model.BannerImagePath = fileName;

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.BannerPicture.CopyToAsync(stream);
                }
            }
            else
            {

            }
            var response = await _wholesalerAppService.AddEditWholesaler(model);
            if (!response.Success)
                return BadRequest(response);
            if (response.Success)
            {
                await _hub.Clients.All.SendAsync("ForceLogout", new { type = "WholeSalerSession", ID = model.ID.ToString()});

            }

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("GetWholesalerByID")]
        public async Task<IActionResult> GetWholesalerByID(Int32 Id)
        {
            var vmodel = await _wholesalerAppService.GetWholesalerByID(Id);
            return Ok(vmodel);
        }
        [AllowAnonymous]
        [HttpGet("GetWholesalerByName")]
        public async Task<IActionResult> GetWholesalerByName(String name)
        {
            var vmodel = await _wholesalerAppService.GetWholesalerByName(name);
            return Ok(vmodel);
        }
        [HttpGet("GetWholesaleUserListBysalerID")]
        public async Task<IActionResult> GetWholesaleUserListBysalerID(Int32 id)
        {
            var vmodel = await _wholesalerAppService.GetWholesaleUserListBysalerID(id);
            return Ok(vmodel);
        }
        [HttpGet("GetWholesalerUserByUserID")]
        public async Task<IActionResult> GetWholesalerUserByUserID(Int32 userID)
        {
            var vmodel = await _wholesalerAppService.GetWholesalerUserByUserID(userID);
            return Ok(vmodel);
        }
        [HttpGet("AddWholesalerLinks")]
        public async Task<IActionResult> AddWholesalerLinks(Int32 userID, Int32 wholesalerID)
        {
            var vmodel = await _wholesalerAppService.AddWholesalerLinks(userID, wholesalerID);
            return Ok(vmodel);
        }

        [HttpGet("UpdateWholesalerStatus")]
        public async Task<IActionResult> UpdateWholesalerStatus(Int32 id)
        {
            var vmodel = await _wholesalerAppService.UpdateWholesalerStatus(id);
            if (vmodel.Success)
            {
                  // var response = await _wholesalerAppService.GetWholersalerCustomerUserListbysalerID(id);
                  // List<UserDetail> userList = response.Data;

               // string userIDs = string.Join(",", userList.Select(k => k.ID));

                await _hub.Clients.All.SendAsync("ForceLogout", new { type = "WholeSaler", ID = id.ToString() });
            }
            return Ok(vmodel);
        }

        [HttpGet("GetWholesalerbyUserID")]
        public async Task<IActionResult> GetWholesalerbyUserID(
     [FromQuery] int pageNumber = 1,
     [FromQuery] int pageSize = 10,
     [FromQuery] string? search = null,
     [FromQuery] string sortColumn = "ID",
     [FromQuery] string sortOrder = "desc"
     )
        {
            try
            {
                var response = _wholesalerAppService.GetWholesalerbyUserID();
                if (!response.Item1.Success)
                {
                    return BadRequest(response.Item1);
                }
                var query = response.Item2; // IQueryable<CustomerModel>

                if (!string.IsNullOrWhiteSpace(search))
                {
                    string lowerSearch = search.ToLower();

                    query = query.Where(s =>
                        s.ID.ToString().Contains(lowerSearch)
                        || (!string.IsNullOrEmpty(s.Name) && s.Name.ToLower().Contains(lowerSearch))
                        || (!string.IsNullOrEmpty(s.Status) && s.Status.ToLower().Contains(lowerSearch))


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


        [HttpPost("SetWholesalerDelveryTime")]
        public async Task<IActionResult> SetWholesalerDelveryTime([FromBody] DeliveryOptionVM model)
        {


            var response = await _wholesalerAppService.SetWholesalerDeliverytime(model);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);

        }

        [HttpGet("GetDeliveryScheduleByWholesaler")]
        public async Task<IActionResult> GetDeliveryScheduleByWholesaler(Int32 wholesalerID)
        {
            var response = await _wholesalerAppService.GetdeliverySchedulerByWholesaler(wholesalerID);
            return Ok(response);
        }

    }

}
