using BidUp_App.Models.Users;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace BidUp_App.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly BidUp_App.Models.Users.User _user;
        private readonly DataContextDataContext _dbContext;

        private bool _isCardVisible;
        private BitmapImage _profileImage;

        public ProfileViewModel()
        {

        }


        public ICommand AddFundsCommand { get; }
        public ICommand DeductFundsCommand { get; }

        public ProfileViewModel(BidUp_App.Models.Users.User user, DataContextDataContext dbContext)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));



            // Commands
            ViewCardCommand = new RelayCommand(ViewCard);
            HideCardCommand = new RelayCommand(HideCard);
            ChangeProfilePictureCommand = new RelayCommand(ChangeProfilePicture);
            AddFundsCommand = new RelayCommand(AddFunds);
            DeductFundsCommand = new RelayCommand(DeductFunds);

            // Initialize properties
            LoadProfile();
        }

        // Proprietăți Binding
        public string FullName => _user.m_fullName;
        public string Email => _user.m_email;
        public string DateOfBirth => _user.m_BirthDate.ToString("d");
        public string Role => _user.m_role;

        public bool IsCardVisible
        {
            get => _isCardVisible;
            set
            {
                _isCardVisible = value;
                OnPropertyChanged(nameof(IsCardVisible));
            }
        }

        public BitmapImage ProfileImage
        {
            get => _profileImage;
            set
            {
                _profileImage = value;
                OnPropertyChanged(nameof(ProfileImage));
            }
        }

        public string CardHolder { get; private set; }
        public string CardNumber { get; private set; }
        public string ExpiryDate { get; private set; }

        // Comenzi
        public ICommand ViewCardCommand { get; }
        public ICommand HideCardCommand { get; }
        public ICommand ChangeProfilePictureCommand { get; }

        private string _addFundsAmount;
        private string _deductFundsAmount;

        public string AddFundsAmount
        {
            get => _addFundsAmount;
            set
            {
                _addFundsAmount = value;
                OnPropertyChanged(nameof(AddFundsAmount));
            }
        }

        public string DeductFundsAmount
        {
            get => _deductFundsAmount;
            set
            {
                _deductFundsAmount = value;
                OnPropertyChanged(nameof(DeductFundsAmount));
            }
        }


        private void AddFunds()
        {
            if (decimal.TryParse(AddFundsAmount, out var amount) && amount > 0)
            {
                var wallet = _dbContext.Wallets.FirstOrDefault(w => w.UserID == _user.m_userID);

                if (wallet != null)
                {
                    wallet.Balance += amount;
                    wallet.LastUpdated = DateTime.Now;
                }
                else
                {
                    wallet = new Wallet
                    {
                        UserID = _user.m_userID,
                        Balance = amount,
                        LastUpdated = DateTime.Now
                    };
                    _dbContext.Wallets.InsertOnSubmit(wallet);
                }

                _dbContext.SubmitChanges();
                MessageBox.Show("Funds added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please enter a valid amount.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            AddFundsAmount = string.Empty;
            OnPropertyChanged(nameof(AddFundsAmount));
        }

        private void DeductFunds()
        {
            if (decimal.TryParse(DeductFundsAmount, out var amount) && amount > 0)
            {
                var wallet = _dbContext.Wallets.FirstOrDefault(w => w.UserID == _user.m_userID);

                if (wallet != null && wallet.Balance >= amount)
                {
                    wallet.Balance -= amount;
                    wallet.LastUpdated = DateTime.Now;
                    _dbContext.SubmitChanges();
                    MessageBox.Show("Funds deducted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Insufficient funds in wallet.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid amount.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            DeductFundsAmount = string.Empty;
            OnPropertyChanged(nameof(DeductFundsAmount));
        }



        private void LoadProfile()
        {
            if (!string.IsNullOrEmpty(_user.ProfilePicturePath) && File.Exists(_user.ProfilePicturePath))
            {
                ProfileImage = new BitmapImage(new Uri(_user.ProfilePicturePath, UriKind.Absolute));
            }

            IsCardVisible = false;
        }

        private void ViewCard()
        {
            string password = PromptForPassword();
            if (IsPasswordValid(password))
            {
                LoadCardDetails();
                IsCardVisible = true;
            }
            else
            {
                MessageBox.Show("Incorrect password. Please try again.", "Authentication Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HideCard()
        {
            IsCardVisible = false;
        }

        private void LoadCardDetails()
        {
            var card = _dbContext.Cards.FirstOrDefault(c => c.OwnerUserID == _user.m_userID);
            if (card != null)
            {
                CardHolder = card.CardHolderName;
                CardNumber = "**** **** **** " + card.CardNumber.Substring(card.CardNumber.Length - 4);
                ExpiryDate = card.ExpiryDate.ToString("MM/yy");
                OnPropertyChanged(nameof(CardHolder));
                OnPropertyChanged(nameof(CardNumber));
                OnPropertyChanged(nameof(ExpiryDate));
            }
            else
            {
                MessageBox.Show("No card information found.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ChangeProfilePicture()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var selectedFilePath = openFileDialog.FileName;
                ProfileImage = new BitmapImage(new Uri(selectedFilePath));
                _user.ProfilePicturePath = selectedFilePath;
                SaveProfilePicturePathToDatabase(_user.m_userID, selectedFilePath);
            }
        }

        private void SaveProfilePicturePathToDatabase(int userId, string profilePicturePath)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.UserID == userId);
            if (user != null)
            {
                user.ProfilePicturePath = profilePicturePath;
                _dbContext.SubmitChanges();
            }
        }

        private string PromptForPassword()
        {
            return Microsoft.VisualBasic.Interaction.InputBox("Enter your password to view card details:", "Password Required", "");
        }

        private bool IsPasswordValid(string enteredPassword)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var enteredPasswordHash = BitConverter.ToString(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(enteredPassword))).Replace("-", "").ToLower();
            return enteredPasswordHash == _user.m_password;
        }

    }
}
