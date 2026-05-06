using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrderPoint.Domain.Common;
using OrderPoint.Domain.DbContexts.Repositories;
using OrderPoint.Domain.Entities;
using OrderPoint.Domain.Helper;
using OrderPoint.Domain.Interfaces.Customers;

using OrderPoint.Domain.ModifyIdentity;
using OrderPoint.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OrderPoint.Persistence.Repositories.Customers
{
    public class WholesalerRepository : IWholesalerRepository
    {
        private readonly IRepository<WholesalerUser> _wholesalerUserRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Wholesaler> _wholesalerRepository;
        private readonly IRepository<DeliveryOptions> _deliveryOptionRepository;
        private readonly UserHelper _userHelper;
        private readonly IRepository<CustomerUser> _customerUserRepository;
        private readonly IRepository<Customer> _customerRepository;
        public WholesalerRepository(UserHelper userHelper, IRepository<WholesalerUser> wholesalerUserRepository, IRepository<Wholesaler> wholesalerRepository, UserManager<ApplicationUser> userManager, IRepository<DeliveryOptions> deliveryOptionRepository, IRepository<Customer> customerRepository , IRepository<CustomerUser> customerUserRepository)
        {
            _userHelper = userHelper;
            _wholesalerUserRepository = wholesalerUserRepository;
            _wholesalerRepository = wholesalerRepository;
            _userManager = userManager;
            _deliveryOptionRepository = deliveryOptionRepository;
            _customerRepository = customerRepository;
            _customerUserRepository = customerUserRepository;
        }
        public async Task<APIResponse> SetWholesalerDeliverytime(DeliveryOptionVM model)
        {
            try
            {

                DeliveryOptions dp = new DeliveryOptions();
                List<DeliveryOptions> dpList = new List<DeliveryOptions>();
                dp.WholesalerID = model.WholesalerID;
                dp.WeekDay = "Monday";
                if (model.MondayStartTime.HasValue)
                {
                    dp.StartTime = model.MondayStartTime.Value;
                }
                if (model.MondayEndTime.HasValue)
                {

                    dp.EndTime = model.MondayEndTime.Value;
                }
                dpList.Add(dp);
                // tusday
                dp = new DeliveryOptions();
                dp.WholesalerID = model.WholesalerID;
                dp.WeekDay = "Tuesday";
                if (model.TuesdayStartTime.HasValue)
                {

                    dp.StartTime = model.TuesdayStartTime.Value;
                }
                if (model.TuesdayEndTime.HasValue)
                {

                    dp.EndTime = model.TuesdayEndTime.Value;
                }
                dpList.Add(dp);
                // Wednesday
                dp = new DeliveryOptions();
                dp.WholesalerID = model.WholesalerID;
                dp.WeekDay = "Wednesday";
                if (model.WednesdayStartTime.HasValue)
                {

                    dp.StartTime = model.WednesdayStartTime.Value;
                }
                if (model.WednesdayEndTime.HasValue)
                {

                    dp.EndTime = model.WednesdayEndTime.Value;
                }
                dpList.Add(dp);
                // Thursday
                dp = new DeliveryOptions();
                dp.WholesalerID = model.WholesalerID;
                dp.WeekDay = "Thursday";
                if (model.ThursdayStartTime.HasValue)
                {

                    dp.StartTime = model.ThursdayStartTime.Value;
                }
                if (model.ThursdayEndTime.HasValue)
                {

                    dp.EndTime = model.ThursdayEndTime.Value;
                }
                dpList.Add(dp);
                //Friday
                dp = new DeliveryOptions();
                dp.WholesalerID = model.WholesalerID;
                dp.WeekDay = "Friday";
                if (model.FridayStartTime.HasValue)
                {

                    dp.StartTime = model.FridayStartTime.Value;
                }
                if (model.ThursdayEndTime.HasValue)
                {

                    dp.EndTime = model.FridayEndTime.Value;
                }
                dpList.Add(dp);

                //Saturday

                dp = new DeliveryOptions();
                dp.WholesalerID = model.WholesalerID;
                dp.WeekDay = "Saturday";
                if (model.FridayStartTime.HasValue)
                {

                    dp.StartTime = model.SaturdayStartTime.Value;
                }
                if (model.ThursdayEndTime.HasValue)
                {

                    dp.EndTime = model.SaturdayEndTime.Value;
                }
                dpList.Add(dp);

                //Sunday

                dp = new DeliveryOptions();
                dp.WholesalerID = model.WholesalerID;
                dp.WeekDay = "Sunday";
                if (model.FridayStartTime.HasValue)
                {

                    dp.StartTime = model.SundayStartTime.Value;
                }
                if (model.ThursdayEndTime.HasValue)
                {

                    dp.EndTime = model.SundayEndTime.Value;
                }
                dpList.Add(dp);

                var old = _deliveryOptionRepository.GetAll(j => j.WholesalerID == model.WholesalerID).ToList();
                if (old != null && old.Count() > 0)
                {
                    _deliveryOptionRepository.DeleteRange(old);
                }

                _deliveryOptionRepository.AddRange(dpList);
                return (APIResponse.Create(true, "Delivery Scheduler has been saved successfully."));
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, ex.Message);
            }

        }
        public async Task<APIResponse> AddEditWholesaler(AddEditWholesalerModel model)
        {
            try
            {
                Wholesaler customerobject = new Wholesaler();


                customerobject.Name = model.Name;
                customerobject.Address = model.Address;
                customerobject.Telephone = model.Telephone;
                customerobject.Email = model.Email;
                customerobject.LandingPageTelephone = model.LandingPageTelephone;
                customerobject.LandingPageEmail = model.LandingPageEmail;
                customerobject.Description = model.Description;
                customerobject.WelcomeMessage = model.WelcomeMessage;
                if (!String.IsNullOrEmpty(model.ImagePath))
                {
                    customerobject.Logo = model.ImagePath;
                }

                if (model.ID > 0)
                {
                    customerobject.ID = Convert.ToInt32(model.ID.Value);
                    var oldobject = _wholesalerRepository.GetBy(k => k.ID == customerobject.ID);
                    if (oldobject != null)
                    {
                        oldobject.ID = customerobject.ID;
                        oldobject.Name = model.Name;
                        oldobject.Address = model.Address;
                        oldobject.Telephone = model.Telephone;
                        oldobject.LandingPageTelephone = model.LandingPageTelephone;
                        oldobject.Description = model.Description;
                        oldobject.WelcomeMessage = model.WelcomeMessage;


                        if (!String.IsNullOrEmpty(model.ImagePath))
                        {
                            oldobject.Logo = model.ImagePath;
                        }

                        if (!String.IsNullOrEmpty(model.BannerImagePath))
                        {
                            oldobject.BannerImage = model.BannerImagePath;
                        }
                        oldobject.Email = model.Email;
                        oldobject.LandingPageEmail = model.LandingPageEmail;


                        _wholesalerRepository.Update(oldobject);
                        return (APIResponse.Create(true, "Wholesaler has been update successfully."));
                    }
                    else
                    {
                        return (APIResponse.Create(false, "Wholesaler is not exist"));
                    }
                }
                else
                {
                    customerobject.RandID = Guid.NewGuid().ToString();
                    _wholesalerRepository.Add(customerobject);
                }

                return (APIResponse.Create(true, "Wholesaler has been saved successfully."));
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, ex.Message);
            }


        }


        public async Task<APIResponse> GetWholesalerByID(long id)
        {
            try
            {
                var wh = _wholesalerRepository.GetBy(c => c.ID == id);
                if (wh != null)
                {
                    var whData = new AddEditWholesalerModel
                    {
                        ID = wh.ID,
                        Name = wh.Name,
                        Address = wh.Address,
                        Email = wh.Email,
                        Telephone = wh.Telephone,
                        ImagePath = wh.Logo,
                        BannerImagePath = wh.BannerImage,
                        LandingPageEmail = wh.LandingPageEmail,
                        LandingPageTelephone = wh.LandingPageTelephone,
                        Description = wh.Description,
                        WelcomeMessage = wh.WelcomeMessage,
                    };
                    return (APIResponse.Create(true, whData));
                }
                else
                {
                    return APIResponse.Create(false, "There is an error to get record.");
                }
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, ex.Message);
            }

        }

        public async Task<APIResponse> GetWholesalerByName(String Name)
        {
            try
            {
                var wh = _wholesalerRepository.GetBy(c => c.Name == Name);
                if (wh != null)
                {
                    var whData = new AddEditWholesalerModel
                    {
                        ID = wh.ID,
                        Name = wh.Name,
                        Address = wh.Address,
                        Email = wh.Email,
                        Telephone = wh.Telephone,
                        ImagePath = wh.Logo,
                        LandingPageEmail = wh.LandingPageEmail,
                        LandingPageTelephone = wh.LandingPageTelephone,
                        Description = wh.Description,
                        BannerImagePath = wh.BannerImage,
                        WelcomeMessage = wh.WelcomeMessage,
                    };
                    return (APIResponse.Create(true, whData));
                }
                else
                {
                    return APIResponse.Create(false, "There is an error to get record.");
                }
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, ex.Message);
            }

        }
        public async Task<AddEditWholesalerModel> GetWholesalerDetailByID(long id)
        {
            try
            {
                AddEditWholesalerModel whD = new AddEditWholesalerModel();
                var wh = _wholesalerRepository.GetBy(c => c.ID == id);
                if (wh != null)
                {
                    var whData = new AddEditWholesalerModel
                    {
                        ID = wh.ID,
                        Name = wh.Name,
                        Address = wh.Address,
                        Email = wh.Email,
                        Telephone = wh.Telephone,
                        ImagePath = wh.Logo,
                    };
                    return whData;
                }
                else
                {
                    return whD;
                }
            }
            catch (Exception ex)
            {
                return new AddEditWholesalerModel();
            }

        }
        public async Task<APIResponse> GetdeliverySchedulerByWholesaler(Int64 id)
        {
            try
            {
                DeliveryOptionVM whD = new DeliveryOptionVM();
                var dw = _deliveryOptionRepository.GetAll(j => j.WholesalerID == id).ToList();

                if (dw.Any())
                {
                    whD.WholesalerID = dw.First().WholesalerID;

                    foreach (var item in dw)
                    {
                        switch (item.WeekDay)
                        {
                            case "Monday":
                                whD.MondayStartTime = item.StartTime;
                                whD.MondayEndTime = item.EndTime;
                                break;

                            case "Tuesday":
                                whD.TuesdayStartTime = item.StartTime;
                                whD.TuesdayEndTime = item.EndTime;
                                break;

                            case "Wednesday":
                                whD.WednesdayStartTime = item.StartTime;
                                whD.WednesdayEndTime = item.EndTime;
                                break;

                            case "Thursday":
                                whD.ThursdayStartTime = item.StartTime;
                                whD.ThursdayEndTime = item.EndTime;
                                break;

                            case "Friday":
                                whD.FridayStartTime = item.StartTime;
                                whD.FridayEndTime = item.EndTime;
                                break;

                            case "Saturday":
                                whD.SaturdayStartTime = item.StartTime;
                                whD.SaturdayEndTime = item.EndTime;
                                break;

                            case "Sunday":
                                whD.SundayStartTime = item.StartTime;
                                whD.SundayEndTime = item.EndTime;
                                break;
                        }
                    }
                }


                return (APIResponse.Create(true, whD));

            }
            catch (Exception)
            {
                return APIResponse.Create(false, "There is an error to get record.");
            }
        }


        public async Task<APIResponse> GetWholesaleUserListBysalerID(long id)
        {
            var holesaleUsers = _wholesalerUserRepository.GetAll(c => c.WholesalerID == id).ToList();
            var users = await _userManager.Users.ToListAsync();
            var userList = (
                  from wh in holesaleUsers
                  join u in users on Convert.ToInt64(wh.UserID) equals u.Id
                  select new UserDetail
                  {
                      ID = wh.UserID,
                      FirstName = u.FirstName,
                      LastName = u.LastName,
                      Email = u.Email,
                      IsBCCToOrderEmail = u.BccOrderConfirmation

                  });
            return APIResponse.Create(true, userList);
        }


        public (APIResponse, IQueryable<WholesalerViewModel>) GetAllWholesalerList()
        {
            try
            {
                var wholesalerUsers = _wholesalerUserRepository.GetAll();
                var wholesalers = _wholesalerRepository.GetAll();
                var list = (
    from ws in wholesalers
    join wu in wholesalerUsers
        on ws.ID equals Convert.ToInt64(wu.WholesalerID) into userGroup
    from wu in userGroup.DefaultIfEmpty() // 👈 LEFT JOIN logic
    group wu by new
    {
        ws.ID,
        ws.Name,
        ws.RandID,
        ws.Logo,
        ws.Status
    } into g
    select new WholesalerViewModel
    {
        ID = g.Key.ID,
        Name = g.Key.Name,
        RandID = g.Key.RandID,
        ImagePath = g.Key.Logo,
        NoOfUser = g.Count(x => x != null), // Only count matched users
        Status = g.Key.Status == 0 ? "Active" : "Suspended"
    });
                return (APIResponse.Create(true), list);
            }
            catch (Exception ex)
            {
                return (APIResponse.Create(false, $"Error: {ex.Message}"), new List<WholesalerViewModel>().AsQueryable());
            }
        }



        public async Task<APIResponse> GetWholesalerUserByUserID(long userID)
        {
            var wholesalerUsers = _wholesalerUserRepository.GetAll(c => c.UserID == userID).ToList();

            var wholesaler = _wholesalerRepository.GetAll();

            var users = await _userManager.Users.ToListAsync();
            var userList = (
                  from cuser in wholesalerUsers
                  join c in wholesaler on Convert.ToInt64(cuser.WholesalerID) equals c.ID
                  select new UserDetail
                  {
                      ID = cuser.UserID,
                      WholesalerID = c.ID,
                      FirstName = c.Name,
                  });
            return APIResponse.Create(true, userList);
        }


        public async Task<APIResponse> AddWholesalerLinks(Int32 userID, Int32 wholesalerID)
        {
            try
            {
                WholesalerUser custom = new WholesalerUser();
                custom.WholesalerID = wholesalerID;
                custom.UserID = userID;
                _wholesalerUserRepository.Add(custom);
                return APIResponse.Create(false, "Link has been created successfully.");
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, "There is no item select to update record.");
            }
        }
        public async Task<APIResponse> UpdateWholesalerStatus(Int32 id)
        {
            try
            {
                var wholesaler = _wholesalerRepository.GetAll(c => c.ID == id).FirstOrDefault();

                if (wholesaler != null)
                {
                    wholesaler.Status = wholesaler.Status == 1 ? 0 : 1;
                }
                _wholesalerRepository.Update(wholesaler);
                return APIResponse.Create(true, "Link has been created successfully.");
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, "There is no item select to update record.");
            }
        }

        public (APIResponse, IQueryable<WholesalerViewModel>) GetWholesalerbyUserID()
        {
            try
            {

                var userId = _userHelper.GetUserId();
                if (!long.TryParse(userId, out var parsedUserId))
                {


                    return (APIResponse.Create(false, "Invalid or missing user ID."), new List<WholesalerViewModel>().AsQueryable());
                }


                var wholesalerUsers = _wholesalerUserRepository.GetAll();
                var wholesalers = _wholesalerRepository.GetAll();

                var wsList = (from wUser in wholesalerUsers
                              join ws in wholesalers on wUser.WholesalerID equals ws.ID
                              where wUser.UserID == parsedUserId
                              orderby wUser.UserID descending  // descending to get the latest
                              select new WholesalerViewModel
                              {
                                  ID = ws.ID,
                                  Name = ws.Name,
                                  RandID = ws.RandID,
                                  ImagePath = ws.Logo,
                                  NoOfUser = wholesalerUsers.Where(k => k.WholesalerID == ws.ID).Count(), // Only count matched users
                                  Status = ws.Status == 0 ? "Active" : "Suspended"
                              });
                return (APIResponse.Create(true), wsList);
            }
            catch (Exception ex)
            {
                return (APIResponse.Create(false, "Invalid or missing user ID."), new List<WholesalerViewModel>().AsQueryable());

            }
        }
        
        public async Task<Response<List<UserDetail>>> GetWholersalerCustomerUserListbysalerID(long id)
        {
            var users = await _userManager.Users.ToListAsync();
            var holesaleUsers = _wholesalerUserRepository.GetAll(c => c.WholesalerID == id).ToList();

            List<UserDetail> userList = new List<UserDetail>();
            if (holesaleUsers != null)
            {
                userList.AddRange((
                     from wh in holesaleUsers
                     join u in users on Convert.ToInt64(wh.UserID) equals u.Id
                     select new UserDetail
                     {
                         ID = wh.UserID,
                         FirstName = u.FirstName,
                         LastName = u.LastName,
                         Email = u.Email,
                         IsBCCToOrderEmail = u.BccOrderConfirmation

                     }).ToList());
            }
            var customer  = _customerRepository.GetBy(k => k.WholesalerID == id);
            if(customer!= null)
            {
                
                var customeruser = _customerUserRepository.GetAll(k => k.CustomerID == customer.ID);
                userList.AddRange ((
                      from c in customeruser
                      join u in users on Convert.ToInt64(c.UserID) equals u.Id
                      select new UserDetail
                      {
                          ID = c.UserID,
                          FirstName = u.FirstName,
                          LastName = u.LastName,
                          Email = u.Email,
                          IsBCCToOrderEmail = u.BccOrderConfirmation

                      }).ToList());
            }


           
            return APIResponse.Create(true, userList);
        }
    }
}
