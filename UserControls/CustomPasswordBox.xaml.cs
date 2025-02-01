using System.Windows;
using System.Windows.Controls;

namespace BidUp_App.UserControls
{
    public partial class CustomPasswordBox : UserControl
    {
        // Definește DependencyProperty pentru `Password`
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                nameof(Password),
                typeof(string),
                typeof(CustomPasswordBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordChanged));

        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        private static void OnPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomPasswordBox customPasswordBox)
            {
                customPasswordBox.PasswordInputBox.Password = (string)e.NewValue;
            }
        }

        // Definește DependencyProperty pentru `Placeholder`
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string), typeof(CustomPasswordBox), new PropertyMetadata(string.Empty));

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public CustomPasswordBox()
        {
            InitializeComponent();
        }

        private void PasswordInputBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Actualizează valoarea `Password` în DependencyProperty
            Password = PasswordInputBox.Password;

            // Afișează sau ascunde Placeholder-ul
            PlaceholderText.Visibility = string.IsNullOrEmpty(PasswordInputBox.Password) ? Visibility.Visible : Visibility.Collapsed;

            // Forțează actualizarea cursorului
            PasswordInputBox.GetType()
                           .GetMethod("Select", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                           ?.Invoke(PasswordInputBox, new object[] { PasswordInputBox.Password.Length, 0 });
        }
    }
}