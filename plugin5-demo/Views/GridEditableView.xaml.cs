using Aliquo.Windows;
using plugin5_demo.ViewModels;
using System.Windows.Controls;

namespace plugin5_demo.Views
{
    /// <summary>
    /// Lógica de interacción para GridEditableView.xaml
    /// </summary>
    public partial class GridEditableView : UserControl
    {
        public GridEditableView(IHost host)
        {
            this.DataContext = new GridEditableViewModel(host);
            InitializeComponent();

            var window = ((GridEditableViewModel)this.DataContext).Window;
            window.Ribbon = this.Ribbon;
            window.Content = this;
            window.StatusBar = this.StatusBar;
        }
    }
}
