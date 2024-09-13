using Aliquo.Core.Exceptions;
using Aliquo.Windows;
using Aliquo.Windows.Extensibility;
using System;
using System.ComponentModel.Composition;

namespace plugin5_demo.Commands
{

    // This is a example to create a button in a table, in this case in Clientes table
    [Export(typeof(Command))]
    [CommandItemMetadata(ViewType = ViewType.Table,
                        ViewKey = "Clientes",
                        CommandSize = CommandSize.Middle,
                        CommandType = CommandType.QuickAction,
                        Text = PlugInTitle,
                        Image = "info.png")]
    class CommandShowRiskCustomer : Command
    {

        private const string PlugInTitle = "Show risk";

        public CommandShowRiskCustomer()
        {
            Execute += Command_ExecuteAsync;
        }

        private async void Command_ExecuteAsync(IHost sender, ExecuteEventArgs e)
        {

            try
            {

                // We obtain the registration identifier
                long? idCustomer = e.View.GetCurrentId();

                if (idCustomer != null)
                {

                    // We get the customer code, prepared data for risk window
                    string codeCustomer = Convert.ToString(await sender.Management.GetDataValueAsync("Clientes", "Codigo", $"Id={idCustomer}"));

                    // Show the risk customer
                    sender.Documents.Views.ShowCustomerRisk(codeCustomer);

                }

            }
            catch (HandledException ex)
            {
                Message.Show(ex.Message, "CommandShowRiskCustomer", MessageImage.Warning);
            }
            catch (Exception ex)
            {
                sender.Management.Views.ShowException(ex);
            }

        }

    }
}
