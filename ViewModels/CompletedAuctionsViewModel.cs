using System.Collections.ObjectModel;
using System.Linq;
using BidUp_App.Models;
using BidUp_App.Models.Users;

namespace BidUp_App.ViewModels
{
    public class CompletedAuctionsViewModel : BaseViewModel
    {
        private readonly DataContextDataContext _dbContext;
        private readonly int _currentBidderId;

        public ObservableCollection<AuctionViewModel> CompletedAuctions { get; set; }


        public CompletedAuctionsViewModel() { }
        public CompletedAuctionsViewModel(int currentBidderId)
        {
            _dbContext = new DataContextDataContext();
            _currentBidderId = currentBidderId;

            CompletedAuctions = new ObservableCollection<AuctionViewModel>();

            LoadCompletedAuctions();
        }

        private void LoadCompletedAuctions()
        {
            var completedAuctions = _dbContext.Auctions
                .Where(a => a.IsClosed == true && a.CurrentBidderID == _currentBidderId)
                .ToList();

            foreach (var auction in completedAuctions)
            {
                CompletedAuctions.Add(new AuctionViewModel
                {
                    AuctionID = auction.AuctionID,
                    ProductName = auction.ProductName,
                    Description = auction.Description,
                    ProductImagePath = auction.ProductImagePath,
                    StartingPrice = auction.StartingPrice,
                    CurrentPrice = auction.CurrentPrice
                });
            }
        }
    }
}
