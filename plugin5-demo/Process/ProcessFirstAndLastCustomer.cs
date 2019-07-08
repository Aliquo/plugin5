using Aliquo.Core.Exceptions;
using Aliquo.Windows;
using System;
using System.Collections.Generic;

namespace plugin5_demo.Process
{
    class ProcessFirstAndLastCustomer
    {

        public async void StartProcess(IHost host)
        {
            try
            {
                // Example of using the GetQueryAsync function
                List<Aliquo.Core.Models.Data> firstCustomerList = await host.Management.GetQueryAsync("SELECT TOP 1 Codigo FROM Clientes WHERE FechaAlta IS NOT NULL ORDER BY FechaAlta");

                // We get the first item in the list
                Aliquo.Core.Models.Data firstCustomer = firstCustomerList[0];

                // Example of using the GetDataValueAsync function
                object lastCustomer = await host.Management.GetDataValueAsync("Clientes", "Codigo", "FechaAlta IS NOT NULL", order: "FechaAlta DESC");

                // The search result is displayed
                Message.Show($"The first client in the database, sorted by date, is the code [{firstCustomer["Codigo"].Value}] and the last one is the code [{lastCustomer.ToString()}]", "First and last customer code by date");

            }
            catch (HandledException ex)
            {
                throw new HandledException(ex.Message, ex);
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

    }
}
