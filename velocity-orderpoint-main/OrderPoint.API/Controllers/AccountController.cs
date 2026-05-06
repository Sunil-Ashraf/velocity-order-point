using Microsoft.AspNetCore.Mvc;
using OrderPoint.Application.Interfaces.User;
using OrderPoint.Domain.Common;
using OrderPoint.Domain.ViewModel;

namespace OrderPoint.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserAppService _userAppService;
        public AccountController(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            return Ok(await _userAppService.LoginAsync(model));
        }
        [HttpGet("IsEmailExists/{emailAddress}")]
        public async Task<IActionResult> IsEmailExists(string emailAddress)
        {
            return Ok(await _userAppService.IsEmailExistsAsync(emailAddress));
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
        {
            return Ok(await _userAppService.ForgotPasswordAsync(model));
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            return Ok(await _userAppService.ResetPasswordAsync(model));
        }
        [HttpGet("LoginWithUserId")]
        public async Task<IActionResult> LoginWithUserId(string userId)
        {
            return Ok(await _userAppService.LoginWithUserIdAsync(userId));
        }
        [HttpPost("Signup")]
        public async Task<IActionResult> Signup(SignupViewModel model)
        {
            return Ok(await _userAppService.SignupAsync(model));
        }
        
        [HttpPost("GetCustomers")]
        public async Task<IActionResult> GetCustomersAsync()
        {
            return Ok(await _userAppService.GetCustomersAsync());
        }
        [HttpPost("GetWholesalers")]
        public async Task<IActionResult> GetWholesalersAsync()
        {
            return Ok(await _userAppService.GetWholesalersAsync());
        }

        //[HttpPost("CheckUserStatus")]
        //public async Task<IActionResult> CheckUserStatusByUserID( )
        //{
        //    return Ok(await _userAppService.CheckUserStatus(model));
        //}
    }
}
