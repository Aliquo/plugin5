using Aliquo.Windows;
using System.Windows.Controls;

namespace plugin5_demo.Views
{
    /// <summary>Interaction logic for EventsConsole.xaml</summary>
    public partial class EventsConsoleView : UserControl
    {
        public EventsConsoleView(IHost host)
        {
            InitializeComponent();

            var viewModel = new ViewModels.EventsConsoleViewModel(host);
            this.DataContext = viewModel;
            viewModel.Window.Content = this;
        }            

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            txt.ScrollToEnd();
        }
    }
}
