using System.Windows;
using BidUp_App.ViewModels;
using BidUp_App.Models.Users;

namespace BidUp_App.Views.Bidder
{
    public partial class BidderDashboard : Window
    {
        public BidderDashboard(BidUp_App.Models.Users.Bidder user)
        {
            InitializeComponent();

            // Setăm DataContext pentru a lega View-ul de ViewModel
            DataContext = new BidderDashboardViewModel(user);
        }

    }
}
