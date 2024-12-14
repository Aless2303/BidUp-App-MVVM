using System.Windows;
using System.Windows.Controls;

namespace BidUp_App.UserControls
{
    public partial class CustomTextBox : UserControl
    {
        // Definește DependencyProperty pentru `Text`
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(CustomTextBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomTextBox customTextBox)
            {
                customTextBox.InputBox.Text = (string)e.NewValue;
            }
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string), typeof(CustomTextBox), new PropertyMetadata(string.Empty));

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public CustomTextBox()
        {
            InitializeComponent();
            InputBox.TextChanged += InputBox_TextChanged;
        }

        private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Actualizează valoarea `Text` în DependencyProperty
            Text = InputBox.Text;

            // Afișează sau ascunde Placeholder-ul
            PlaceholderText.Visibility = string.IsNullOrEmpty(InputBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
