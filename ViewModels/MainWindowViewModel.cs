using BidUp_App.Models.Users; // Pentru clasele de utilizatori din aplicație
using System.Data.Entity; // Entity Framework
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace BidUp_App.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private string _email;
        private string _password;
        private string _role;

        public MainWindowViewModel()
        {
            // Inițializează comenzile
            SignInCommand = new RelayCommand(SignIn);
            SignUpCommand = new RelayCommand(SignUp);
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(nameof(Email)); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        public string Role
        {
            get => _role;
            set { _role = value; OnPropertyChanged(nameof(Role)); }
        }

        public ICommand SignInCommand { get; }
        public ICommand SignUpCommand { get; }

        private void SignIn()
        {
            if (!ValidateInputs())
                return;

            using (var context = new BidUpEntities()) // Contextul Entity Framework
            {
                // Hash the password
                string passwordHash = HashPassword(Password);

                // Retrieve user from database
                var dbUser = context.Users
                    .SingleOrDefault(u => u.Email == Email && u.PasswordHash == passwordHash);

                if (dbUser != null)
                {
                    // Validare rol
                    if (string.IsNullOrEmpty(dbUser.Role) || !IsValidRole(dbUser.Role))
                    {
                        MessageBox.Show("Invalid role assigned to this user. Please contact support.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Creare utilizator pe baza rolului
                    var user = UserFactory.CreateUser(dbUser.Role);

                    user.m_userID = dbUser.UserID;
                    user.m_fullName = dbUser.FullName;
                    user.m_email = dbUser.Email;
                    user.m_BirthDate = dbUser.BirthDate;
                    user.ProfilePicturePath = dbUser.ProfilePicturePath;
                    user.m_password = dbUser.PasswordHash;

                    // Încarcă Card-ul pentru Bidder și Seller
                    if (user is Bidder || user is Seller)
                    {
                        var card = context.Cards
                            .SingleOrDefault(c => c.OwnerUserID == dbUser.UserID);

                        if (card != null)
                        {
                            var userCard = new Models.Card
                            {
                                CardID = card.CardID,
                                CardNumber = card.CardNumber,
                                CardHolderName = card.CardHolderName,
                                ExpiryDate = card.ExpiryDate,
                                CVV = card.CVV,
                                Balance = (float)card.Balance
                            };

                            if (user is Bidder bidder)
                            {
                                bidder.card = userCard;
                            }
                            else if (user is Seller seller)
                            {
                                seller.card = userCard;
                            }
                        }
                    }

                    // Afișează dashboard-ul potrivit rolului
                    user.displayDasboard();

                    // Închide fereastra curentă
                    CloseCurrentWindow();
                }
                else
                {
                    // Mesaj pentru autentificare eșuată
                    MessageBox.Show("Invalid email or password. Please try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SignUp()
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            CloseCurrentWindow();
        }

        private void CloseCurrentWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow)
                {
                    window.Close();
                    break;
                }
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Please enter both email and password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(Role) || Role == "Select Role")
            {
                MessageBox.Show("Please select a valid role.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private bool IsValidRole(string role)
        {
            string[] validRoles = { "Admin", "Bidder", "Seller" };
            return validRoles.Contains(role);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                return string.Concat(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)).Select(b => b.ToString("x2")));
            }
        }
    }
}
