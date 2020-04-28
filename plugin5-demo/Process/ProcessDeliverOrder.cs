using Aliquo.Core.Exceptions;
using Aliquo.Windows;
using System;

namespace plugin5_demo.Process
{
    class ProcessDeliverOrder
    {

        public async void StartProcess(IHost host)
        {
            try
            {

                // Get an order pending delivery
                long idOrder = 436781; // Convert.ToInt64(await host.Management.GetDataValueAsync("Notas", "Id", "CodTipoNota='X' AND Estado='A' AND PedidoConEntregas=0", "NEWID()"));

                // Show current order status
                host.Documents.Views.ShowNote(idOrder);

                Aliquo.Core.Models.Note note = await host.Documents.GetNoteAsync(idOrder);

                // We changed the type of note to record a delivery note
                note.Type = Aliquo.Core.NoteType.SalesDeliveryNote;

                // And some more information
                note.Date = host.Environment.WorkDate;

                // Clean the number to assigned a new number
                note.Number = null;

                foreach (Aliquo.Core.Models.Line line in note.Lines)
                {
                    // Half the quantity is delivered
                    line.Quantity = (line.Quantity / 2);
                }

                // Test
                if (note.Lines.Count > 1)
                    note.Lines.RemoveAt(1);

                Aliquo.Core.Models.Line newLine = note.Lines[0].Copy();

                note.Lines.Add(newLine);

                // Delivery is made
                long newId = await host.Documents.DeliverOrderAsync(note);

                // The two documents are shown, the modified order and the created delivery note
                host.Documents.Views.ShowNote(idOrder);
                host.Documents.Views.ShowNote(newId);

            }
            catch (HandledException ex)
            {
                Message.Show(ex.Message, "ProcessDeliverOrder", MessageImage.Warning);
            }
            catch (System.Exception ex)
            {
                host.Management.Views.ShowException(ex);
            }
        }

    }
}
