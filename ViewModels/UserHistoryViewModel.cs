using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BidUp_App.Models.Loguri;
using BidUp_App.Views.Admin;

namespace BidUp_App.ViewModels
{
    public class UserHistoryViewModel : BaseViewModel
    {
        private readonly BidUp_App.Models.Users.User _user;
        private readonly BidUpEntities _dbContext;

        // Proprietate pentru DataGrid
        public ObservableCollection<Logs> Logs { get; set; }

        // Comanda pentru butonul Back
        public ICommand BackCommand { get; }

        public UserHistoryViewModel(BidUp_App.Models.Users.User user)
        {
            _user = user;
            _dbContext = new BidUpEntities();
            Logs = new ObservableCollection<Logs>();

            // Inițializare comandă Back
            BackCommand = new RelayCommand(GoBack);

            // Încărcăm log-urile
            LoadUserHistory();
        }

        private void LoadUserHistory()
        {
            // Preluăm toate log-urile din baza de date și aducem datele în memorie
            var allLogs = _dbContext.Logs
                .AsNoTracking() // Optimizăm performanța dacă nu actualizăm datele
                .ToList();

            // Aplicăm filtrul în memorie folosind LINQ to Objects
            var filteredLogs = allLogs
                .Where(log => log.DynamicData.Contains($"\"BidderID\":{_user.m_userID}")
                           || log.DynamicData.Contains($"\"SellerID\":{_user.m_userID}"))
                .Select(log => new BidUp_App.Models.Loguri.Logs
                {
                    LogID = log.LogID,
                    Timestamp = (System.DateTime)log.Timestamp,
                    EventType = log.EventType,
                    Message = log.Message,
                    DynamicData = log.DynamicData
                }).ToList();

            // Populăm colecția ObservableCollection
            Logs.Clear();
            foreach (var log in filteredLogs)
            {
                Logs.Add(log);
            }
        }



        private void GoBack()
        {
            // Navighează înapoi la UserDetailsView
            var parentWindow = Application.Current.Windows.OfType<AdminDashboard>().FirstOrDefault();

            if (parentWindow?.DataContext is AdminDashboardViewModel adminViewModel)
                    adminViewModel.CurrentView = new UserDetailsView(_user);
        }
    }
}
