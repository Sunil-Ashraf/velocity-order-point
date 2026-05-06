using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderPoint.Application.Interfaces.Customers;
using OrderPoint.Application.Interfaces.Email;
using OrderPoint.Application.Services.Customers;
using OrderPoint.Domain.Common;
using OrderPoint.Domain.Constant;
using OrderPoint.Domain.Entities;
using OrderPoint.Domain.ViewModel;
using System.Collections.Generic;

namespace OrderPoint.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class EmailTemplatesController : ControllerBase
    {
        private readonly IEmailTemplatesAppService _emailTemplatesAppService;
        public EmailTemplatesController(IEmailTemplatesAppService emailTemplatesAppService)
        {
            _emailTemplatesAppService = emailTemplatesAppService;
        }



        [HttpGet("GetEmailTypes")]
        public async Task<IActionResult> GetEmailTypes()
        {
            var response = await _emailTemplatesAppService.GetListsByTypeName(AppLists.EmailTypes);
            if (!response.Item1.Success)
            {
                return BadRequest(response.Item1);
            }
            var query = response.Item2;

            return Ok(APIResponse.Create(true, query));
        }

        [HttpGet("GetEmailPlaceholder")]
        public async Task<IActionResult> GetEmailPlaceholder()
        {
            var response = await _emailTemplatesAppService.GetListsByTypeName(AppLists.EmailPlaceholder);
            if (!response.Item1.Success)
            {
                return BadRequest(response.Item1);
            }
            var query = response.Item2;

            return Ok(APIResponse.Create(true, query));
        }

        [HttpGet("GetTemplatesAll")]
        public async Task<IActionResult> GetTemplatesAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string sortColumn = "CreationTime",
            [FromQuery] string sortOrder = "desc",
            [FromQuery] Int32? wholesalerID  =null
            )
        {
            try
            {
                var response =  _emailTemplatesAppService.GetTemplatesAll(wholesalerID);

                if (!response.Item1.Success)
                {
                    return BadRequest(response.Item1);
                }

                var query = response.Item2;
                // Apply searching
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(
                        s => (s.Name != null && s.Name.ToLower().Contains(search.ToLower()))
                    || (s.Subject != null && s.Subject.ToLower().Contains(search.ToLower()))
                    );
                }

                // Apply sorting dynamically

                query = sortOrder.ToLower() == "desc"
                    ? query.OrderByDescendingDynamic(sortColumn)
                    : query.OrderByDynamic(sortColumn);

                // Get total count before pagination
                    
                      var totalRecords = await query.CountAsync();

                // Apply pagination
                var emails = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                // Return structured paginated response
                return Ok(new
                {
                    TotalRecords = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Data = emails
                });
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpPost("CreateTemplate")]
        public async Task<IActionResult> CreateTemplate([FromBody] EmailTemplateViewModel model)
        {
            var response = await _emailTemplatesAppService.Create(model);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpPut("UpdateTemplate")]
        public async Task<IActionResult> UpdateTemplate([FromBody] EmailTemplateViewModel model)
        {
            var response = await _emailTemplatesAppService.Update(model);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpDelete("DeleteTemplate/{Id}")]
        public async Task<IActionResult> DeleteTemplate(Guid Id)
        {
            try
            {
                var response = await _emailTemplatesAppService.Delete(Id);
                if (!response.Success)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
            {
                var sqlException = e.GetBaseException() as Microsoft.Data.SqlClient.SqlException;
                if (sqlException != null)
                {
                    if (sqlException.Errors.Count > 0)
                    {
                        switch (sqlException.Errors[0].Number)
                        {
                            case 547: // Foreign Key violation}
                                return BadRequest(new APIResponse { Success = false, Message = "The data contained in this record is being used by the system and cannot be deleted at this time." });
                        }
                    }
                }
                return BadRequest(new APIResponse { Success = false, Message = "Something went wrong. Please try again." });
            }
            catch (Exception ex)
            {
                return BadRequest(new APIResponse { Success = false, Message = "Something went wrong. Please try again." });
            }
        }
        [HttpGet("SetAsDefaultTemplate/{Id}")]
        public async Task<IActionResult> SetAsDefaultTemplate(Guid Id)
        {
            var response = await _emailTemplatesAppService.SetTemplateAsDefault(Id);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }


        [HttpGet("GetSystemSettingList")]
        public async Task<IActionResult> GetSystemSettingList()
        {
            var response = await _emailTemplatesAppService.GetSystemSettingList();
            if (!response.Item1.Success)
            {
                return BadRequest(response.Item1);
            }
            var query = response.Item2;

            return Ok(APIResponse.Create(true, query));  
        }
        [HttpPost("SaveSystemSetting")]
        public async Task<IActionResult> SaveSystemSetting(List<SystemSetting> model)
        {
             var response = await _emailTemplatesAppService.UpdateSystemSetting(model);
             if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
