using System.Windows.Input;
using BidUp_App.Models;
using BidUp_App.Models.Users;
using BidUp_App.Views.Admin;
using BidUp_App.Views.Bidder;

namespace BidUp_App.ViewModels
{
    public class AdminDashboardViewModel : BaseViewModel
    {
        private readonly BidUp_App.Models.Users.User _admin;
        private readonly DataContextDataContext _dbContext;

        private object _currentView;

        public AdminDashboardViewModel() { }

        public AdminDashboardViewModel(BidUp_App.Models.Users.User admin)
        {
            _admin = admin;
            _dbContext = new DataContextDataContext();

            // Inițializare comenzi pentru navigare
            ProfileCommand = new RelayCommand(LoadProfileView);
            ManageUsersCommand = new RelayCommand(LoadManageUsersView);
            ManageAuctionsCommand = new RelayCommand(LoadManageAuctionsView);

            // Setăm view-ul implicit
            LoadProfileView();
        }

        // Proprietatea pentru View curent
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        // Comenzi pentru navigare
        public ICommand ProfileCommand { get; }
        public ICommand ManageUsersCommand { get; }
        public ICommand ManageAuctionsCommand { get; }

        private void LoadProfileView()
        {
            // Creează ProfileViewModel pentru Admin
            var profileViewModel = new AdminProfileViewModel(_admin);

            // Creează AdminProfileView și setează DataContext
            var profileView = new AdminProfileView
            {
                DataContext = profileViewModel
            };

            // Setează View-ul curent
            CurrentView = profileView;
        }

        private void LoadManageUsersView()
        {
            // Creează ManageUsersViewModel (dacă este necesar)
            var manageUsersViewModel = new ManageUsersViewModel();

            // Creează ManageUsersView și setează DataContext
            var manageUsersView = new ManageUsersView
            {
                DataContext = manageUsersViewModel
            };

            // Setează View-ul curent
            CurrentView = manageUsersView;
        }

        private void LoadManageAuctionsView()
        {
            var manageAuctionsViewModel = new ManageAuctionsViewModel();
            var manageAuctionsView = new ManageAuctionsView
            {
                DataContext = manageAuctionsViewModel
            };

            CurrentView = manageAuctionsView;
        }


    }
}
