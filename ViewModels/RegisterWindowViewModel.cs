using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

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
                    _role = null; // Evităm selectarea implicită invalidă
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
            // Populate zile, luni și ani
            Days = new ObservableCollection<int>(Enumerable.Range(1, 31));
            Months = new ObservableCollection<string>(DateTimeFormatInfo.CurrentInfo.MonthNames.Take(12)); // Primele 12 luni
            Years = new ObservableCollection<int>(Enumerable.Range(1900, DateTime.Now.Year - 1900 + 1)); // 1900 - Anul curent

            RegisterCommand = new RelayCommand(Register);
            BackToSignInCommand = new RelayCommand(BackToSignIn);
        }

        private void Register()
        {
            // Validare date
            if (!ValidateInputs())
                return;

            // Crearea utilizatorului
            try
            {
                using (var context = new DataContextDataContext())
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

                    context.Users.InsertOnSubmit(newUser);
                    context.SubmitChanges();

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

                        context.Cards.InsertOnSubmit(newCard);
                        context.SubmitChanges();
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

            if (Password != ConfirmPassword)
            {
                MessageBox.Show("Passwords do not match. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Regex.IsMatch(CardNumber, @"^\d{13}$"))
            {
                MessageBox.Show("Card number must be exactly 13 digits.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void BackToSignIn()
        {
            // Deschide fereastra principală (Sign In)
            var mainWindow = new MainWindow();
            mainWindow.Show();

            // Închide fereastra curentă
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
