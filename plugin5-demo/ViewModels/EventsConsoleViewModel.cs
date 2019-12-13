using Aliquo.Windows.Base;
using System.Windows.Input;

namespace plugin5_demo.ViewModels
{
    class EventsConsoleViewModel : ViewModelBase
    {

        private string text;

        private int indentLevel;

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


        public EventsConsoleViewModel()
        {
            this.commandClear = new DelegateCommand(() => Clear());
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
