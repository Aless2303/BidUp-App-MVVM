using System.Windows.Controls;
using BidUp_App.ViewModels;

namespace BidUp_App.Views.Bidder
{
    public partial class ViewAuctionsControl : UserControl
    {
        public ViewAuctionsControl(int currentBidderId)
        {
            InitializeComponent();

            // Setăm DataContext pentru a lega View-ul de ViewModel
            DataContext = new ViewAuctionsViewModel(currentBidderId);
        }
    }
}
