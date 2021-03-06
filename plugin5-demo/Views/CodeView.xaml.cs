﻿using Aliquo.Windows;
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

            }

        }
    }
}
