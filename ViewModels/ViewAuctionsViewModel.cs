using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using BidUp_App.Models;
using BidUp_App.Models.Users;
using BidUp_App.Views.Bidder;

namespace BidUp_App.ViewModels
{
    public class ViewAuctionsViewModel : BaseViewModel
    {
        private readonly DataContextDataContext _dbContext;
        private readonly int _currentBidderId;
        private readonly DispatcherTimer _timer;
        private readonly DispatcherTimer _notificationTimer;

        public ObservableCollection<AuctionViewModel> Auctions { get; set; }

        public ICommand RefreshCommand { get; }
        public ICommand BidCommand { get; }

        public ViewAuctionsViewModel()
        {
            // Constructor pentru design-time (sau când nu ai currentBidderId)
            _dbContext = new DataContextDataContext();
            _currentBidderId = -1; // Sau un ID implicit
            Auctions = new ObservableCollection<AuctionViewModel>();
        }

        public ViewAuctionsViewModel(int currentBidderId)
        {
            _dbContext = new DataContextDataContext();
            _currentBidderId = currentBidderId;

            Auctions = new ObservableCollection<AuctionViewModel>();

            RefreshCommand = new RelayCommand(RefreshAuctions);
            BidCommand = new RelayCommand<int>(BidOnAuction);




            LoadAuctions();
            CheckNotifications();

            // Timer for updating auction time remaining
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();

            // Timer for checking notifications
            _notificationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3) // Check notifications every 3 seconds
            };
            _notificationTimer.Tick += NotificationTimer_Tick;
            _notificationTimer.Start();
        }

        private void LoadAuctions()
        {
            _dbContext.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, _dbContext.Auctions);

            var auctions = _dbContext.Auctions
                .Where(a => a.EndTime > DateTime.Now)
                .Select(a => new AuctionViewModel
                {
                    AuctionID = a.AuctionID,
                    ProductName = a.ProductName,
                    Description = a.Description,
                    StartingPrice = a.StartingPrice,
                    CurrentPrice = a.CurrentPrice,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    ProductImagePath = a.ProductImagePath,
                    RemainingTime = a.StartTime > DateTime.Now
                        ? $"Start in: {GetRemainingTime(a.StartTime)}"
                        : $"Time Left: {GetRemainingTime(a.EndTime)}"
                }).ToList();

            Auctions.Clear();
            foreach (var auction in auctions)
            {
                Auctions.Add(auction);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Creează o listă temporară pentru a actualiza timpul rămas
            var updatedAuctions = Auctions.Select(auction =>
            {
                if (auction.StartTime > DateTime.Now)
                {
                    auction.RemainingTime = $"Start in: {GetRemainingTime(auction.StartTime)}";
                }
                else if (auction.EndTime > DateTime.Now)
                {
                    auction.RemainingTime = $"Time Left: {GetRemainingTime(auction.EndTime)}";
                }
                else
                {
                    auction.RemainingTime = "Expired";
                }
                return auction;
            }).ToList();

            // Actualizează colecția Auctions și notifică UI-ul
            Auctions.Clear();
            foreach (var auction in updatedAuctions)
            {
                Auctions.Add(auction);
            }

            // Notifică UI-ul că s-au făcut modificări
            OnPropertyChanged(nameof(Auctions));
        }



        private string GetRemainingTime(DateTime time)
        {
            var remainingTime = time - DateTime.Now;

            if (remainingTime.TotalSeconds <= 0)
            {
                return "Expired";
            }

            return $"{remainingTime.Days}d {remainingTime.Hours}h {remainingTime.Minutes}m {remainingTime.Seconds}s";
        }


        private void BidOnAuction(int auctionId)
        {
            var selectedAuction = _dbContext.Auctions.FirstOrDefault(a => a.AuctionID == auctionId);

            if (selectedAuction != null)
            {
                if (selectedAuction.StartTime > DateTime.Now)
                {
                    System.Windows.MessageBox.Show("This auction has not started yet. Please check back later.", "Auction Not Started", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    return;
                }

                // Open the BidWindow (modal)
                var bidWindow = new BidWindow(selectedAuction, _currentBidderId);
                if (bidWindow.ShowDialog() == true)
                {
                    LoadAuctions();
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Auction not found.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void NotificationTimer_Tick(object sender, EventArgs e)
        {
            CheckNotifications();
        }

        private void CheckNotifications()
        {
            var notifications = _dbContext.Notifications
                .Where(n => n.BidderID == _currentBidderId && !n.IsRead)
                .ToList();

            foreach (var notification in notifications)
            {
                System.Windows.MessageBox.Show(notification.Message, "New Notification", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                notification.IsRead = true;
            }

            if (notifications.Any())
            {
                _dbContext.SubmitChanges();
            }
        }

        private void RefreshAuctions()
        {
            LoadAuctions();
        }
    }
}
