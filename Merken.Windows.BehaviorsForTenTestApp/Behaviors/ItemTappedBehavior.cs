using Merken.Windev.BehaviorsForTen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Merken.Windev.BehaviorsForTenTestApp.Behaviors
{
    public class ItemTappedBehavior : BehaviorBase<FrameworkElement>
    {
        protected override void ElementAttached()
        {
            base.ElementAttached();
            AssociatedElement.Tapped += AssociatedElementTapped;
        }

        protected override void ElementDetached()
        {
            AssociatedElement.Tapped -= AssociatedElementTapped;
            base.ElementDetached();
        }

        private void AssociatedElementTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            foreach (IAction action in Actions)
                action.Execute(this, null);
        }

        public ActionCollection Actions
        {
            get { return (ActionCollection)GetValue(ActionsProperty); }
            set { SetValue(ActionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Actions.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActionsProperty =
            DependencyProperty.Register("Actions", typeof(ActionCollection), typeof(ItemTappedBehavior), new PropertyMetadata(new ActionCollection()));
    }
}
