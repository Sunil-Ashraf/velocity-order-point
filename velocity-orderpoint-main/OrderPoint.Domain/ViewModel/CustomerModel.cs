using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.ViewModel
{
    public class CustomerModel
    {
        public long Id { get; set; }
        public string? CustomerName { get; set; }
        public string? Wholesaler { get; set; }
        public Int32? WholesalerID { get; set; }
        public string WholesalerReference { get; set; }
        public string WholesalerName { get; set; }
        public Int32? NoOfuser { get; set; }
       
    }

    public class AddEditCustomerModel
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Required")]
        [MaxLength(200, ErrorMessage = "First name cannot be more than 200 characters.")]

        public string CustomerName { get; set; }
        public string? WholesalerReference { get; set; }
        [Required(ErrorMessage = "Required")]
        public Int32? WholesalerID { get; set; }
        public bool? IsCreateCustomerForWholesalePrtal { get; set; }


    }
}
