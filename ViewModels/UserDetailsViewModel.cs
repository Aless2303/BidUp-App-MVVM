using BidUp_App.Models.Users;
using BidUp_App.Views.Admin;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace BidUp_App.ViewModels
{
    public class UserDetailsViewModel : BaseViewModel
    {
        private readonly BidUp_App.Models.Users.User _user;

        public UserDetailsViewModel() { }

        public UserDetailsViewModel(BidUp_App.Models.Users.User user)
        {
            _user = user;

            // Inițializare comenzi
            ViewHistoryCommand = new RelayCommand(ViewHistory);
            BackCommand = new RelayCommand(GoBack);
        }

        // Proprietăți expuse pentru Binding
        public string FullName => _user.m_fullName;
        public string Role => _user.m_role;
        public string Email => _user.m_email;
        public string Phone => "N/A"; // Dacă nu există, returnăm un placeholder
        public string ProfilePicturePath => _user.ProfilePicturePath;

        // Comenzi
        public ICommand ViewHistoryCommand { get; }
        public ICommand BackCommand { get; }

        private void ViewHistory()
        {
            if (_user == null)
            {
                MessageBox.Show("User data is missing!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var userHistoryView = new UserHistoryView(_user);
            var parentWindow = Application.Current.Windows.OfType<AdminDashboard>().FirstOrDefault();

            if (parentWindow != null)
            {
                // Accesăm DataContext-ul AdminDashboard pentru a schimba CurrentView
                if (parentWindow.DataContext is AdminDashboardViewModel adminViewModel)
                {
                    adminViewModel.CurrentView = userHistoryView;
                }
            }
        }


        private void GoBack()
        {
            var manageUsersView = new ManageUsersView
            {
                DataContext = new ManageUsersViewModel()
            };

            var parentWindow = Application.Current.Windows.OfType<AdminDashboard>().FirstOrDefault();

            if (parentWindow != null)
            {
                // Accesăm DataContext-ul AdminDashboard pentru a schimba CurrentView
                if (parentWindow.DataContext is AdminDashboardViewModel adminViewModel)
                {
                    adminViewModel.CurrentView = manageUsersView;
                }
            }
        }

    }
}
