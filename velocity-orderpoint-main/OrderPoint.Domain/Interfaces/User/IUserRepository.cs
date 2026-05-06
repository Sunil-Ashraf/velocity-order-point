using OrderPoint.Domain.Common;
using OrderPoint.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.Interfaces.User
{
    public interface IUserRepository
    {
        Task<APIResponse> LoginAsync(LoginViewModel model);
        Task<APIResponse> CheckUserStatus(LoginViewModel model);
        Task<APIResponse> IsEmailExistsAsync(string emailAddress);
        Task<APIResponse> ForgotPasswordAsync(ForgotPasswordViewModel model);
        Task<APIResponse> ResetPasswordAsync(ResetPasswordViewModel model);
        Task<APIResponse> LoginWithUserIdAsync(string userId);
        Task<APIResponse> SignupAsync(SignupViewModel model);
        Task<APIResponse> UpdateUserAsync(EditUseViewModel model);
        Task<APIResponse> GetCustomersAsync();
        Task<(APIResponse, IQueryable<AdminList>)> GetAdminsAsync();
        Task<APIResponse> GetWholesalersAsync();
   
        Task<APIResponse> GetUserbyIdAsync(String userID);

        Task<APIResponse> Removeuser(String userID);
        Task<APIResponse> Removeuser(String userID, long? customerID, long? wholersalerID);



        // (APIResponse, IQueryable<UserDetail>) GetAllUser();
        Task<(APIResponse, IQueryable<UserDetail>)> GetAllUser(String userType);
        Task<APIResponse> GetUserLinksByRoles(string userID, String userType);
        
        Task<APIResponse> GetuserListbyUserType(String userType);
    }
}
