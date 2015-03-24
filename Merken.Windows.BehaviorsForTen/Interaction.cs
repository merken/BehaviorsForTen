using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace Merken.Windev.BehaviorsForTen
{
    public sealed class Interaction
    {
        public static readonly DependencyProperty BehaviorsProperty = DependencyProperty.RegisterAttached("Behaviors", typeof(BehaviorCollection), typeof(Interaction), new PropertyMetadata(null, new PropertyChangedCallback(Interaction.OnBehaviorsChanged)));
        private Interaction()
        {
        }
        public static BehaviorCollection GetBehaviors(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            BehaviorCollection behaviorCollection = (BehaviorCollection)obj.GetValue(Interaction.BehaviorsProperty);
            if (behaviorCollection == null)
            {
                behaviorCollection = new BehaviorCollection();
                obj.SetValue(Interaction.BehaviorsProperty, behaviorCollection);
            }
            return behaviorCollection;
        }
        public static void SetBehaviors(DependencyObject obj, BehaviorCollection value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            obj.SetValue(Interaction.BehaviorsProperty, value);
        }
        public static IEnumerable<object> ExecuteActions(object sender, ActionCollection actions, object parameter)
        {
            List<object> list = new List<object>();
            if (actions == null || DesignMode.DesignModeEnabled)
            {
                return list;
            }
            foreach (IAction action in actions)
                list.Add(action.Execute(sender, parameter));
            return list;
        }
        private static void OnBehaviorsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            BehaviorCollection behaviorCollection = (BehaviorCollection)args.OldValue;
            BehaviorCollection behaviorCollection2 = (BehaviorCollection)args.NewValue;
            if (behaviorCollection == behaviorCollection2)
            {
                return;
            }
            if (behaviorCollection != null && behaviorCollection.AssociatedObject != null)
            {
                behaviorCollection.Detach();
            }
            if (behaviorCollection2 != null && sender != null)
            {
                behaviorCollection2.Attach(sender);
            }
        }
    }
}