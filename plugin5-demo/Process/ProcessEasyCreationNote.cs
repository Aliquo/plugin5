﻿using Aliquo.Core.Exceptions;
using Aliquo.Windows;
using Aliquo.Windows.Extensibility;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace plugin5_demo.Process
{

    [Export(typeof(Command))]
    [CommandItemMetadata(ViewType = ViewType.SalesOrder,
                         CommandSize = CommandSize.Middle,
                         Text = "Create new document",
                         CommandType = CommandType.Action,
                         ViewStyle = ViewStyle.List)]
    class ProcessEasyCreationNote : Command
    {

        public ProcessEasyCreationNote()
        {
            Execute += Command_Execute;
        }

        private async void Command_Execute(IHost sender, ExecuteEventArgs e)
        {

            try
            {

                // We create the model to store the data
                Aliquo.Core.Models.Note note = new Aliquo.Core.Models.Note();

                // The collection starts
                note.Lines = new List<Aliquo.Core.Models.Line>();

                // This is the basic information to create a note
                note.Type = Aliquo.Core.NoteType.SalesOrder;
                note.PropertyCode = "001";

                Aliquo.Core.Models.Line line = new Aliquo.Core.Models.Line
                {
                    Type = Aliquo.Core.LineType.Product,
                    Code = "0275",
                    Quantity = 1
                };

                // The line is assigned to the model list
                note.Lines.Add(line);

                // Call the function to create the note
                long id = await sender.Documents.SetNoteAsync(note);

                // Update the list
                e.View.Refresh();

                // Now, if necessary, the document created on the screen is displayed
                sender.Documents.Views.ShowNote(id, isActive: true);

            }
            catch (HandledException ex)
            {
                Message.Show(ex.Message, "ProcessEasyCreationNote", MessageImage.Warning);
            }
            catch (System.Exception ex)
            {
                sender.Management.Views.ShowException(ex);
            }

        }

    }
}
