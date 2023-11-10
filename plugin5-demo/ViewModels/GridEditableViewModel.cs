using Aliquo.Windows;
using Aliquo.Windows.Base;
using Aliquo.Windows.Models;
using plugin5_demo.Helpers;
using plugin5_demo.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsResources = Aliquo.Windows.Properties.Resources;

namespace plugin5_demo.ViewModels
{
    class GridEditableViewModel : SfGridViewModelBase<Product>
    {
        private const string TABLE_PRODUCTS = "Articulos";
        
        //Nombre con el que se guardará la configuración de columnas
        //que haga el usuario desde la pantalla
        private string settingsCode = "Articulos_editableGrid"; 
        public List<Product> DeletedItems { get; set; }
 
        private bool isEditing;
        /// <summary>
        /// Indica si se está editando la lista
        /// </summary>
        public bool IsEditing
        {
            get { return isEditing; }
            set { SetProperty(ref isEditing, value); }
        }

        /// <summary>Código para bloqueo de vista</summary>
        public string LockCode
        {
            get { return $"{TABLE_PRODUCTS}_editable"; }
        }
        #region Commands
        public Aliquo.Windows.Base.DelegateCommand ClearFiltersCommand { get; set; }
        #endregion

        public GridEditableViewModel(IHost host) : base(host)
        {
            this.Title = "Editable grid demo";
            this.Window = host.Management.Views.CreateWindowView(this.Title, SharedResources.GetBitmapImage("window_grid_edit.png"));

            this.Window.Closing += Window_ClosingAsync;
            this.PropertyChanged += ViewModel_PropertyChanged;

            Rules.Add(new Aliquo.Windows.Base.DelegateRule(nameof(Items),
                "Se han encontrado códigos de artículos duplicados.",
                () => !CheckDuplicatedCode()));

            SetIsBusy(true);
            Task.Run(async () =>
            {   
                await LoadVisibleFieldsSettingsAsync(settingsCode);
                await LoadDataAsync();
            });
            SetIsBusy(false);
        }

        /// <summary>
        /// Guarda/restaura la configuracion de los campos del grid
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="restore"></param>
        /// <returns></returns>
        public async Task<bool> SaveVisibleFields(System.Collections.Generic.List<Aliquo.Windows.Controls.Models.FieldSetting> fields, bool restore)
        {
            bool handleEvent = false;
            if (restore)
            {
                await SaveRestoreVisibleFieldsSettingsAsync(settingsCode, false);
            }
            else
            {
                VisibleFields = fields.Cast<Aliquo.Windows.Controls.Models.Field>().ToList();
                await SaveVisibleFieldsSettingsAsync(settingsCode, false);
            }
            return handleEvent;
        }

        /// <summary>
        /// Rellena un modelo para ser usado en el grid al dar de alta un elemento
        /// </summary>
        /// <param name="data"></param>
        public override void AddNewRowInitiating(Aliquo.Windows.Models.ValidationData data)
        {
            InitializeNewItem((Product)(data.DataObject));
            base.AddNewRowInitiating(data);
        }

        /// <summary>
        /// Realiza las validaciones de los campos con enlace.
        /// En caso de escribir a mano el código, busca el nombre correspondiente.
        /// Si no encuentra, deja en blanco los valores del código y el nombre
        /// </summary>               
        public async Task CurrentCellValidatedAsync(ValidationData data)
        {
            ///El objeto product es una referencia al objeto del control (data)
            Product product = (Product)data.DataObject;
            if (product == null)
                return;
            try
            {
                if (data.PropertyName.Equals(nameof(product.CodFamilia)))
                {
                    await CheckFamily(data, product, byCode: true);
                }
                if (data.PropertyName.Equals(nameof(product.NombreFamilia)))
                {
                    await CheckFamily(data, product, byCode: false);
                }
                if (data.PropertyName.Equals(nameof(product.CodSubFamilia)))
                {
                    await CheckSubfamily(data, product, byCode: false);
                }
                if (data.PropertyName.Equals(nameof(product.NombreSubFamilia)))
                {
                    await CheckSubfamily(data, product, byCode: true);
                }
            }
            catch (Exception ex)
            {
                this.Host.Management.Views.ShowException(ex);
            }
        }

        /// <summary>
        /// Inicializa datos basicos del elemento a crear
        /// Pueden fechas, campos numericos, ... en general valores por defecto que queremos que tenga el elemento
        /// </summary>
        /// <param name="newItem"></param>
        private void InitializeNewItem(Product newItem)
        {
            newItem.FechaCreacion = System.DateTime.Now;
            newItem.PrecioVenta = 0;
        }

        /// <summary>
        /// Comprueba que el código de familia existe
        /// Rellena con el nombre de familia en caso de existir
        /// </summary>
        /// <param name="data">Objeto con el nuevo valor introducido</param>
        /// <param name="product">Objeto Product con los nuevos valores</param>
        /// <param name="byCode">Indica si queremos validar por el código de la familia o por el nombre</param>        
        /// <returns></returns>
        private async Task CheckFamily(ValidationData data, Product product, bool byCode)
        {
            if (string.IsNullOrWhiteSpace(data.NewValue.ToString()))
            {
                return;
            }

            object filterValue = data.NewValue;
            string tableName = "Familias";
            string codeField = "Codigo";
            string nameField = "Nombre";
            string fields = $"{codeField}, {nameField}";
            StringBuilder condition = new StringBuilder();

            object newFamily = data.NewValue;

            if (byCode)
                condition.Append($"{tableName}.codigo = {Aliquo.Core.Data.ToSQLString(newFamily)}");
            else
                condition.Append(value: $"{tableName}.Nombre = {Aliquo.Core.Data.ToSQLString(newFamily)}");

            var dataTable = await Host.Management.GetDataTableAsync(tableName.ToString(), fields, condition: condition.ToString(), order: string.Empty);

            //Si la validación es fallida, borramos la descripción y el origen asociados y
            //mostramos el grid para elegir origin con un autofiltro en el nombre  
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                product.CodFamilia = string.Empty;
                product.CodSubFamilia = string.Empty;
                product.NombreFamilia = string.Empty;
                product.NombreSubFamilia = string.Empty;

                //Si la referencia al elemento seleccionado es distinta a la que actualmente está seleccionada (por cambiarlo en la pantalla
                //al estar en un método asincrono), no mostramos la ventana de selección.
                if (product == CurrentItem)
                {
                    ShowFamiliesGrid(filter: filterValue, byCode: byCode);
                }
            }
            else
            {
                //Si la validación está ok, rellena codigo y el nombre
                product.CodFamilia = Aliquo.Core.Convert.ValueToString(dataTable.Rows[0]["Codigo"]);
                product.NombreFamilia = Aliquo.Core.Convert.ValueToString(dataTable.Rows[0]["Nombre"]);
            }

        }

        /// <summary>
        /// Comprueba que el código de subfamilia existe
        /// Rellena con el nombre de subfamilia en caso de existir
        /// </summary>
        /// <param name="data">Objeto con el nuevo valor introducido</param>
        /// <param name="product">Objeto Product con los nuevos valores</param>
        /// <param name="byCode">Indica si queremos validar por el código de la subfamilia o por el nombre</param>        
        /// <returns></returns>
        private async Task CheckSubfamily(ValidationData data, Product product, bool byCode)
        {
            if (string.IsNullOrWhiteSpace(data.NewValue.ToString()))
            {
                return;
            }
            object filterValue = data.NewValue;
            string tableName = "FamiliaSubfamilias";
            string codeField = "Codigo";
            string nameField = "Nombre";
            string fields = $"{codeField}, {nameField}";
            StringBuilder condition = new StringBuilder();

            object newSubfamily = data.NewValue;

            if (byCode)
            {
                condition.Append($"{tableName}.CodFamilia={Aliquo.Core.Data.ToSQLString(product.CodFamilia)} and {tableName}.codigo = {Aliquo.Core.Data.ToSQLString(newSubfamily)}");
            }
            else
            {
                condition.Append($"{tableName}.CodFamilia={Aliquo.Core.Data.ToSQLString(product.CodFamilia)} and {tableName}.nombre = {Aliquo.Core.Data.ToSQLString(newSubfamily)}");
            }

            var dataTable = await Host.Management.GetDataTableAsync(tableName.ToString(), fields, condition: condition.ToString(), order: string.Empty);

            //Si la validación es fallida, borramos la descripción y el origen asociados y
            //mostramos el grid para elegir origin con un autofiltro en el nombre  
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                product.CodSubFamilia = string.Empty;
                product.NombreSubFamilia = string.Empty;

                //Si la referencia al elemento seleccionado es distinta a la que actualmente está seleccionada (por cambiarlo en la pantalla
                //al estar en un método asincrono), no mostramos la ventana de selección.
                if (product == CurrentItem)
                {
                    ShowSubFamiliesGrid(filter: filterValue, byCode: byCode);
                }
            }
            else
            {
                //Si la validación está ok, rellena codigo y el nombre
                product.CodSubFamilia = Aliquo.Core.Convert.ValueToString(dataTable.Rows[0]["Codigo"]);
                product.NombreSubFamilia = Aliquo.Core.Convert.ValueToString(dataTable.Rows[0]["Nombre"]);
            }
        }

        /// <summary>
        /// Carga los datos en el elemento Items
        /// </summary>
        private async Task LoadDataAsync()
        {
            IsBusy = true;

            try
            {
                DataAccess dataAccess = new DataAccess(this.Host);
                //Traemos los datos
                Items = new ObservableCollection<Product>(await dataAccess.LoadProducts());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Guarda los datos en base de datos si las validaciones son correctas.
        /// </summary>
        private async Task<bool> SaveDataAsync()
        {
            if (!ValidateAll())
            {
                var errorMessages = GetErrorMessages();
                Message.Show(string.Format(WindowsResources.Message_ValidationRequiredReview, errorMessages), Title, MessageImage.Warning);
                return false;
            }
            else
            {
                SetIsBusy(true);
                try
                {
                    DataAccess dataAccess = new DataAccess(this.Host);
                    await dataAccess.SaveProductsAsync(Items.Where(product => product.IsDirty).ToList(), DeletedItems);
                    SetModelAsNoDirty();
                    SetIsBusy(false);
                    await LoadDataAsync();
                    return true;
                }
                catch (Aliquo.Core.Exceptions.HandledException ex)
                {
                    Message.Show(ex.Message, Title, MessageImage.Warning);
                    return false;
                }
                catch (Exception ex)
                {
                    Host.Management.Views.ShowException(ex);
                    return false;
                }
            }
        }

        private void CloseWindow()
        {
            this.Window.Closing -= Window_ClosingAsync;
            this.Window.Close();
        }

        /// <summary>
        /// Al cerrar la ventana se comprueba si estamos en edicion
        /// Para pedir confirmacion al usuario de guardar / descartar los cambios
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Window_ClosingAsync(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            await ConfirmCancelEditAsync();

            if (!this.IsEditing)
                CloseWindow();

        }
        
        /// <summary>
        /// Pide confirmacion la usuario para guardar / descartar los cambios
        /// si estamos en edicion
        /// </summary>
        /// <returns></returns>        
        private async Task ConfirmCancelEditAsync()
        {
            if (!this.isEditing) return;

            try
            {
                this.Window.CloseChildren();

                if (this.IsDirty)
                {
                    var cancellingResult = Message.Show(WindowsResources.Message_SaveChanges, Title, MessageButton.YesNoCancel, MessageImage.Question);
                    switch (cancellingResult)
                    {
                        case MessageResult.Cancel:
                            return;

                        case MessageResult.Yes:
                            await SaveDataAsync();
                            DeletedItems = new List<Product>();
                            return;

                        case MessageResult.No:
                            await LoadDataAsync();
                            await UnlockViewAsync();
                            DeletedItems = new List<Product>();
                            IsEditing = false;
                            break;
                    }
                }

                await SetNotEditingAsync();

            }
            catch (Aliquo.Core.Exceptions.HandledException ex)
            {
                Message.Show(ex.Message, Title, MessageImage.Warning);
            }
            catch (Exception ex)
            {
                this.Host.Management.Views.ShowException(ex);
            }

            return;
        }

        /// <summary>
        /// Desbloquea la vista (tabla bloqueos)
        /// y la deja en modo No Edición
        /// </summary>
        /// <returns></returns>
        private async Task SetNotEditingAsync()
        {
            await UnlockViewAsync();
            IsEditing = false;
        }

        /// <summary>
        /// Muestra la ventana de busqueda (tabla) para las familias
        /// <para><paramref name="byCode"/> </para>
        /// <para>True => Indica que el filtra se aplica sobre el campo Codigo</para>        
        /// <para>False => Indica que el filtra se aplica sobre el campo Nombre</para>        
        /// </summary>                
        /// <param name="byCode">
        /// True => Indica que el filtro se aplica al campo Codigo
        /// False => Indica que el filtro se aplica al campo Nombre</param>
        /// <param name="filter">Cadena con un texto que hace de filtro que aplicar sobre el campo elegido con <see cref="byCode"/></param>
        private void ShowFamiliesGrid(bool byCode=true, object filter = null)
        {
            var location = SharedWindow.LocationToRect(this.ElementLocation);
            string condition = string.Empty;
            
            string table = "Familias";
            string nameField = "Nombre";
            string codeField = "Codigo";

            IWindowGrid grid = Host.Management.Views.CreateWindowGrid(table, condition: condition, windowOwner: this.Window);
            grid.MultiSelect = false;
            grid.LinkField = nameField;

            if (!grid.ContainsField($"{table}.{codeField}"))
            {
                grid.AddField($"{table}.{codeField}", text: codeField);
            }

            grid.Top = location.Bottom;
            grid.Left = location.Left;

            grid.Selected += Grid_Families_Selected;
            grid.Closed += Grid_Closed;

            if (filter != null)
            {
                if (byCode)
                {
                    grid.SetAutoFilter($"{table}.{codeField}", $"{filter.ToString()}%");
                }
                else
                {
                    grid.SetAutoFilter($"{table}.{nameField}", $"{filter.ToString()}%");
                }
            }

            grid.Show();
        }

        /// <summary>
        /// Evento que se lanza cuando se selecciona un elemento en la ventana de busqueda (tabla)
        /// </summary>
        /// <param name="sender">Objeto con la referencia a la ventana de busqueda (tabla)</param>      
        private void Grid_Families_Selected(object sender, System.EventArgs e)
        {
            var grid = sender as Aliquo.Windows.IWindowGrid;
            if (grid == null) return;

            var selectedRow = grid.GetSelectedRow();
            if (CurrentItem != null && selectedRow != null)
            {
                var selectedFamilyCode = Aliquo.Core.Convert.ValueToString(grid.GetSelectedRow()["familias.codigo"]);
                var selectedFamilyName = Aliquo.Core.Convert.ValueToString(grid.GetSelectedLinkValue());

                //Si cambia la familia, tiene que cambiar la subfamilia
                if (!selectedFamilyCode.Equals(CurrentItem.CodFamilia))
                {
                    CurrentItem.NombreFamilia = selectedFamilyName;
                    CurrentItem.NombreSubFamilia = string.Empty;
                    CurrentItem.CodFamilia = selectedFamilyCode;
                    CurrentItem.CodSubFamilia = string.Empty;
                }
            }

            grid.Close();
        }

        // <summary>
        /// Muestra la ventana de busqueda (tabla) para las subfamilias
        /// <para><paramref name="byCode"/> </para>
        /// <para>True => Indica que el filtra se aplica sobre el campo Codigo</para>        
        /// <para>False => Indica que el filtra se aplica sobre el campo Nombre</para>        
        /// </summary>                
        /// <param name="byCode">
        /// True => Indica que el filtro se aplica al campo Codigo
        /// False => Indica que el filtro se aplica al campo Nombre</param>
        /// <param name="filter">Cadena con un texto que hace de filtro que aplicar sobre el campo elegido con <see cref="byCode"/></param>
        private void ShowSubFamiliesGrid(bool byCode = true, object filter = null)
        {
            string table = "FamiliaSubfamilias";
            string nameField = "Nombre";
            string codeField = "Codigo";

            string codFamilia = string.Empty;
            if (CurrentItem != null)
            {
                codFamilia = CurrentItem.CodFamilia;
            }
            if (string.IsNullOrEmpty(codFamilia))
            {
                Helper.SendNotification(this.Host, Title, "Para seleccionar una subfamilia debe indicar una familia primero", add: false);
                return;
            }

            //Posicion de la ventana con el grid de busqueda
            var location = SharedWindow.LocationToRect(this.ElementLocation);

            string condition = $"FamiliaSubfamilias.CodFamilia={Aliquo.Core.Data.ToSQLString(codFamilia)}";

            IWindowGrid grid = Host.Management.Views.CreateWindowGrid(table, condition: condition, windowOwner: this.Window);
            grid.MultiSelect = false;
            grid.LinkField = nameField;

            if (!grid.ContainsField($"{table}.{codeField}"))
            {
                grid.AddField($"{table}.{codeField}", text: codeField);
            }

            grid.Top = location.Bottom;
            grid.Left = location.Left;

            grid.Selected += Grid_SubFamilies_Selected;
            grid.Closed += Grid_Closed;

            if (filter != null)
            {
                if (byCode)
                {
                    grid.SetAutoFilter($"{table}.{codeField}", $"{filter.ToString()}%");
                }
                else
                {
                    grid.SetAutoFilter($"{table}.{nameField}", $"{filter.ToString()}%");
                }
            }

            grid.Show();
        }
        
        /// <summary>
        /// Evento que se lanza cuando se selecciona un elemento en la ventana de busqueda (tabla)
        /// </summary>
        /// <param name="sender">Objeto con la referencia a la ventana de busqueda (tabla)</param>        
        private void Grid_SubFamilies_Selected(object sender, System.EventArgs e)
        {
            var grid = sender as Aliquo.Windows.IWindowGrid;
            if (grid == null) return;

            var selectedRow = grid.GetSelectedRow();
            if (CurrentItem != null && selectedRow != null)
            {
                var selectedSubFamilyCode = Aliquo.Core.Convert.ValueToString(grid.GetSelectedRow()["FamiliaSubfamilias.codigo"]);
                var selectedSubFamilyName = Aliquo.Core.Convert.ValueToString(grid.GetSelectedLinkValue());

                CurrentItem.NombreSubFamilia = selectedSubFamilyName;
                CurrentItem.CodSubFamilia = selectedSubFamilyCode;
            }

            grid.Close();
        }
        
        /// <summary>
        /// Evento que se lanza al cerra la ventana de busqueda (tabla)
        /// </summary>        
        private void Grid_Closed(object sender, System.EventArgs e)
        {
            var grid = sender as IWindowGrid;
            if (grid != null)
            {
                grid = null;
            }
        }

        /// <summary>
        /// Evento que se lanza cuando se cambia una propiedad del viewmodel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IsEditing):
                    {
                        ViewCommand?.RaiseCanExecuteChanged();
                        break;
                    }
                case nameof(IsDirty):
                    {
                        if (ViewCommand != null)
                            ViewCommand.RaiseCanExecuteChanged();
                        break;
                    }
            }
        }

        /// <summary>
        /// Guarda en una lista los elementos eliminados
        /// y los quita de la lista de Items
        /// </summary>
        private void RemoveSelectedItems()
        {
            if (DeletedItems == null)
                DeletedItems = new List<Product>();

            if (SelectedItems?.Count > 1)
            {
                foreach (var itemToRemove in SelectedItems.Cast<Product>().ToList())
                {
                    DeletedItems.Add(itemToRemove);
                    Items.Remove(itemToRemove);
                }
            }
            else
            {
                DeletedItems.Add(SelectedItem);
                Items.Remove(SelectedItem);
            }
        }

        /// <summary>
        /// Verifica que no hay codigos de articulos duplicados
        /// </summary>        
        private bool CheckDuplicatedCode()
        {
            return !(Items.GroupBy(product => product.Codigo).Count() == Items.Count());
        }

        protected override async void ViewCommandExecute(object parameter)
        {
            switch (Aliquo.Core.Convert.ValueToString(parameter).ToLower())
            {
                case "additem":
                    {
                        var product = new Product();
                        InitializeNewItem(product);

                        InsertItem(product, AddNewItemPosition.BelowSelected);
                        break;
                    }
                case "duplicateitem":
                    {
                        if (SelectedItem == null) return;
                        Product newProduct = Aliquo.Core.Serialization.Clone<Product>(SelectedItem);
                        newProduct.Id = null;

                        InsertItem(newProduct, AddNewItemPosition.BelowSelected);
                        break;
                    }
                case "deleteitem":
                    {
                        if (!IsEditing || SelectedItem == null) return;
                        var deletingResult = Message.Show(WindowsResources.WorkOrder_Questions_DeleteLines,
                                               Title, MessageButton.YesNo, MessageImage.Question);
                        if (deletingResult == MessageResult.Yes)
                        {
                            RemoveSelectedItems();
                        }
                        break;
                    }
                case "modify":
                    {
                        SetIsBusy(true);
                        if (await LockViewAsync(this.LockCode, 0))
                        {
                            SetModelAsNoDirty();
                            IsEditing = true;
                            if (Items != null && Items.Count > 0)
                                SelectedItem = Items.First();

                            SelectRowCommand?.Execute(null);
                        }

                        SetIsBusy(false);
                        break;
                    }
                case "accept":
                    {
                        var result = await SaveDataAsync();
                        if (result)
                        {
                            await UnlockViewAsync();
                            IsEditing = false;
                            Helper.SendNotification(this.Host, Title, "Datos actualizados correctamente", add: false);
                            DeletedItems = new List<Product>();
                        }

                        break;
                    }
                case "refresh":
                    {
                        this.Window.CloseChildren();
                        await LoadDataAsync();
                        break;
                    }
                case "cancel":
                    {
                        this.Window.CloseChildren();
                        if (IsDirty)
                        {
                            await ConfirmCancelEditAsync();
                        }
                        else
                        {
                            await UnlockViewAsync();
                            IsEditing = false;
                        }
                        break;
                    }
            }
        }


        protected override bool ViewCommandCanExecute(object parameter)
        {
            switch (Aliquo.Core.Convert.ValueToString(parameter).ToLower())
            {
                case "add":
                    return this.IsEditing;

                case "duplicate":
                    return this.IsEditing;

                case "delete":
                    return this.IsEditing;

                case "accept":
                    return this.IsEditing && this.IsDirty;

                case "cancel":
                    return this.IsEditing;

                case "modify":
                    return !this.IsEditing;

                case "refresh":
                    return !this.IsEditing;

            }

            return true;
        }

        /// <summary>
        /// Comando que se lanza cuando queremos abrir una ventana de selección de tabla
        /// //Si estamos en modo lectura, se muestra la pantalla de datos completos (ficha) en lugar de la pantalla de busqueda
        /// </summary>
        /// <param name="parameter"></param>
        protected override void LinkCommandExecute(object parameter)
        {
            if (CurrentItem == null)
                return;

            var table = string.Empty;
            var nameField = string.Empty;
            var codeField = string.Empty;
            switch (Aliquo.Core.Convert.ValueToString(parameter))
            {                
                case nameof(Product.CodFamilia):
                    {
                        table = "Familias";
                        nameField = "Nombre";
                        codeField = "Codigo";
                        if (IsEditing)
                        {
                            ShowFamiliesGrid();
                        }
                        else if (CurrentItem != null)
                        {
                            this.Host.Management.Views.ShowRow(table, $"{table}.{codeField} = {Aliquo.Core.Data.ToSQLString(this.CurrentItem.CodFamilia)}");
                        }
                        break;
                    }
                case nameof(Product.NombreFamilia):
                    {
                        table = "Familias";
                        nameField = "Nombre";
                        codeField = "Codigo";
                        if (IsEditing)
                        {
                            ShowFamiliesGrid(byCode: false);
                        }
                        else if (CurrentItem != null)
                        {
                            this.Host.Management.Views.ShowRow(table, $"{table}.{codeField} = {Aliquo.Core.Data.ToSQLString(this.CurrentItem.CodFamilia)}");
                        }
                        break;
                    }
                case nameof(Product.CodSubFamilia):
                    {
                        table = "FamiliaSubfamilias";
                        nameField = "Nombre";
                        codeField = "Codigo";
                        if (IsEditing)
                        {
                            ShowSubFamiliesGrid();
                        }
                        else if (CurrentItem != null)
                        {
                            this.Host.Management.Views.ShowRow(table, $"{table}.codFamilia = {Aliquo.Core.Data.ToSQLString(this.CurrentItem.CodFamilia)} and {table}.{codeField} = {Aliquo.Core.Data.ToSQLString(this.CurrentItem.CodSubFamilia)}");
                        }

                        break;
                    }
                case nameof(Product.NombreSubFamilia):
                    {
                        table = "FamiliaSubfamilias";
                        nameField = "Nombre";
                        codeField = "Codigo";
                        if (IsEditing)
                        {
                            ShowSubFamiliesGrid(byCode: false);
                        }
                        else if (CurrentItem != null)
                        {
                            this.Host.Management.Views.ShowRow(table, $"{table}.codFamilia = {Aliquo.Core.Data.ToSQLString(this.CurrentItem.CodFamilia)} and {table}.{codeField} = {Aliquo.Core.Data.ToSQLString(this.CurrentItem.CodSubFamilia)}");
                        }

                        break;
                    }
            }
        }
    }
}
