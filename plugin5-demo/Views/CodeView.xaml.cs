using Aliquo.Windows;
using System.Windows.Controls;

namespace plugin5_demo.Views
{
    /// <summary>Interaction logic for CodeView.xaml</summary>
    public partial class CodeView : UserControl
    {

        private const string Title = "Code examples";

        IHost Host;

        public CodeView(IHost host)
        {

            InitializeComponent();

            this.Host = host;

            System.Windows.Media.Imaging.BitmapImage Image = SharedResources.GetBitmapImage("action.png");
            IWindowView window = this.Host.Management.Views.CreateWindowView(Title, Image);

            window.Ribbon = this.Ribbon;
            window.Content = this;
            window.StatusBar = this.StatusBar;

        }

        private void Controls_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Views.ControlsView examples = new Views.ControlsView(this.Host);
        }

        private void Images_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Views.ImagesView images = new Views.ImagesView(this.Host);
        }
    }
}
