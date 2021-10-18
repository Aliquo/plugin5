using Aliquo.Core;
using Aliquo.Windows;
using Aliquo.Windows.Wizard;
using Aliquo.Windows.Wizard.Controls;
using Aliquo.Windows.Wizard.Link;
using System.Collections.Generic;
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

            ListActions.Items.Add(new Action() { Code = "CommandMenuCompanyEnvironment", Name = "Company environment", Description = "Displays information about the current environment. For example, the company name, user, group" });

            ListActions.Items.Add(new Action() { Code = "ProcessCreateNoteWithWizard", Name = "Create note with wizard", Description = "Using an assistant shows the creation of a sales order" });
            ListActions.Items.Add(new Action() { Code = "ProcessViewAddNewNote", Name = "View add new note", Description = "Example of how to activate the creation view of a new order" });

            ListActions.Items.Add(new Action() { Code = "WizardWithEvents", Name = "Wizard with events", Description = "Example of how to perform events, between the controls themselves, of a custom assistant" });

            ListActions.Items.Add(new Action() { Code = "ProcessFirstAndLastCustomer", Name = "First and last customer", Description = "Search for the first and last client.\n\nUsing functions GetQueryTableAsync and GetDataValueAsync" });

            ListActions.Items.Add(new Action() { Code = "ProcessCreateServiceInvoice", Name = "Create service invoice", Description = "Create a service invoice" });

            ListActions.Items.Add(new Action() { Code = "ProcessDeliverOrder", Name = "Create delivery from order (sales)", Description = "Create a delivery note from order (sales)" });

            ListActions.Items.Add(new Action() { Code = "ProcessUpdateInvoicePayments", Name = "Change the payment date of an invoice (purchases)", Description = "Get the last payment and change the date (purchases)" });

            ListActions.Items.Add(new Action() { Code = "DatagridView", Name = "Demo of Datagrid using MVVM and columns setup", Description = "Demo of Datagrid using MVVM, columns setup (saving configuration to a database) and different types of controls." });

            ListActions.Items.Add(new Action() { Code = "DatagridViewParams", Name = "Demo of Datagrid using MVVM and columns setup and conditional loading", Description = "Demo of Datagrid using MVVM, conditional loading, columns setup (saving configuration to a database) and different types of controls. It uses the same viewmodel as previous demo code" });

            ListActions.Items.Add(new Action() { Code = "Editablegrid", Name = "Demo of editable grid using MVVM", Description = "Demo of editable grid using MVVM. It loads and saves changes to a database, locks on editing and implements some data validations." });

            ListActions.Items.Add(new Action() { Code = "EditableSingleItem", Name = "Demo of editable view using MVVM", Description = "Demo of editable view using MVVM. It loads and saves changes to a database, locks on editing and implements some data validations." });
        }

        private void Controls_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new Views.ControlsView(this.Host);
        }

        private void Images_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new Views.ImagesView(this.Host);
        }

        private void Grid_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new Views.GridView(this.Host);
        }

        private void List_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new Views.ListQuery(this.Host);
        }

        private void Wizard_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new Process.ProcessWizardExample(this.Host);
        }

        private void ButtonAction_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var action = ((sender as Button).DataContext as Action);

            switch (action?.Code)
            {
                case "CommandMenuCompanyEnvironment":
                    new Process.CommandMenuCompanyEnvironment(this.Host);
                    break;

                case "ProcessCreateNoteWithWizard":
                    Process.ProcessCreateNoteWithWizard processCreateNoteWithWizard = new Process.ProcessCreateNoteWithWizard();
                    processCreateNoteWithWizard.ShowWizard(this.Host);
                    break;

                case "ProcessViewAddNewNote":
                    new Process.ProcessViewAddNewNote(this.Host);
                    break;

                case "WizardWithEvents":
                    new Process.WizardWithEvents(this.Host);
                    break;

                case "ProcessFirstAndLastCustomer":
                    Process.ProcessFirstAndLastCustomer processFirstAndLastCustomer = new Process.ProcessFirstAndLastCustomer();
                    processFirstAndLastCustomer.StartProcess(this.Host);
                    break;

                case "ProcessCreateServiceInvoice":
                    Process.ProcessCreateServiceInvoice processCreateServiceInvoice = new Process.ProcessCreateServiceInvoice();
                    processCreateServiceInvoice.StartProcess(this.Host);
                    break;

                case "ProcessDeliverOrder":
                    Process.ProcessDeliverOrder processDeliverOrder = new Process.ProcessDeliverOrder();
                    processDeliverOrder.StartProcess(this.Host);
                    break;

                case "ProcessUpdateInvoicePayments":
                    Process.ProcessUpdateInvoicePayments processUpdateInvoicePayments = new Process.ProcessUpdateInvoicePayments();
                    processUpdateInvoicePayments.StartProcess(this.Host);
                    break;
                case "DatagridView":
                    new Views.DataGridView(this.Host);
                    break;
                case "DatagridViewParams":
                    {
                        var wizard = BuildCountriesWizard();
                        ITask task = Host.Management.Views.WizardCustom("Select customers from country", string.Empty, wizard.ToString());
                        task.Finishing += Task_Finishing;
                        break;
                    }
                case "Editablegrid":
                    new Views.GridEditableView(this.Host);
                    break;
                case "EditableSingleItem":
                    {
                        var wizard = BuildProductsWizard();
                        ITask task = Host.Management.Views.WizardCustom("Products", string.Empty, wizard.ToString());
                        task.Finishing += TaskProducts_Finishing;
                        break;
                    }
            }
        }

        private void TaskProducts_Finishing(object sender, FinishingEventArgs e)
        {  // Load values from wizard
            List<Aliquo.Core.Models.DataField> result = (List<Aliquo.Core.Models.DataField>)e.Result;
            var singleItemView = new SingleItemEditableView(this.Host);
            singleItemView.Load(Aliquo.Core.Convert.ValueToInt32(Data.FindField(result, "IdProduct").Value));
        }

        private void Task_Finishing(object sender, FinishingEventArgs e)
        {
            // Load values from wizard
            List<Aliquo.Core.Models.DataField> result = (List<Aliquo.Core.Models.DataField>)e.Result;
            new Views.DataGridView(this.Host, Aliquo.Core.Convert.ValueToString(Data.FindField(result, "CodCountry")));
        }

        private WizardView BuildCountriesWizard()
        {
            WizardView wizard = new WizardView();
            var wizardStep = new WizardStep();

            wizardStep.StepText = "Countries for customers";

            var tableLink = new WizardTableLink();

            tableLink.Fields.Add("Codigo");
            tableLink.Fields.Add("Nombre");
            tableLink.FieldKey = "Codigo";
            tableLink.FieldText = "Nombre";
            tableLink.Table = "Paises";
            WizardText txtCodCountry = new WizardText() { Name = "CodCountry", Text = "Country", Required = true };
            txtCodCountry.SetLink(tableLink);
            wizardStep.AddControl(txtCodCountry);

            wizard.AddStep(wizardStep);

            return wizard;
        }
        private WizardView BuildProductsWizard()
        {
            WizardView wizard = new WizardView();
            var wizardStep = new WizardStep();

            wizardStep.StepText = "Products";
            var tableLink = new WizardTableLink();

            tableLink.Fields.Add("Id");
            tableLink.Fields.Add("Nombre");
            tableLink.FieldKey = "Id";
            tableLink.FieldText = "Nombre";
            tableLink.Table = "Articulos";
            WizardText txtIdProduct = new WizardText() { Name = "IdProduct", Text = "Product", Required = true };
            txtIdProduct.SetLink(tableLink);
            wizardStep.AddControl(txtIdProduct);

            wizard.AddStep(wizardStep);

            return wizard;
        }
    }
}
