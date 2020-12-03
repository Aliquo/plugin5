using Aliquo.Core.Exceptions;
using Aliquo.Windows;
using Aliquo.Windows.Extensibility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace plugin5_demo.Commands
{

    // This is a example to create a button in a subprocess call SalesDeliveryNote, the table Notas have subprocess for especific types.
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
                    var wizard = new Aliquo.Windows.Wizard.WizardView();
                    var wizardStep = new Aliquo.Windows.Wizard.WizardStep();

                    wizardStep.AddControl(new Aliquo.Windows.Wizard.Controls.WizardText()
                    {
                        Name = "Email",
                        Text = "E-mail",
                        Required = true,
                        Style = Aliquo.Windows.Wizard.Styles.WizardTextStyle.Email
                    });

                    wizardStep.AddControl(new Aliquo.Windows.Wizard.Controls.WizardText()
                    {
                        Name = "Subject",
                        Text = "Subject",
                        Default = $"Delivery note material {Aliquo.Core.Formats.SerialAndNumber(note.SerialCode, note.Number)}"
                    });

                    wizardStep.AddControl(new Aliquo.Windows.Wizard.Controls.WizardText()
                    {
                        Name = "Message",
                        Text = "Message",
                        Default = $"Enclosed we send you information about the delivery of the {Aliquo.Core.Formats.SerialAndNumber(note.SerialCode, note.Number)}.",
                        Rows = 9,
                        Length = 2048
                    });

                    wizard.AddStep(wizardStep);

                    ITask task = this.Host.Management.Views.WizardCustom(PlugInTitle, string.Empty, wizard);

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
            catch (HandledException ex)
            {
                Message.Show(ex.Message, "CommandSendNoteByPDF", MessageImage.Warning);
            }
            catch (System.Exception ex)
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
