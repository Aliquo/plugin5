using Aliquo.Windows;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace plugin5_demo.Views
{
    /// <summary>Interaction logic for ImagesView.xaml</summary>
    public partial class ImagesView : UserControl
    {

        private const string Title = "Images available in resources";

        public ImagesView(IHost host)
        {
            InitializeComponent();

            System.Windows.Media.Imaging.BitmapImage Image = SharedResources.GetBitmapImage("image.png");
            IWindowView window = host.Management.Views.CreateWindowView(Title, Image);

            window.Content = this;
            window.StatusBar = this.StatusBar;

            List<string> imageList = SharedResources.GetResourceImagesList();

            foreach (string resourceName in imageList)
            {

                var newControl = new Image
                {
                    Source = SharedResources.GetBitmapImage(resourceName),
                    Width = 32,
                    Height = 32,
                    ToolTip = resourceName,
                    Margin = new Thickness(10)
                };

                WrapCanvas.Children.Add(newControl);

            }
        }
    }
}
