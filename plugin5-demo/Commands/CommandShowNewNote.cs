﻿using Aliquo.Windows;
using Aliquo.Windows.Extensibility;
using System;
using System.ComponentModel.Composition;

namespace plugin5_demo.Commands
{

    [Export(typeof(Command))]
    [CommandItemMetadata(ViewType = ViewType.SalesDeliveryNote, CommandSize = CommandSize.Middle, Text = "Show new document")]
    class CommandShowNewNote : Command
    {

        public CommandShowNewNote()
        {
            Execute += Command_Execute;
        }

        private void Command_Execute(IHost sender, ExecuteEventArgs e)
        {

            try
            {

                // We create the model to store the data
                Aliquo.Core.Models.Note note = new Aliquo.Core.Models.Note();

                // This is the basic information to create a note
                note.Type = Aliquo.Core.NoteType.SalesDeliveryNote;

                // Show the window with the model 
                sender.Documents.Views.AddNote(note);

                e.View.Refresh();

            }
            catch (Exception ex)
            {
                sender.Management.Views.ShowException(ex);
            }

        }

    }
}
