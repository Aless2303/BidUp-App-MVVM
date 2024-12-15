using System.Windows.Controls;
using BidUp_App.ViewModels;

namespace BidUp_App.Views.Seller
{
    public partial class ViewAuctionsControl : UserControl
    {
        public ViewAuctionsControl(int sellerId)
        {
            InitializeComponent();
            DataContext = new ViewAuctionsSellerViewModel(sellerId);
        }
    }
}
