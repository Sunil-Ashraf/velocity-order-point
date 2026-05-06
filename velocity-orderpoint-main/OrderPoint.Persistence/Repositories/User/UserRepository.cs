using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using OrderPoint.Application.Interfaces.Email;
using OrderPoint.Domain.Common;
using OrderPoint.Domain.Constant;
using OrderPoint.Domain.DbContexts;
using OrderPoint.Domain.DbContexts.Repositories;
using OrderPoint.Domain.Entities;
using OrderPoint.Domain.Interfaces.User;
using OrderPoint.Domain.ModifyIdentity;
using OrderPoint.Domain.ViewModel;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace OrderPoint.Persistence.Repositories.Account
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsPrincipalFactory;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _env;
        private readonly IEmailAppService _emailAppService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;
        private readonly IRepository<CustomerUser> _customerUserRepository;
        private readonly IRepository<WholesalerUser> _wholesaleruserRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Wholesaler> _wholesalerRepository;
        private readonly IRepository<Lists> _listruserRepository;
        private readonly IRepository<EmailTemplate> _emailTemplaterRepository;

        public UserRepository(UserManager<ApplicationUser> userManager, IUserClaimsPrincipalFactory<ApplicationUser> claimsPrincipalFactory,
            IConfiguration configuration, IHostEnvironment env, IEmailAppService emailAppService, IHttpContextAccessor httpContextAccessor, AppDbContext context, IRepository<CustomerUser> customerUserRepository, IRepository<WholesalerUser> wholesaleruserRepository, IRepository<Lists> listruserRepository, IRepository<EmailTemplate> emailTemplaterRepository, IRepository<Wholesaler> wholesalerRepository, IRepository<Customer> customerRepository)
        {
            _userManager = userManager;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _configuration = configuration;
            _env = env;
            _emailAppService = emailAppService;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _customerUserRepository = customerUserRepository;
            _wholesaleruserRepository = wholesaleruserRepository;
            _listruserRepository = listruserRepository;
            _emailTemplaterRepository = emailTemplaterRepository;
            _wholesalerRepository = wholesalerRepository;
            _customerRepository = customerRepository;
        }

        public async Task<APIResponse> LoginAsync(LoginViewModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null)
                    return APIResponse.Create(false, "Invalid email or password");

                var roles = await _userManager.GetRolesAsync(user);
                if (String.Join(",", roles) != Roles.Admin && model.IsAdminPortal == true)
                    return APIResponse.Create(false, "Invalid email or password");

                if ((String.Join(",", roles) == Roles.Admin) && model.IsCustomerPortal == true)
                    return APIResponse.Create(false, "Invalid email or password");

                var isValid = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!isValid)
                    return APIResponse.Create(false, "Invalid email or password");

                if (!user.EmailConfirmed)
                    return APIResponse.Create(false, "Your email is not confirmed. Please check your email.");

                var (token, isSuspended, wholesalerID) = await GenerateJwtToken(user, model.RememberMe);

                if (isSuspended)
                    return APIResponse.Create(false, "Wholesaler account has been suspended. You can not log in at this time.");

                user.LastLoginDate = DateTime.Now;
                await _userManager.UpdateAsync(user);
                return APIResponse.Create(true, token);
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, ex.Message);
            }
        }

        public async Task<APIResponse> CheckUserStatus(LoginViewModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null)
                    return APIResponse.Create(false, "Invalid email or password");

                var isValid = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!isValid)
                    return APIResponse.Create(false, "Invalid email or password");

                if (!user.EmailConfirmed)
                    return APIResponse.Create(false, "Your email is not confirmed. Please check your email.");

                bool Wholesalersuspended = false;
                var userRoles = await _userManager.GetRolesAsync(user);
                if (String.Join(",", userRoles) == Roles.Wholesaler)
                {
                    var wholesaleruser = _wholesaleruserRepository.GetBy(k => k.UserID == user.Id);
                    var wholesaler = _wholesalerRepository.GetBy(k => k.ID == wholesaleruser.WholesalerID);
                    if (wholesaler != null)
                        Wholesalersuspended = wholesaler.Status == 1 ? true : false;
                }
                else if (String.Join(",", userRoles) == Roles.Customer)
                {
                    var customeruser = _customerUserRepository.GetBy(k => k.UserID == user.Id);
                    if (customeruser != null)
                    {
                        var customer = _customerRepository.GetBy(k => k.ID == customeruser.CustomerID);
                        if (customer != null)
                        {
                            var wholesaler = _wholesalerRepository.GetBy(k => k.ID == customer.WholesalerID);
                            if (wholesaler != null)
                                Wholesalersuspended = wholesaler.Status == 1 ? true : false;
                        }
                    }
                }

                if (Wholesalersuspended)
                    return APIResponse.Create(false, "Wholesaler account has been suspended. You can not log in at this time.");

                user.LastLoginDate = DateTime.Now;
                await _userManager.UpdateAsync(user);
                return APIResponse.Create(true, "");
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, ex.Message);
            }
        }

        public async Task<APIResponse> IsEmailExistsAsync(string emailAddress)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(emailAddress);
                if (user == null)
                    return APIResponse.Create(true);
                else
                    return APIResponse.Create(false, "Email already exist.");
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, ex.Message);
            }
        }

        private async Task<(string Token, bool IsWholesalerSuspended, Int32 wholesalerID)> GenerateJwtToken(ApplicationUser user, bool rememberMe)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            String wholesalerName = String.Empty;
            String wholesalerLogo = String.Empty;
            Int32 wholesalerID = 0;
            Int32 customerID = 0;
            bool Wholesalersuspended = false;

            if (String.Join(",", userRoles) == Roles.Wholesaler)
            {
                var wholesaleruser = _wholesaleruserRepository.GetBy(k => k.UserID == user.Id);
                var wholesaler = _wholesalerRepository.GetBy(k => k.ID == wholesaleruser.WholesalerID);
                if (wholesaler != null)
                {
                    wholesalerID = wholesaler.ID;
                    wholesalerName = wholesaler.Name;
                    wholesalerLogo = !String.IsNullOrEmpty(wholesaler.Logo) ? wholesaler.Logo : string.Empty;
                    Wholesalersuspended = wholesaler.Status == 1 ? true : false;
                }
            }
            else if (String.Join(",", userRoles) == Roles.Customer)
            {
                var customeruser = _customerUserRepository.GetBy(k => k.UserID == user.Id);
                if (customeruser != null)
                {
                    var customer = _customerRepository.GetBy(k => k.ID == customeruser.CustomerID);
                    customerID = customer.ID;
                    if (customer != null)
                    {
                        var wholesaler = _wholesalerRepository.GetBy(k => k.ID == customer.WholesalerID);
                        if (wholesaler != null)
                        {
                            wholesalerID = wholesaler.ID;
                            wholesalerName = wholesaler.Name;
                            wholesalerLogo = !String.IsNullOrEmpty(wholesaler.Logo) ? wholesaler.Logo : string.Empty;
                            Wholesalersuspended = wholesaler.Status == 1 ? true : false;
                        }
                    }
                }
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("WholesalerName", wholesalerName),
                new Claim("WholesalerID", wholesalerID.ToString()),
                new Claim("WholesalerLogo", wholesalerLogo),
                new Claim("CustomerID", customerID.ToString())
            };

            foreach (var role in userRoles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            if (key.Key.Length < 32)
                throw new ArgumentException("The JWT signing key is too short.");

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: rememberMe == true ? DateTime.UtcNow.AddDays(Convert.ToInt32(_configuration["Jwt:RememberMeTokenExpiryDays"])) : DateTime.UtcNow.AddHours(Convert.ToInt32(_configuration["Jwt:TokenExpiryHours"])),
                signingCredentials: creds
            );
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return (tokenString, Wholesalersuspended, wholesalerID);
        }

        public async Task<APIResponse> ForgotPasswordAsync(ForgotPasswordViewModel model)
        {
            try
            {
                string contentRootPath = _env.ContentRootPath;
                String wholerID = String.Empty;
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return APIResponse.Create(false, "User not found.");

                var scheme = _httpContextAccessor.HttpContext?.Request.Scheme ?? "https";
                var host = _httpContextAccessor.HttpContext?.Request.Host.Value ?? "yourdomain.com";
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                string frontEndDomain = String.Empty;
                if (model.IsResetByAdmin == true)
                    frontEndDomain = _configuration["Web_App_URL"];
                else
                    frontEndDomain = _configuration["Customer_Web_App_URL"];

                var userRoles = await _userManager.GetRolesAsync(user);
                String wholesalerLogo = String.Empty;
                Int32 customerID = 0;

                if (String.Join(",", userRoles) == Roles.Wholesaler)
                {
                    var wholesaleruser = _wholesaleruserRepository.GetBy(k => k.UserID == user.Id);
                    var wholesaler = _wholesalerRepository.GetBy(k => k.ID == wholesaleruser.WholesalerID);
                    if (wholesaler != null)
                    {
                        wholesalerLogo = !String.IsNullOrEmpty(wholesaler.Logo) ? wholesaler.Logo : string.Empty;
                        wholerID = wholesaler.ID.ToString();
                    }
                }
                else if (String.Join(",", userRoles) == Roles.Customer)
                {
                    var customeruser = _customerUserRepository.GetBy(k => k.UserID == user.Id);
                    if (customeruser != null)
                    {
                        var customer = _customerRepository.GetBy(k => k.ID == customeruser.CustomerID);
                        customerID = customer.ID;
                        if (customer != null)
                        {
                            var wholesaler = _wholesalerRepository.GetBy(k => k.ID == customer.WholesalerID);
                            if (wholesaler != null)
                            {
                                wholesalerLogo = !String.IsNullOrEmpty(wholesaler.Logo) ? wholesaler.Logo : string.Empty;
                                wholerID = wholesaler.ID.ToString();
                            }
                        }
                    }
                }

                var resetLink = $"{frontEndDomain}/reset-password?email={model.Email}&token={Uri.EscapeDataString(token)}&IsNewUserRequest={false}&whlogo={wholesalerLogo}&wholerID={wholerID}";

                var list = await _listruserRepository.GetAll().FirstOrDefaultAsync(i => i.Name == AppLists.ForgotPassword);
                if (list != null)
                {
                    EmailTemplate emailTemplate = new EmailTemplate();
                    if (model.IsResetByAdmin == true)
                        emailTemplate = await _emailTemplaterRepository.GetAll().FirstOrDefaultAsync(s => s.EmailTypeId == list.Id && s.IsDefault && !s.WholesalerID.HasValue);
                    else
                        emailTemplate = await _emailTemplaterRepository.GetAll().FirstOrDefaultAsync(s => s.EmailTypeId == list.Id && s.IsDefault && s.WholesalerID.HasValue && s.WholesalerID.Value == Convert.ToInt32(wholerID));

                    string mainbody = emailTemplate.Body;
                    string subject = emailTemplate.Subject;
                    EmailViewModel emailbodyModel = new EmailViewModel();
                    emailbodyModel.FirstName = user.FirstName;
                    emailbodyModel.LastName = user.LastName;
                    emailbodyModel.ClickHere = resetLink;
                    string body = EmailContent.ReplaceContent(emailbodyModel, mainbody);
                    await _emailAppService.SendEmailAsync(user.Email, String.Empty, String.Empty, subject, body);
                    return APIResponse.Create(true, "Password reset link has been sent to your email.");
                }
                else
                {
                    return APIResponse.Create(false, "There is a problem sending reset password link. Please try again.");
                }
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, ex.Message);
            }
        }

        public async Task<APIResponse> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return APIResponse.Create(false, "User not found.");

                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                if (!result.Succeeded)
                {
                    string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return APIResponse.Create(false, errors);
                }
                return APIResponse.Create(true, "Password has been reset successfully.");
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, ex.Message);
            }
        }

        public async Task<APIResponse> LoginWithUserIdAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return APIResponse.Create(false, "User ID is required.");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return APIResponse.Create(false, "User not found.");

                var (token, isSuspended, wholesalerID) = await GenerateJwtToken(user, false);
                return APIResponse.Create(true, token);
            }
            catch (Exception ex)
            {
                return APIResponse.Create(true, ex.Message);
            }
        }

        public async Task<APIResponse> SignupAsync(SignupViewModel model)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(model.EmailAddress);
                if (existingUser != null)
                    return APIResponse.Create(false, "Email already exists.");

                var user = new ApplicationUser
                {
                    UserName = model.EmailAddress,
                    Email = model.EmailAddress,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    NotifyNewUser = model.NotifyNewUser,
                    CreationTime = DateTime.Now,
                    EmailConfirmed = true,
                    BccOrderConfirmation = model.IsBCCOrderConfirmation
                };

                var createUserResult = await _userManager.CreateAsync(user, model.Password.Trim());
                if (!createUserResult.Succeeded)
                    return APIResponse.Create(false, string.Join(", ", createUserResult.Errors.Select(e => e.Description)));

                string frontEndDomain = String.Empty;
                string role = string.Empty;
                String wLogo = String.Empty;
                bool isAdmin = false;
                String wholerID = String.Empty;

                if (model.UserType.ToLower() == Roles.Admin.ToLower())
                {
                    isAdmin = true;
                    role = Roles.Admin;
                    frontEndDomain = _configuration["Web_App_URL"];
                }
                else if (model.UserType.ToLower() == Roles.Customer.ToLower())
                {
                    role = Roles.Customer;
                    frontEndDomain = _configuration["Customer_Web_App_URL"];
                }
                else if (model.UserType.ToLower() == Roles.Wholesaler.ToLower())
                {
                    role = Roles.Wholesaler;
                    frontEndDomain = _configuration["Customer_Web_App_URL"];
                }
                else
                {
                    return APIResponse.Create(false, "Incorrect user role.");
                }

                var addToRoleResult = await _userManager.AddToRoleAsync(user, role);
                if (!addToRoleResult.Succeeded)
                    return APIResponse.Create(false, "User created but role assignment failed.");

                String wholesalerName = String.Empty;
                if (model.CreateFor.ToLower() == Roles.Customer.ToLower() && (model.CustomerID.HasValue && model.CustomerID.Value > 0))
                {
                    var objcustomer = _customerRepository.GetBy(k => k.ID == model.CustomerID.Value);
                    if (objcustomer != null)
                    {
                        var objwholesaler = _wholesalerRepository.GetBy(k => k.ID == objcustomer.WholesalerID);
                        if (objwholesaler != null)
                        {
                            wLogo = objwholesaler.Logo;
                            wholesalerName = objwholesaler.Name;
                            wholerID = objwholesaler.ID.ToString();
                        }
                    }
                    CustomerUser cuser = new CustomerUser();
                    cuser.CustomerID = model.CustomerID.Value;
                    cuser.UserID = Convert.ToInt32(user.Id);
                    _customerUserRepository.Add(cuser);
                }

                if (model.CreateFor.ToLower() == Roles.Wholesaler.ToLower() && (model.WholesalerID.HasValue && model.WholesalerID.Value > 0))
                {
                    var objwholesaler = _wholesalerRepository.GetBy(k => k.ID == model.WholesalerID.Value);
                    if (objwholesaler != null)
                    {
                        wLogo = objwholesaler.Logo;
                        wholesalerName = objwholesaler.Name;
                        wholerID = objwholesaler.ID.ToString();
                    }
                    WholesalerUser cuser = new WholesalerUser();
                    cuser.WholesalerID = model.WholesalerID.Value;
                    cuser.UserID = Convert.ToInt32(user.Id);
                    _wholesaleruserRepository.Add(cuser);
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = $"{frontEndDomain}/reset-password?email={user.Email}&token={Uri.EscapeDataString(token)}&IsNewUserRequest={true}&whlogo={wLogo}&wholerID={wholerID}";
                frontEndDomain = resetLink;

                if (!model.Id.HasValue && model.NotifyNewUser == true)
                {
                    var list = await _listruserRepository.GetAll().FirstOrDefaultAsync(i => i.Name == AppLists.UserCreated);
                    if (list != null)
                    {
                        EmailTemplate emailTemplate = new EmailTemplate();
                        if (isAdmin)
                            emailTemplate = await _emailTemplaterRepository.GetAll().FirstOrDefaultAsync(s => s.EmailTypeId == list.Id && s.IsDefault && !s.WholesalerID.HasValue);
                        else
                            emailTemplate = await _emailTemplaterRepository.GetAll().FirstOrDefaultAsync(s => s.EmailTypeId == list.Id && s.IsDefault && s.WholesalerID.HasValue && s.WholesalerID.Value == Convert.ToInt32(wholerID));

                        if (emailTemplate != null)
                        {
                            string body = emailTemplate.Body;
                            string subject = emailTemplate.Subject;
                            EmailViewModel emailbodyModel = new EmailViewModel();
                            emailbodyModel.FirstName = user.FirstName;
                            emailbodyModel.LastName = user.LastName;
                            emailbodyModel.Email = user.Email;
                            emailbodyModel.Username = user.Email;
                            emailbodyModel.Supplier = wholesalerName;
                            emailbodyModel.ClickHere = frontEndDomain;
                            string rpBody = EmailContent.ReplaceContent(emailbodyModel, body);
                            await _emailAppService.SendEmailAsync(user.Email, String.Empty, String.Empty, subject, rpBody);
                        }
                    }
                    else
                    {
                        await SendVerificationMail(user);
                    }
                }

                return APIResponse.Create(true, "User registered successfully.");
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, ex.Message);
            }
        }

        public async Task<APIResponse> UpdateUserAsync(EditUseViewModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.ID.ToString());
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.EmailAddress;
                    user.BccOrderConfirmation = model.isBccEmail;

                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                        return APIResponse.Create(false, string.Join(", ", updateResult.Errors.Select(e => e.Description)));

                    return APIResponse.Create(true, "User updated successfully.");
                }
                return APIResponse.Create(true, "User Updated successfully.");
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, ex.Message);
            }
        }

        private async Task SendVerificationMail(ApplicationUser user)
        {
            string contentRootPath = _env.ContentRootPath;
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = GenerateConfirmationLink(user, token);

            string path = Path.Combine(contentRootPath, "EmailTemplates\\Signup.html");
            Hashtable ht = new Hashtable
            {
                { "<!--Subject-->", "Verify your email" },
                { "<!--Fullname-->", user.FirstName + " " + user.LastName },
                { "<!--URL-->", HtmlEncoder.Default.Encode(confirmationLink) },
            };
            string body = EmailHelper.GetContextFromHTML(ht, path);
            await _emailAppService.SendEmailAsync(user.Email, String.Empty, String.Empty, "Verify your email", body);
        }

        public string GenerateConfirmationLink(ApplicationUser user, string token)
        {
            return $"{_configuration["Web_App_URL"]}/signup/confirmation?userId={user.Id}&token={Uri.EscapeDataString(token)}";
        }

        public async Task<APIResponse> GetCustomersAsync()
        {
            try
            {
                var users = await _userManager.GetUsersInRoleAsync(Roles.Customer);
                var lstCustomers = users.Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.LastLoginDate,
                    u.CreationTime
                });
                return APIResponse.Create(true, lstCustomers);
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, ex.Message);
            }
        }

        public async Task<(APIResponse, IQueryable<AdminList>)> GetAdminsAsync()
        {
            try
            {
                var users = await _userManager.GetUsersInRoleAsync(Roles.Admin);
                var lstAdmins = users.Select(u => new AdminList
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    LastLoginDate = u.LastLoginDate,
                    CreationTime = u.CreationTime
                }).AsQueryable();

                return (APIResponse.Create(true), lstAdmins);
            }
            catch (Exception ex)
            {
                return (APIResponse.Create(false, ex.Message), new List<AdminList>().AsQueryable());
            }
        }

        public async Task<APIResponse> GetWholesalersAsync()
        {
            try
            {
                var users = await _userManager.GetUsersInRoleAsync(Roles.Wholesaler);
                var lstWholesalers = users.Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.LastLoginDate,
                    u.CreationTime
                });
                return APIResponse.Create(true, lstWholesalers);
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, ex.Message);
            }
        }

        public async Task<APIResponse> GetUserbyIdAsync(String userID)
        {
            try
            {
                var users = await _userManager.FindByIdAsync(userID);
                var roles = await _userManager.GetRolesAsync(users);
                var lstWholesalers = new EditUseViewModel
                {
                    ID = users.Id,
                    FirstName = users.FirstName,
                    LastName = users.LastName,
                    EmailAddress = users.Email,
                    UserType = String.Join(",", roles),
                    isBccEmail = users.BccOrderConfirmation == true ? true : false,
                };
                return APIResponse.Create(true, lstWholesalers);
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, ex.Message);
            }
        }

        public async Task<APIResponse> Removeuser(String userID)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userID);
                var deleteResult = await _userManager.DeleteAsync(user);
                if (deleteResult.Succeeded)
                {
                    var oldwuser = _wholesaleruserRepository.GetBy(c => c.UserID == Convert.ToInt64(userID));
                    if (oldwuser != null)
                        _wholesaleruserRepository.Delete(oldwuser);

                    var olduser = _customerUserRepository.GetBy(c => c.UserID == Convert.ToInt64(userID));
                    if (olduser != null)
                        _customerUserRepository.Delete(olduser);

                    var list = await _listruserRepository.GetAll().FirstOrDefaultAsync(i => i.Name == AppLists.AccountDeleted);
                    if (list != null)
                    {
                        var emailTemplate = await _emailTemplaterRepository.GetAll().FirstOrDefaultAsync(s => s.EmailTypeId == list.Id && s.IsDefault && !s.WholesalerID.HasValue);
                        string mainbody = emailTemplate.Body;
                        string subject = emailTemplate.Subject;
                        EmailViewModel emailbodyModel = new EmailViewModel();
                        emailbodyModel.FirstName = user.FirstName;
                        emailbodyModel.LastName = user.LastName;
                        emailbodyModel.Email = user.Email;
                        string body = EmailContent.ReplaceContent(emailbodyModel, mainbody);
                        await _emailAppService.SendEmailAsync(user.Email, String.Empty, String.Empty, subject, body);
                    }
                    return APIResponse.Create(true, "User has been removed successfully.");
                }
                else
                {
                    return APIResponse.Create(false, string.Join(", ", deleteResult.Errors.Select(e => e.Description)));
                }
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, ex.Message);
            }
        }

        public async Task<APIResponse> Removeuser(String userID, long? customerID, long? wholersalerID)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userID);
                if (user != null)
                {
                    if (wholersalerID.HasValue)
                    {
                        var olduser = _wholesaleruserRepository.GetAll(c => c.UserID == Convert.ToInt64(userID) && c.WholesalerID == wholersalerID.Value);
                        _wholesaleruserRepository.DeleteRange(olduser);
                    }
                    else if (customerID.HasValue)
                    {
                        var olduser = _customerUserRepository.GetAll(c => c.UserID == Convert.ToInt64(userID) && c.CustomerID == customerID.Value);
                        _customerUserRepository.DeleteRange(olduser);
                    }

                    var deleteResult = await _userManager.DeleteAsync(user);
                    if (!deleteResult.Succeeded)
                        return APIResponse.Create(false, string.Join(", ", deleteResult.Errors.Select(e => e.Description)));
                    else
                    {
                        var list = await _listruserRepository.GetAll().FirstOrDefaultAsync(i => i.Name == AppLists.AccountDeleted);
                        if (list != null)
                        {
                            var emailTemplate = await _emailTemplaterRepository.GetAll().FirstOrDefaultAsync(s => s.EmailTypeId == list.Id && s.IsDefault && !s.WholesalerID.HasValue);
                            string mainbody = emailTemplate.Body;
                            string subject = emailTemplate.Subject;
                            EmailViewModel emailbodyModel = new EmailViewModel();
                            emailbodyModel.FirstName = user.FirstName;
                            emailbodyModel.LastName = user.LastName;
                            emailbodyModel.Email = user.Email;
                            string body = EmailContent.ReplaceContent(emailbodyModel, mainbody);
                            await _emailAppService.SendEmailAsync(user.Email, String.Empty, String.Empty, subject, body);
                        }
                    }
                    return APIResponse.Create(true, "User has been removed successfully.");
                }
                else
                {
                    return APIResponse.Create(false, "There is no user exists in the system.");
                }
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, ex.Message);
            }
        }

        public async Task<(APIResponse, IQueryable<UserDetail>)> GetAllUser(String userType)
        {
            var result = new List<UserDetail>();
            if (!String.IsNullOrEmpty(userType))
            {
                var query = from user in _userManager.Users
                            join userRole in _context.UserRoles on user.Id equals userRole.UserId into ur
                            from userRole in ur.DefaultIfEmpty()
                            join role in _context.Roles on userRole.RoleId equals role.Id into r
                            from role in r.DefaultIfEmpty()
                            where role.Name.ToLower().Trim() == userType.ToLower().Trim()
                            group role by user into g
                            select new UserDetail
                            {
                                ID = g.Key.Id,
                                Email = g.Key.Email,
                                FirstName = g.Key.FirstName,
                                LastName = g.Key.LastName,
                                UserType = string.Join(", ", g.Select(x => x.Name).Where(n => n != null)),
                                LastLogin = g.Key.LastLoginDate,
                                DateCreated = g.Key.CreationTime
                            };
                result = await query.ToListAsync();
            }
            else
            {
                var query = from user in _userManager.Users
                            join userRole in _context.UserRoles on user.Id equals userRole.UserId into ur
                            from userRole in ur.DefaultIfEmpty()
                            join role in _context.Roles on userRole.RoleId equals role.Id into r
                            from role in r.DefaultIfEmpty()
                            group role by user into g
                            select new UserDetail
                            {
                                ID = g.Key.Id,
                                Email = g.Key.Email,
                                FirstName = g.Key.FirstName,
                                LastName = g.Key.LastName,
                                UserType = string.Join(", ", g.Select(x => x.Name).Where(n => n != null)),
                                LastLogin = g.Key.LastLoginDate,
                                DateCreated = g.Key.CreationTime
                            };
                result = await query.ToListAsync();
            }

            if (!result.Any())
                return (APIResponse.Create(false, "Users not found."), Enumerable.Empty<UserDetail>().AsQueryable());

            return (APIResponse.Create(true), result.AsQueryable());
        }

        public async Task<APIResponse> GetUserLinksByRoles(string userID, String userType)
        {
            try
            {
                var result = new List<UserDetail>();
                UserDetail userdetail = new UserDetail();
                var user = await _userManager.FindByIdAsync(userID);
                var roles = await _userManager.GetRolesAsync(user);
                if (roles != null && string.Join(", ", roles).Contains(userType))
                {
                    if (userType == Roles.Wholesaler)
                    {
                        var res = _context.WholesalerUsers.Where(i => i.UserID == user.Id).ToList();
                        foreach (var item in res)
                        {
                            var resc = _context.Wholesalers.Where(i => i.ID == item.WholesalerID).ToList();
                            if (resc.Count > 0)
                            {
                                userdetail = new UserDetail();
                                userdetail.FirstName = resc.FirstOrDefault().Name;
                                userdetail.WholesalerID = item.WholesalerID;
                                userdetail.UserType = userType;
                                result.Add(userdetail);
                            }
                        }
                    }
                    else if (userType == Roles.Customer)
                    {
                        var res = _context.CustomerUsers.Where(i => i.UserID == user.Id).ToList();
                        foreach (var item in res)
                        {
                            var resc = _context.Customers.Where(i => i.ID == item.CustomerID).ToList();
                            if (resc.Count > 0)
                            {
                                userdetail = new UserDetail();
                                userdetail.FirstName = resc.FirstOrDefault().Name;
                                userdetail.CustomerID = item.CustomerID;
                                userdetail.UserType = userType;
                                result.Add(userdetail);
                            }
                        }
                    }
                    else
                    {
                        var res = _context.CustomerUsers.Where(i => i.UserID == user.Id).ToList();
                        foreach (var item in res)
                        {
                            var resc = _context.Customers.Where(i => i.ID == item.CustomerID).ToList();
                            if (resc.Count > 0)
                            {
                                userdetail = new UserDetail();
                                userdetail.FirstName = resc.FirstOrDefault().Name;
                                userdetail.CustomerID = item.CustomerID;
                                userdetail.UserType = Roles.Customer;
                                result.Add(userdetail);
                            }
                        }
                        var ress = _context.WholesalerUsers.Where(i => i.UserID == user.Id).ToList();
                        foreach (var item in ress)
                        {
                            var resc = _context.Wholesalers.Where(i => i.ID == item.WholesalerID).ToList();
                            if (ress.Count > 0)
                            {
                                userdetail = new UserDetail();
                                userdetail.FirstName = resc.FirstOrDefault().Name;
                                userdetail.WholesalerID = item.WholesalerID;
                                userdetail.UserType = Roles.Wholesaler;
                                result.Add(userdetail);
                            }
                        }
                    }
                }
                return APIResponse.Create(true, result);
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, ex.Message);
            }
        }

        public async Task<APIResponse> GetuserListbyUserType(String userType)
        {
            try
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(userType);
                var result = usersInRole.Select(user => new UserDetail
                {
                    ID = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                }).ToList();
                return APIResponse.Create(true, result);
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, ex.Message);
            }
        }
    }
}