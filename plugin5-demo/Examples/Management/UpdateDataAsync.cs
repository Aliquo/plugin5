namespace plugin5_demo.Examples
{
    class UpdateDataAsync
    {

        async void UseExampleAsync(Aliquo.Windows.IHost host)
        {
            Aliquo.Core.Models.Data data = new Aliquo.Core.Models.Data
            {
                Table = "Clientes"
            };

            data.Fields.Add(new Aliquo.Core.Models.DataField("Codigo", "999999", true));
            data.Fields.Add(new Aliquo.Core.Models.DataField("Nombre", "Test client"));

            Aliquo.Core.Models.Data result = await host.Management.UpdateDataAsync(data);
        }

    }
}
