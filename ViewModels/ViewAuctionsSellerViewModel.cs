using System.Collections.ObjectModel;
using System.Linq;
using BidUp_App.Models;

namespace BidUp_App.ViewModels
{
    public class ViewAuctionsSellerViewModel : BaseViewModel
    {
        private readonly BidUpEntities _dbContext;

        public ObservableCollection<AuctionItemViewModel> Auctions { get; set; }

        public ViewAuctionsSellerViewModel(int sellerId)
        {
            _dbContext = new BidUpEntities();
            Auctions = new ObservableCollection<AuctionItemViewModel>();

            LoadAuctions(sellerId);
        }

        private void LoadAuctions(int sellerId)
        {
            var auctions = _dbContext.Auctions
                .Where(a => a.SellerID == sellerId)
                .AsEnumerable() // Adu datele în memorie înainte de formatări
                .Select(a => new AuctionItemViewModel
                {
                    AuctionID = a.AuctionID,
                    ProductName = a.Product.ProductName,
                    CurrentPrice = a.CurrentPrice,
                    StartTime = a.StartTime.ToString("g"), // Format: general short date/time
                    EndTime = a.EndTime.ToString("g")
                }).ToList();

            Auctions.Clear();
            foreach (var auction in auctions)
            {
                Auctions.Add(auction);
            }
        }

    }

    public class AuctionItemViewModel
    {
        public int AuctionID { get; set; }
        public string ProductName { get; set; }
        public double CurrentPrice { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
