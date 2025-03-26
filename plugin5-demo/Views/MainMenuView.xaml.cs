using Aliquo.Windows.Extensibility;
using System.ComponentModel.Composition;

namespace plugin5_demo.Views
{
    [Export(typeof(MenuView))]
    [MenuViewMetadata(MenuText = "Demo main menu")]
    /// <summary>
    /// Interaction logic for MainMenuView.xaml
    /// </summary>
    public partial class MainMenuView
    {
        public MainMenuView()
        {
            InitializeComponent();
        }
    }
}
