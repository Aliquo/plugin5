using Aliquo.Windows;
using Aliquo.Windows.Extensibility;
using System;
using System.ComponentModel.Composition;

namespace plugin5_demo.Events
{

    // This is an example to subscribe to the events of a table, in this case in Facturas_Proveedores table
    [Export(typeof(ViewEvents))]
    [ViewEventsMetadata(ViewKey = "Facturas_Proveedores",   
                        ViewStyle = ViewStyle.Normal | ViewStyle.List ,
                        ViewType = ViewType.Table)]
    class EventsInvoiceSupplier : ViewEvents
    {
        public EventsInvoiceSupplier()
        {
            this.Loaded += EventsInvoiceSupplier_Loaded;
            this.DataDeleting += EventsInvoiceSupplier_DataDeleting;
        }

        private void EventsInvoiceSupplier_Loaded(object sender, EventArgs e)
        {
            Message.Show("Loaded event", "Events in Invoice Supplier");
        }

        private void EventsInvoiceSupplier_DataDeleting(object sender, DataDeletingEventArgs e)
        {
            Message.Show("Data deleting event", "Events in Invoice Supplier");
        }
    } 
}
