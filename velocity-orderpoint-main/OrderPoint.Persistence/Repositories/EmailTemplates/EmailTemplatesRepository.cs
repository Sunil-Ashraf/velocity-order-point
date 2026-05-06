using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrderPoint.Application.Interfaces.Email;
using OrderPoint.Domain.Common;
using OrderPoint.Domain.DbContexts.Repositories;
using OrderPoint.Domain.Entities;
using OrderPoint.Domain.Helper;
using OrderPoint.Domain.ModifyIdentity;
using OrderPoint.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderPoint.Application.Services.EmailTemplates
{
    public class EmailTemplatesRepository : IEmailTemplatesRepository
    {
        private readonly IRepository<EmailTemplate> _emailTemplateRepository;
        private readonly IRepository<Lists> _listRepository;
        private readonly IRepository<SystemSetting> _systemSettingRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserHelper _userHelper;

        public EmailTemplatesRepository(IRepository<EmailTemplate> emailTemplateRepository, IRepository<Lists> listRepository, UserManager<ApplicationUser> userManager, UserHelper userHelper, IRepository<SystemSetting> systemSettingRepository)
        {
            _emailTemplateRepository = emailTemplateRepository;
            _listRepository = listRepository;
            _userManager = userManager;
            _userHelper = userHelper;
            _systemSettingRepository = systemSettingRepository;
        }

        public (APIResponse, IQueryable<EmailTemplateViewModel>) GetTemplatesAll(Int32? wholesalerID)
        {
            var temp = wholesalerID.HasValue ? _emailTemplateRepository.GetAll(j => j.WholesalerID == wholesalerID.Value)
                        : _emailTemplateRepository.GetAll(k => !(k.WholesalerID.HasValue));
            var email = from x in temp
                        select new EmailTemplateViewModel
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Subject = x.Subject,
                            IsDefault = x.IsDefault,
                            Body = x.Body,
                            EmailType = x.EmailTypeList.Name,
                            EmailTypeId = x.EmailTypeList.Id,
                            CreatedBy = Convert.ToString(x.CreatedBy),
                            UpdatedBy = Convert.ToString(x.UpdatedBy),
                            UpdationTime = x.UpdationTime,
                            CreationTime = x.CreationTime,
                        };

            return (APIResponse.Create(true), email);
        }

        public async Task<(APIResponse, List<object>)> GetListsByTypeName(string listType)
        {
            var list = await _listRepository.GetAll().Where(x => x.ListTypes.Name == listType && x.IsActive).OrderBy(x => x.SortOrder).Select(x => new
            {
                Value = x.Id.ToString(),
                Text = x.Name,
            }).ToListAsync();

            if (list == null || list.Count == 0)
                return (APIResponse.Create(false, "List not found."), new List<object>());

            return (APIResponse.Create(true), list.Cast<object>().ToList());
        }

        public async Task<APIResponse> Create(EmailTemplateViewModel model)
        {
            try
            {
                var userId = _userHelper.GetUserId();
                long parsedUserId;
                bool isParsed = long.TryParse(userId, out parsedUserId);

                if (!isParsed)
                    return APIResponse.Create(false, "User not found.");

                if (string.IsNullOrWhiteSpace(userId))
                    return APIResponse.Create(false, "User not found.");

                EmailTemplate temp = new EmailTemplate();
                temp.Id = model.Id;
                temp.Name = model.Name;
                temp.Subject = model.Subject;
                temp.IsDefault = model.IsDefault;
                temp.Body = model.Body;
                temp.EmailTypeId = model.EmailTypeId;
                temp.CreatedBy = parsedUserId;
                temp.CreationTime = DateTime.Now;
                temp.WholesalerID = model.WholesalerID;
                await _emailTemplateRepository.AddAsync(temp);

                List<EmailTemplate> oldList = new List<EmailTemplate>();
                if (temp.WholesalerID.HasValue)
                    oldList = _emailTemplateRepository.GetAll().Where(s => s.EmailTypeId == model.EmailTypeId && s.WholesalerID == temp.WholesalerID.Value).ToList();
                else
                    oldList = _emailTemplateRepository.GetAll().Where(s => s.EmailTypeId == model.EmailTypeId && !s.WholesalerID.HasValue).ToList();

                foreach (var item in oldList)
                {
                    if (item.Id != model.Id)
                    {
                        item.IsDefault = false;
                        await _emailTemplateRepository.UpdateAsync(item);
                    }
                }

                return APIResponse.Create(true, "Email template has been successfully added.");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<APIResponse> Update(EmailTemplateViewModel model)
        {
            var userId = _userHelper.GetUserId();
            long parsedUserId;
            bool isParsed = long.TryParse(userId, out parsedUserId);

            if (!isParsed)
                return APIResponse.Create(false, "User not found.");

            var template = await _emailTemplateRepository.GetAll().FirstOrDefaultAsync(s => s.Id == model.Id);
            if (template == null)
                return APIResponse.Create(false, "Email template not found.");

            template.Id = model.Id;
            template.Name = model.Name;
            template.Subject = model.Subject;
            template.IsDefault = model.IsDefault;
            template.Body = model.Body;
            template.EmailTypeId = model.EmailTypeId;
            template.WholesalerID = model.WholesalerID;
            template.UpdatedBy = parsedUserId;
            template.UpdationTime = DateTime.Now;
            await _emailTemplateRepository.UpdateAsync(template);

            return APIResponse.Create(true, "Email template has been successfully updated.");
        }

        public async Task<APIResponse> Delete(Guid Id)
        {
            var template = await _emailTemplateRepository.GetAll().FirstOrDefaultAsync(s => s.Id == Id);
            if (template == null)
                return APIResponse.Create(false, "Email template not found.");

            await _emailTemplateRepository.DeleteAsync(template);
            return APIResponse.Create(true, "Email template deleted successfully.");
        }

        public async Task<APIResponse> SetTemplateAsDefault(Guid Id)
        {
            var userId = _userHelper.GetUserId();
            long parsedUserId;
            bool isParsed = long.TryParse(userId, out parsedUserId);

            if (!isParsed)
                return APIResponse.Create(false, "User not found.");

            var template = await _emailTemplateRepository.GetAll().FirstOrDefaultAsync(s => s.Id == Id);
            if (template == null)
                return APIResponse.Create(false, "Email template not found.");

            template.IsDefault = true;
            template.UpdatedBy = parsedUserId;
            template.UpdationTime = DateTime.Now;
            await _emailTemplateRepository.UpdateAsync(template);

            List<EmailTemplate> oldTemplatesList = new List<EmailTemplate>();
            if (template.WholesalerID.HasValue)
                oldTemplatesList = _emailTemplateRepository.GetAll().Where(s => s.EmailTypeId == template.EmailTypeId && s.WholesalerID == template.WholesalerID).ToList();
            else
                oldTemplatesList = _emailTemplateRepository.GetAll().Where(s => s.EmailTypeId == template.EmailTypeId && !s.WholesalerID.HasValue).ToList();

            foreach (var item in oldTemplatesList)
            {
                if (item.Id != template.Id)
                {
                    item.IsDefault = false;
                    await _emailTemplateRepository.UpdateAsync(item);
                }
            }

            return APIResponse.Create(true, "Email template has been successfully set as default.");
        }

        public async Task<(APIResponse, EmailTemplateViewModel)> GetTemplateForEmail(string type)
        {
            try
            {
                var list = await _listRepository.GetAll().FirstOrDefaultAsync(i => i.Name == type);
                if (list == null)
                    return (APIResponse.Create(false, "Email type not found."), new EmailTemplateViewModel());

                var emailTemplate = await _emailTemplateRepository.GetAll().FirstOrDefaultAsync(s => s.EmailTypeId == list.Id && s.IsDefault);
                if (emailTemplate == null)
                    return (APIResponse.Create(false, "Email template not found."), new EmailTemplateViewModel());

                var model = new EmailTemplateViewModel();
                model.Name = emailTemplate.Name;
                model.Subject = emailTemplate.Subject;
                model.IsDefault = emailTemplate.IsDefault;
                model.Body = emailTemplate.Body;
                model.EmailType = emailTemplate.EmailTypeList.Name;
                model.EmailTypeId = emailTemplate.EmailTypeList.Id;
                model.CreatedBy = Convert.ToString(emailTemplate.CreatedBy);
                model.UpdatedBy = Convert.ToString(emailTemplate.UpdatedBy);
                model.UpdationTime = emailTemplate.UpdationTime;
                model.CreationTime = emailTemplate.CreationTime;

                return (APIResponse.Create(true), model);
            }
            catch (Exception ex)
            {
                return (APIResponse.Create(false, ex.Message), new EmailTemplateViewModel());
            }
        }

        public async Task<(APIResponse, List<SystemSetting>)> GetSystemSettingList()
        {
            var list = await _systemSettingRepository.GetAll().ToListAsync();
            if (list == null || list.Count == 0)
                return (APIResponse.Create(false, "List not found."), new List<SystemSetting>());

            return (APIResponse.Create(true), list);
        }

        public async Task<APIResponse> UpdateSystemSetting(List<SystemSetting> model)
        {
            try
            {
                _systemSettingRepository.UpdateList(model);
                return APIResponse.Create(true, "Setting has been updated successfully.");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}