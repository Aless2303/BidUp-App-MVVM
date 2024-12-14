using System.Windows;
using BidUp_App.ViewModels;

namespace BidUp_App.Views.Bidder
{
    public partial class BidWindow : Window
    {
        public BidWindow(Auction selectedAuction, int currentBidderId)
        {
            InitializeComponent();
            var viewModel = new BidWindowViewModel(selectedAuction, currentBidderId);
            viewModel.CloseAction = this.Close;
            DataContext = viewModel;
        }
    }
}
