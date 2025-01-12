using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BidUp_App.Models;
using System.Data.Entity;
using BidUp_App.Models.Users;

namespace BidUp_App.ViewModels
{
    public class ManageAuctionsViewModel : BaseViewModel
    {
        private readonly BidUpEntities _dbContext;

        public ObservableCollection<AuctionViewModel> Auctions { get; set; }
        public ICommand CloseAuctionCommand { get; }
        public ICommand RefuseAuctionCommand { get; }
        public ICommand AcceptAuctionCommand { get; }

        public ManageAuctionsViewModel()
        {
            _dbContext = new BidUpEntities();
            Auctions = new ObservableCollection<AuctionViewModel>();

            CloseAuctionCommand = new RelayCommand<int>(CloseAuction);
            RefuseAuctionCommand = new RelayCommand<int>(RefuseAuction);
            AcceptAuctionCommand = new RelayCommand<int>(AcceptAuction);

            LoadAuctions();
        }

        private void LoadAuctions()
        {
            Auctions.Clear();

            var auctionsFromDb = _dbContext.Auctions
                .Include("User")    // Seller
                .Include("User1")   // CurrentBidder
                .ToList();

            foreach (var auction in auctionsFromDb)
            {
                Auctions.Add(new AuctionViewModel
                {
                    AuctionID = auction.AuctionID,
                    ProductName = auction.ProductName,
                    Description = auction.Description,
                    ProductImagePath = auction.ProductImagePath,
                    StartingPrice = auction.StartingPrice,
                    CurrentPrice = auction.CurrentPrice,
                    StartTime = auction.StartTime,
                    EndTime = auction.EndTime,
                    IsClosed = auction.IsClosed == true || auction.EndTime <= DateTime.Now,
                    AuctionStatus = auction.AuctionStatus,
                    SellerName = auction.User1?.FullName ?? "Unknown",
                    LastBidderName = auction.User?.FullName ?? "None",
                    ShowPendingActions = auction.AuctionStatus == "Pending",
                    ShowCloseButton = auction.AuctionStatus == "Accepted" && !(auction.IsClosed ?? false),
                    StatusDisplay = auction.AuctionStatus == "Refused" ? "Refused" :
                                    (auction.IsClosed ?? false) ? "Closed" :
                                    auction.AuctionStatus == "Accepted" ? "Open" : "Pending"
                });
            }
        }


        private void CloseAuction(int auctionId)
        {
            var auction = _dbContext.Auctions.FirstOrDefault(a => a.AuctionID == auctionId);
            if (auction != null)
            {
                auction.IsClosed = true;
                _dbContext.SaveChanges();
                LoadAuctions();
            }
        }

        private void RefuseAuction(int auctionId)
        {
            var auction = _dbContext.Auctions.FirstOrDefault(a => a.AuctionID == auctionId);
            if (auction != null && auction.AuctionStatus== "Pending")
            {
                auction.AuctionStatus = "Refused";
                _dbContext.SaveChanges();
                LoadAuctions();
            }
        }

        private void AcceptAuction(int auctionId)
        {
            var auction = _dbContext.Auctions.FirstOrDefault(a => a.AuctionID == auctionId);
            if (auction != null && auction.AuctionStatus == "Pending")
            {
                auction.AuctionStatus = "Accepted";
                _dbContext.SaveChanges();
                LoadAuctions();
            }
        }
    }

}
