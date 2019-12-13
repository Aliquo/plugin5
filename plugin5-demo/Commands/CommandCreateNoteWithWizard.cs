using Aliquo.Core.Exceptions;
using Aliquo.Windows;
using Aliquo.Windows.Extensibility;
using System;
using System.ComponentModel.Composition;

namespace plugin5_demo.Commands
{

    // This is a example to create a button in the sales order list
    [Export(typeof(Command))]
    [CommandItemMetadata(ViewType = ViewType.SalesOrder,
                         CommandSize = CommandSize.Middle,
                         CommandType = CommandType.QuickAction,
                         Text = PlugInTitle,
                         Image = "document_new.png")]
    class CommandCreateNoteWithWizard : Command
    {

        private const string PlugInTitle = "Create note with wizard";

        public CommandCreateNoteWithWizard()
        {
            Execute += Command_Execute;
        }

        private void Command_Execute(IHost sender, ExecuteEventArgs e)
        {

            try
            {

                Process.ProcessCreateNoteWithWizard process = new Process.ProcessCreateNoteWithWizard();
                process.ShowWizard(sender);

            }
            catch (HandledException ex)
            {
                Message.Show(ex.Message, "CommandCreateNoteWithWizard", MessageImage.Warning);
            }
            catch (Exception ex)
            {
                sender.Management.Views.ShowException(ex);
            }

        }

    }
}
