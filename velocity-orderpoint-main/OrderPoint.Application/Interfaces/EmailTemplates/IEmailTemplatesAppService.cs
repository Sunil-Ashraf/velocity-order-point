using OrderPoint.Domain.Common;
using OrderPoint.Domain.Entities;
using OrderPoint.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderPoint.Application.Interfaces.Email
{
    public interface IEmailTemplatesAppService
    {
        Task<(APIResponse, List<object>)> GetListsByTypeName(string listType);
        (APIResponse, IQueryable<EmailTemplateViewModel>) GetTemplatesAll(Int32? wholesalerID);
        Task<APIResponse> Update(EmailTemplateViewModel model);
        Task<APIResponse> Create(EmailTemplateViewModel model);
        Task<APIResponse> UpdateSystemSetting(List<SystemSetting> model);
        Task<APIResponse> Delete(Guid Id);
        Task<(APIResponse, EmailTemplateViewModel)> GetTemplateForEmail(string type);
        Task<APIResponse> SetTemplateAsDefault(Guid Id);
        Task<(APIResponse, List<SystemSetting>)> GetSystemSettingList();
    }
}