using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OrderPoint.Application.Interfaces.User;
using OrderPoint.Domain.Common;
using OrderPoint.Domain.Interfaces.User;
using OrderPoint.Domain.ModifyIdentity;
using OrderPoint.Domain.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace OrderPoint.Application.Services.User
{
    public class UserAppService : IUserAppService
    {
        private readonly IUserRepository _userRepository;
        public UserAppService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<APIResponse> LoginAsync(LoginViewModel model)
        {
            return await _userRepository.LoginAsync(model);
        }
       
        public async Task<APIResponse> CheckUserStatus(LoginViewModel model)
        {
            return await _userRepository.CheckUserStatus(model);
        }
        public async Task<APIResponse> IsEmailExistsAsync(string emailAddress)
        {
            return await _userRepository.IsEmailExistsAsync(emailAddress);
        }
        public async Task<APIResponse> ForgotPasswordAsync(ForgotPasswordViewModel model)
        {
            return await _userRepository.ForgotPasswordAsync(model);
        }
        public async Task<APIResponse> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            return await _userRepository.ResetPasswordAsync(model);
        }
        public async Task<APIResponse> LoginWithUserIdAsync(string userId)
        {
            return await _userRepository.LoginWithUserIdAsync(userId);
        }
        public async Task<APIResponse> SignupAsync(SignupViewModel model)
        {
            return await _userRepository.SignupAsync(model);
        }

        public async Task<APIResponse> GetCustomersAsync()
        {
            return await _userRepository.GetCustomersAsync();
        }
        public async Task<(APIResponse, IQueryable<AdminList>)> GetAdminsAsync()
        {
            return await _userRepository.GetAdminsAsync();
        }
        public async Task<APIResponse> GetWholesalersAsync()
        {
            return await _userRepository.GetWholesalersAsync();
        }
        public async Task<APIResponse> GetuserbyId(String id)
        {

            return await _userRepository.GetUserbyIdAsync(id);
        }
        public async Task<APIResponse> UpdateUser(EditUseViewModel model)
        {
            return await _userRepository.UpdateUserAsync(model);
        }

        public async Task<APIResponse> RemoveUser(String userID)
        {
            return await _userRepository.Removeuser(userID);
        }
        public async Task<APIResponse> RemoveUser(String userID, long? customerID, long? wholersalerID)
        {
            return await _userRepository.Removeuser(userID,   customerID,  wholersalerID);
        }
        // public  (APIResponse, IQueryable<UserDetail>) GetAllUser()
        public async Task<(APIResponse, IQueryable<UserDetail>)> GetAllUser(String userType)
        {
            return await _userRepository.GetAllUser(userType);
        }
        public async Task<APIResponse> GetUserLinksByRoles(string userID, String userType)
        {
            return await _userRepository.GetUserLinksByRoles(userID, userType);
        }

        public async Task<APIResponse> GetuserListbyUserType(String userType)
        {
            return await _userRepository.GetuserListbyUserType(userType);
        }


    }
}
