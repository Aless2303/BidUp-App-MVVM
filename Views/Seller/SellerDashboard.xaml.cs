using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using BidUp_App.Models.Users;
using BidUp_App.ViewModels;

namespace BidUp_App.Views.Seller
{
    public partial class SellerDashboard : Window
    {
        private readonly SellerDashboardViewModel _viewModel;

        public SellerDashboard(BidUp_App.Models.Users.Seller user)
        {
            InitializeComponent();

            if (user is not BidUp_App.Models.Users.Seller seller)
            {
                MessageBox.Show("Invalid user type. This dashboard is only accessible to sellers.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            // Set ViewModel with Seller data
            _viewModel = new SellerDashboardViewModel(seller);
            DataContext = _viewModel;
        }
    }
}
