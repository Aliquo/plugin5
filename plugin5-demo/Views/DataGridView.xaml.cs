using Aliquo.Windows;
using plugin5_demo.ViewModels;
using System.Windows.Controls;

namespace plugin5_demo.Views
{
    /// <summary>
    /// Lógica de interacción para DataGrid.xaml
    /// </summary>
    public partial class DataGridView : UserControl
    {
        public DataGridView(IHost host, string codCountry = null)
        {
            this.DataContext = new DataGridViewModel(host, codCountry);
            InitializeComponent();

            var window = ((DataGridViewModel)this.DataContext).Window;
            window.Ribbon = this.Ribbon;
            window.Content = this;
            window.StatusBar = this.StatusBar;
        }
    }
}
