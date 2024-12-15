using System.Windows;
using BidUp_App.Models.Users;
using BidUp_App.ViewModels;

namespace BidUp_App.Views.Admin
{
    public partial class AdminDashboard : Window
    {
        public AdminDashboard(BidUp_App.Models.Users.User user)
        {
            InitializeComponent();
            DataContext = new AdminDashboardViewModel(user);
        }
    }
}
