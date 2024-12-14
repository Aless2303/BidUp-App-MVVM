using System.Windows.Controls;
using BidUp_App.ViewModels;

namespace BidUp_App.Views.Bidder
{
    public partial class LastBids : UserControl
    {
        public LastBids(int bidderId)
        {
            InitializeComponent();
            DataContext = new LastBidsViewModel(bidderId);
        }
    }
}
