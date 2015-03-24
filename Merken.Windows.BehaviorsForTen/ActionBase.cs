using Merken.Windev.BehaviorsForTen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Merken.Windev.BehaviorsForTen.Actions
{
    /// <summary>
    /// Please use this as a base class for your custom action
    /// If you are planning on using DependencyObjectCollection as a dependency property on your custom action,
    /// Please re-instantiate the DependecyCollection in a parameterless CTOR of your custom action.
    /// </summary>
    public abstract class ActionBase : DependencyObject, IAction
    {
        public abstract object Execute(object sender, object parameter);
    }
}
