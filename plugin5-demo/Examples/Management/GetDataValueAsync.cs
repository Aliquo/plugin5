namespace plugin5_demo.Examples
{
    class GetDataValueAsync
    {

        async void UseExampleAsync(Aliquo.Windows.IHost host)
        {

            var code = await host.Management.GetDataValueAsync("Clientes", "Codigo", "Id=1");

        }

    }
}
