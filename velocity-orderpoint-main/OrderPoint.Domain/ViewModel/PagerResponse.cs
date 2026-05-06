using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.ViewModel
{

    public class PagerResponse<T>
    {
        public int totalRecords { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public List<T> data { get; set; }

        public AddEditWholesalerModel wholesalerDetail { get; set; }
    }
}
