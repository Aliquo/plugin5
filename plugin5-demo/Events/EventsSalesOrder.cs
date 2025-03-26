using Aliquo.Core;
using Aliquo.Windows;
using Aliquo.Windows.Extensibility;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;

namespace plugin5_demo.Events
{

    // This is an example to subscribe to the events of a view, in this case in Sales Order view
    [Export(typeof(ViewEvents))]
    [ViewEventsMetadata(ViewStyle = ViewStyle.Normal,
                        ViewType = ViewType.SalesOrder)]
    class EventsSalesOrder : ViewNoteEvents
    {

        public EventsSalesOrder()
        {
            this.BeforeShowData += EventsSalesOrder_BeforeShowData;
            this.AfterShowData += EventsSalesOrder_AfterShowData;

            this.Loaded += EventsSalesOrder_Loaded;
            this.Closing += EventsSalesOrder_Closing;
            this.Closed += EventsSalesOrder_Closed;

            this.DataDeleting += EventsSalesOrder_DataDeleting;
            this.DataDeleted += EventsSalesOrder_DataDeleted;

            this.DataUpdating += EventsSalesOrder_DataUpdating;
            this.DataUpdated += EventsSalesOrder_DataUpdated;

            this.SelectionChanged += EventsSalesOrder_SelectionChanged;
        }

        /// <summary>Event that occurs before the data has displayed</summary>
        private void EventsSalesOrder_BeforeShowData(object sender, EventArgs e)
        {

            Debug.WriteLine("EventsSalesOrder_BeforeShowData");

            IViewNote view = ((IViewNote)sender);

            // An example to get all the public information of the Note class
            Aliquo.Core.Models.Note note = view.GetNote();

            if (view.IsEditing() && note.MethodPaymentCode == null)
            {
                IHost host = ((IView)sender).GetHost();

                object methodPayment = Task.Run(async () => await host.Management.GetDataValueAsync("MediosPago", "Codigo", null, "id")).Result;

                view.SetNoteValue(nameof(note.MethodPaymentCode), methodPayment);
            }
        }

        /// <summary>Event that occurs after the data has displayed</summary>
        private void EventsSalesOrder_AfterShowData(object sender, EventArgs e)
        {
            Debug.WriteLine("EventsSalesOrder_AfterShowData");

            IViewNote view = ((IViewNote)sender);

            if (view.IsEditing())
            {
                view.SetNoteValue(nameof(Aliquo.Core.Models.Note.TransportAmount), 100);
            }
        }

        /// <summary>Event that occurs when the view has been loaded</summary>
        private void EventsSalesOrder_Loaded(object sender, EventArgs e)
        {
            Debug.WriteLine("EventsSalesOrder_Loaded");
        }

        /// <summary>Event that occurs before closing the window view, allowing to cancel</summary>
        private void EventsSalesOrder_Closing(object sender, CancelEventArgs e)
        {
            Debug.WriteLine($"EventsSalesOrder_Closing (Cancel={e.Cancel})");
        }

        /// <summary>Event that occurs when the window view is closed</summary>
        private void EventsSalesOrder_Closed(object sender, EventArgs e)
        {
            Debug.WriteLine("EventsSalesOrder_Closed");
        }

        /// <summary>Event that occurs before deleting the note, allowing to cancel</summary>
        private void EventsSalesOrder_DataDeleting(object sender, DataDeletingEventArgs e)
        {
            Aliquo.Core.Models.Note data = (Aliquo.Core.Models.Note)e.Data;

            Debug.WriteLine($"EventsSalesOrder_NoteDeleting (Cancel={e.Cancel}, Note.Reference={data.Reference}");
        }

        /// <summary>Event that occurs after deleting note</summary>
        private void EventsSalesOrder_DataDeleted(object sender, DataDeletedEventArgs e)
        {
            Aliquo.Core.Models.Note data = (Aliquo.Core.Models.Note)e.Data;

            Debug.WriteLine($"EventsSalesOrder_NoteDeleted (Note.Reference={data.Reference})");
        }

        /// <summary>Event that occurs before updating the note, allowing to cancel</summary>
        private void EventsSalesOrder_DataUpdating(object sender, DataUpdatingEventArgs e)
        {
            Aliquo.Core.Models.Note oldData = (Aliquo.Core.Models.Note)e.OldData;
            Aliquo.Core.Models.Note newData = (Aliquo.Core.Models.Note)e.NewData;

            Debug.WriteLine($"EventsSalesOrder_NoteUpdating (Cancel={e.Cancel}, oldData.Reference={oldData?.Reference}, newData.Reference={newData.Reference})");

            IViewNote view = ((IViewNote)sender);

            // You can change the data before save

            object oldReference = view.GetNoteValue("Reference");

            view.SetNoteValue("Reference", "Reference from PlugIn");

            object newReference = view.GetNoteValue("Reference");
                                               
        }

        /// <summary>Event that occurs after updating the note</summary>
        private void EventsSalesOrder_DataUpdated(object sender, DataUpdatedEventArgs e)
        {
            Debug.WriteLine("EventsSalesOrder_NoteUpdated");

            Aliquo.Core.Models.Note oldData = (Aliquo.Core.Models.Note)e.OldData;
            Aliquo.Core.Models.Note newData = (Aliquo.Core.Models.Note)e.NewData;

        }

        /// <summary>Event that occurs when the selection changes</summary>
        private void EventsSalesOrder_SelectionChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("EventsSalesOrder_SelectionChanged");
        }

        /// <summary>Event that occurs when a view is updated</summary>
        private void TableUpdated(object sender, TableUpdatedEventArgs e)
        {
            Debug.WriteLine($"TableUpdated (Table={e.Table}, Id={e.Id}, Type={e.Type.ToString()}, Reference={e.Reference})");
        }
        

    }
}
