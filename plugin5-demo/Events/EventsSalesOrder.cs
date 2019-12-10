using Aliquo.Windows;
using Aliquo.Windows.Extensibility;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace plugin5_demo.Events
{

    // This is an example to subscribe to the events of a view, in this case in Sales Order view
    [Export(typeof(ViewEvents))]
    [ViewEventsMetadata(ViewStyle = ViewStyle.Normal,
                        ViewType = ViewType.SalesOrder)]
    class EventsSalesOrder : ViewEvents
    {

        /// <summary>Event that occurs when the view has been loaded</summary>
        private void EventsSalesOrder_Loaded(object sender, EventArgs e)
        {
            if (console == null)
                CreateConsoleWindow((IView)sender);

            this.console?.Append("EventsSalesOrder_Loaded");
        }

        /// <summary>Event that occurs before closing the window view, allowing to cancel</summary>
        private void EventsSalesOrder_Closing(object sender, CancelEventArgs e)
        {
            this.console?.Append($"EventsSalesOrder_Closing (Cancel={e.Cancel})");
        }

        /// <summary>Event that occurs when the window view is closed</summary>
        private void EventsSalesOrder_Closed(object sender, EventArgs e)
        {
            this.console?.Append("EventsSalesOrder_Closed");
        }

        /// <summary>Event that occurs before deleting the note, allowing to cancel</summary>
        private void EventsSalesOrder_DataDeleting(object sender, DataDeletingEventArgs e)
        {
            Aliquo.Core.Models.Note data = (Aliquo.Core.Models.Note)e.Data;

            this.console?.Append($"EventsSalesOrder_NoteDeleting (Cancel={e.Cancel}, Note.Reference={data.Reference}");
        }

        /// <summary>Event that occurs after deleting note</summary>
        private void EventsSalesOrder_DataDeleted(object sender, DataDeletedEventArgs e)
        {
            Aliquo.Core.Models.Note data = (Aliquo.Core.Models.Note)e.Data;

            this.console?.Append($"EventsSalesOrder_NoteDeleted (Note.Reference={data.Reference})");
        }

        /// <summary>Event that occurs before updating the note, allowing to cancel</summary>
        private void EventsSalesOrder_DataUpdating(object sender, DataUpdatingEventArgs e)
        {
            Aliquo.Core.Models.Note oldData = (Aliquo.Core.Models.Note)e.OldData;
            Aliquo.Core.Models.Note newData = (Aliquo.Core.Models.Note)e.NewData;

            this.console?.Append($"EventsSalesOrder_NoteUpdating (Cancel={e.Cancel}, oldData.Reference={oldData.Reference}, newData.Reference={newData.Reference})");
        }

        /// <summary>Event that occurs after updating the note</summary>
        private void EventsSalesOrder_DataUpdated(object sender, DataUpdatedEventArgs e)
        {
            this.console?.Append("EventsSalesOrder_NoteUpdated");
        }

        /// <summary>Event that occurs when the selection changes</summary>
        private void EventsSalesOrder_SelectionChanged(object sender, EventArgs e)
        {
            this.console?.Append("EventsSalesOrder_SelectionChanged");
        }

        /// <summary>Event that occurs when a view is updated</summary>
        private void TableUpdated(object sender, TableUpdatedEventArgs e)
        {
            this.console?.Append($"TableUpdated (Table={e.Table}, Id={e.Id}, Type={e.Type.ToString()}, Reference={e.Reference})");
        }

        #region Auxiliary functions

        private ViewModels.EventsConsoleViewModel console;

        public EventsSalesOrder()
        {
            this.Loaded += EventsSalesOrder_Loaded;
            this.Closing += EventsSalesOrder_Closing;
            this.Closed += EventsSalesOrder_Closed;

            this.DataDeleting += EventsSalesOrder_DataDeleting;
            this.DataDeleted += EventsSalesOrder_DataDeleted;

            this.DataUpdating += EventsSalesOrder_DataUpdating;
            this.DataUpdated += EventsSalesOrder_DataUpdated;

            this.SelectionChanged += EventsSalesOrder_SelectionChanged;
        }

        /// <summary>Activate the window to update the events received</summary>
        private void CreateConsoleWindow(IView view)
        {
            var host = view.GetHost();

            host.Events.TableUpdated -= TableUpdated;

            // A debug view is created
            var eventView = new Views.EventsConsole();
            console = (ViewModels.EventsConsoleViewModel)eventView.DataContext;

            var debugWindow = host.Management.Views.CreateWindowView("Console sales order events");
            debugWindow.Content = eventView;

            debugWindow.Closed += ConsoleWindow_Closed;
            host.Events.TableUpdated += TableUpdated;

            // Focus is returned to source view
            view.IsActive = true;
        }

        /// <summary>Debug view closure is controlled</summary>
        private void ConsoleWindow_Closed(object sender, EventArgs e)
        {
            var debugWindow = (IWindowView)sender;

            var host = debugWindow.View.GetHost();

            debugWindow.Closed -= ConsoleWindow_Closed;
            host.Events.TableUpdated -= TableUpdated;

            console = null;
        }

        #endregion

    }
}
