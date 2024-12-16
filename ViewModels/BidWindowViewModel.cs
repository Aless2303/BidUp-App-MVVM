using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BidUp_App.Models;

namespace BidUp_App.ViewModels
{
    public class BidWindowViewModel : BaseViewModel
    {
        private readonly Auction _selectedAuction;
        private readonly int _currentBidderId;
        private readonly BidUpEntities _dbContext;
        private decimal _bidAmount;


        public BidWindowViewModel()
        {

        }

        public BidWindowViewModel(Auction selectedAuction, int currentBidderId)
        {
            _selectedAuction = selectedAuction;
            _currentBidderId = currentBidderId;
            _dbContext = new BidUpEntities();

            PlaceBidCommand = new RelayCommand(PlaceBid);
            CancelCommand = new RelayCommand(Cancel);
        }

        public decimal BidAmount
        {
            get => _bidAmount;
            set
            {
                _bidAmount = value;
                OnPropertyChanged(nameof(BidAmount));
            }
        }

        public ICommand PlaceBidCommand { get; }
        public ICommand CancelCommand { get; }

        private void PlaceBid()
        {
            try
            {
                if (BidAmount <= 0)
                {
                    MessageBox.Show("Please enter a valid bid amount.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var auction = _dbContext.Auctions.FirstOrDefault(a => a.AuctionID == _selectedAuction.AuctionID);
                if (auction == null)
                {
                    MessageBox.Show("The selected auction does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (BidAmount <= (decimal)auction.CurrentPrice)
                {
                    MessageBox.Show("The bid amount must be higher than the current price.", "Invalid Bid", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var currentBidderWallet = _dbContext.Wallets.FirstOrDefault(w => w.UserID == _currentBidderId);
                if (currentBidderWallet == null || currentBidderWallet.Balance < BidAmount)
                {
                    MessageBox.Show("You do not have enough funds in your wallet to place this bid.", "Insufficient Funds", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (auction.CurrentBidderID != null)
                {
                    int previousBidderId = auction.CurrentBidderID.Value;
                    var previousBidderWallet = _dbContext.Wallets.FirstOrDefault(w => w.UserID == previousBidderId);
                    if (previousBidderWallet != null)
                    {
                        previousBidderWallet.Balance += (decimal)auction.CurrentPrice;

                        var notification = new Notification
                        {
                            BidderID = previousBidderId,
                            AuctionID = auction.AuctionID,
                            Message = $"Your previous bid of {auction.CurrentPrice:C} was outbid for {auction.ProductName}.",
                            CreatedAt = DateTime.Now,
                            IsRead = false
                        };
                        _dbContext.Notifications.Add(notification);
                    }
                }

                currentBidderWallet.Balance -= BidAmount;

                auction.CurrentPrice = (double)BidAmount;
                auction.CurrentBidderID = _currentBidderId;

                var bidderName = _dbContext.Users.FirstOrDefault(u => u.UserID == _currentBidderId)?.FullName;
                var message = $"Bidder {bidderName} placed a bid of {BidAmount:C} on {auction.ProductName}.";

                var log = new Log
                {
                    Timestamp = DateTime.Now,
                    EventType = "Bid",
                    Message = message,
                    DynamicData = Newtonsoft.Json.JsonConvert.SerializeObject(new
                    {
                        BidderID = _currentBidderId,
                        ProductID = auction.ProductID,
                        BidAmount
                    })
                };

                _dbContext.Logs.Add(log);
                _dbContext.SaveChanges();

                MessageBox.Show("Your bid has been placed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            CloseAction?.Invoke();
        }

        public Action CloseAction { get; set; }
    }
}
