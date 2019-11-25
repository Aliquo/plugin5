using Aliquo.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace plugin5_demo.Views
{
    /// <summary>
    /// Interaction logic for GridView.xaml
    /// </summary>
    public partial class GridView : UserControl
    {

        private const string Title = "Example Grid";

        public GridView(IHost host)
        {
            InitializeComponent();

            System.Windows.Media.Imaging.BitmapImage Image = SharedResources.GetBitmapImage("window_grid.png");
            IWindowView window = host.Management.Views.CreateWindowView(Title, Image);

            window.Ribbon = this.Ribbon;
            window.Content = this;
            window.StatusBar = this.StatusBar;

            // Insert example data
            List<GridItem> listItemsGrid = new List<GridItem>
            {
                new GridItem { Selected = false, Name = "Element 1", Quantity = 2, Amount = Convert.ToDecimal(125.65), DateDelivery=Convert.ToDateTime("2019-01-25") },
                new GridItem { Selected = true, Name = "Element 2", Quantity = 12, Amount = Convert.ToDecimal(252.68), DateDelivery=Convert.ToDateTime("2019-02-05") },
                new GridItem { Selected = false, Name = "Element 3", Quantity = 3, Amount = Convert.ToDecimal(5682.56), DateDelivery=Convert.ToDateTime("2019-02-12") },
                new GridItem { Selected = true, Name = "Element 4", Quantity = 56, Amount = Convert.ToDecimal(65.32), DateDelivery=Convert.ToDateTime("2019-02-18") },
                new GridItem { Selected = false, Name = "Element 5", Quantity = 46, Amount = Convert.ToDecimal(52.65), DateDelivery=Convert.ToDateTime("2019-03-02") },
                new GridItem { Selected = true, Name = "Element 6", Quantity = 76, Amount = Convert.ToDecimal(135.24), DateDelivery=Convert.ToDateTime("2019-03-19") },
                new GridItem { Selected = false, Name = "Element 7", Quantity = 9, Amount = Convert.ToDecimal(9812.36), DateDelivery=Convert.ToDateTime("2019-03-31") },
                new GridItem { Selected = true, Name = "Element 8", Quantity = 98, Amount = Convert.ToDecimal(412.52), DateDelivery=Convert.ToDateTime("2019-04-01") },
                new GridItem { Selected = false, Name = "Element 9", Quantity =521, Amount = Convert.ToDecimal(5.32), DateDelivery=Convert.ToDateTime("2019-04-09") },
                new GridItem { Selected = true, Name = "Element 10", Quantity = 49, Amount = Convert.ToDecimal(214.05), DateDelivery=Convert.ToDateTime("2019-04-11") }
            };

            GridSyncfusion.ItemsSource = listItemsGrid;
        }

        private void Info_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            int rowsSelected = 0;

            foreach (GridItem row in (List<GridItem>)GridSyncfusion.ItemsSource)
            {
                if (row.Selected)
                    ++rowsSelected;
            }

            Message.Show($"There are {rowsSelected} rows selected", "Save button", MessageImage.Information);
        }
    }

    // This is the model of example grid
    public class GridItem
    {
        public bool Selected { get; set; }

        public string Name { get; set; }

        public decimal Quantity { get; set; }

        public decimal Amount { get; set; }

        public DateTime DateDelivery { get; set; }
    }
}
