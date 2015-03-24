using Merken.Windev.BehaviorsForTen.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Merken.Windev.BehaviorsForTenTestApp.Actions
{
    public class ShowMessageDialogAction : ActionBase
    {
        public override object Execute(object sender, object parameter)
        {
            MessageDialog dlg = new MessageDialog("TEST");
            dlg.ShowAsync();

            return true;
        }
    }
}
