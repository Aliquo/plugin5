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
                System.Text.StringBuilder settings = new System.Text.StringBuilder();
                settings.AppendFormat("<? NAME='CustomerCode' TYPE='STRING' TEXT='Customer' WIDTH=100 TABLE='Clientes' FIELD='Codigo' FIELDTEXT='Nombre' REQUIRED=1>");
                settings.AppendFormat("<? NAME='DateNote' TYPE='DATE' TEXT='Date' WIDTH=100 REQUIRED=1 DEFAULT='{0}'>", DateTime.Now);
                settings.AppendFormat("<? NAME='ProductCode' TYPE='STRING' TEXT='Product' WIDTH=100 TABLE='Articulos' FIELD='Codigo' FIELDTEXT='Nombre' REQUIRED=1>");

                ITask task = host.Management.Views.WizardCustom("Create new document with wizard", string.Empty, settings.ToString());

                task.Finishing += ExecuteWizardFinishingAsync;

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

                Aliquo.Core.Models.Line line = new Aliquo.Core.Models.Line
                {
                    Type = Aliquo.Core.LineType.Product,
                    Code = productCode,
                    Quantity = 4
                };

                // The line is assigned to the model list
                note.Lines.Add(line);

                // Call the function to create the note                
                long id = await Host.Documents.SetNoteAsync(note);

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
