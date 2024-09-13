using Aliquo.Windows;
using plugin5_demo.Models;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace plugin5_demo.Helpers
{
    public class DataAccess
    {
        private IHost host;

        public DataAccess(IHost host)
        {
            this.host = host;
        }
        /// <summary>
        /// Realiza la carga de los productos y los convierte al modelo
        /// </summary>
        /// <returns></returns>
        public async Task<List<Product>> LoadProducts()
        {
            StringBuilder tableName = new StringBuilder();
            StringBuilder fields = new StringBuilder();

            tableName.Append("Articulos LEFT JOIN Familias F on Articulos.CodFamilia = F.Codigo");
            tableName.Append(" LEFT JOIN FamiliaSubfamilias S on Articulos.CodFamilia = S.CodFamilia AND Articulos.CodSubFamilia = S.Codigo");
            tableName.Append(" LEFT JOIN CFG_Empresa_Usuarios UC on UC.Id = Articulos.IdUsuarioCreacion ");
            tableName.Append(" LEFT JOIN CFG_Empresa_Usuarios UM on UM.Id = Articulos.IdUsuarioModificacion ");
            tableName.Append(" INNER JOIN Impuestos_Tipos IT on Articulos.CodTipoImpuesto = IT.codigo");


            fields.Append("Articulos.Id, Articulos.Codigo, Articulos.Nombre, Articulos.CosteMedio, Articulos.PVP, Articulos.CodFamilia, Articulos.CodSubFamilia, ");
            fields.Append("Articulos.CodTipoImpuesto, Articulos.FechaCreacion, Articulos.FechaModificacion, ");
            fields.Append("F.Nombre as Familia, S.Nombre as Subfamilia, ");
            fields.Append("IT.Nombre as TipoImpuesto, ");
            fields.Append("UC.Nombre as UsuarioCreacion, UM.Nombre as UsuarioModificacion");

            var dataTable = await host.Management.GetDataTableAsync(tableName.ToString(), fields.ToString(), condition: string.Empty, order: string.Empty);

            return ConvierteDataTableProductsToModel(dataTable);
        }
        public async Task<Product> LoadProduct(long idProduct)
        {
            StringBuilder tableName = new StringBuilder();
            StringBuilder fields = new StringBuilder();
            StringBuilder condition = new StringBuilder();

            tableName.Append("Articulos LEFT JOIN Familias F on Articulos.CodFamilia = F.Codigo");
            tableName.Append(" LEFT JOIN FamiliaSubfamilias S on Articulos.CodFamilia = S.CodFamilia AND Articulos.CodSubFamilia = S.Codigo");
            tableName.Append(" LEFT JOIN CFG_Empresa_Usuarios UC on UC.Id = Articulos.IdUsuarioCreacion ");
            tableName.Append(" LEFT JOIN CFG_Empresa_Usuarios UM on UM.Id = Articulos.IdUsuarioModificacion ");
            tableName.Append(" INNER JOIN Impuestos_Tipos IT on Articulos.CodTipoImpuesto = IT.codigo");

            fields.Append("Articulos.Id, Articulos.Codigo, Articulos.Nombre, Articulos.CosteMedio, Articulos.PVP, Articulos.CodFamilia, Articulos.CodSubFamilia, ");
            fields.Append("Articulos.CodTipoImpuesto, Articulos.FechaCreacion, Articulos.FechaModificacion, ");
            fields.Append("F.Nombre as Familia, S.Nombre as Subfamilia, ");
            fields.Append("IT.Nombre as TipoImpuesto, ");
            fields.Append("UC.Nombre as UsuarioCreacion, UM.Nombre as UsuarioModificacion");

            condition.Append($"Articulos.Id={idProduct}");

            var dataTable = await host.Management.GetDataTableAsync(tableName.ToString(), fields.ToString(), condition: condition.ToString(), order: string.Empty);

            return ConvierteDataTableProductsToModel(dataTable)[0];
        }

        public async Task<string> GetFamilyName(string codFamily)
        {
            StringBuilder tableName = new StringBuilder();
            StringBuilder fields = new StringBuilder();
            StringBuilder condition = new StringBuilder();
            string result = string.Empty;

            tableName.Append("Familias ");
            fields.Append("Familias.Nombre");
            condition.Append($"Familias.Codigo= {Aliquo.Core.Data.ToSQLString(codFamily)}");

            var dataTable = await host.Management.GetDataTableAsync(tableName.ToString(), fields.ToString(), condition: condition.ToString(), order: string.Empty);

            if (dataTable != null && dataTable.Rows.Count > 0)
                result = Aliquo.Core.Convert.ValueToString(dataTable.Rows[0]["Nombre"]);

            return result;
        }

        public async Task<string> GetTaxName(string codTax)
        {
            StringBuilder tableName = new StringBuilder();
            StringBuilder fields = new StringBuilder();
            StringBuilder condition = new StringBuilder();
            string result = string.Empty;

            tableName.Append("Impuestos_Tipos ");
            fields.Append("Impuestos_Tipos.Nombre");
            condition.Append($"Impuestos_Tipos.Codigo= {Aliquo.Core.Data.ToSQLString(codTax)}");

            var dataTable = await host.Management.GetDataTableAsync(tableName.ToString(), fields.ToString(), condition: condition.ToString(), order: string.Empty);

            if (dataTable != null && dataTable.Rows.Count > 0)
                result = Aliquo.Core.Convert.ValueToString(dataTable.Rows[0]["Nombre"]);

            return result;
        }
        public async Task<string> GetSubFamilyName(string codSubfamily)
        {
            StringBuilder tableName = new StringBuilder();
            StringBuilder fields = new StringBuilder();
            StringBuilder condition = new StringBuilder();
            string result = string.Empty;

            tableName.Append("FamiliaSubfamilias ");
            fields.Append("FamiliaSubfamilias.Nombre");
            condition.Append($"FamiliaSubfamilias.Codigo= {Aliquo.Core.Data.ToSQLString(codSubfamily)}");

            var dataTable = await host.Management.GetDataTableAsync(tableName.ToString(), fields.ToString(), condition: condition.ToString(), order: string.Empty);

            if (dataTable != null && dataTable.Rows.Count > 0)
                result = Aliquo.Core.Convert.ValueToString(dataTable.Rows[0]["Nombre"]);

            return result;
        }

        public async Task SaveProductsAsync(List<Product> updatedItems, List<Product> deletedItems)
        {
            List<Aliquo.Core.Models.Data> updateOrInsertData = new List<Aliquo.Core.Models.Data>();
            foreach (var item in updatedItems)
            {
                var data = new Aliquo.Core.Models.Data("Articulos");
                data.Fields.Add(new Aliquo.Core.Models.DataField() { Field = "Id", Value = item.Id, IsKey = true });
                data.Fields.Add(new Aliquo.Core.Models.DataField() { Field = "CodFamilia", Value = string.IsNullOrWhiteSpace(item.CodFamilia) ? null : item.CodFamilia });
                data.Fields.Add(new Aliquo.Core.Models.DataField() { Field = "Codsubfamilia", Value = string.IsNullOrWhiteSpace(item.CodSubFamilia) ? null : item.CodSubFamilia });
                data.Fields.Add(new Aliquo.Core.Models.DataField() { Field = "Codigo", Value = item.Codigo });
                data.Fields.Add(new Aliquo.Core.Models.DataField() { Field = "Nombre", Value = item.Nombre });
                data.Fields.Add(new Aliquo.Core.Models.DataField() { Field = "PVP", Value = item.PrecioVenta ?? null });

                updateOrInsertData.Add(data);
            }

            await host.Management.UpdateDataAsync(updateOrInsertData);
        }
        public async Task<long> SaveProductAsync(Product updatedItem)
        {
            var data = new Aliquo.Core.Models.Data("Articulos");
            data.Fields.Add(new Aliquo.Core.Models.DataField() { Field = "Id", Value = updatedItem.Id, IsKey = true });
            data.Fields.Add(new Aliquo.Core.Models.DataField() { Field = "CodFamilia", Value = string.IsNullOrWhiteSpace(updatedItem.CodFamilia) ? null : updatedItem.CodFamilia });
            data.Fields.Add(new Aliquo.Core.Models.DataField() { Field = "Codsubfamilia", Value = string.IsNullOrWhiteSpace(updatedItem.CodSubFamilia) ? null : updatedItem.CodSubFamilia });
            data.Fields.Add(new Aliquo.Core.Models.DataField() { Field = "Codigo", Value = updatedItem.Codigo });
            data.Fields.Add(new Aliquo.Core.Models.DataField() { Field = "Nombre", Value = updatedItem.Nombre });
            data.Fields.Add(new Aliquo.Core.Models.DataField() { Field = "PVP", Value = updatedItem.PrecioVenta ?? null });
            data.Fields.Add(new Aliquo.Core.Models.DataField() { Field = "CodTipoImpuesto", Value = updatedItem.CodTipoImpuesto });
            var result = await host.Management.UpdateDataAsync(data);
            if (result != null)
                return Aliquo.Core.Convert.ValueToInt32(result["Id"].Value);
            else
                return 0;
        }

        public async Task DeleteProduct(long idProduct)
        {
            var data = new Aliquo.Core.Models.Data("Articulos");
            data.Fields.Add(new Aliquo.Core.Models.DataField() { Field = "Id", Value = idProduct, IsKey = true });
            await host.Management.DeleteDataAsync(data);
        }

        private List<Product> ConvierteDataTableProductsToModel(DataTable dataTable)
        {
            var products = new List<Product>();
            foreach (DataRow dr in dataTable.Rows)
            {
                Product product = new Product()
                {
                    Id = Aliquo.Core.Convert.ValueToInt32(dr["Id"]),
                    CodFamilia = Aliquo.Core.Convert.ValueToString(dr["CodFamilia"]),
                    CodSubFamilia = Aliquo.Core.Convert.ValueToString(dr["CodSubFamilia"]),
                    NombreFamilia = Aliquo.Core.Convert.ValueToString(dr["Familia"]),
                    NombreSubFamilia = Aliquo.Core.Convert.ValueToString(dr["Subfamilia"]),
                    Codigo = Aliquo.Core.Convert.ValueToString(dr["Codigo"]),
                    Nombre = Aliquo.Core.Convert.ValueToString(dr["Nombre"]),
                    CosteMedio = Aliquo.Core.Convert.ValueToDecimalNullable(dr["CosteMedio"]),
                    PrecioVenta = Aliquo.Core.Convert.ValueToDecimalNullable(dr["PVP"]),
                    FechaCreacion = Aliquo.Core.Convert.ValueToDateTime(dr["FechaCreacion"]),
                    FechaModificacion = Aliquo.Core.Convert.ValueToDateTime(dr["FechaModificacion"]),
                    UsuarioCreacion = Aliquo.Core.Convert.ValueToString(dr["UsuarioCreacion"]),
                    UsuarioModificacion = Aliquo.Core.Convert.ValueToString(dr["UsuarioModificacion"]),
                    CodTipoImpuesto = Aliquo.Core.Convert.ValueToString(dr["CodTipoImpuesto"]),
                    NombreTipoImpuesto = Aliquo.Core.Convert.ValueToString(dr["TipoImpuesto"])
                };
                products.Add(product);
            }

            return products;
        }
    }
}
