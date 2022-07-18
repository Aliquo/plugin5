using Aliquo.Windows;
using Aliquo.Windows.Base;
using Aliquo.Windows.Controls;
using Aliquo.Windows.Controls.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace plugin5_demo.ViewModels
{
    public class DataGridViewModel : ViewModelBase, IDisposable
    {
        private IHost host;
        public IHost Host
        {
            get { return host; }
            set
            {
                SetProperty(ref host, value);
            }
        }

        public string TitleWindow = "Clientes";

        //Nombre con el que se guardará la configuración de columnas
        //que haga el usuario desde la pantalla
        private string settingsCode = "Clientes_Datagrid"; 

        private List<FieldSetting> FieldSettingBase;

        private IWindowView window;
        /// <summary>
        /// Contiene la referencia a la ventana
        /// </summary>
        public IWindowView Window
        {
            get { return window; }
        }

        private List<Aliquo.Windows.Controls.Models.FieldSetting> fields;
        /// <summary>
        /// Contiene la colección de configuraciones de campos dinámicos de la vista
        /// </summary>
        public List<Aliquo.Windows.Controls.Models.FieldSetting> Fields
        {
            get
            {
                return fields;
            }
            set { SetProperty(ref fields, value); }
        }

        private byte fixedColumn;
        /// <summary>
        /// Numero de columnas fijas en el grid, será el elemento a bindear con SfDataGridBehavior.FrozenColumnCount
        /// </summary>
        public byte FixedColumns
        {
            get { return fixedColumn; }
            set { SetProperty(ref fixedColumn, value); }
        }

        private string tables;
        /// <summary>
        /// Tablas de datos
        /// </summary>
        public string Tables
        {
            get { return tables; }
            set { SetProperty(ref tables, value); }
        }

        private object selectedItem;
        /// <summary>
        /// Objeto (fila) seleccionado en el grid
        /// </summary>
        public object SelectedItem
        {
            get { return selectedItem; }
            set
            {
                SetProperty(ref selectedItem, value);
                this.ViewCommand.RaiseCanExecuteChanged();
            }
        }

        private string condition;
        /// <summary>
        /// Propiedad con el código de proyecto a filtrar (en caso de que así se defina)
        /// </summary>
        public string Condition
        {
            get { return condition; }
            set { SetProperty(ref condition, value); }
        }


        Aliquo.Windows.Base.DelegateCommand refreshCommand;
        /// <summary>
        /// Command para refrescar el grid
        /// </summary>
        public Aliquo.Windows.Base.DelegateCommand RefreshCommand
        {
            get { return refreshCommand; }
            set { SetProperty(ref refreshCommand, value); }
        }

        Aliquo.Windows.Base.DelegateCommand setupCommand;
        /// <summary>
        /// Command para lanzar la ventana de configuración de columnas
        /// </summary>
        public Aliquo.Windows.Base.DelegateCommand SetupCommand
        {
            get { return setupCommand; }
            set { SetProperty(ref setupCommand, value); }
        }

        private DelegateCommand<object> viewCommand;
        /// <summary>
        /// Comando de la vista
        /// </summary>
        public DelegateCommand<object> ViewCommand
        {
            get
            {
                if (this.viewCommand == null)
                    this.viewCommand = new DelegateCommand<object>(ViewCommandExecute, ViewCommandCanExecute);

                return viewCommand;
            }
        }

        /// <summary>
        /// Constructor de ViewModel
        /// </summary>
        /// <param name="host">variable con el objeto host</param>
        /// <param name="idCustomer">Id de cliente por el que se quiere filtrar o nulo si se quieren todos</param>
        public DataGridViewModel(IHost host, string codCountry)
        {
            this.host = host;

            //Definición de tablas de datos
            Tables = "Clientes LEFT JOIN Paises ON Clientes.CodPais = Paises.Codigo ";
            Tables += "LEFT JOIN MediosPago ON Clientes.CodMedioPago = MediosPago.Codigo ";
            Tables += "LEFT JOIN TerminosPago ON Clientes.CodTerminoPago = TerminosPago.Codigo ";

            if (!string.IsNullOrEmpty(codCountry))
            {
                this.Condition = $"Clientes.codPais={Aliquo.Core.Data.ToSQLString(codCountry)}";
            }

            CreateWindow();

            Task.Run(async () => await CreateFieldsAsync());
            IsBusy = false;
        }

        /// <summary>
        /// En este caso se pasa el objeto ConfigureFieldsAfterEventArgs para poder actualizar la propiedad Handled
        /// ya que al haber llamadas asincronas, se devuelve la ejecucion sin poder devolver el valor correcto.
        /// </summary>
        public async void SaveVisibleFields(ConfigureFieldsAfterEventArgs args)
        {
            args.Handled = false;
            if (args.Restore)
            {
                args.Handled = true;
                await Host.Configuration.SetCustomSettingsAsync(settingsCode, null, true);
                await CreateFieldsAsync();
                FixedColumns = 0;
            }
            else
            {
                // guardar la nueva configuración
                GridSettings gridSettings = new GridSettings();
                gridSettings.Fields = args.VisibleFields.ToFields();
                gridSettings.FixedColumns = args.FixedColumns;
                await Host.Configuration.SetCustomSettingsAsync(settingsCode, Aliquo.Core.Serialization.JsonSerializeObject(gridSettings), true);
            }
        }
        public void Dispose()
        {
            window.PropertyChanged -= Window_PropertyChanged;
        }
        /// <summary>
        /// Crea los campos para el grid
        /// </summary>
        /// <returns></returns>
        private async Task CreateFieldsAsync()
        {
            //Se comprueba si el usuario tiene definida una configuración
            var fieldConfiguration = await Host.Configuration.GetCustomSettingsAsync(settingsCode);
            InitializeFieldsSettingsBase();

            if (string.IsNullOrWhiteSpace(fieldConfiguration))
            {
                //No hay configuracion, se ponen las columnas por defecto                
                this.Fields = this.FieldSettingBase;
            }
            else
            {
                //Se ponen las columnas definidas por el usuario                
                var gridSettings = Aliquo.Core.Serialization.JsonDeserializeObject<GridSettings>(fieldConfiguration);
                var visibleFields = gridSettings.Fields;
                this.Fields = this.FieldSettingBase.MergeVisibleSettings(visibleFields, true).ToList();
                this.FixedColumns = gridSettings.FixedColumns;
            }
        }
        private void InitializeFieldsSettingsBase()
        {
            //Configuración de campos dinámicos
            //de las tablas definidas como origen en el Tables, se pueden extraer los campos que se necesiten para configurar los campos
            //En cada campo se puede definir:
            //- Name, que es nombre que tiene el campo en el origen de datos
            //- Text, que mostrará en la cabecera de la columna
            //- IsHidden, que indica que el campo por defecto está oculto. Para mostrarlo debemos ir a la configuración de columnas
            //- NotSettable, que indica que un campo no se muestra en la pantalla de configuración de columnas por lo que su definición no se puede modificar
            //- SortDirection, que indica si la ordenación es descendente o ascendente
            //- SortOrder, que indica la prioridad al hacer la ordenacion en caso de tener varias columnas para ordenar

            FieldSettingBase = new List<Aliquo.Windows.Controls.Models.FieldSetting>();
            FieldSettingBase.Add(new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Clientes.Id", Text = "Id. de cliente", IsHidden = true, Alias = "Id", NotSettable = true });
            FieldSettingBase.Add(new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Clientes.Codigo", Text = "Cód. cliente"});
            FieldSettingBase.Add(new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Clientes.Nombre", Text = "Cliente" });
            FieldSettingBase.Add(new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Clientes.Telefono", Text = "Teléfono" });
            FieldSettingBase.Add(new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Clientes.Email", Text = "Email" });
            FieldSettingBase.Add(new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Clientes.CIF", Text = "NIF" });
            FieldSettingBase.Add(new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Clientes.CodPais", Text = "Cód. Pais" });
            FieldSettingBase.Add(new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Clientes.CodMedioPago", Text = "CodMedioPago", IsHidden=true });
            FieldSettingBase.Add(new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "MediosPago.Nombre", Text = "Nombre medio pago" });
            FieldSettingBase.Add(new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Clientes.CodTerminoPago", Text = "CodTerminoPago", IsHidden = true });
            FieldSettingBase.Add(new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "TerminosPago.Nombre", Text = "Nombre término pago" });
            FieldSettingBase.Add(new Aliquo.Windows.Controls.Models.FieldSetting() { Name = "Clientes.FechaAlta", Text = "Fecha estado", SortDirection = Aliquo.Windows.Controls.FieldSortDirection.Descending });
        }

        /// <summary>
        /// Ejecución del comando de vista
        /// </summary>
        /// <param name="parameter">Acción que se quiere ejecutar</param>
        private void ViewCommandExecute(object parameter)
        {
            switch (Aliquo.Core.Convert.ValueToString(parameter).ToLower())
            {               
                case "refresh": //Accion de refrescar los datos
                    {
                        //Se indica que se estan cargando datos
                        SetIsBusy(true);
                        RefreshCommand.Execute(null);
                        
                        //Se indica que se ha terminado la carga de datos
                        SetIsBusy(false);
                        break;
                    }
                case "setup": //Accion de configurar las columnas del grid
                    {  
                        SetupCommand.Execute(null);
                        break;
                    }
            }
        }

        /// <summary>
        /// Metodo que indica si una accion del comando se puede ejecutar
        /// </summary>
        /// <param name="parameter">Accion que se quiere comprobar</param>
        /// <returns></returns>
        private bool ViewCommandCanExecute(object parameter)
        {
            //Por ejemplo, mientras se están cargando los datos (IsBusy=true)
            //No se pueden ejecutar estas acciones
            switch (Aliquo.Core.Convert.ValueToString(parameter).ToLower())
            {               
                case "refresh":
                    {
                        return !IsBusy;
                    }
                case "setup":
                    {
                        return !IsBusy;
                    }
            }

            return false;
        }
        /// <summary>
        /// Indica que se está realizando alguna operación en la vista
        /// </summary>
        private void SetIsBusy(bool isBusy)
        {
            IsBusy = isBusy;

            //Se fuerza a revisar si el ViewCommand se puede ejecutar
            ViewCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Evento que se lanza cuando ha cambiado alguna propiedad del objeto ventana        
        /// </summary>        
        private void Window_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //En este caso lo usamos para saber si ha cambiado la propiedad IsActive
            if (window != null && e.PropertyName == nameof(window.IsActive))
            {
                //Si se ha activado la ventana, se ejecuta el comando de refresco
                if (window.IsActive)
                {                    
                    RefreshCommand.Execute(null);
                }
            }
        }
        private void CreateWindow()
        {
            System.Windows.Media.Imaging.BitmapImage Image = SharedResources.GetBitmapImage("window_grid.png");
            this.window = host.Management.Views.CreateWindowView(this.TitleWindow, Image);

            window.PropertyChanged += Window_PropertyChanged;
        }
    }
}
