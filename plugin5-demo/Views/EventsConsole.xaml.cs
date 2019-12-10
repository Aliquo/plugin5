using System.Windows.Controls;

namespace plugin5_demo.Views
{
    /// <summary>Interaction logic for EventsConsole.xaml</summary>
    public partial class EventsConsole : UserControl
    {
        public EventsConsole()
        {
            InitializeComponent();

            this.DataContext = new ViewModels.EventsConsoleViewModel();

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            TextBox txt = (TextBox)sender;
            txt.ScrollToEnd();

        }
    }
}
