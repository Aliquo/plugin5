using Aliquo.Core.Exceptions;
using Aliquo.Windows;
using System;

namespace plugin5_demo.Process
{
    class ProcessCreateServiceInvoice
    {
        public async void StartProcess(IHost host)
        {
            try
            {

                // A random client is obtained, which is not blocked
                object codeCustomer = await host.Management.GetDataValueAsync("Clientes", "Codigo", "Bloqueado=0", "NEWID()");

                // Get a tax code
                object codeVAT= await host.Management.GetDataValueAsync("Impuestos_Porcentajes", "CodTipo", "FechaFin IS NULL AND PorcIVA>0", "NEWID()");

                // We create the sales invoice model
                Aliquo.Core.Models.SalesInvoice invoice = new Aliquo.Core.Models.SalesInvoice();

                invoice.CustomerCode = codeCustomer.ToString();
                invoice.Date = DateTime.Now;
                invoice.Description = "Sample text of a service invoice";
                invoice.AmountBaseVAT1 = 100;
                invoice.CodeVAT1 = codeVAT.ToString();
                invoice.MethodPaymentCode = "PAGARE";

                // The invoice is created
                long id = await host.Documents.CreateSalesInvoiceAsync(invoice);
                
                // The invoice is shown
                host.Management.Views.ShowRow("Facturas_Clientes", id);

            }
            catch (HandledException ex)
            {
                Message.Show(ex.Message, "ProcessCreateServiceInvoice", MessageImage.Warning);
            }
            catch (System.Exception ex)
            {
                host.Management.Views.ShowException(ex);
            }
        }

    }
}
