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

                // Example of using the GetDataValueAsync function
                object codeCustomer = await host.Management.GetDataValueAsync("Clientes", "Codigo", null);

                Aliquo.Core.Models.SalesInvoice invoice = new Aliquo.Core.Models.SalesInvoice();

                invoice.CustomerCode = codeCustomer.ToString();
                invoice.Date = DateTime.Now;
                invoice.Description = "Sample text of a service invoice";
                invoice.AmountBaseVAT1 = 100;
                invoice.CodeVAT1 = "GENERAL";

                long id = await host.Documents.CreateSalesInvoiceAsync(invoice);

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
