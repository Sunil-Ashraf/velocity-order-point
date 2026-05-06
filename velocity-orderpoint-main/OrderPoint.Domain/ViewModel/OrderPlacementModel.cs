using OrderPoint.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.ViewModel
{
    public class OrderPlacementModel
    {
        //[RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Only letters and numbers are allowed")]

        public String? OrderNumber { get; set; }
        [Required(ErrorMessage = "Required")]
        public  DateTime OrderDate { get; set; }
       // [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Only letters and numbers are allowed")]

        public String? OrderNotes { get; set; }
   

        public List<ProductsViewModel> OrderItems { get; set; }
        public Int32? OrderID { get; set; }
    }
 

}
