using BidUp_App.ViewModels;
using System.Windows.Controls;

namespace BidUp_App.Views.Bidder
{
    /// <summary>
    /// Interaction logic for CompletedAuctionsView.xaml
    /// </summary>
    public partial class CompletedAuctionsView : UserControl
    {
        public CompletedAuctionsView()
        {
            InitializeComponent();
        }

        public CompletedAuctionsView(object dataContext) : this()
        {
            DataContext = dataContext;
        }


        public CompletedAuctionsView(CompletedAuctionsViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

    }
}
