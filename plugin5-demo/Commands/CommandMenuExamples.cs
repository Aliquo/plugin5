using Aliquo.Windows;
using Aliquo.Windows.Extensibility;
using System.ComponentModel.Composition;

namespace plugin5_demo.Commands
{
    // Create a process and an option in the menu. It is important create a process to apply security later in the program
    [Export(typeof(Command))]
    [ProcessMetadata(CategoryProcess = ProcessCategory.Tools,
                     CodeProcess = "_PLG_DEMO",
                     DescriptionProcess = "Show PlugIn examples",
                     NameProcess = "Show PlugIn examples",
                     ModulesProcess = Aliquo.Core.LicencyModules.Basic)]
    [CommandMenuMetadata(MenuText = "Show PlugIn examples",
                         MenuImage = "action.png",
                         Process = "_PLG_DEMO")]
    class CommandMenuExamples : Command
    {

        public CommandMenuExamples()
        {
            Execute += Command_Execute;
        }

        private void Command_Execute(IHost sender, ExecuteEventArgs e)
        {

            // Call the view of code examples
            Views.CodeView examples = new Views.CodeView(sender);

        }
    }
}
