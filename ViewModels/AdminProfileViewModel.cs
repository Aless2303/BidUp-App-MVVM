using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BidUp_App.ViewModels
{
    public class AdminProfileViewModel : BaseViewModel
    {
        private readonly BidUp_App.Models.Users.User _user;

        public AdminProfileViewModel(BidUp_App.Models.Users.User user)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));
            LoadProfile();
        }

        // Proprietăți pentru Binding
        public string FullName => _user.m_fullName;
        public string Email => _user.m_email;
        public string DateOfBirth => _user.m_BirthDate.ToString("d");
        public string Role => _user.m_role;

        private BitmapImage _profileImage;
        public BitmapImage ProfileImage
        {
            get => _profileImage;
            set
            {
                _profileImage = value;
                OnPropertyChanged(nameof(ProfileImage));
            }
        }

        private void LoadProfile()
        {
            if (!string.IsNullOrEmpty(_user.ProfilePicturePath) && System.IO.File.Exists(_user.ProfilePicturePath))
            {
                ProfileImage = new BitmapImage(new Uri(_user.ProfilePicturePath, UriKind.Absolute));
            }
        }
    }
}