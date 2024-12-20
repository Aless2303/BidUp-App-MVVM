using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using BidUp_App.Models;

namespace BidUp_App.ViewModels
{
    public class AddAuctionViewModel : BaseViewModel
    {
        private readonly BidUpEntities _dbContext;
        private readonly int _sellerId;

        public AddAuctionViewModel() { }

        public AddAuctionViewModel(int sellerId)
        {
            _dbContext = new BidUpEntities();
            _sellerId = sellerId;

            UploadImageCommand = new RelayCommand(UploadImage);
            AddAuctionCommand = new RelayCommand(AddAuction);
            CancelCommand = new RelayCommand(Cancel);
        }

        // Properties
        private string _productName;
        public string ProductName
        {
            get => _productName;
            set => SetProperty(ref _productName, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private string _startingPrice;
        public string StartingPrice
        {
            get => _startingPrice;
            set => SetProperty(ref _startingPrice, value);
        }

        private DateTime? _startTime;
        public DateTime? StartTime
        {
            get => _startTime;
            set => SetProperty(ref _startTime, value);
        }

        private DateTime? _endTime;
        public DateTime? EndTime
        {
            get => _endTime;
            set => SetProperty(ref _endTime, value);
        }

        private string _productImagePath;
        public string ProductImagePath
        {
            get => _productImagePath;
            set => SetProperty(ref _productImagePath, value);
        }

        // Commands
        public ICommand UploadImageCommand { get; }
        public ICommand AddAuctionCommand { get; }
        public ICommand CancelCommand { get; }

        private void UploadImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ProductImagePath = openFileDialog.FileName;
            }
        }

        private void AddAuction()
        {
            try
            {
                // Validate fields
                if (string.IsNullOrEmpty(ProductName) || string.IsNullOrEmpty(StartingPrice) ||
                    !StartTime.HasValue || !EndTime.HasValue)
                {
                    MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!double.TryParse(StartingPrice, out double startingPrice))
                {
                    MessageBox.Show("Invalid starting price.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (EndTime <= StartTime)
                {
                    MessageBox.Show("End Time must be after Start Time.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Create product and auction
                var newProduct = new Product
                {
                    ProductName = ProductName,
                    Description = Description,
                    ProductImagePath = ProductImagePath,
                    Category = "Default",
                    CreationDate = DateTime.Now,
                    SellerID = _sellerId
                };

                _dbContext.Products.Add(newProduct);
                _dbContext.SaveChanges();

                var newAuction = new Auction
                {
                    ProductID = newProduct.ProductID,
                    ProductName = ProductName,
                    ProductImagePath = ProductImagePath,
                    StartingPrice = startingPrice,
                    CurrentPrice = startingPrice,
                    SellerID = _sellerId,
                    StartTime = StartTime.Value,
                    EndTime = EndTime.Value,
                    IsClosed = false,
                    AuctionStatus = "Pending"
                };

                _dbContext.Auctions.Add(newAuction);
                _dbContext.SaveChanges();

                MessageBox.Show("Auction added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            MessageBox.Show("Auction creation canceled.", "Canceled", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
