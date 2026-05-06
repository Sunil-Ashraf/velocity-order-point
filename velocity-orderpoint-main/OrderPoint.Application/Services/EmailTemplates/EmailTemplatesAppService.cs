using OrderPoint.Application.Interfaces.Email;
using OrderPoint.Domain.Common;
using OrderPoint.Domain.Entities;
using OrderPoint.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderPoint.Application.Services.EmailTemplates
{
    public class EmailTemplatesAppService : IEmailTemplatesAppService
    {
        private readonly IEmailTemplatesRepository _EmailTemplatesRepository;

        public EmailTemplatesAppService(IEmailTemplatesRepository emailTemplatesRepository)
        {
            _EmailTemplatesRepository = emailTemplatesRepository;
        }

        public Task<(APIResponse, List<object>)> GetListsByTypeName(string listType)
        {
            return _EmailTemplatesRepository.GetListsByTypeName(listType);
        }

        public Task<(APIResponse, List<SystemSetting>)> GetSystemSettingList()
        {
            return _EmailTemplatesRepository.GetSystemSettingList();
        }

        public (APIResponse, IQueryable<EmailTemplateViewModel>) GetTemplatesAll(Int32? wholesalerID)
        {
            return _EmailTemplatesRepository.GetTemplatesAll(wholesalerID);
        }

        public Task<APIResponse> Update(EmailTemplateViewModel model)
        {
            return _EmailTemplatesRepository.Update(model);
        }

        public Task<APIResponse> Create(EmailTemplateViewModel model)
        {
            return _EmailTemplatesRepository.Create(model);
        }

        public Task<APIResponse> UpdateSystemSetting(List<SystemSetting> model)
        {
            return _EmailTemplatesRepository.UpdateSystemSetting(model);
        }

        public Task<APIResponse> Delete(Guid Id)
        {
            return _EmailTemplatesRepository.Delete(Id);
        }

        public Task<(APIResponse, EmailTemplateViewModel)> GetTemplateForEmail(string type)
        {
            return _EmailTemplatesRepository.GetTemplateForEmail(type);
        }

        public Task<APIResponse> SetTemplateAsDefault(Guid Id)
        {
            return _EmailTemplatesRepository.SetTemplateAsDefault(Id);
        }
    }
}