using OrderPoint.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.Common
{
    public static class EmailContent
    {
        public static string ReplaceContent(EmailViewModel model, string content)
        {
       

              content = content.Replace("[[First Name]]", model.FirstName);
              content = content.Replace("[[Last Name]]", model.LastName);
              content = content.Replace("[[Email]]", model.Email); 
         
              content = content.Replace("[[Username]]", model.Username);
              content = content.Replace("[[Password]]", model.Password);
              content = content.Replace("[[Supplier]]", model.Supplier);
            content = content.Replace("[[Telephone]]", model.Telephone);		
            content = content.Replace("[[Address]]", model.Address);		
            content = content.Replace("[[Date]]", model.Date);			
            content = content.Replace("[[RequiredDate]]", model.RequiredDate);
            content = content.Replace("[[ItemList]]", model.ItemList);
            content = content.Replace("[[RefernceNumber]]", model.ReferenceNumber);
            content = content.Replace("[[Notes]]", model.Notes);

            if (!String.IsNullOrEmpty(model.ClickHere))
            {
               string link = "<a href=" + model.ClickHere + " style='display: inline-block;background-color: #007bff;color: #ffffff; padding: 12px 20px;text-decoration: none;\r\n            border-radius: 5px;\r\n            font-size: 16px;'> Click here</a>";
               content = content.Replace("[[Click Here]]", link);
            }

            return content;
        }
    }
}
