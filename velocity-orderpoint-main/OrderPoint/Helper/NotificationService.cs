using OrderPoint.Domain.ViewModel;

namespace OrderPoint.Helper
{
    public class NotificationService
    {
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public OrderPlacementModel CurrentOrder { get; set; }
    }
}
