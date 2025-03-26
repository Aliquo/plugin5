using Aliquo.Core.Exceptions;
using Aliquo.Windows;
using Aliquo.Windows.Extensibility;
using System.ComponentModel.Composition;

namespace plugin5_demo.Commands
{
    [Export(typeof(Command))]
    [CommandItemMetadata(ViewType = ViewType.Table,
                         ViewKey = "Clientes",
                         CommandSize = CommandSize.Middle,
                         CommandType = CommandType.QuickAction,
                         Text = PlugInTitle,
                         Image = "lock.png")]
    class CommandLockCustomer : Command
    {

        private const string PlugInTitle = "Block customer";

        public CommandLockCustomer()
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

                    // We get the customer code, prepared data
                    var codeCustomer = await sender.Management.GetDataValueAsync("Clientes", "Codigo", $"Id={idCustomer}");

                    // We create the registry update structure
                    Aliquo.Core.Models.Data data = new Aliquo.Core.Models.Data("Clientes");
                    data.Fields.Add(new Aliquo.Core.Models.DataField("Codigo", codeCustomer.ToString(), true));
                    data.Fields.Add(new Aliquo.Core.Models.DataField("Bloqueado", 1));

                    // The field is updated, to block the customer
                    await sender.Management.UpdateDataAsync(data);

                    // We force the update of the client list
                    e.View.Refresh();

                }

            }
            catch (HandledException ex)
            {
                Message.Show(ex.Message, "CommandLockCustomer", MessageImage.Warning);
            }
            catch (System.Exception ex)
            {
                sender.Management.Views.ShowException(ex);
            }
        }
    }
}
