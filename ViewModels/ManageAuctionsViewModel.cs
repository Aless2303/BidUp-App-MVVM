using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BidUp_App.Models;
using BidUp_App.Models.Users;

namespace BidUp_App.ViewModels
{
    public class ManageAuctionsViewModel : BaseViewModel
    {
        private readonly DataContextDataContext _dbContext;

        public ObservableCollection<AuctionViewModel> Auctions { get; set; }
        public ICommand CloseAuctionCommand { get; }

        public ManageAuctionsViewModel()
        {
            _dbContext = new DataContextDataContext();
            Auctions = new ObservableCollection<AuctionViewModel>();

            CloseAuctionCommand = new RelayCommand<int>(CloseAuction);
            LoadAuctions();
        }

        private void LoadAuctions()
        {
            Auctions.Clear();

            var auctions = _dbContext.Auctions
                .ToList() // Preluăm datele pentru a procesa local
                .Select(auction => new AuctionViewModel
                {
                    AuctionID = auction.AuctionID,
                    ProductName = auction.ProductName,
                    Description = auction.Description,
                    ProductImagePath = auction.ProductImagePath,
                    StartingPrice = auction.StartingPrice,
                    CurrentPrice = auction.CurrentPrice,
                    StartTime = auction.StartTime,
                    EndTime = auction.EndTime,
                    IsClosed = auction.IsClosed == true || auction.EndTime <= DateTime.Now, // Verificăm timpul
                    SellerName = _dbContext.Users
                        .Where(user => user.UserID == auction.SellerID)
                        .Select(user => user.FullName)
                        .FirstOrDefault(),
                    LastBidderName = auction.CurrentBidderID != null
                        ? _dbContext.Users
                            .Where(user => user.UserID == auction.CurrentBidderID)
                            .Select(user => user.FullName)
                            .FirstOrDefault()
                        : "None",
                    CloseButtonVisibility = (auction.IsClosed == true || auction.EndTime <= DateTime.Now)
                        ? Visibility.Collapsed
                        : Visibility.Visible // Ascundem butonul dacă licitația este închisă
                })
                .ToList();

            foreach (var auction in auctions)
            {
                Auctions.Add(auction);
            }
        }

        private void CloseAuction(int auctionId)
        {
            var auction = _dbContext.Auctions.FirstOrDefault(a => a.AuctionID == auctionId);

            if (auction != null && (auction.IsClosed == false || auction.IsClosed == null))
            {
                auction.IsClosed = true;
                _dbContext.SubmitChanges();

                MessageBox.Show($"Auction '{auction.ProductName}' has been successfully closed!",
                                "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadAuctions();
            }
            else
            {
                MessageBox.Show("Auction not found or already closed.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
