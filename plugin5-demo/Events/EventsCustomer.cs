using Aliquo.Core;
using Aliquo.Windows;
using Aliquo.Windows.Extensibility;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace plugin5_demo.Events
{

    // This is an example to subscribe to the events of a table, in this case in Clientes table
    [Export(typeof(ViewEvents))]
    [ViewEventsMetadata(ViewKey = "Clientes",
                        ViewStyle = ViewStyle.Normal,
                        ViewType = ViewType.Table)]
    class EventsCustomer : ViewEvents
    {

        public EventsCustomer()
        {
            this.Loaded += EventsCustomer_Loaded;
            this.Closing += EventsCustomer_Closing;
            this.Closed += EventsCustomer_Closed;

            this.DataDeleting += EventsCustomer_DataDeleting;
            this.DataDeleted += EventsCustomer_DataDeleted;

            this.DataUpdating += EventsCustomer_DataUpdating;
            this.DataUpdated += EventsCustomer_DataUpdated;

            this.SelectionChanged += EventsCustomer_SelectionChanged;

        }

        /// <summary>Event that occurs when the view has been loaded</summary>
        private void EventsCustomer_Loaded(object sender, EventArgs e)
        {
            Debug.WriteLine("EventsCustomer_Loaded");

            IViewTable view = (IViewTable)sender;
            view.FieldValueChanged += EventsCustomer_FieldValueChanged;

            // Required field
            view.SetFieldRequired("CIF", true);

            // Set rules of fields (only numbers in Postal Code)
            view.SetFieldRule("CodigoPostal", "Only numbers", () => { return Aliquo.Core.Convert.ValueIsNumeric(view.GetValue("CodigoPostal")); });
        }

        private void EventsCustomer_FieldValueChanged(object sender, FieldChangedEventArgs e)
        {
            IViewTable view = (IViewTable)sender;

            // Check if field is CodigoPostal
            if (e.FieldName == "CodigoPostal")
            {
                // Get value of field ContraCodigo
                if (string.IsNullOrWhiteSpace(Aliquo.Core.Convert.ValueToString(view.GetValue("ContraCodigo"))))
                {
                    view.SetValue("ContraCodigo", view.GetValue(e.FieldName));
                }
            }
        }

        /// <summary>Event that occurs before closing the window view, allowing to cancel</summary>
        private void EventsCustomer_Closing(object sender, CancelEventArgs e)
        {
            Debug.WriteLine($"EventsCustomer_Closing (Cancel={e.Cancel})");
        }

        /// <summary>Event that occurs when the window view is closed</summary>
        private void EventsCustomer_Closed(object sender, EventArgs e)
        {
            Debug.WriteLine("EventsCustomer_Closed");
        }

        /// <summary>Event that occurs before deleting the data, allowing to cancel</summary>
        private void EventsCustomer_DataDeleting(object sender, DataDeletingEventArgs e)
        {
            Aliquo.Core.Models.Data data = (Aliquo.Core.Models.Data)e.Data;

            Debug.WriteLine($"EventsCustomer_DataDeleting (Cancel={e.Cancel}, Data.Nombre={data["Nombre"].Value})");
        }

        /// <summary>Event that occurs after deleting data</summary>
        private void EventsCustomer_DataDeleted(object sender, DataDeletedEventArgs e)
        {
            Aliquo.Core.Models.Data data = (Aliquo.Core.Models.Data)e.Data;

            Debug.WriteLine($"EventsCustomer_DataDeleted (Data.Nombre={data["Nombre"].Value})");
        }

        /// <summary>Event that occurs before updating the data, allowing to cancel</summary>
        private void EventsCustomer_DataUpdating(object sender, DataUpdatingEventArgs e)
        {
            Aliquo.Core.Models.Data oldData = (Aliquo.Core.Models.Data)e.OldData;
            Aliquo.Core.Models.Data newData = (Aliquo.Core.Models.Data)e.NewData;

            Debug.WriteLine($"EventsCustomer_DataUpdating (Cancel={e.Cancel}, oldData.Nombre={oldData?["Nombre"].Value}, newData.Nombre={newData["Nombre"].Value})");
        }

        /// <summary>Event that occurs after updating the data</summary>
        private void EventsCustomer_DataUpdated(object sender, DataUpdatedEventArgs e)
        {
            Debug.WriteLine("EventsCustomer_DataUpdated");
        }

        /// <summary>Event that occurs when the selection changes</summary>
        private void EventsCustomer_SelectionChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("EventsCustomer_SelectionChanged");
        }

    }
}
