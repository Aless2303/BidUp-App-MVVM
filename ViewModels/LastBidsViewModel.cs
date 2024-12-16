using System;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using BidUp_App.Models;

namespace BidUp_App.ViewModels
{
    public class LastBidsViewModel : BaseViewModel
    {
        private readonly BidUpEntities _dbContext;
        private readonly int _bidderId;

        public ObservableCollection<dynamic> Bids { get; set; }


        public LastBidsViewModel() { }

        public LastBidsViewModel(int bidderId)
        {
            _bidderId = bidderId;
            _dbContext = new BidUpEntities();
            Bids = new ObservableCollection<dynamic>();
            LoadLastBids();
        }

        private void LoadLastBids()
        {
            try
            {
                // Obține toate log-urile cu EventType "Bid" și BidderID corespunzător
                var bids = _dbContext.Logs
                    .Where(log => log.EventType == "Bid" && log.DynamicData != null)
                    .AsEnumerable() // Adu datele în memorie pentru a permite operații dinamice
                    .Where(log =>
                    {
                        var dynamicData = JsonConvert.DeserializeObject<dynamic>(log.DynamicData);
                        return dynamicData.BidderID == _bidderId;
                    })
                    .Select(log =>
                    {
                        var dynamicData = JsonConvert.DeserializeObject<dynamic>(log.DynamicData);
                        int productId = (int)dynamicData.ProductID;
                        string productName = GetProductNameById(productId);
                        return new
                        {
                            ProductName = productName,
                            BidAmount = (decimal)dynamicData.BidAmount,
                            Timestamp = log.Timestamp
                        };
                    })
                    .ToList();

                // Actualizează colecția observabilă
                Bids.Clear();
                foreach (var bid in bids)
                {
                    Bids.Add(bid);
                }
            }
            catch (Exception ex)
            {
                // Gestionare eroare
                Console.WriteLine($"An error occurred while loading last bids: {ex.Message}");
            }
        }

        private string GetProductNameById(int productId)
        {
            // Găsește produsul după ProductID și returnează numele acestuia
            var product = _dbContext.Products.FirstOrDefault(p => p.ProductID == productId);
            return product?.ProductName ?? "Unknown Product";
        }
    }
}
