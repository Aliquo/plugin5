using Aliquo.Core;
using Aliquo.Core.Configuration;
using Aliquo.Windows;
using Aliquo.Windows.Base;
using Aliquo.Windows.Controls;
using plugin5_demo.Helpers;
using plugin5_demo.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace plugin5_demo.ViewModels
{
    class SingleItemEditableViewModel : ValidationViewModelBase, IViewMethods, IDisposable
    {
        private DataAccess dataAccess;
        private Aliquo.Windows.IHost host;
        private string lockCode = "Articulo";
        public IHost Host
        {
            get { return host; }
            set
            {
                SetProperty(ref host, value);
            }

        }

        /// <summary>
        /// Proveedor de sugerencias usado para mostrar la sugerencia de código al pulsar una tecla definida
        /// </summary>
        public TextSuggestionProviderAsync ProductCodeSuggestionProvider { get; set; }

        public string Table { get { return "Articulos"; } }

        public string WindowSourceId { get; }

        public bool OriginalView { get; }

        public string DescriptionTable
        {
            get
            {
                return "Products";
            }
        }

        /// <summary>Vista de ventana</summary>
        private IWindowView window;
        public IWindowView Window
        {
            get { return window; }
            set
            {
                if (window != null)
                {
                    window.Closing -= Window_Closing_Async;
                    window.Closed -= Window_Closed;
                }

                SetProperty(ref window, value);

                if (window != null)
                {
                    window.Closing += Window_Closing_Async;
                    window.Closed += Window_Closed;
                }
            }
        }

        /// <summary>Contiene el título de la ventana</summary>
        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                SetProperty(ref this.title, value);
            }
        }


        private Product product;

        /// <summary>
        /// Objeto con la informacion del producto
        /// </summary>
        public Product Product
        {
            get { return product; }
            set { SetProperty(ref product, value); }
        }

        private DelegateCommand<object> viewCommand;
        public DelegateCommand<object> ViewCommand
        {
            get
            {
                if (this.viewCommand == null)
                    this.viewCommand = new DelegateCommand<object>(ViewCommandExecute, ViewCommandCanExecute);

                return viewCommand;
            }
        }

        private bool isEditMode;

        /// <summary>
        /// Propiedad que indica si la ventana esta en modo edicion
        /// </summary>
        public bool IsEditMode
        {
            get { return isEditMode; }
            set
            {
                if (Window != null)
                    Window.ActiveRibbonFirstVisibleItem();

                SetProperty(ref isEditMode, value);
            }
        }

        private bool isInserting;

        /// <summary>
        /// Propiedad que indica si la ventana esta en modo insercion
        /// </summary>
        public bool IsInserting
        {
            get { return isInserting; }
            set { SetProperty(ref isInserting, value); }
        }

        private bool showSuggestionInfo;
        /// <summary>
        /// Indica si se tiene que mostrar el texto de sugerencia en el campo código
        /// en la barra de estado
        /// </summary>
        public bool ShowSuggestionInfo
        {
            get { return showSuggestionInfo; }
            set { SetProperty(ref showSuggestionInfo, value); }
        }

        private bool showContextInfo;
        /// <summary>
        /// Indica si se tiene que mostrar el texto con la información de contexto en el campo usuarios resumen
        /// </summary>
        public bool ShowContextInfo
        {
            get { return showContextInfo; }
            set { SetProperty(ref showContextInfo, value); }
        }

        private string location;
        /// <summary>
        /// Contiene la posicion de un control en pantalla
        /// </summary>
        public string Location
        {
            get { return location; }
            set { location = value; }
        }

        private long? currentProductId = 0;

        /// <summary>
        /// Identificador del producto actual
        /// </summary>
        public long? CurrentProductId
        {
            get { return currentProductId; }
            set { SetProperty(ref currentProductId, value); }
        }

        /// <summary>
        /// Cadena con la situacion de la pantalla
        /// - Añadiendo
        /// - Actualizando
        /// Se usa en la barra de estado
        /// </summary>
        public string InfoStatus
        {
            get
            {
                if (IsInserting && IsEditMode)
                    return Aliquo.Core.Properties.Resources.Inserting;

                if (IsEditMode)
                    return Aliquo.Core.Properties.Resources.Updating;

                return null;
            }
        }

        /// <summary>
        /// Cadena con el código de producto que se esta tratando.
        /// Se usa para el tooltip de control
        /// </summary>
        public string InfoDataCode
        {
            get
            {
                if (Product != null)
                    return Product.Codigo;

                return null;
            }
        }

        /// <summary>
        /// Cadena con el nombre de producto que se esta tratando
        /// Se usa para el tooltip de control
        /// </summary>
        public string InfoDataDescription
        {
            get
            {
                if (Product != null)
                    return Product.Nombre;

                return null;
            }
        }

        public SingleItemEditableViewModel(IHost host, string windowSourceId, bool originalView)
        {

            this.Host = host;
            this.OriginalView = originalView;
            this.dataAccess = new DataAccess(host);

            this.WindowSourceId = windowSourceId;

            CurrentProductId = null;

            this.ValidationProvider.UseValidationError = true;

            ProductCodeSuggestionProvider = new TextSuggestionProviderAsync(TaskCodeSuggestionAsync);
        }

        public void Dispose()
        {
            this.Host = null;
            this.Window = null;
            this.dataAccess = null;
        }


        #region Implementación de IViewMethods

        public long? GetCurrentId()
        {
            return CurrentProductId;
        }

        public List<long> GetSelectedIds()
        {
            if (CurrentProductId.HasValue)
                return new List<long> { CurrentProductId.Value };

            return null;
        }

        public void Refresh()
        {
            if (this.ViewCommandCanExecute("refresh"))
                this.ViewCommandExecute("refresh");
        }

        #endregion

        /// <summary>
        /// Metodo que se ejecuta al pulsar una tecla predefinida sobre el control Codigo
        /// </summary>
        /// <param name="filter">Texto con el código</param>
        /// <returns></returns>
        public async Task<string> TaskCodeSuggestionAsync(string filter)
        {
            //Si el usuario pulsa flecha abajo en modo Inserción
            //estando posicionado en el control Codigo, se intenta ofrece el siguiente codigo
            //basandose en el texto escrito, si lo hubiera
            if (IsInserting)
            {
                string result = await host.Management.SuggestCodeAsync("Articulos", "Codigo", filter);
                return result;
            }

            return null;
        }
        public async void ViewCommandExecute(object parameter)
        {
            try
            {
                switch (Aliquo.Core.Convert.ValueToString(parameter).ToLower())
                {
                    case "add":
                        {
                            Create();
                            break;
                        }

                    case "modify":
                        {
                            await EditionAsync(Product.Id.Value);
                            break;
                        }

                    case "delete":
                        {
                            await DeleteAsync(Product.Id.Value);
                            break;
                        }

                    case "duplicate":
                        {
                            await DuplicateAsync(Product.Id.Value);
                            break;
                        }
                    case "refresh":
                        {
                            await ShowAsync(Product.Id.Value);
                            break;
                        }
                    case "accept_edition":
                        {
                            await SaveChangesAsync();
                            break;
                        }
                    case "cancel_edition":
                        {
                            await ConfirmCancelEditAsync();
                            break;
                        }
                    case "family_selection":
                        {
                            if (IsEditMode)
                            {
                                IWindowGrid grid = host.Management.Views.CreateWindowGrid("Familias", windowOwner: this.window);

                                grid.LinkField = "Codigo";
                                grid.Selected += FamilyName_Selected;
                                grid.Closed += FamilyName_Closed;

                                grid.SetStartPositionRelative(this.Location);
                                grid.Show();
                            }
                            else
                            {
                                this.Host.Management.Views.ShowRow("Familias", string.Format("{0}.Codigo = '{1}'", "Familias", this.Product.CodFamilia));
                            }
                            break;
                        }
                    case "subfamily_selection":
                        {
                            if (IsEditMode)
                            {
                                IWindowGrid grid = host.Management.Views.CreateWindowGrid("FamiliaSubfamilias", windowOwner: this.window);

                                grid.LinkField = "Codigo";
                                grid.Selected += SubFamilyName_Selected;
                                grid.Closed += SubFamilyName_Closed;

                                grid.SetStartPositionRelative(this.Location);
                                grid.Show();
                            }
                            else
                            {
                                this.Host.Management.Views.ShowRow("FamiliaSubfamilias", string.Format("{0}.Codigo = '{1}'", "FamiliaSubfamilias", this.Product.CodSubFamilia));
                            }
                            break;
                        }
                    case "tax_selection":
                        {
                            if (IsEditMode)
                            {
                                IWindowGrid grid = host.Management.Views.CreateWindowGrid("Impuestos_Tipos", windowOwner: this.window);

                                grid.LinkField = "Codigo";
                                grid.Selected += TaxName_Selected;
                                grid.Closed += TaxName_Closed;

                                grid.SetStartPositionRelative(this.Location);
                                grid.Show();
                            }
                            else
                            {
                                this.Host.Management.Views.ShowRow("Impuestos_Tipos", string.Format("{0}.Codigo = '{1}'", "Impuestos_Tipos", this.Product.CodTipoImpuesto));
                            }
                            break;
                        }
                    default:
                        throw new NotImplementedException(string.Format(Aliquo.Windows.Properties.Resources.Command_NotImplemented, parameter));
                }
            }
            catch (Exception ex)
            {
                this.Host.Management.Views.ShowException(ex);
            }
        }
        public bool ViewCommandCanExecute(object parameter)
        {
            if (Host == null)
                return false;

            switch (Aliquo.Core.Convert.ValueToString(parameter).ToLower())
            {
                case "view":
                    return this.Host.Configuration.AllowTableAccess(this.Table) && this.Product != null && this.Product.Id.HasValue && this.Product.Id > 0;

                case "add":
                    return HasPermission(TablePermissions.Add);

                case "modify":
                    return HasPermission(TablePermissions.Modify) && this.Product != null && this.Product.Id.HasValue && this.Product.Id > 0;

                case "delete":
                    return HasPermission(TablePermissions.Delete) && this.Product != null && this.Product.Id.HasValue && this.Product.Id > 0;

                case "duplicate":
                    return this.Host.Configuration.HasTablePermission(this.Table, Aliquo.Core.TablePermissions.Add) && this.Product != null && this.Product.Id.HasValue && this.Product.Id > 0;

                case "refresh":
                    return this.Product != null && this.Product.Id.HasValue && this.Product.Id > 0;

            }

            return true;

        }
        /// <summary>
        /// Carga un producto y pone la ventana en modo edicion
        /// </summary>
        /// <param name="productId">Identificador del producto a cargar</param>        
        public async Task EditionAsync(long? productId)
        {
            try
            {
                if (productId.HasValue)
                {
                    if (productId != currentProductId)
                        await GetDataFromBdAsync(productId.Value);

                    //Application.Current.Dispatcher.Invoke(() => SetViewEditMode(productId.Value));
                    await SetViewEditMode(productId.Value);
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() => SetViewShowMode());
                    Application.Current.Dispatcher.Invoke(() => InitialiceProduct());
                }
                RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => SetViewShowMode());
                Host.Management.Views.ShowException(ex);
            }
        }
        /// <summary>
        /// Inicializa articulo en blanco o con los valores indicados en el parametro        
        /// Pone la ventana en modo insercion
        /// </summary>
        /// <param name="product"></param>
        public void Create(Product product = null)
        {
            try
            {
                SetViewInsertMode();
                InitialiceProduct(product);
                RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => SetViewShowMode());
                this.Host?.Management.Views.ShowException(ex);
            }

        }
        /// <summary>
        /// Muestra la información de un articulo con la ventana en modo solo lectura
        /// </summary>
        /// <param name="productId">Identificador del producto</param>                
        public async Task ShowAsync(long? productId)
        {
            try
            {
                if (productId.HasValue)
                {
                    await GetDataFromBdAsync(productId.Value);
                    Application.Current.Dispatcher.Invoke(() => SetViewShowMode());
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() => SetViewShowMode());
                    Application.Current.Dispatcher.Invoke(() => InitialiceProduct());
                }
                RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                this.Host?.Management.Views.ShowException(ex);
            }
        }

        /// <summary>
        /// Crea un duplicado de una tarea
        /// </summary>
        /// <param name="productId">Identificador del articulo a duplicar</param>
        /// <returns></returns>
        public async Task DuplicateAsync(long? productId)
        {
            try
            {
                if (productId.HasValue)
                {
                    await GetDataFromBdAsync(productId.Value);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SetViewInsertMode();

                        CurrentProductId = null;
                        this.Product.Id = null;
                        this.Product.Codigo = string.Empty;
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() => SetViewInsertMode());
                    Application.Current.Dispatcher.Invoke(() => InitialiceProduct());
                }
                RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                this.Host?.Management.Views.ShowException(ex);
            }
        }

        public async Task DeleteAsync(long? productId)
        {
            if (productId.HasValue)
            {
                MessageResult resultQuestion = Aliquo.Windows.Message.Show(Aliquo.Windows.Properties.Resources.Message_DeleteSelectedRow, this.Title, MessageButton.YesNo, MessageImage.Question);
                if (resultQuestion == MessageResult.Yes)
                {
                    try
                    {
                        await dataAccess.DeleteProduct(productId.Value);

                        SetViewShowMode();
                        InitialiceProduct();
                        RaiseCanExecuteChanged();

                        this.Window.Title = this.Title;
                    }
                    catch (Exception ex)
                    {
                        this.Host?.Management.Views.ShowException(ex);
                    }
                }
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => SetViewShowMode());
                Application.Current.Dispatcher.Invoke(() => InitialiceProduct());
                Aliquo.Windows.Message.Show("No se ha encontrado el artículo indicado", this.Title, MessageButton.OK, MessageImage.Information);

            }
        }

        /// <summary>
        /// Pide confirmación al usuario de que quiere cancelar la edicion / insercion de datos
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ConfirmCancelEditAsync()
        {
            bool result = false;
            var resultQuestion = Aliquo.Windows.Message.Show(Aliquo.Windows.Properties.Resources.Message_SaveChanges, this.Title, MessageButton.YesNoCancel, MessageImage.Question);

            switch (resultQuestion)
            {
                case MessageResult.Yes:
                    {
                        result = await SaveChangesAsync();
                        break;
                    }
                case MessageResult.No:
                    {
                        try
                        {
                            if (Product.Id.HasValue)
                                await Host.Management.UnlockRowAsync(lockCode, Product.Id.Value);

                            this.ValidationProvider.ClearErrorsInRules(null);
                            SetViewShowMode();
                            await ShowAsync(Product.Id);
                            result = true;
                        }
                        catch (Exception ex)
                        {
                            this.Host.Management.Views.ShowException(ex);
                        }
                        break;
                    }
            }
            return result;
        }

        /// <summary>
        /// Realiza la busqueda de una familia cuando el control pierde el foco
        /// Rellena el nombre de la familia con la informacion leida
        /// </summary>        
        public async Task CodFamilyLostFocusEventHandlerAsync(object textboxValue)
        {
            if (Product == null)
                return;

            if (textboxValue is string value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    Product.NombreFamilia = string.Empty;
                    return;
                }

                if (value == Product.NombreFamilia)
                    return;

                var familyName = await dataAccess.GetFamilyName(value);
                if (!string.IsNullOrEmpty(familyName))
                {
                    Product.CodFamilia = value;
                    Product.NombreFamilia = familyName;
                }
            }
        }

        /// <summary>
        /// Realiza la busqueda de una subfamilia cuando el control pierde el foco
        /// Rellena el nombre de la subfamilia con la informacion leida
        /// </summary>  
        public async Task CodSubFamilyLostFocusEventHandlerAsync(object textboxValue)
        {
            if (textboxValue is string value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    Product.NombreSubFamilia = string.Empty;
                    return;
                }

                if (value == Product.NombreSubFamilia)
                    return;

                var familyName = await dataAccess.GetSubFamilyName(value);
                if (!string.IsNullOrEmpty(familyName))
                {
                    Product.CodSubFamilia = value;
                    Product.NombreSubFamilia = familyName;
                }
            }
        }

        /// <summary>
        /// Realiza la busqueda de un impuesto cuando el control pierde el foco
        /// Rellena el nombre de la familia con la informacion leida
        /// </summary>        
        public async Task CodTaxLostFocusEventHandlerAsync(object textboxValue)
        {
            if (textboxValue is string value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    Product.NombreTipoImpuesto = string.Empty;
                    return;
                }

                if (value == Product.NombreTipoImpuesto)
                    return;

                var taxName = await dataAccess.GetTaxName(value);
                if (!string.IsNullOrEmpty(taxName))
                {
                    Product.CodTipoImpuesto = value;
                    Product.NombreTipoImpuesto = taxName;
                }
            }
        }

        private void FamilyName_Closed(object sender, EventArgs e)
        {
            var grid = sender as IWindowGrid;
            if (grid != null)
            {
                grid.Selected -= FamilyName_Selected;
                grid.Closed -= FamilyName_Closed;
            }
        }

        private async void FamilyName_Selected(object sender, EventArgs e)
        {
            var grid = sender as IWindowGrid;
            if (grid != null && grid.GetSelectedLinkValue() != null && !string.IsNullOrEmpty(Aliquo.Core.Convert.ValueToString(grid.GetSelectedLinkValue())))
            {
                var selectedValue = Aliquo.Core.Convert.ValueToString(grid.GetSelectedLinkValue());
                var familyName = await dataAccess.GetFamilyName(selectedValue);
                if (!string.IsNullOrEmpty(familyName))
                {
                    Product.CodFamilia = selectedValue;
                    Product.NombreFamilia = familyName;
                }
                grid.Close();
            }
        }

        private void SubFamilyName_Closed(object sender, EventArgs e)
        {
            var grid = sender as IWindowGrid;
            if (grid != null)
            {
                grid.Selected -= SubFamilyName_Selected;
                grid.Closed -= SubFamilyName_Closed;
            }
        }
        private async void SubFamilyName_Selected(object sender, EventArgs e)
        {
            var grid = sender as IWindowGrid;
            if (grid != null && grid.GetSelectedLinkValue() != null && !string.IsNullOrEmpty(Aliquo.Core.Convert.ValueToString(grid.GetSelectedLinkValue())))
            {
                var selectedValue = Aliquo.Core.Convert.ValueToString(grid.GetSelectedLinkValue());
                var subFamilyName = await dataAccess.GetSubFamilyName(selectedValue);
                if (!string.IsNullOrEmpty(subFamilyName))
                {
                    Product.CodSubFamilia = selectedValue;
                    Product.NombreSubFamilia = subFamilyName;
                }
                grid.Close();
            }
        }

        private void TaxName_Closed(object sender, EventArgs e)
        {
            var grid = sender as IWindowGrid;
            if (grid != null)
            {
                grid.Selected -= TaxName_Selected;
                grid.Closed -= TaxName_Closed;
            }
        }
        private async void TaxName_Selected(object sender, EventArgs e)
        {
            var grid = sender as IWindowGrid;
            if (grid != null && grid.GetSelectedLinkValue() != null && !string.IsNullOrEmpty(Aliquo.Core.Convert.ValueToString(grid.GetSelectedLinkValue())))
            {
                var selectedValue = Aliquo.Core.Convert.ValueToString(grid.GetSelectedLinkValue());
                var taxName = await dataAccess.GetTaxName(selectedValue);
                if (!string.IsNullOrEmpty(taxName))
                {
                    Product.CodTipoImpuesto = selectedValue;
                    Product.NombreTipoImpuesto = taxName;
                }
                grid.Close();
            }
        }


        private void Window_Closed(object sender, System.EventArgs e)
        {
            this.Dispose();
        }

        private async void Window_Closing_Async(object sender, CancelEventArgs e)
        {
            //Siempre cancelamos el evento para que nos permita realizar operaciones asíncronas y esperar a que terminen para cerrar la ventana
            e.Cancel = true;
            if (IsEditMode)
            {
                var result = await ConfirmCancelEditAsync();
                if (result)
                    CloseWindow();
            }
            else
            {
                CloseWindow();
            }
        }
        /// <summary>
        /// Cierra ventana, desbloqueando la vista antes si es necesario
        /// </summary>
        /// <returns></returns>
        private void CloseWindow()
        {
            Window.Closing -= Window_Closing_Async;
            Window.Close();
        }
        /// <summary>
        /// Refresca la posibilidad de ejecutar los comandos
        /// </summary>
        private void RaiseCanExecuteChanged()
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => { RaiseCanExecuteChanged(); });
            }
            else
            {
                // se notifican cambios en los CanExecute
                this.ViewCommand.RaiseCanExecuteChanged();
            }
        }
        private bool HasPermission(TablePermissions permission)
        {
            return this.Host.Configuration.HasTablePermission(this.Table, permission);
        }

        /// <summary>
        /// Realiza las validaciones de datos necesarias y llama al servidor
        /// para realizar la modificacion / insercion de datos.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SaveChangesAsync()
        {
            bool result = false;

            if (!this.ValidationProvider.Validate() || !Product.Validate())
            {
                Aliquo.Windows.Message.Show(Aliquo.Windows.Properties.Resources.Message_ValidationRequiredReviewMark, this.Title, MessageButton.OK, MessageImage.Warning);
            }
            else
            {
                try
                {
                    var resultBd = await dataAccess.SaveProductAsync(Product);
                    if (resultBd > 0)
                    {
                        this.ValidationProvider.ClearErrorsInRules(null);
                        Product.Id = resultBd;
                        await Host.Management.UnlockRowAsync(lockCode, Product.Id.Value);

                        await ShowAsync(resultBd);
                        RaiseCanExecuteChanged();
                        Helper.SendNotification(host, Title, "Se han guardado los datos correctamente", add: false);
                        result = true;
                    }

                }
                catch (Exception ex)
                {
                    if (this.Host != null)
                        this.Host.Management.Views.ShowException(ex);
                }
            }
            return result;
        }
        /// <summary>
        /// Lee la informacion de un producto del servidor
        /// </summary>
        /// <param name="productId">Identificador del artículo</param>        
        /// <returns></returns>
        private async Task GetDataFromBdAsync(long productId)
        {
            Product = await dataAccess.LoadProduct(productId);
            CurrentProductId = productId;

            Application.Current.Dispatcher.Invoke(() => this.Window.Title = Aliquo.Core.Formats.CodeAndName(Product.Codigo, "Artículo", true));
        }
        /// <summary>
        /// Crea un objeto Product vacío o con la informacion en product (si tiene valor)
        /// </summary>
        /// <param name="product"></param>
        private void InitialiceProduct(Product product = null)
        {
            this.CurrentProductId = null;
            if (product != null)
                this.Product = product;
            else
                this.Product = new Product();

            RaisePropertyChanged(nameof(InfoStatus));
            RaisePropertyChanged(nameof(InfoDataCode));
            RaisePropertyChanged(nameof(InfoDataDescription));
        }
        /// <summary>
        /// Pone la pantalla en modo inserción
        /// </summary>
        private void SetViewInsertMode()
        {
            IsEditMode = true;
            IsInserting = true;

            Application.Current.Dispatcher.Invoke(() => this.Title = "Artículo");
            this.Window.Title = "Artículo";
        }
        /// <summary>
        /// Pone la pantalla en modo edicion
        /// </summary>
        private async Task SetViewEditMode(long productId)
        {
            IsEditMode = true;
            IsInserting = false;
            try
            {
                if (productId != 0)
                    await Host.Management.LockRowAsync(this.lockCode, productId, "articulos_lock");

                RaisePropertyChanged(nameof(InfoStatus));
                RaisePropertyChanged(nameof(InfoDataCode));
                RaisePropertyChanged(nameof(InfoDataDescription));
            }
            catch (Exception ex)
            {
                IsEditMode = false;
                IsInserting = false;
                Message.Show(ex.Message, this.Title, MessageImage.Warning);
                return;
            }
        }
        /// <summary>
        /// Pone la pantalla en modo solo lectura
        /// </summary>
        private void SetViewShowMode()
        {
            IsEditMode = false;
            IsInserting = false;
            RaisePropertyChanged(nameof(InfoStatus));
            RaisePropertyChanged(nameof(InfoDataCode));
            RaisePropertyChanged(nameof(InfoDataDescription));
        }
    }
}
