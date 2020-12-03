using Aliquo.Core.Exceptions;
using Aliquo.Windows;
using System;

namespace plugin5_demo.Process
{
    class ProcessWizardExample
    {
        private IHost Host;

        public ProcessWizardExample(IHost host)
        {
            try
            {
                this.Host = host;

                // The assistant is configured
                System.Text.StringBuilder settings = new System.Text.StringBuilder();

                // Different available types 
                // Date
                settings.AppendFormat("<? NAME='Date' TEXT='Date' TYPE='DATETIME' STYLE='DATE' DEFAULT='{0}'>", DateTime.Now);
                // Date and time
                settings.AppendFormat("<? NAME='DateTime' TEXT='Date and time' TYPE='DATETIME' STYLE='DATETIME' DEFAULT='{0}'>", DateTime.Now);
                // Time
                settings.AppendFormat("<? NAME='Time' TEXT='Time' TYPE='DATETIME' STYLE='TIME' DEFAULT='{0}'>", DateTime.Now);
                // Time
                settings.AppendFormat("<? NAME='TimeShort' TEXT='Time short' TYPE='DATETIME' STYLE='TIMESHORT' DEFAULT='{0}'>", DateTime.Now);

                // With STEP we mark on which screen we want to show the control
                // STEPTEXT is the title of the window

                // String
                settings.Append ("<? NAME='Text' TEXT='Text' TYPE='STRING' STEP=2 STEPTEXT='Different styles of type string'>");
                // String (only letters)
                settings.Append("<? NAME='Letters1' TEXT='Letters' TYPE='STRING' STYLE='LETTERS' STEP=2>");
                // String (all except numbers)
                settings.Append("<? NAME='Letters2' TEXT='Letters (with Invalid)' TYPE='STRING' STYLE='INVALIDCHARACTERS;0123456789' STEP=2>");
                // String (only numbers)
                settings.Append("<? NAME='Numbers' TEXT='Numbers' TYPE='STRING' STYLE='VALIDCHARACTERS;0123456789' STEP=2>");
                // String (email)
                settings.Append("<? NAME='Email' TEXT='Email' TYPE='STRING' STYLE='EMAIL' STEP=2>");
                // String (list)
                settings.Append("<? NAME='List' TEXT='List' TYPE='STRING' LIST='Value 1;1|Value 2;2|Value 3;3|Value 4;4|Value 5;5|Value 6;6' DEFAULT='3' STEP=2>");


                // Others uses
                // DEFAULT
                settings.AppendFormat("<? NAME='TextDefault' TEXT='Text (with value)' TYPE='STRING' DEFAULT='{0}' STEP=3 STEPTEXT='Others uses of wizard'>", "Text by default");
                // REQUIRED
                settings.Append("<? NAME='TextRequired' TEXT='Text required' TYPE='STRING' REQUIRED=1 STEP=3>"); 
                // WIDTH
                settings.Append("<? NAME='TextWidth' TEXT='Text (width)' TYPE='STRING' WIDTH=100 STEP=3>");
                // LENGTH
                settings.Append("<? NAME='TextMaxLength' TEXT='Text (maxlength 5)' TYPE='STRING' LENGTH=5 STEP=3>");

                // Connect with tables           
                settings.Append("<? NAME='CustomerCode' TEXT='Customer' TYPE='STRING' TABLE='Clientes' FIELD='Codigo' FIELDTEXT='Nombre' STEP=4>");
                // Connect with filter           
                settings.Append("<? NAME='CustomerCode' TEXT='Customer (only of Madrid)' TYPE='STRING' TABLE='Clientes' FIELD='Codigo' FIELDTEXT='Nombre' FILTER=\"Clientes.CodigoPostal LIKE '28%'\" STEP=4>");
                // List of periodicity rows
                settings.Append("<? NAME='ListTable' TEXT='Periodicity' TYPE='STRING' TABLE='Tipos_Periodicidad' LIST='Nombre;Id' ROWS=5 LISTMULTISELECT=1 STEP=4>");


                // System dialog boxes
                settings.Append("<? NAME='OpenFile' TEXT='Open file' TYPE='STRING' STYLE='OPENFILE;Test documents (*.txt)|*.txt|All files (*.*)|*.*' STEP=5 STEPTEXT='These options show the system dialog boxes'>");
                settings.Append("<? NAME='SaveFile' TEXT='Save file' TYPE='STRING' STYLE='SAVEFILE;Test documents (*.txt)|*.txt|All files (*.*)|*.*' STEP=5>");
                settings.Append("<? NAME='Directory' TEXT='Folder browser' TYPE='STRING' STYLE='FOLDERBROWSER' STEP=5>");

                ITask task = host.Management.Views.WizardCustom("Examples", "Long description on the initial screen of the wizard.\r\n\r\nNormally, the operation or warnings are explained to the user.\r\n\r\nYou can find more information about wizards at https://www.aliquo.software/config-parametros-asistente/", settings.ToString());

                // You can find more information about wizards at https://www.aliquo.software/config-parametros-asistente/

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
