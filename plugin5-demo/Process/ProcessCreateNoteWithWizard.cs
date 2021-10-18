using Aliquo.Core.Exceptions;
using Aliquo.Windows;
using System;
using System.Collections.Generic;

namespace plugin5_demo.Process
{
    class ProcessCreateNoteWithWizard
    {

        private IHost Host;

        public void ShowWizard(IHost host)
        {
            try
            {
                this.Host = host;

                // The assistant is configured
                var wizard = new Aliquo.Windows.Wizard.WizardView();
                var wizardStep = new Aliquo.Windows.Wizard.WizardStep();

                Aliquo.Windows.Wizard.Controls.WizardText wizardTableClientes = new Aliquo.Windows.Wizard.Controls.WizardText()
                {
                    Name = "CustomerCode",
                    Text = "Customer",
                    Width = 100,
                    Required = true
                };
                wizardTableClientes.SetLink(new Aliquo.Windows.Wizard.Link.WizardTableLink()
                {
                    Table = "Clientes",
                    FieldKey = "Codigo",
                    FieldText = "Nombre"
                });
                wizardStep.AddControl(wizardTableClientes);

                wizardStep.AddControl(new Aliquo.Windows.Wizard.Controls.WizardDateTime()
                {
                    Name = "DateNote",
                    Text = "Date",
                    Default = this.Host.Environment.WorkDate,
                    Width = 100,
                    Required = true
                });

                Aliquo.Windows.Wizard.Controls.WizardText wizardTableArticulos = new Aliquo.Windows.Wizard.Controls.WizardText()
                {
                    Name = "ProductCode",
                    Text = "Product",
                    Width = 100,
                    Required = true
                };
                wizardTableArticulos.SetLink(new Aliquo.Windows.Wizard.Link.WizardTableLink()
                {
                    Table = "Articulos",
                    FieldKey = "Codigo",
                    FieldText = "Nombre"
                });
                wizardStep.AddControl(wizardTableArticulos);

                wizard.AddStep(wizardStep);

                ITask task = host.Management.Views.WizardCustom("Create new document with wizard", string.Empty, wizard);

                task.Finishing += ExecuteWizardFinishingAsync;

            }
            catch (HandledException ex)
            {
                Message.Show(ex.Message, "ProcessCreateNoteWithWizard", MessageImage.Warning);
            }
            catch (System.Exception ex)
            {
                host.Management.Views.ShowException(ex);
            }
        }

        private async void ExecuteWizardFinishingAsync(object sender, FinishingEventArgs e)
        {
            try
            {

                // The values indicated in the wizard are loaded
                List<Aliquo.Core.Models.DataField> result = (List<Aliquo.Core.Models.DataField>)e.Result;
                string customerCode = Aliquo.Core.Data.FindField(result, "CustomerCode").Value.ToString();
                DateTime dateNote = Aliquo.Core.Convert.ValueToDate(Aliquo.Core.Data.FindField(result, "DateNote").Value);
                string productCode = Aliquo.Core.Data.FindField(result, "ProductCode").Value.ToString();

                // We create the model to store the data
                Aliquo.Core.Models.Note note = new Aliquo.Core.Models.Note();

                // The collection starts
                note.Lines = new List<Aliquo.Core.Models.Line>();

                // This is the basic information to create a note
                note.Type = Aliquo.Core.NoteType.SalesOrder;
                note.PropertyCode = customerCode;
                note.Date = dateNote;

                // Get info product
                Aliquo.Core.Models.RateQuery rateQuery = new Aliquo.Core.Models.RateQuery();
                rateQuery.Type = note.Type;
                rateQuery.Date = note.Date;
                rateQuery.PropertyCode = note.PropertyCode;

                rateQuery.ProductCode = productCode;
                rateQuery.Quantity = 4;

                List<Aliquo.Core.Models.InfoProduct> listOfInfoProduct = await Host.Documents.GetRatesQueryAsync(rateQuery);

                foreach (Aliquo.Core.Models.InfoProduct lineInfoProduct in listOfInfoProduct)
                {
                    // Prepare the line of note
                    Aliquo.Core.Models.Line line = new Aliquo.Core.Models.Line
                    {
                        Type = Aliquo.Core.LineType.Product,
                        Code = lineInfoProduct.Code,
                        Name = lineInfoProduct.Name,
                        Quantity = lineInfoProduct.Quantity,
                        Price = lineInfoProduct.Price,
                        CodeVAT = lineInfoProduct.CodeVAT,
                        PercentDiscount = lineInfoProduct.PercentDiscount
                    };

                    // The line is assigned to the model list
                    note.Lines.Add(line);
                }

                // Call the function to create the note                
                long id = await Host.Documents.SetNoteAsync(note);

                // The created note is displayed
                Host.Documents.Views.ShowNote(id);

            }
            catch (HandledException ex)
            {
                Message.Show(ex.Message, "ProcessCreateNoteWithWizard_ExecuteWizardFinishingAsync", MessageImage.Warning);
            }
            catch (System.Exception ex)
            {
                Host.Management.Views.ShowException(ex);
            }

        }

    }
}
