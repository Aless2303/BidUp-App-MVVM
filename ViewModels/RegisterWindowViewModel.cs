using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace BidUp_App.ViewModels
{
    public class RegisterWindowViewModel : BaseViewModel
    {
        // Proprietăți pentru binding
        private string _fullName;
        private string _email;
        private string _password;
        private string _confirmPassword;
        private string _role;
        private string _cardNumber;
        private string _cardHolderName;
        private string _expiryDate;
        private string _cvv;
        private int? _birthDay;
        private string _birthMonth;
        private int? _birthYear;

        public string FullName { get => _fullName; set { _fullName = value; OnPropertyChanged(nameof(FullName)); } }
        public string Email { get => _email; set { _email = value; OnPropertyChanged(nameof(Email)); } }
        public string Password { get => _password; set { _password = value; OnPropertyChanged(nameof(Password)); } }
        public string ConfirmPassword { get => _confirmPassword; set { _confirmPassword = value; OnPropertyChanged(nameof(ConfirmPassword)); } }
        public string Role
        {
            get => _role;
            set
            {
                if (value == "Select Role")
                {
                    _role = null;
                }
                else
                {
                    _role = value;
                }
                OnPropertyChanged(nameof(Role));
            }
        }

        public string CardNumber { get => _cardNumber; set { _cardNumber = value; OnPropertyChanged(nameof(CardNumber)); } }
        public string CardHolderName { get => _cardHolderName; set { _cardHolderName = value; OnPropertyChanged(nameof(CardHolderName)); } }
        public string ExpiryDate { get => _expiryDate; set { _expiryDate = value; OnPropertyChanged(nameof(ExpiryDate)); } }
        public string CVV { get => _cvv; set { _cvv = value; OnPropertyChanged(nameof(CVV)); } }
        public int? BirthDay { get => _birthDay; set { _birthDay = value; OnPropertyChanged(nameof(BirthDay)); } }
        public string BirthMonth { get => _birthMonth; set { _birthMonth = value; OnPropertyChanged(nameof(BirthMonth)); } }
        public int? BirthYear { get => _birthYear; set { _birthYear = value; OnPropertyChanged(nameof(BirthYear)); } }

        // Colecții pentru zile, luni și ani
        public ObservableCollection<int> Days { get; }
        public ObservableCollection<string> Months { get; }
        public ObservableCollection<int> Years { get; }

        // Comenzi
        public ICommand RegisterCommand { get; }
        public ICommand BackToSignInCommand { get; }

        public RegisterWindowViewModel()
        {
            Days = new ObservableCollection<int>(Enumerable.Range(1, 31));
            Months = new ObservableCollection<string>(DateTimeFormatInfo.CurrentInfo.MonthNames.Take(12));
            Years = new ObservableCollection<int>(Enumerable.Range(1900, DateTime.Now.Year - 1900 + 1));

            RegisterCommand = new RelayCommand(Register);
            BackToSignInCommand = new RelayCommand(BackToSignIn);
        }

        private void Register()
        {
            if (!ValidateInputs())
                return;

            try
            {
                using (var context = new BidUpEntities()) // Entity Framework Context
                {
                    var newUser = new User
                    {
                        FullName = FullName,
                        Email = Email,
                        PasswordHash = HashPassword(Password),
                        Role = Role,
                        BirthDate = new DateTime(BirthYear ?? 1900, DateTime.ParseExact(BirthMonth, "MMMM", null).Month, BirthDay ?? 1),
                        ProfilePicturePath = @"C:\Users\Florea\source\repos\BidUp-App\Resources\profil2.png",
                        CreatedAt = DateTime.Now
                    };

                    // Adaugă utilizator în context și salvează
                    context.Users.Add(newUser);
                    context.SaveChanges();

                    if (Role == "Bidder" || Role == "Seller")
                    {
                        var newCard = new Card
                        {
                            CardNumber = CardNumber,
                            CardHolderName = CardHolderName,
                            ExpiryDate = DateTime.ParseExact(ExpiryDate, "MM/yy", null),
                            CVV = CVV,
                            Balance = 0,
                            OwnerUserID = newUser.UserID
                        };

                        context.Cards.Add(newCard);
                        context.SaveChanges();
                    }

                    MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) ||
                string.IsNullOrEmpty(ConfirmPassword) || string.IsNullOrEmpty(CardNumber) || string.IsNullOrEmpty(CardHolderName) ||
                string.IsNullOrEmpty(ExpiryDate) || string.IsNullOrEmpty(CVV) || !BirthDay.HasValue || string.IsNullOrEmpty(BirthMonth) || !BirthYear.HasValue)
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid email format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            using (var context = new BidUpEntities())
            {
                if (context.Users.Any(u => u.Email == Email))
                {
                    MessageBox.Show("Email already exists in the database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            if (Password != ConfirmPassword)
            {
                MessageBox.Show("Passwords do not match. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Regex.IsMatch(Password, @"^(?=.[a-z])(?=.[A-Z])(?=.*\d).{6,}$"))
            {
                MessageBox.Show("Password must contain at least one lowercase letter, one uppercase letter, one digit, and be at least 6 characters long.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Regex.IsMatch(CardNumber, @"^\d{16}$"))
            {
                MessageBox.Show("Card number must be exactly 16 digits.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Regex.IsMatch(CVV, @"^\d{3}$"))
            {
                MessageBox.Show("CVV must be exactly 3 digits.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Regex.IsMatch(ExpiryDate, @"^(0[1-9]|1[0-2])/\d{2}$") || !IsValidExpiryDate(ExpiryDate))
            {
                MessageBox.Show("Invalid Expiry Date format. Please use MM/YY.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }


        // Funcție pentru validarea datei de expirare
        private bool IsValidExpiryDate(string expiryDate)
        {
            try
            {
                var parts = expiryDate.Split('/');
                int month = int.Parse(parts[0]);
                int year = int.Parse(parts[1]) + 2000; // Transformăm YY în YYYY

                var expiry = new DateTime(year, month, DateTime.DaysInMonth(year, month));
                return expiry >= DateTime.Now;
            }
            catch
            {
                return false;
            }
        }


        private void BackToSignIn()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();

            foreach (Window window in Application.Current.Windows)
            {
                if (window is RegisterWindow)
                {
                    window.Close();
                    break;
                }
            }
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