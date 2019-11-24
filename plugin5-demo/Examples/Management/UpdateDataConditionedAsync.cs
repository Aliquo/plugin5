namespace plugin5_demo.Examples
{
    class UpdateDataConditionedAsync
    {

        async void UseExampleAsync(Aliquo.Windows.IHost host)
        {
            Aliquo.Core.Models.UpdateDataConditioned data = new Aliquo.Core.Models.UpdateDataConditioned
            {
                Table = "Articulos",
                Condition = "CodFamilia = 'PIN'"
            };

            data.Fields.Add(new Aliquo.Core.Models.DataField("CodigoTecnico", "PINTEC"));
            data.Fields.Add(new Aliquo.Core.Models.DataField("NombreTecnico", "Pintura técnica"));

            int recordsCount = await host.Management.UpdateDataConditionedAsync(data);
        }

    }
}
