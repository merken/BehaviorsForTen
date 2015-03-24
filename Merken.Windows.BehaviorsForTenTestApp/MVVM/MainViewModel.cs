using Merken.Windev.BehaviorsForTenTestApp.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Merken.Windev.BehaviorsForTenTestApp.MVVM
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            ClickCommand = new RelayCommand((o) =>
            {
                Clicked = DateTime.Now.ToString();
            });
        }

        private string clicked;
        public string Clicked
        {
            get
            {
                return clicked;
            }
            set
            {
                clicked = value;
                RaisePropertyChanged();
            }
        }

        public ICommand ClickCommand
        {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
