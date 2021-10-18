using Aliquo.Windows;
using plugin5_demo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
