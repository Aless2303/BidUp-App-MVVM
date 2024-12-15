using System.Windows.Controls;
using BidUp_App.ViewModels;

namespace BidUp_App.Views.Seller
{
    public partial class AddAuctionControl : UserControl
    {
        public AddAuctionControl(int sellerId)
        {
            InitializeComponent();
            DataContext = new AddAuctionViewModel(sellerId);
        }
    }
}
