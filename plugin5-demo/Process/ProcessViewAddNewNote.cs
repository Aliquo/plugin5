using Aliquo.Windows;
using System;

namespace plugin5_demo.Process
{

    class ProcessViewAddNewNote
    {

        public ProcessViewAddNewNote(IHost sender)
        {

            try
            {

                // We create the model to store the data
                Aliquo.Core.Models.Note note = new Aliquo.Core.Models.Note();

                // This is the basic information to create a note
                note.Type = Aliquo.Core.NoteType.SalesOrder;

                // Show the window with the model 
                sender.Documents.Views.AddNote(note);

            }
            catch (Exception ex)
            {
                sender.Management.Views.ShowException(ex);
            }

        }

    }
}
