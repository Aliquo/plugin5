using Aliquo.Core;
using Aliquo.Core.Exceptions;
using Aliquo.Windows;
using Aliquo.Windows.Wizard;
using Aliquo.Windows.Wizard.Controls;
using Aliquo.Windows.Wizard.Link;
using Aliquo.Windows.Wizard.List;
using Aliquo.Windows.Wizard.Styles;
using System;

namespace plugin5_demo.Process
{
    class ProcessWizardExample
    {
        public ProcessWizardExample(IHost host)
        {
            try
            {
                // The assistant is configured
                WizardView wizardView = new WizardView();

                // Different available types 
                WizardStep wizardStep1 = new WizardStep();

                // Different available types 
                // Label
                wizardStep1.AddControl(new WizardLabel()
                {
                    Rows = 4,
                    Text = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industrys standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book."
                });

                // Date
                wizardStep1.AddControl(new WizardDateTime()
                {
                    Name = "Date",
                    Text = "Date",
                    Default = DateTime.Now.ToString(),
                    Style = WizardDateTimeStyle.Date
                });

                // Date and time
                wizardStep1.AddControl(new WizardDateTime()
                {
                    Name = "DateTime",
                    Text = "Date and time",
                    Default = DateTime.Now.ToString(),
                    Style = WizardDateTimeStyle.DateTime
                });

                // Time
                wizardStep1.AddControl(new WizardDateTime()
                {
                    Name = "Time",
                    Text = "Time",
                    Default = DateTime.Now.ToString(),
                    Style = WizardDateTimeStyle.Time
                });

                // Time
                wizardStep1.AddControl(new WizardDateTime()
                {
                    Name = "TimeShort",
                    Text = "Time short",
                    Default = DateTime.Now.ToString(),
                    Style = WizardDateTimeStyle.TimeShort
                });

                // With STEP we mark on which screen we want to show the control
                // StepText is the title of the window
                WizardStep wizardStep2 = new WizardStep { StepText = "Different styles of type string" };

                // String
                wizardStep2.AddControl(new WizardText()
                {
                    Name = "Text",
                    Text = "Text"
                });

                // String (only letters)
                wizardStep2.AddControl(new WizardText()
                {
                    Name = "Letters1",
                    Text = "Letters",
                    Style = WizardTextStyle.Letters
                });

                // String (all except numbers)
                wizardStep2.AddControl(new WizardText()
                {
                    Name = "Letters2",
                    Text = "Letters (with Invalid)",
                    Style = WizardTextStyle.InvalidCharacters("0123456789")
                });

                // String (only numbers)
                wizardStep2.AddControl(new WizardText()
                {
                    Name = "Numbers",
                    Text = "Numbers",
                    Style = WizardTextStyle.ValidCharacters("0123456789")
                });

                // String (email)
                wizardStep2.AddControl(new WizardText()
                {
                    Name = "Email",
                    Text = "Email",
                    Style = WizardTextStyle.Email
                });

                // String (list)
                WizardList wizardTextList = new WizardList()
                {
                    Name = "List",
                    Text = "List",
                    Default = "3"
                };
                wizardTextList.SetList(new WizardValuesList()
                {
                    Items =
                    {
                        new  WizardListItem { Key = '1', Text = "Value 1" },
                        new  WizardListItem { Key = '2', Text = "Value 2" },
                        new  WizardListItem { Key = '3', Text = "Value 3" },
                        new  WizardListItem { Key = '4', Text = "Value 4" },
                        new  WizardListItem { Key = '5', Text = "Value 5" },
                        new  WizardListItem { Key = '6', Text = "Value 6" }
                    }
                });
                wizardStep2.AddControl(wizardTextList);

                // Others uses
                WizardStep wizardStep3 = new WizardStep { StepText = "Others uses of wizard" };
                // DEFAULT
                wizardStep3.AddControl(new WizardText()
                {
                    Name = "TextDefault",
                    Text = "Text (with value)",
                    Default = "Text by default"
                });

                // REQUIRED
                wizardStep3.AddControl(new WizardText()
                {
                    Name = "TextRequired",
                    Text = "Text required",
                    Required = true
                });

                // WIDTH
                wizardStep3.AddControl(new WizardText()
                {
                    Name = "TextWidth",
                    Text = "Text (width)",
                    Width = 100
                });

                // LENGTH
                wizardStep3.AddControl(new WizardText()
                {
                    Name = "TextMaxLength",
                    Text = "Text (maxlength 5)",
                    Length = 5
                });


                // String (AccountingAccount)
                wizardStep3.AddControl(new WizardText()
                {
                    Name = "AccountingAccount1",
                    Text = "AccountingAccount (Always)",
                    Style = WizardTextStyle.AccountingAccount(AccountAutocompleteType.Always)
                });

                // String (AccountingAccount)
                wizardStep3.AddControl(new WizardText()
                {
                    Name = "AccountingAccount1",
                    Text = "AccountingAccount (Never)",
                    Style = WizardTextStyle.AccountingAccount(AccountAutocompleteType.Never)
                });

                // String (AccountingAccount)
                wizardStep3.AddControl(new WizardText()
                {
                    Name = "AccountingAccount1",
                    Text = "AccountingAccount (WithDot)",
                    Style = WizardTextStyle.AccountingAccount(AccountAutocompleteType.WithDot)
                });

                WizardStep wizardStep4 = new WizardStep { StepText = "Others uses of wizard" };

                // Connect with tables
                WizardText wizardTableLink = new WizardText() { Name = "CustomerCode", Text = "Customer" };
                wizardTableLink.SetLink(new WizardTableLink()
                {
                    Table = "Clientes",
                    FieldKey = "Codigo",
                    FieldText = "Nombre",
                    Fields = { "Codigo", "Nombre", "CIF" },
                    Filter = "Clientes.Provincia='Madrid'",
                    Order = "Clientes.Nombre"
                });
                wizardStep4.AddControl(wizardTableLink);
                // List of periodicity rows
                WizardList wizardTableLinkMultiSelect = new WizardList() { Name = "ListTable", Text = "Periodicity", Rows = 5, Multiselect = true };
                var list = new WizardTableList()
                {
                    Table = "Tipos_Periodicidad",
                    FieldKey = "Id",
                    FieldText = "Nombre"
                };
                wizardTableLinkMultiSelect.SetList(list);
                wizardStep4.AddControl(wizardTableLinkMultiSelect);

                WizardStep wizardStep5 = new WizardStep { StepText = "These options show the system dialog boxes" };

                // System dialog boxes
                wizardStep5.AddControl(new WizardOpenFile()
                {
                    Name = "OpenFile",
                    Text = "Open file",
                    DefaultExtension = ".txt",
                    FilterFiles = "Test documents (*.txt)|*.txt|All files (*.*)|*.*"
                });

                wizardStep5.AddControl(new WizardOpenFileServer()
                {
                    FolderType = Aliquo.Core.FilesFolderType.Templates,
                    Name = "OpenFileServer",
                    Text="Open file server",
                    Subfolder = "Notificaciones"
                });

                wizardStep5.AddControl(new WizardSaveFile()
                {
                    Name = "SaveFile",
                    Text = "Save file",
                    DefaultExtension = ".txt",
                    FilterFiles = "Test documents (*.txt)|*.txt|All files (*.*)|*.*"
                });

                wizardStep5.AddControl(new WizardFolderBrowser()
                {
                    Name = "Directory",
                    Text = "Folder browser",
                    Default = @"C:\"
                });

                wizardView.AddStep(wizardStep1);
                wizardView.AddStep(wizardStep2);
                wizardView.AddStep(wizardStep3);
                wizardView.AddStep(wizardStep4);
                wizardView.AddStep(wizardStep5);

                ITask taskFromWizardView = host.Management.Views.WizardCustom("Examples", "Long description on the initial screen of the wizard.\r\n\r\nNormally, the operation or warnings are explained to the user.\r\n\r\nYou can find more information about wizards at https://www.aliquo.software/config-parametros-asistente/", wizardView);

            }
            catch (HandledException ex)
            {
                Message.Show(ex.Message, "ProcessWizardExample", MessageImage.Warning);
            }
            catch (System.Exception ex)
            {
                host.Management.Views.ShowException(ex);
            }
        }
    }
}
