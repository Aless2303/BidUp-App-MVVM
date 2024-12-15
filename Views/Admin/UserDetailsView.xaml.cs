using System.Windows;
using System.Windows.Controls;
using BidUp_App.Models.Users;
using BidUp_App.ViewModels;

namespace BidUp_App.Views.Admin
{
    public partial class UserDetailsView : UserControl
    {
        public UserDetailsView(BidUp_App.Models.Users.User user)
        {
            InitializeComponent();

            if (user != null)
            {
                DataContext = new UserDetailsViewModel(user);
            }
            else
            {
                MessageBox.Show("User data is missing!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
