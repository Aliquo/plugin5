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
    class CommandModifyNoteWithWizard : Command
    {

        private const string PlugInTitle = "Modify note with wizard";

        public CommandModifyNoteWithWizard()
        {
            Execute += Command_ExecuteAsync;
        }

        private async void Command_ExecuteAsync(IHost sender, ExecuteEventArgs e)
        {

            try
            {

                Aliquo.Core.Models.Note note = await sender.Documents.GetNoteAsync((long)e.View.GetCurrentId());

                note.Lines.Clear();

                Aliquo.Core.Models.Line line = new Aliquo.Core.Models.Line
                {
                    Type = Aliquo.Core.LineType.Product,
                    Code = "0110",
                    Quantity = 1,
                    CodeTax = "",
                    PriceTax = 0
                };

                note.Lines.Add(line);

                long id = await sender.Documents.SetNoteAsync(note);

                sender.Documents.Views.ShowNote(id);

            }
            catch (HandledException ex)
            {
                Message.Show(ex.Message, "CommandModifyNoteWithWizard", MessageImage.Warning);
            }
            catch (Exception ex)
            {
                sender.Management.Views.ShowException(ex);
            }

        }

    }
}
