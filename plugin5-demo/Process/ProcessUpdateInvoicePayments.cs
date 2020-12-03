using Aliquo.Core.Exceptions;
using Aliquo.Windows;
using System.Collections.Generic;
using System.Windows;

namespace plugin5_demo.Process
{
    class ProcessUpdateInvoicePayments
    {

        public async void StartProcess(IHost host)
        {
            try
            {

                // We obtain the invoice of the last payment supplier
                long idInvoice = Aliquo.Core.Convert.ValueToInt64(await host.Management.GetDataValueAsync("Vencimientos_Pagos", "IdFactura", "IdFactura IS NOT NULL AND CodSituacionVto='P' AND FechaVto < GETDATE()", "Id DESC"));

                // We obtain all the payments of the invoice
                List<Aliquo.Core.Models.Payment> payments = await host.Documents.GetInvoicePaymentsAsync(Aliquo.Core.PropertyDocumentType.Supplier, idInvoice);

                // We obtain the last payment
                Aliquo.Core.Models.Payment pendingPayment = payments[payments.Count-1];

                // Update the date
                pendingPayment.DueDate = System.DateTime.Now;

                // Save changes
                await host.Documents.UpdateInvoicePaymentsAsync(Aliquo.Core.PropertyDocumentType.Supplier, idInvoice, payments);

            }
            catch (HandledException ex)
            {
                Message.Show(ex.Message, "ProcessUpdateInvoicePayments", MessageImage.Warning);
            }
            catch (System.Exception ex)
            {
                host.Management.Views.ShowException(ex);
            }
        }

    }
}
