using Aliquo.Core.Exceptions;
using Aliquo.Windows;
using System;
using System.Collections.Generic;
using System.Linq;

namespace plugin5_demo.Process
{
    class WizardWithEvents
    {

        private const string PlugInTitle = "Wizard example";

        public WizardWithEvents(IHost sender)
        {

            try
            {
                // The assistant is configured
                var wizard = new Aliquo.Windows.Wizard.WizardView();
                var wizardStep = new Aliquo.Windows.Wizard.WizardStep { StepText = "Uses the TextChanged event to copy the text to another control" };

                wizardStep.AddControl(new Aliquo.Windows.Wizard.Controls.WizardText()
                {
                    Name = "SourceText",
                    Text = "Source text"
                });

                wizardStep.AddControl(new Aliquo.Windows.Wizard.Controls.WizardText()
                {
                    Name = "TargetText",
                    Text = "Target text",
                    ReadOnly = true
                });

                wizard.AddStep(wizardStep);

                // The reference to the ControlAddedEvent event is added                
                ITask task = sender.Management.Views.WizardCustom(PlugInTitle, string.Empty, wizard, controlAdded: ControlAddedEvent);

            }
            catch (HandledException ex)
            {
                Message.Show(ex.Message, "WizardWithEvents", MessageImage.Warning);
            }
            catch (System.Exception ex)
            {
                sender.Management.Views.ShowException(ex);
            }

        }

        private Dictionary<string, System.Windows.Forms.Control> controls = new Dictionary<string, System.Windows.Forms.Control>();

        /// <summary>
        /// Collect the creation of each assistant control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlAddedEvent(object sender, ControlAddedEventArgs e)
        {

            switch (e.Settings?["Name"].Value?.ToString().ToLower())
            {
                case "sourcetext":
                    // The TextChanged event is assigned
                    (e.Control as System.Windows.Forms.Control).TextChanged += Step1_TextChanged;

                    // It is added to the collection of controls
                    this.controls.Add(e.Settings?["Name"].Value?.ToString(), (System.Windows.Forms.Control)e.Control);
                    break;

                case "targettext":
                    // It is added to the collection of controls
                    this.controls.Add(e.Settings?["Name"].Value?.ToString(), (System.Windows.Forms.Control)e.Control);
                    break;

            }

        }

        private void Step1_TextChanged(object sender, EventArgs e)
        {

            System.Windows.Forms.Control control = (sender as System.Windows.Forms.Control);

            if (!String.IsNullOrWhiteSpace(control.Text))
            {
                // The control is searched in the list
                System.Windows.Forms.Control controlSourceText = this.controls.FirstOrDefault(t => t.Key == "SourceText").Value;
                System.Windows.Forms.Control controlTargetText = this.controls.FirstOrDefault(t => t.Key == "TargetText").Value;

                // The value of the property is assigned
                controlTargetText.Text = controlSourceText.Text;

            }
        }

    }
}
