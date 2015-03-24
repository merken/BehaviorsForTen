using Merken.Windev.BehaviorsForTen.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Merken.Windev.BehaviorsForTen
{
    /// <summary>
    /// This is a base implementation for custom behaviors
    /// All of the plumbing has already been setup by this base class.
    /// To create a custom behavior, please inherit from this class.
    /// If you are planning on using DependencyObjectCollection as a dependency property on your custom behavior,
    /// Please re-instantiate the DependencyObjectCollection in a parameterless CTOR of your custom behavion.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BehaviorBase<T> : DependencyObject, IBehavior
        where T : FrameworkElement
    {
        private T element;

        /// <summary
        /// !Important!
        /// Whenever you define Dependency Properties on Behaviors (or any DependencyObject for that matter), you need to set a private static function to act on PropertyMetadata changed.
        /// Since this is static, this can be executed multiple times from DependencyObjects that are not part of the main UI thread anymore
        /// So whenever you want to change something on the UI thread or push changes of bindingexpressions to the source, you need to surround it with an if-check:
        /// if(IsActive){
        /// //your UI and bindingexpression code here
        /// }
        /// </summary>
        private bool isActive = false;

        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
            }
        }

        public T AssociatedElement { get { return element; } }

        public DependencyObject AssociatedObject { get { return element; } }

        public void Attach(DependencyObject associatedObject)
        {
            if (associatedObject as T == null)
                throw new NotSupportedException("The associated object must be of type " + typeof(T).Name);
            element = associatedObject as T;
            ElementAttached();
        }

        /// <summary>
        /// Override this method if you wish to handle the Attached event.
        /// Call the base implementation first, this will ensure that the AssociatedElement has been set.
        /// </summary>
        protected virtual void ElementAttached()
        {
            element.Loaded += ElementLoaded;
            isActive = true;
        }

        /// <summary>
        /// You can optionally override this method to capture the loading event of the AssociatedElement.
        /// </summary>
        protected virtual void ElementLoaded(object sender, RoutedEventArgs e)
        {
            var page = ControlHelper.GetParentPage(element);
            if (page != null && page.NavigationCacheMode == NavigationCacheMode.Disabled)//Subcribe to the unloading event to handle state
                page.Unloaded += PageUnloaded;
        }

        private void PageUnloaded(object sender, RoutedEventArgs e)
        {
            this.Detach();
        }

        public void Detach()
        {
            ElementDetached();
            isActive = false;
        }

        /// <summary>
        /// Override this method if you wish to handle the detachment of the AssociatedElement.
        /// Call base.ElementDetached() as the last line of the override.
        /// </summary>
        protected virtual void ElementDetached()
        {
            var page = ControlHelper.GetParentPage(element);
            if (page != null && page.NavigationCacheMode == NavigationCacheMode.Disabled)//Subcribe to the unloading event to handle state
                page.Unloaded -= PageUnloaded;
            element.Loaded -= ElementLoaded;
        }
    }
}
