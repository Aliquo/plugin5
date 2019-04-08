using Aliquo.Windows;

namespace plugin5_demo.Process
{
    class CommandMenuCompanyEnvironment
    {

        public CommandMenuCompanyEnvironment(IHost sender)
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
