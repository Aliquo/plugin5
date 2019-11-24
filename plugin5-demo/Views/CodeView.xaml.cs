using Aliquo.Windows;
using System.Windows.Controls;

namespace plugin5_demo.Views
{
    /// <summary>Interaction logic for CodeView.xaml</summary>
    public partial class CodeView : UserControl
    {

        private const string Title = "Code examples";

        internal class Action
        {

            public string Code { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

        }

        private IHost Host;

        public CodeView(IHost host)
        {

            InitializeComponent();

            this.Host = host;

            System.Windows.Media.Imaging.BitmapImage Image = SharedResources.GetBitmapImage("action.png");
            IWindowView window = this.Host.Management.Views.CreateWindowView(Title, Image);

            window.Ribbon = this.Ribbon;
            window.Content = this;
            window.StatusBar = this.StatusBar;

            ListActions.Items.Add(new Action() { Code = "CommandMenuCompanyEnvironment", Name = "Company environment", Description = "Displays information about the current environment. For example, the company name, user, group." });

            ListActions.Items.Add(new Action() { Code = "ProcessCreateNoteWithWizard", Name = "Create note with wizard", Description = "Using an assistant shows the creation of a sales order." });
            ListActions.Items.Add(new Action() { Code = "ProcessViewAddNewNote", Name = "View add new note", Description = "Example of how to activate the creation view of a new order." });

            ListActions.Items.Add(new Action() { Code = "WizardWithEvents", Name = "Wizard with events", Description = "Example of how to perform events, between the controls themselves, of a custom assistant." });

            ListActions.Items.Add(new Action() { Code = "ProcessFirstAndLastCustomer", Name = "First and last customer", Description = "Search for the first and last client.\n\nUsing functions GetQueryTableAsync and GetDataValueAsync." });

            ListActions.Items.Add(new Action() { Code = "ProcessCreateInvoiceFromNotes", Name = "First and last customer", Description = "Search for the first and last client.\n\nUsing functions GetQueryTableAsync and GetDataValueAsync." });

        }

        private void Controls_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Views.ControlsView examples = new Views.ControlsView(this.Host);
        }

        private void Images_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Views.ImagesView images = new Views.ImagesView(this.Host);
        }

        private void Grid_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Views.GridView images = new Views.GridView(this.Host);
        }

        private void ButtonAction_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var action = ((sender as Button).DataContext as Action);

            switch (action?.Code)
            {
                case "CommandMenuCompanyEnvironment":
                    new Process.CommandMenuCompanyEnvironment(Host);
                    break;

                case "ProcessCreateNoteWithWizard":
                    Process.ProcessCreateNoteWithWizard processCreateNoteWithWizard = new Process.ProcessCreateNoteWithWizard();
                    processCreateNoteWithWizard.ShowWizard(Host);
                    break;

                case "ProcessViewAddNewNote":
                    new Process.ProcessViewAddNewNote(Host);
                    break;

                case "WizardWithEvents":
                    new Process.WizardWithEvents(Host);
                    break;

                case "ProcessFirstAndLastCustomer":
                    Process.ProcessFirstAndLastCustomer processFirstAndLastCustomer = new Process.ProcessFirstAndLastCustomer();
                    processFirstAndLastCustomer.StartProcess(Host);
                    break;

                default:
                    break;
            }


        }
    }
}
