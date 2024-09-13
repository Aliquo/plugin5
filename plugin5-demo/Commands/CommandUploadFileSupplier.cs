using Aliquo.Core.Exceptions;
using Aliquo.Windows;
using Aliquo.Windows.Extensibility;
using Aliquo.Windows.Wizard.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace plugin5_demo.Commands
{

    // This is an example to upload a file to the file manager associated with a specific register.
    [Export(typeof(Command))]
    [CommandItemMetadata(ViewType = ViewType.Table,
                         ViewKey = "Proveedores",
                         CommandSize = CommandSize.Large,
                         CommandType = CommandType.QuickAction,
                         Text = PlugInTitle,
                         Image = "document_import.png")]
    class CommandUploadFileSupplier : Command
    {
        private IHost Host;
        private long idSupplier;

        private const string PlugInTitle = "Upload file";

        public CommandUploadFileSupplier()
        {
            Execute += Command_Execute;
        }

        private void Command_Execute(IHost sender, ExecuteEventArgs e)
        {

            this.Host = sender;

            try
            {

                if (e.View.GetCurrentId() != null)
                {
                    idSupplier = Aliquo.Core.Convert.ValueToInt64(e.View.GetCurrentId());

                    // The assistant is configured
                    var wizard = new Aliquo.Windows.Wizard.WizardView();
                    var wizardStep = new Aliquo.Windows.Wizard.WizardStep();

                    wizardStep.AddControl(new WizardOpenFile()
                    {
                        Name = "UploadFile",
                        Text = "Upload file",
                        DefaultExtension = ".txt",
                        FilterFiles = "Text documents (*.txt)|*.txt|All files (*.*)|*.*"
                    });

                    wizard.AddStep(wizardStep);

                    ITask task = this.Host.Management.Views.WizardCustom(PlugInTitle, string.Empty, wizard);

                    task.Finishing += ExecuteWizardFinishing;
                }

            }
            catch (HandledException ex)
            {
                Message.Show(ex.Message, "CommandUploadFileSupplier", MessageImage.Warning);
            }
            catch (System.Exception ex)
            {
                sender.Management.Views.ShowException(ex);
            }

        }

        private void ExecuteWizardFinishing(object sender, FinishingEventArgs e)
        {

            try
            {

                // The values indicated in the wizard are loaded
                List<Aliquo.Core.Models.DataField> result = (List<Aliquo.Core.Models.DataField>)e.Result;
                string fileName = Aliquo.Core.Data.FindField(result, "UploadFile").Value.ToString();

                // The object for file storage management is created
                ITaskStorage task = this.Host.Management.FilesStorage();

                // An Event is created to indicate when the file upload has finished
                task.Finishing += UploadFileFinishing;

                // The necessary information is filled in to place the file in the indicated record.
                Aliquo.Core.Models.TableFileProperties fileProperties = new Aliquo.Core.Models.TableFileProperties
                {
                    Table = "Proveedores",
                    IdData = idSupplier
                };

                task.UploadTableFile(fileName, fileProperties);

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

        private void UploadFileFinishing(object sender, FinishingEventArgs e)
        {
            Aliquo.Core.Models.TableFileProperties result = (Aliquo.Core.Models.TableFileProperties)e.Result;

            Message.Show($"The {result.FileInfo.Name} file has been uploaded to record {idSupplier} of the Proveedores table.", "CommandUploadFileSupplier", MessageImage.Information);
        }
    }
}
