using Aliquo.Windows;
using Aliquo.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace plugin5_demo.Views
{
    /// <summary>
    /// Interaction logic for ListQuery.xaml
    /// </summary>
    public partial class ListQuery : UserControl
    {

        private const string Title = "Example list from query";

        public ListQuery(IHost host)
        {
            InitializeComponent();

            // Creation of a new window
            System.Windows.Media.Imaging.BitmapImage Image = SharedResources.GetBitmapImage("window_grid.png");
            IWindowView window = host.Management.Views.CreateWindowView(Title, Image);

            // We indicate the table and the fields, as well as their definition
            datagrid.Host = host;
            datagrid.Tables = "Facturas_Clientes AS Facturas INNER JOIN Clientes ON Facturas.CodCliente=Clientes.Codigo LEFT JOIN Proyectos ON Facturas.CodProyecto=Proyectos.Codigo";
            datagrid.Fields = new List<Aliquo.Windows.Controls.Models.FieldSetting>()
            {
                new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Facturas.Id", IsHidden = true, IsKey = true, NotSettable = true },
                new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Facturas.Abono", Text = "Return" },

                // Also you can use the Aliquo Resources, for example, these
                new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Facturas.CodSerie", Text = Aliquo.Windows.Properties.Resources.Invoice_Serial },
                new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Facturas.Numero", Text = Aliquo.Windows.Properties.Resources.Invoice_Number, Format = Aliquo.Core.Formats.NumberPattern(0, false) },
                new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Facturas.Fecha", Text = Aliquo.Windows.Properties.Resources.Invoice_Date, Format = Aliquo.Core.Formats.DatePattern(), SortDirection = FieldSortDirection.Descending },

                new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Facturas.ImporteNETO", Text = "Amount", Format = Aliquo.Core.Formats.NumberPattern(), Summary = FieldSummaryType.Sum },
                new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Facturas.CodCliente", Text = "Customer code" },
                new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Clientes.Nombre", Text = "Customer name" },
                new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Clientes.Poblacion", Text = "Customer city", FilterElements = new ObservableCollection<Aliquo.Windows.Controls.Models.FilterElement>() },
                new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Proyectos.Nombre", Text = "Project name" },
                new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Facturas.FechaCreacion", Text = "Creation date", Format = Aliquo.Core.Formats.DatePattern() },
                new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Facturas.FechaCreacion", Alias = "Facturas:HoraCreacion", Text = "Creation time", Format = Aliquo.Core.Formats.TimePattern() },
                new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Facturas.ImporteDtoHabitual", Alias = "Facturas:DtoHabitual", Text = "Usual discount", Caption = "Discounts" },
                new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Facturas.ImporteDtoPPago", Alias = "Facturas:DtoPPago", Text = "Discount soon pay", Caption = "Discounts" },
                new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Facturas.ImporteDtoEspecial", Alias = "Facturas:DtoEspecial", Text = "Special discount", Caption = "Discounts" },
                new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "(Facturas.ImporteDtoHabitual + Facturas.ImporteDtoPPago + Facturas.ImporteDtoEspecial)", Alias = "Facturas:Dtos", Text = "Amount discounts", Caption = "Discounts" }
            };

            window.Ribbon = this.Ribbon;
            window.Content = this;
            window.StatusBar = this.StatusBar;
        }

        private void Refresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            datagrid.Load();
        }

        private async void RefreshAsync_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            await datagrid.LoadAsync();
        }
    }
}
