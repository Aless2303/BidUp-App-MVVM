using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BidUp_App.ViewModels
{
    public class ToastNotificationViewModel : BaseViewModel
    {
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
