

using Aliquo.Core.Exceptions;
using Aliquo.Windows;
using Aliquo.Windows.Extensibility;
using System;
using System.ComponentModel.Composition;

namespace plugin5_demo.Commands
{

    [Export(typeof(Command))]
    [CommandItemMetadata(ViewType = ViewType.SalesOrder,
                         CommandSize = CommandSize.Middle, Text = PlugInTitle)]
    class CommandCreateWithWizard : Command
    {

        private const string PlugInTitle = "Create with wizard";

        public CommandCreateWithWizard()
        {
            Execute += Command_Execute;
        }

        private void Command_Execute(IHost sender, ExecuteEventArgs e)
        {

            try
            {

                Process.ProcessCreateWithWizard process = new Process.ProcessCreateWithWizard();
                process.ShowWizard(sender, e.View);

            }
            catch (HandledException ex)
            {
                Message.Show(ex.Message, PlugInTitle, MessageImage.Warning);
            }
            catch (Exception ex)
            {
                sender.Management.Views.ShowException(ex);
            }

        }

    }
}
