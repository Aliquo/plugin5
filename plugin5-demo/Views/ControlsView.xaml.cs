using Aliquo.Windows;
using System;
using System.Windows.Controls;

namespace plugin5_demo.Views
{
    /// <summary>Interaction logic for ControlsView.xaml</summary>
    public partial class ControlsView : UserControl
    {

        private const string Title = "Example of controls";

        IHost Host;

        public ControlsView(IHost host)
        {
            InitializeComponent();

            this.Host = host;

            System.Windows.Media.Imaging.BitmapImage Image = SharedResources.GetBitmapImage("action.png");
            IWindowView window = host.Management.Views.CreateWindowView(Title, Image);

            window.Ribbon = this.Ribbon;
            window.Content = this;
            window.StatusBar = this.StatusBar;
        }

        #region Show the grid with a list of customers

        private void ButtonCustomer_LinkButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            // Show the window with a list of customers
            IWindowGrid grid = this.Host.Management.Views.CreateWindowGrid("Clientes", fields: "Codigo, Nombre", windowOwner: this);
            grid.LinkField = "Codigo";
            grid.Show();

            grid.Selected += Grid_SelectedCustomer;
            grid.Closed += Grid_ClosedCustomer;

        }

        private void Grid_SelectedCustomer(object sender, EventArgs e)
        {
            IWindowGrid grid = (IWindowGrid)sender;

            System.Data.DataRow row = grid.GetSelectedRow();

            ButtonCustomer.Text = row["Clientes.Codigo"].ToString();
            TextBoxCustomerName.Text = row["Clientes.Nombre"].ToString();

            grid.Close();
        }

        private void Grid_ClosedCustomer(object sender, EventArgs e)
        {
            IWindowGrid grid = (IWindowGrid)sender;

            grid.Selected -= Grid_SelectedCustomer;
            grid.Closed -= Grid_ClosedCustomer;
        }

        #endregion

    }
}
