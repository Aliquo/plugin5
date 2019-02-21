using Aliquo.Core.Exceptions;
using Aliquo.Windows;
using Aliquo.Windows.Extensibility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace plugin5_demo.Commands
{

    [Export(typeof(Command))]
    [CommandItemMetadata(ViewType = ViewType.SalesDeliveryNote,
                         CommandSize = CommandSize.Large,
                         Text = PlugInTitle,
                         Image = "document_email.png")]
    class CommandSendNoteByPDF : Command
    {

        private IHost Host;
        private long idNote;
        private string configEmail;

        private const string PlugInTitle = "Send PDF by E-mail";

        public CommandSendNoteByPDF()
        {
            Execute += Command_Execute;
        }

        private async void Command_Execute(IHost sender, ExecuteEventArgs e)
        {

            this.Host = sender;

            try
            {

                if (e.View.GetCurrentId() != null)
                {
                    idNote = Aliquo.Core.Convert.ValueToInt64(e.View.GetCurrentId());

                    Aliquo.Core.Models.Note note = await this.Host.Documents.GetNoteAsync(idNote);

                    // The assistant is configured
                    System.Text.StringBuilder settings = new System.Text.StringBuilder();
                    settings.AppendFormat("<? NAME='Email' TYPE='STRING' TEXT='E-mail' STYLE='EMAIL' REQUIRED=1>");
                    settings.AppendFormat("<? NAME='Subject' TYPE='STRING' TEXT='Subject' DEFAULT='Delivery note material {0}'>", Aliquo.Core.Formats.SerialAndNumber(note.SerialCode, note.Number));
                    settings.AppendFormat("<? NAME='Message' TYPE='STRING' TEXT='Message' DEFAULT='Enclosed we send you information about the delivery of the {0}.' ROWS=9 LENGTH=2048>", Aliquo.Core.Formats.SerialAndNumber(note.SerialCode, note.Number));

                    ITask task = this.Host.Management.Views.WizardCustom(PlugInTitle, string.Empty, settings.ToString());

                    task.Finishing += ExecuteWizardFinishingAsync;

                    // Check that the parameter is filled
                    configEmail = this.Host.Configuration.GetParameter("EMAIL_SERVER");

                    if (string.IsNullOrEmpty(configEmail))
                    {
                        task.Cancel();
                        Message.Show("Confirm that the configuration of the EMAIL_SERVER parameter is complete.", "EMAIL_SERVER parameter");
                    }

                }

            }
            catch (Exception ex)
            {
                sender.Management.Views.ShowException(ex);
            }

        }

        private async void ExecuteWizardFinishingAsync(object sender, FinishingEventArgs e)
        {
            try
            {

                // The values indicated in the wizard are loaded
                List<Aliquo.Core.Models.DataField> result = (List<Aliquo.Core.Models.DataField>)e.Result;
                string emailTo = Aliquo.Core.Data.FindField(result, "Email").Value.ToString();
                string subject = Aliquo.Core.Data.FindField(result, "Subject").Value.ToString();
                string message = Aliquo.Core.Data.FindField(result, "Message").Value.ToString();

                List<long> listId = new List<long> { idNote };
                string file = await this.Host.Management.CreatePdfDocumentAsync(7, listId);

                // We prepare the sending object of e-mail
                Aliquo.Core.Tools.SendEmail email = new Aliquo.Core.Tools.SendEmail();
                email.AddConfig(configEmail);
                email.ToAdd(emailTo);
                email.Subject = subject;
                email.Body = message;
                email.AttachmentsAdd(file, "order.pdf");

                Exception exceptionResult = null;
                if (email.Send(ref exceptionResult))
                {
                    // We notify the user that the e-mail was sent
                    this.Host.Management.Views.ShowNotification(new Aliquo.Core.Models.Notification
                    {
                        HideStyle = Aliquo.Core.NotificationHideStyle.AutoClose,
                        Title = PlugInTitle,
                        Message = String.Format("The e-mail was sent to {0}", emailTo)
                    });
                }
                else
                {
                    // In case of error it is shown
                    Message.Show(exceptionResult.Message, PlugInTitle, MessageImage.Error);
                }

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
