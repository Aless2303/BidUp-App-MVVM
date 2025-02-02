using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BidUp_App.Models;
using BidUp_App.Models.Users;
using BidUp_App.Views.Admin;

namespace BidUp_App.ViewModels
{
    public class ManageUsersViewModel : BaseViewModel
    {
        private readonly BidUpEntities _dbContext;

        public ObservableCollection<User> Users { get; private set; }

        // Comenzi utilizate
        public ICommand ShowUserDetailsCommand { get; }
        public ICommand DeleteUserCommand { get; }

        public ManageUsersViewModel()
        {
            _dbContext = new BidUpEntities();
            Users = new ObservableCollection<User>();

            // Inițializare comenzi
            ShowUserDetailsCommand = new RelayCommand<int>(ShowUserDetails);
            DeleteUserCommand = new RelayCommand<int>(DeleteUser);

            LoadUsers();
        }

        private void LoadUsers()
        {
            var users = _dbContext.Users
                .Where(u => u.Role == "Admin" || u.Role == "User") // Filtrează doar Admin și User
                .ToList();

            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(user);
            }
        }

        private void ShowUserDetails(int userId)
        {
            // Găsește utilizatorul din baza de date
            var dbUser = _dbContext.Users.SingleOrDefault(u => u.UserID == userId);

            if (dbUser != null)
            {
                // Inițializează instanța potrivită în funcție de rol
                BidUp_App.Models.Users.User user = null;

                switch (dbUser.Role)
                {
                    case "Admin":
                        user = new BidUp_App.Models.Users.Admin
                        {
                            m_userID = dbUser.UserID,
                            m_fullName = dbUser.FullName,
                            m_email = dbUser.Email,
                            m_BirthDate = dbUser.BirthDate,
                            ProfilePicturePath = dbUser.ProfilePicturePath,
                            m_password = dbUser.PasswordHash
                        };
                        break;

                    case "User":
                        user = new BidUp_App.Models.Users.Bidder // Folosim Bidder pentru User
                        {
                            m_userID = dbUser.UserID,
                            m_fullName = dbUser.FullName,
                            m_email = dbUser.Email,
                            m_BirthDate = dbUser.BirthDate,
                            ProfilePicturePath = dbUser.ProfilePicturePath,
                            m_password = dbUser.PasswordHash
                        };
                        break;
                }

                if (user != null)
                {
                    // Creează UserDetailsView și setează-l
                    var parentWindow = Application.Current.Windows.OfType<AdminDashboard>().FirstOrDefault();
                    if (parentWindow != null && parentWindow.DataContext is AdminDashboardViewModel adminViewModel)
                    {
                        // Setăm UserDetailsView în CurrentView
                        adminViewModel.CurrentView = new UserDetailsView(user)
                        {
                            DataContext = new UserDetailsViewModel(user)
                        };
                    }
                }
                else
                {
                    MessageBox.Show("Unknown user role.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("User not found.");
            }
        }

        private void DeleteUser(int userId)
        {
            try
            {
                var user = _dbContext.Users.SingleOrDefault(u => u.UserID == userId);

                if (user != null)
                {
                    // Ștergere notificări asociate
                    var notifications = _dbContext.UserNotifications.Where(n => n.UserID == userId).ToList();
                    _dbContext.UserNotifications.RemoveRange(notifications);

                    // Acum ștergem utilizatorul
                    _dbContext.Users.Remove(user);
                    _dbContext.SaveChanges();

                    MessageBox.Show($"User with ID: {userId} has been deleted.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadUsers();
                }
                else
                {
                    MessageBox.Show("User not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}