using Microsoft.AspNetCore.Mvc.Rendering;
using OrderPoint.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Application.Interfaces.Email
{
    public interface IEmailAppService
    {

        Task SendEmailAsync(string email, string ccEmail, string bccEmail, string subject, string message);


    }
}
