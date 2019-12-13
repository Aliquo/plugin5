using Aliquo.Core.Exceptions;
using Aliquo.Windows;

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

                // We fill in some data to be shown in the new note
                note.Description = "Example of description";
                note.Observations = "Example of observations";

                // Show the window with the model 
                sender.Documents.Views.AddNote(note);

            }
            catch (HandledException ex)
            {
                Message.Show(ex.Message, "ProcessViewAddNewNote", MessageImage.Warning);
            }
            catch (System.Exception ex)
            {
                sender.Management.Views.ShowException(ex);
            }

        }

    }
}
