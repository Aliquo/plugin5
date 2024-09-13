using Aliquo.Windows;
using plugin5_demo.ViewModels;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace plugin5_demo.Views
{
    /// <summary>
    /// Lógica de interacción para SingleItemEditableView.xaml
    /// </summary>
    public partial class SingleItemEditableView : UserControl
    {
        private SingleItemEditableViewModel ViewModel { get; set; }
        public SingleItemEditableView(IHost host, bool isActive = true, string windowSourceId = null, bool originalView = true)
        {
            InitializeComponent();
            this.ViewModel = new SingleItemEditableViewModel(host, windowSourceId, originalView);

            this.DataContext = this.ViewModel;

            this.ViewModel.Window = this.ViewModel.Host.Management.Views.CreateWindowView("Articulo",
                                                                                              viewType: ViewType.Table, viewKey: this.ViewModel.Table, viewStyle: ViewStyle.Normal,
                                                                                              image: SharedResources.GetBitmapImage("notification_task.png"), isActive: isActive);

            this.ViewModel.Window.Content = this;

            this.ViewModel.Window.Ribbon = this.RibbonDetail;
            this.ViewModel.Window.StatusBar = this.StatusBarView;

        }
        /// <summary>
        /// Carga la vista en modo solo lectura
        /// </summary>        
        public void Load(int idProduct)
        {
            Task.Run(async () => await this.ViewModel.ShowAsync(idProduct));
        }
    }
}
