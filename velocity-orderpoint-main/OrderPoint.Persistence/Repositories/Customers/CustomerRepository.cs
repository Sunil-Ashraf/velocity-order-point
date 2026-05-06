using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrderPoint.Application.Interfaces.Email;
using OrderPoint.Domain.Common;
using OrderPoint.Domain.Constant;
using OrderPoint.Domain.DbContexts;
using OrderPoint.Domain.DbContexts.Repositories;
using OrderPoint.Domain.Entities;
using OrderPoint.Domain.Helper;
using OrderPoint.Domain.Interfaces.Customers;

using OrderPoint.Domain.ModifyIdentity;
using OrderPoint.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Persistence.Repositories.Customers
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IRepository<CustomerUser> _customerUserRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Wholesaler> _wholesalerRepository;
        private readonly IRepository<WholesalerUser> _wholesalerUserRepository;
        private readonly IRepository<OrderLineItem> _orderItemRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly UserHelper _userHelper;
        private readonly IEmailAppService _emailAppService;
        private readonly IRepository<Lists> _listruserRepository;
        private readonly IRepository<DeliveryOptions> _deliveryOptions;
        private readonly IRepository<EmailTemplate> _emailTemplaterRepository;
        private readonly AppDbContext _context;
        public CustomerRepository(UserHelper userHelper, IRepository<CustomerUser> customerUserRepository, IRepository<Customer> customerRepository, IRepository<Wholesaler> wholesalerRepository, UserManager<ApplicationUser> userManager, IRepository<WholesalerUser> wholesalerUserRepository, IRepository<Order> orderRepository, IRepository<OrderLineItem> orderItemRepository, IRepository<Lists> listruserRepository, IRepository<EmailTemplate> emailTemplaterRepository, IEmailAppService emailAppService, AppDbContext context, IRepository<DeliveryOptions> deliveryOptions)
        {
            _userHelper = userHelper;
            _customerUserRepository = customerUserRepository;
            _customerRepository = customerRepository;
            _wholesalerRepository = wholesalerRepository;
            _userManager = userManager;
            _wholesalerUserRepository = wholesalerUserRepository;
            _orderItemRepository = orderItemRepository;
            _orderRepository = orderRepository;
            _listruserRepository = listruserRepository;
            _emailTemplaterRepository = emailTemplaterRepository;
            _emailAppService = emailAppService;
            _context = context;
            _deliveryOptions = deliveryOptions;

        }

        public async Task<APIResponse> CreateCustomer(AddEditCustomerModel model)
        {
            try
            {
              
                Customer customerobject = new Customer();
                List<Customer> customerList = new List<Customer>();
                if (model.IsCreateCustomerForWholesalePrtal == true)
                {
                    var userID = _userHelper.GetUserId();
                    if (!String.IsNullOrEmpty(userID))
                    {
                       long id = long.Parse(userID);
                       var wholeruser= _wholesalerUserRepository.GetAll(k => k.UserID == id).ToList();
                        if(wholeruser!=null && wholeruser.Count() > 0)
                        {
                            foreach(var item in wholeruser)
                            {
                                customerobject = new Customer();
                                customerobject.WholesalerID = item.WholesalerID;
                                customerobject.Name = model.CustomerName;
                                customerobject.WholesalerReference = !String.IsNullOrEmpty(model.WholesalerReference)? model.WholesalerReference: String.Empty ;
                                customerList.Add(customerobject);
                            }
                       
                        }
                        _customerRepository.AddRange(customerList);

                    }
                 
                }
                else
                {

                  
                    customerobject.Name = model.CustomerName;
                    customerobject.WholesalerID = Convert.ToInt32(model.WholesalerID);
                    customerobject.WholesalerReference = !String.IsNullOrEmpty(model.WholesalerReference) ? model.WholesalerReference : String.Empty;
                    _customerRepository.Add(customerobject);
                }
               

                return (APIResponse.Create(true, "Customer has been created successfully."));
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, ex.Message);
            }


        }  
        public async Task<APIResponse> CreateOrder(OrderPlacementModel model)
        {
            try
            {
                

                var userId = _userHelper.GetUserId();
                if (!long.TryParse(userId, out var parsedUserId))
                    return (APIResponse.Create(false, "Invalid or missing user ID."));

                Order order = new Order();


                List<OrderLineItem> orderItemList = new List<OrderLineItem>();
                OrderLineItem orderItem = new OrderLineItem();
                var user = _userManager.Users.FirstOrDefault(k => k.Id == parsedUserId);

                var customeruser = _customerUserRepository.GetBy(k => k.UserID == parsedUserId);
                if (customeruser == null)
                    return (APIResponse.Create(false, "Invalid or missing user ID."));
                var customer = _customerRepository.GetBy(k => k.ID == customeruser.CustomerID);
                if (customer == null)
                    return (APIResponse.Create(false, "Invalid or missing user ID."));
            
                 

                if (customer != null)
                {
                    order.OrderedDate = DateTime.Now;
                    order.RequiredDate = model.OrderDate;
                    var doption = _deliveryOptions.GetAll(j => j.WholesalerID == customer.WholesalerID).ToList();

                    if(doption!= null && doption.Count>0 && order.OrderedDate.Date== order.RequiredDate.Date)
                    {
                        string weekday = order.OrderedDate.DayOfWeek.ToString();
                      var currentDateendtime = doption.Where(k => k.WeekDay.ToLower().Trim() == weekday.ToLower()).FirstOrDefault();
                        TimeSpan currenttime = order.OrderedDate.TimeOfDay;
                        if(currentDateendtime.EndTime< currenttime)
                        {
                            return APIResponse.Create(false, "The ordering time is closed. for the " + model.OrderDate.ToString("MM/dd/yy") + " Please choose the next available day for your order.");
                        }
                    }
                    order.CustomerID = customer.ID;
                    order.Rand_ID = Guid.NewGuid().ToString();
                    order.WholeSalerID = customer.WholesalerID;
                   
                    
                    order.OrderNumber = model.OrderNumber;
                    order.Notes = model.OrderNotes;
                     await _orderRepository.AddAsync(order);
                    //  _orderRepository.AddAsync(order);
                    model.OrderID = order.ID;
                    string itemList = string.Empty;
                    StringBuilder sb = new StringBuilder();
                    Int32 orderID = model.OrderID.Value;
                    foreach (var item in model.OrderItems)
                    {
                        orderItem = new OrderLineItem();
                        orderItem.OrderID = orderID;
                        orderItem.ProductID = Convert.ToInt32(item.Id);
                        orderItem.Notes = item.Description;
                        orderItem.Quantity = Math.Round((double)item.Quantity, 3); // (double)item.Quantity;
                        orderItem.QuantityType = item.QuantityTypeId.Value;
                        itemList = item.ProductName + "<br/> Quantity: " + item.Quantity + "<br/> Notes" + item.Description + "<br/>";
                        sb.AppendLine(itemList);
                        orderItemList.Add(orderItem);
                       // _orderItemRepository.Add(orderItem);


                    }
                    await _orderItemRepository.AddRangeAsync(orderItemList);
                    return (APIResponse.Create(true, "Order has Created successfully."));
                }
                else
                {
                    return APIResponse.Create(false, "There is no customer exist");
                }

            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, ex.Message);
            }


        }

        public async Task<APIResponse> AddOrderItems(OrderPlacementModel model)
        {
            try
            {
                var userId = _userHelper.GetUserId();
                if (!long.TryParse(userId, out var parsedUserId))
                    return (APIResponse.Create(false, "Invalid or missing user ID."));

                Order order = new Order();


                List<OrderLineItem> orderItemList = new List<OrderLineItem>();
                OrderLineItem orderItem = new OrderLineItem();
                var user = _userManager.Users.FirstOrDefault(k => k.Id == parsedUserId);

                var customeruser = _customerUserRepository.GetBy(k => k.UserID == parsedUserId);
                if (customeruser == null)
                    return (APIResponse.Create(false, "Invalid or missing user ID."));
                var customer = _customerRepository.GetBy(k => k.ID == customeruser.CustomerID);
                if (customer == null)
                    return (APIResponse.Create(false, "Invalid or missing user ID."));
                var wholesaler = _wholesalerRepository.GetBy(k => k.ID == customer.WholesalerID);
                if (wholesaler == null)
                    return (APIResponse.Create(false, "Invalid or missing user ID."));
                

                if (customer != null)
                {
                    var holesaleUsers = _wholesalerUserRepository
     .GetAll(c => c.WholesalerID == customer.WholesalerID)
     .ToList();

                    var userList = _userManager.Users
                        .ToList() // Force in-memory evaluation
                        .Where(k => k.BccOrderConfirmation == true &&
                                    holesaleUsers.Any(l => l.UserID == k.Id))
                        .ToList();

 
                    String bccEmail = String.Empty;
                    if(userList!= null && userList.Count() > 0)
                    {
                        bccEmail = String.Join(",", userList.Select(k=>k.Email).ToList());
                    }

                    string itemList = string.Empty;
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in model.OrderItems)
                    {
                        if (item.QuantityTypeId == 0)
                        {
                            itemList = item.ProductName + "<br/> Quantity (KG): " + item.Quantity + "<br/> Price: " + (Convert.ToDecimal(item.Price).ToString("F3")) + "<br/> Notes: " + item.Description + "<br/> <br/>";
                        }
                        else
                        {
                            itemList = item.ProductName + "<br/> Quantity: " + item.Quantity + "<br/> Price: " + (Convert.ToDecimal(item.Price).ToString("F3")) + "<br/> Notes: " + item.Description + "<br/> <br/>";
                        }
                        sb.AppendLine(itemList);
                    }
                    var list =   _listruserRepository.GetAll().FirstOrDefault(i => i.Name == AppLists.NewOrder);
                    if (list != null)
                    {
                        var emailTemplate =   _emailTemplaterRepository.GetAll().FirstOrDefault(s => s.EmailTypeId == list.Id && s.IsDefault && s.WholesalerID== customer.WholesalerID);

                        if (emailTemplate != null)
                        {
                            string mainbody = emailTemplate.Body;
                            string subject = emailTemplate.Subject;
                            EmailViewModel emailbodyModel = new EmailViewModel();
                            emailbodyModel.FirstName = user.FirstName;
                            emailbodyModel.LastName = user.LastName;
                            emailbodyModel.Supplier = wholesaler.Name;
                            emailbodyModel.Email = wholesaler.Email;
                            emailbodyModel.Telephone = wholesaler.Telephone;
                            emailbodyModel.Address = wholesaler.Address;
                            emailbodyModel.Notes = model.OrderNotes;
                            emailbodyModel.ReferenceNumber = model.OrderNumber;
                            emailbodyModel.RequiredDate = String.Format(AppLists.UkDateFormat, model.OrderDate);
                            emailbodyModel.Date = String.Format(AppLists.UkDateFormatwithtime, DateTime.Now);

                            if (sb != null)
                            {
                                emailbodyModel.ItemList = sb.ToString();
                            }

                            string body = EmailContent.ReplaceContent(emailbodyModel, mainbody);
                            await _emailAppService.SendEmailAsync(user.Email, String.Empty, bccEmail, subject, body);
                        }
                        return APIResponse.Create(true, "Order has been Succesfully Created");
                    }
                    else
                    {
                        return APIResponse.Create(false, "Order has been Succesfully Created.but there is no email template exist, Please create email template to receive email.");
                    }
                    return (APIResponse.Create(true, "Order has Created successfully."));
                }
                else
                {
                    return APIResponse.Create(false, "There is no customer exist");
                }

            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, ex.Message);
            }


        }
         


        public async Task<APIResponse> GetCustomerbyId(long id)
        {
            try
            {
                var customer = _customerRepository.GetBy(c => c.ID == id);
                if (customer != null)
                {
                     
                        var customerData = new AddEditCustomerModel
                        {
                            Id = customer.ID,
                            CustomerName = customer.Name,
                            WholesalerID = customer.WholesalerID,
                            WholesalerReference = customer.WholesalerReference,
                        };

                    return (APIResponse.Create(true, customerData));
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

        public async Task<APIResponse> UpdateCustomer(AddEditCustomerModel model)
        {
            try
            {
                if (model.Id > 0)
                {
                    var customer = _customerRepository.GetBy(c => c.ID == model.Id);
                    if (customer != null)
                    {
                        customer.Name = model.CustomerName;
                        if (model.IsCreateCustomerForWholesalePrtal==true)
                        {
                            customer.WholesalerID = Convert.ToInt32(customer.WholesalerID);
                        }
                        else
                        {
                            customer.WholesalerID = Convert.ToInt32(model.WholesalerID);
                        }
                        customer.WholesalerReference = model.WholesalerReference;
                        _customerRepository.Update(customer);
                        return (APIResponse.Create(true, "Customer has been updated successfully."));
                    }
                    else
                    {
                        return APIResponse.Create(false, "There is no item select to update record. ");
                    }
                }
                else
                {
                    return APIResponse.Create(false, "There is no item select to update record. ");
                }
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, ex.Message);
            }
        }
        public (APIResponse, IQueryable<CustomerModel>) GetAllCustomers(long? wholesalerID)
        {
            try
            {

                var customerUsers = _customerUserRepository.GetAll();
                var customers = _customerRepository.GetAll();
                if (wholesalerID.HasValue && wholesalerID.Value>0)
                {

                    var customersList = (
                        from cus in customers
                        join ws in _wholesalerRepository.GetAll() on Convert.ToInt64(cus.WholesalerID) equals ws.ID
                        where cus.WholesalerID== wholesalerID.Value
                        select new CustomerModel
                        {
                            Id = cus.ID,
                            CustomerName = cus.Name,
                            WholesalerReference = cus.WholesalerReference,
                            WholesalerID = cus.WholesalerID,
                            NoOfuser = customerUsers.Count(cu => cu.CustomerID == cus.ID),
                            WholesalerName = ws.Name
                        });

                    return (APIResponse.Create(true), customersList);
                }
                else
                {
                    var customersList = (
                        from cus in customers
                        join ws in _wholesalerRepository.GetAll() on Convert.ToInt64(cus.WholesalerID) equals ws.ID
                        
                        select new CustomerModel
                        {
                            Id = cus.ID,
                            CustomerName = cus.Name,
                            WholesalerReference = cus.WholesalerReference,
                            WholesalerID = cus.WholesalerID,
                            NoOfuser = customerUsers.Count(cu => cu.CustomerID == cus.ID),
                            WholesalerName = ws.Name
                        });

                    return (APIResponse.Create(true), customersList);

                }
            }
            catch (Exception ex)
            {
                return (APIResponse.Create(false, $"Error: {ex.Message}"), new List<CustomerModel>().AsQueryable());
            }
        }


        public (APIResponse, IQueryable<CustomerModel>) GetWholesalerCustomers()
        {
            try
            {

             
                 var userID = _userHelper.GetUserId();
                long id = 0;
                if (!String.IsNullOrEmpty(userID))
                {
                    id = long.Parse(userID);
                }
                
                var customerUsers = _customerUserRepository.GetAll();
                var customers = _customerRepository.GetAll();
                var wholerlist = _wholesalerUserRepository.GetAll();
                 

                    var customersList = (
                        from cus in customers
                        join ws in _wholesalerRepository.GetAll() on Convert.ToInt64(cus.WholesalerID) equals ws.ID
                        join wsuser in _wholesalerUserRepository.GetAll() on ws.ID equals wsuser.WholesalerID
                        where wsuser.UserID == id
                        select new CustomerModel
                        {
                            Id = cus.ID,
                            CustomerName = cus.Name,
                            WholesalerReference = cus.WholesalerReference,
                            WholesalerID = cus.WholesalerID,
                            NoOfuser = customerUsers.Count(cu => cu.CustomerID == cus.ID),
                            WholesalerName = ws.Name
                        });

                    return (APIResponse.Create(true), customersList);
 
               
            }
            catch (Exception ex)
            {
                return (APIResponse.Create(false, $"Error: {ex.Message}"), new List<CustomerModel>().AsQueryable());
            }
        }
        public async Task<APIResponse> GetAllWholesalerList()
        {
            var wholesaler=  _wholesalerRepository.GetAll();
            var lstAdmins = wholesaler.Select(u => new WholesalerViewModel
            {
               ID= u.ID,
                Name=u.Name,
            });
            return APIResponse.Create(true, lstAdmins);
        }

        public async Task<APIResponse> GetCustomerUserByCustomerID(long id)
        {
            var customerUsers = _customerUserRepository.GetAll(c => c.CustomerID == id).ToList();
            var users = await _userManager.Users.ToListAsync();
            var userList = (
                  from cuser in customerUsers
                  join u in users on Convert.ToInt64(cuser.UserID) equals u.Id
                  select new UserDetail
                  {
                      ID = cuser.UserID,
                      FirstName = u.FirstName,
                      LastName = u.LastName,
                      Email = u.Email,
                   
                  });
            return APIResponse.Create(true, userList);
        }
        public async Task<APIResponse> GetCustomerUserByUserID(long id)
        {
            var customerUsers = _customerUserRepository.GetAll(c => c.UserID == id).ToList();

       var customer=  _customerRepository.GetAll();

            var users = await _userManager.Users.ToListAsync();
            var userList = (
                  from cuser in customerUsers
                  join c in customer on Convert.ToInt64(cuser.CustomerID) equals c.ID
                  select new UserDetail
                  {
                      ID = cuser.UserID,
                      CustomerID = c.ID,
                      FirstName = c.Name,
                  });
            return APIResponse.Create(true, userList);
        }

        public async Task<APIResponse> AddCustomerLinks(Int32 userID,Int32 customerID)
        {
            try
            {
                CustomerUser custom = new CustomerUser();
                custom.CustomerID = customerID;
                custom.UserID = userID;
                _customerUserRepository.Add(custom);
               return APIResponse.Create(false, "Link has been created successfully.");
            }
            catch (Exception ex)
            {
                return APIResponse.Create(false, "There is no item select to update record. ");
            }
        }
    }
}
