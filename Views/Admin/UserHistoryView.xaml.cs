using System.Windows.Controls;
using BidUp_App.ViewModels;

namespace BidUp_App.Views.Admin
{
    public partial class UserHistoryView : UserControl
    {
        public UserHistoryView(BidUp_App.Models.Users.User user)
        {
            InitializeComponent();
            DataContext = new UserHistoryViewModel(user);
        }
    }
}
