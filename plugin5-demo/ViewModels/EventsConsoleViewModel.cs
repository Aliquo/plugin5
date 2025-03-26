using Aliquo.Core;
using Aliquo.Windows;
using Aliquo.Windows.Base;
using System.Windows.Input;

namespace plugin5_demo.ViewModels
{
    class EventsConsoleViewModel : ViewModelBase
    {

        private IHost host;

        private string text;

        private int indentLevel;

        private IWindowView window;
        private DelegateCommand commandClear;

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                this.RaisePropertyChanged();
            }
        }

        public int IndentLevel
        {
            get
            {
                return indentLevel;
            }
            set
            {
                if (value < 0)
                    value = 0;

                indentLevel = value;
                this.RaisePropertyChanged();
            }
        }

        public ICommand CommandClear
        {
            get
            {
                return this.commandClear;
            }
        }

        public IWindowView Window
        {
            get
            {
                return this.window;
            }
        }


        public EventsConsoleViewModel(IHost host)
        {
            this.host = host;
            this.host.Events.TableUpdated += TableUpdated;

            this.commandClear = new DelegateCommand(() => Clear());

            this.window = host.Management.Views.CreateWindowView("Console host events");
            this.window.Closed += Window_Closed;
        }
     
        /// <summary>Event that occurs when a table is updated</summary>
        private void TableUpdated(object sender, TableUpdatedEventArgs e)
        {
            Append($"TableUpdated (Table={e.Table}, Id={e.Id}, Type={e.Type.ToString()}, Reference={e.Reference})");
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            this.host.Events.TableUpdated -= TableUpdated;
        }


        public void Indent()
        {
            this.IndentLevel += 1;
        }

        public void Unindent()
        {
            this.IndentLevel -= 1;
        }

        public void Clear()
        {
            this.Text = null;
        }

        public void Append(string text = "")
        {
            this.Text += $"{System.DateTime.Now}:   {new string(System.Convert.ToChar(" "), this.IndentLevel * 4)}{text}\n";
        }
    }
}
