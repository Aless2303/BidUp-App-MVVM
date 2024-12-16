using BidUp_App.Models;
using BidUp_App.Models.Users;
using BidUp_App.Views.Bidder;
using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace BidUp_App.ViewModels
{
    public class BidderDashboardViewModel : BaseViewModel
    {
        private readonly Bidder _bidder;
        private readonly BidUpEntities _dbContext;
        private readonly DispatcherTimer _walletUpdateTimer;

        private string _walletBalance;
        private object _currentView;

        public BidderDashboardViewModel()
        {
            // Constructor pentru design-time
        }

        public BidderDashboardViewModel(Bidder bidder)
        {
            _bidder = bidder;
            _dbContext = new BidUpEntities();

            // Inițializăm comenzile
            ProfileCommand = new RelayCommand(LoadProfileView);
            NewAuctionsCommand = new RelayCommand(LoadNewAuctionsView);
            LastBidsCommand = new RelayCommand(LoadLastBidsView);

            // Inițializăm și încărcăm soldul inițial
            LoadWalletBalance();

            // Setăm view-ul implicit
            LoadProfileView();

            // Inițializăm timer-ul pentru actualizarea soldului
            _walletUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _walletUpdateTimer.Tick += WalletUpdateTimer_Tick;
            _walletUpdateTimer.Start();
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

        // Comenzi pentru navigare
        public ICommand ProfileCommand { get; }
        public ICommand NewAuctionsCommand { get; }
        public ICommand LastBidsCommand { get; }

        private void LoadWalletBalance()
        {
            try
            {
                var wallet = _dbContext.Wallets.FirstOrDefault(w => w.UserID == _bidder.m_userID);
                WalletBalance = wallet != null ? $"{wallet.Balance:C}" : "$0.00";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading wallet balance: {ex.Message}");
                WalletBalance = "$0.00";
            }
        }

        private void WalletUpdateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                var wallet = _dbContext.Wallets.FirstOrDefault(w => w.UserID == _bidder.m_userID);

                if (wallet != null && decimal.TryParse(WalletBalance, System.Globalization.NumberStyles.Currency,
                    System.Globalization.CultureInfo.CurrentCulture, out var currentDisplayedBalance) &&
                    wallet.Balance != currentDisplayedBalance)
                {
                    WalletBalance = $"{wallet.Balance:C}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating wallet balance: {ex.Message}");
            }
        }

        private void LoadProfileView()
        {
            // Creează ProfileViewModel și setează-l ca DataContext pentru ProfileView
            var profileViewModel = new ProfileViewModel(_bidder, _dbContext);
            var profileView = new ProfileView
            {
                DataContext = profileViewModel
            };

            // Atribuie ProfileView ca View curent
            CurrentView = profileView;
        }

        private void LoadNewAuctionsView()
        {
            // Creează ViewModel-ul pentru licitații noi
            var auctionsViewModel = new ViewAuctionsViewModel(_bidder.m_userID);

            // Creează View-ul și setează DataContext-ul
            var auctionsView = new ViewAuctionsControl(_bidder.m_userID);

            // Setează View-ul curent
            CurrentView = auctionsView;
        }

        private void LoadLastBidsView()
        {
            // Creează ViewModel-ul pentru ultimele licitații
            var lastBidsViewModel = new LastBidsViewModel(_bidder.m_userID);

            // Creează View-ul și setează DataContext-ul
            var lastBidsView = new LastBids(_bidder.m_userID)
            {
                DataContext = lastBidsViewModel
            };

            // Setează View-ul curent
            CurrentView = lastBidsView;
        }
    }
}
