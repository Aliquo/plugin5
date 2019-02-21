using Aliquo.Windows;
using Aliquo.Windows.Extensibility;
using System.ComponentModel.Composition;

namespace plugin5_demo.Commands
{
    [Export(typeof(Command))]
    [ProcessMetadata(CategoryProcess = ProcessCategory.Tools,
                     CodeProcess = "_PLG_DEMOCOMPANYINFO",
                     DescriptionProcess = "Shows information of the current session",
                     NameProcess = "Company information")]
    [CommandMenuMetadata(MenuText = "Company information",
                         MainPageItemType = MainPageItemType.Menu,
                         Process = "_PLG_DEMOCOMPANYINFO")]
    class CommandMenuCompanyEnvironment : Command
    {

        public CommandMenuCompanyEnvironment()
        {
            Execute += Command_Execute;
        }

        private void Command_Execute(IHost sender, ExecuteEventArgs e)
        {

            // The environment information is collected
            IEnvironment environment = sender.Environment;

            System.Text.StringBuilder message = new System.Text.StringBuilder();
            message.AppendLine($"Company name [{environment.IdCompany}]: {environment.Company}");
            message.AppendLine($"Current user [{environment.IdUser}]: {environment.User}");
            message.AppendLine($"Current group [{environment.IdGroup}]: {environment.Group}");

            Aliquo.Core.ModulesAliquo companyModules = new Aliquo.Core.ModulesAliquo();
            companyModules.Value = environment.CompanyModules;

            Aliquo.Core.ModulesAliquo userModules = new Aliquo.Core.ModulesAliquo();
            userModules.Value = environment.UserModules;

            message.AppendLine($"It has the store module the company [{companyModules.StoresModuleAvailable}] and the user [{userModules.StoresModuleAvailable}]");

            Message.Show(message.ToString(), environment.Company, MessageButton.OK, MessageImage.Information);

        }
    }
}
