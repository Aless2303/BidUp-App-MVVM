using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Threading;
using BidUp_App.Models.Users;
using BidUp_App.Views.Bidder;
using BidUp_App.Views.Seller;

namespace BidUp_App.ViewModels
{
    public class SellerDashboardViewModel : BaseViewModel
    {
        private readonly BidUpEntities _dbContext;
        private readonly Seller _seller;
        private readonly DispatcherTimer _walletUpdateTimer;

        private string _walletBalance;
        private object _currentView;

        public SellerDashboardViewModel() { }

        public SellerDashboardViewModel(Seller seller)
        {
            _dbContext = new BidUpEntities();
            _seller = seller;

            WalletBalance = GetWalletBalance();

            // Initialize commands
            ProfileCommand = new RelayCommand(LoadProfileView);
            AddAuctionCommand = new RelayCommand(LoadAddAuctionView);
            ViewAuctionsCommand = new RelayCommand(LoadViewAuctionsView);

            // Load wallet balance
            LoadWalletBalance();

            // Set default view
            LoadProfileView();

            // Start timer for wallet balance update
            _walletUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _walletUpdateTimer.Tick += WalletUpdateTimer_Tick;
            _walletUpdateTimer.Start();
        }

        private void WalletUpdateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                var wallet = _dbContext.Wallets.FirstOrDefault(w => w.UserID == _seller.m_userID);

                if (wallet != null)
                {
                    // Reîncarcă entitatea din baza de date
                    _dbContext.Entry(wallet).Reload();

                    if (decimal.TryParse(WalletBalance, System.Globalization.NumberStyles.Currency,
                        System.Globalization.CultureInfo.CurrentCulture, out var currentDisplayedBalance) &&
                        wallet.Balance != currentDisplayedBalance)
                    {
                        WalletBalance = $"{wallet.Balance:C}";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating wallet balance: {ex.Message}");
            }
        }

        private void LoadWalletBalance()
        {
            try
            {
                var wallet = _dbContext.Wallets.FirstOrDefault(w => w.UserID == _seller.m_userID);
                WalletBalance = wallet != null ? $"{wallet.Balance:C}" : "$0.00";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading wallet balance: {ex.Message}");
                WalletBalance = "$0.00";
            }
        }

        public string WalletBalance
        {
            get => _walletBalance;
            set
            {
                _walletBalance = value;
                OnPropertyChanged(nameof(WalletBalance));
            }
        }

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public ICommand ProfileCommand { get; }
        public ICommand AddAuctionCommand { get; }
        public ICommand ViewAuctionsCommand { get; }

        private string GetWalletBalance()
        {
            var wallet = _dbContext.Wallets.FirstOrDefault(w => w.UserID == _seller.m_userID);
            return wallet != null ? $"{wallet.Balance:C}" : "$0.00";
        }

        private void LoadProfileView()
        {
            // Initialize ProfileViewModel and ProfileView
            var profileViewModel = new ProfileViewModel(_seller, _dbContext);
            var profileView = new ProfileView
            {
                DataContext = profileViewModel
            };

            CurrentView = profileView;
        }

        private void LoadAddAuctionView()
        {
            // Initialize AddAuctionViewModel with seller's ID
            var addAuctionViewModel = new AddAuctionViewModel(_seller.m_userID);

            // Initialize AddAuctionControl and set DataContext
            var addAuctionControl = new AddAuctionControl(_seller.m_userID)
            {
                DataContext = addAuctionViewModel
            };

            CurrentView = addAuctionControl;
        }

        private void LoadViewAuctionsView()
        {
            // Initialize ViewAuctionsControl for seller's auctions
            var viewAuctionsView = new BidUp_App.Views.Seller.ViewAuctionsControl(_seller.m_userID);
            CurrentView = viewAuctionsView;
        }
    }
}
