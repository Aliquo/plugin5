using Aliquo.Windows;
using Aliquo.Windows.Extensibility;
using System;
using System.ComponentModel.Composition;

namespace plugin5_demo.Commands
{

    // This is a example to create a process in the start of Aliquo
    [Export(typeof(ViewEvents))]
    [ViewEventsMetadata(ViewType = ViewType.Home)]
    class CommandHome : ViewEvents
    {

        public CommandHome()
        {
            Loaded += ViewEvents_Loaded;
        }

        private void ViewEvents_Loaded(object sender, EventArgs e)
        {

            IHost host = ((IView)sender).GetHost();

            // Get name of the company
            var NameCompany = host.Environment.Company;

        }
    }
}
