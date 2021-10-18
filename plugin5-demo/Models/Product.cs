using Aliquo.Core.Base;
using Aliquo.Windows.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plugin5_demo.Models
{
    public class Product : BindableCore, IExtendedModel
    {
        #region Propiedades
        private long? id;
        public long? Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }


        private string codigo;

        public string Codigo
        {
            get { return codigo; }
            set { SetProperty(ref codigo, value); }
        }


        private string nombre;

        public string Nombre
        {
            get { return nombre; }
            set { SetProperty(ref nombre, value); }
        }


        private string codFamilia;
        public string CodFamilia
        {
            get { return codFamilia; }
            set { SetProperty(ref codFamilia, value); }
        }


        private string nombreFamilia;
        public string NombreFamilia
        {
            get { return nombreFamilia; }
            set { SetProperty(ref nombreFamilia, value); }
        }


        private string codSubFamilia;

        public string CodSubFamilia
        {
            get { return codSubFamilia; }
            set { SetProperty(ref codSubFamilia, value); }
        }


        private string nombreSubfamilia;

        public string NombreSubFamilia
        {
            get { return nombreSubfamilia; }
            set { SetProperty(ref nombreSubfamilia, value); }
        }


        private decimal? costeMedio;

        public decimal? CosteMedio
        {
            get { return costeMedio; }
            set { SetProperty(ref costeMedio, value); }
        }


        private decimal? precioVenta;

        public decimal? PrecioVenta
        {
            get { return precioVenta; }
            set { SetProperty(ref precioVenta, value); }
        }



        private string codTipoImpuesto;

        public string CodTipoImpuesto
        {
            get { return codTipoImpuesto; }
            set { SetProperty(ref codTipoImpuesto, value); }
        }


        private string nombreTipoImpuesto;

        public string NombreTipoImpuesto
        {
            get { return nombreTipoImpuesto; }
            set { SetProperty(ref nombreTipoImpuesto, value); }
        }


        private DateTime fechaCreacion;
        public DateTime FechaCreacion
        {
            get { return fechaCreacion; }
            set { SetProperty(ref fechaCreacion, value); }
        }


        private DateTime fechaModificacion;

        public DateTime FechaModificacion
        {
            get { return fechaModificacion; }
            set { SetProperty(ref fechaModificacion, value); }
        }


        private int idUsuarioCreacion;

        public int IdUsuarioCreacion
        {
            get { return idUsuarioCreacion; }
            set { SetProperty(ref idUsuarioCreacion, value); }
        }


        private int idUsuarioModificacion;

        public int IdUsuarioModificacion
        {
            get { return idUsuarioModificacion; }
            set { SetProperty(ref idUsuarioModificacion, value); }
        }


        private string usuarioCreacion;

        public string UsuarioCreacion
        {
            get { return usuarioCreacion; }
            set { SetProperty(ref usuarioCreacion, value); }
        }



        private string usuarioModificacion;

        public string UsuarioModificacion
        {
            get { return usuarioModificacion; }
            set { SetProperty(ref usuarioModificacion, value); }
        }


        private bool isDirty;
        /// <summary>
        /// Indica si el modelo está cambiado desde su carga inicial
        /// </summary>        
        public bool IsDirty
        {
            get { return isDirty; }
            set
            {
                SetProperty(ref isDirty, value);
            }
        }
        #endregion

        #region Constructor
        public Product()
        {
            ValidationProvider = new ValidationProvider();
            ValidationProvider.ErrorsChanged += (s, e) => { ErrorsChanged?.Invoke(this, e); };

            //Se establecen validaciones a nivel de modelo
            ValidationProvider.Rules.Add(new DelegateRule(nameof(PrecioVenta), "El precio de venta debe ser mayor que el coste", () => IsPvpGreaterCost()));

            this.PropertyChanged += (sender, e) =>
            {
                //Si cambia cualquier propiedad (que no sea IsDirty), set is Dirty to true
                if (!e.PropertyName.Equals(nameof(IsDirty)))
                {
                    IsDirty = true;
                }

                ValidationProvider.ApplyRules(e.PropertyName);
            };
        }
        #endregion

        public bool Validate()
        {
            return ValidationProvider.Validate();
        }

        private bool IsPvpGreaterCost()
        {            
            return PrecioVenta.HasValue && CosteMedio.HasValue? PrecioVenta > CosteMedio : true;
        }
        #region IExtendedModel
        /// <summary>
        /// Indica si la entidad tiene errores en vista o en reglas
        /// </summary>
        public bool HasErrors => ValidationProvider.HasErrors;

        /// <summary>
        /// Evento que se produce cuando los errores del modelo han cambiado
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;


        /// <summary>
        /// ValidationProvider para enganche con INotifyDataErrorInfo, INotifyDataErrorInView
        /// </summary>  
        public ValidationProvider ValidationProvider { get; set; }

        /// <summary>
        /// Coge todos los erroes, en vista y en reglas
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public IEnumerable GetErrors(string propertyName)
        {
            return ValidationProvider.GetErrors(propertyName);
        }


        /// <summary>
        /// Coge los errores de la vista y se los incluye en el modelo pertinente
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="errorContent"></param>
        public void NotifyErrorInView(string propertyName, object errorContent)
        {
            ValidationProvider.NotifyErrorInView(propertyName, errorContent);
        }
        #endregion
    }
}
