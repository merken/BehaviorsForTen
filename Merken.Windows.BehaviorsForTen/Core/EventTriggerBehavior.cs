using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Merken.Windev.BehaviorsForTen.Core
{
    [ContentProperty(Name = "Actions")]
    public sealed class EventTriggerBehavior : DependencyObject, IBehavior
    {
        public static readonly DependencyProperty ActionsProperty = DependencyProperty.Register("Actions", typeof(ActionCollection), typeof(EventTriggerBehavior), new PropertyMetadata(null));
        public static readonly DependencyProperty EventNameProperty = DependencyProperty.Register("EventName", typeof(string), typeof(EventTriggerBehavior), new PropertyMetadata("Loaded", new PropertyChangedCallback(EventTriggerBehavior.OnEventNameChanged)));
        public static readonly DependencyProperty SourceObjectProperty = DependencyProperty.Register("SourceObject", typeof(object), typeof(EventTriggerBehavior), new PropertyMetadata(null, new PropertyChangedCallback(EventTriggerBehavior.OnSourceObjectChanged)));
        private DependencyObject associatedObject;
        private object resolvedSource;
        private Delegate eventHandler;
        private bool isLoadedEventRegistered;
        private bool isWindowsRuntimeEvent;
        public ActionCollection Actions
        {
            get
            {
                ActionCollection actionCollection = (ActionCollection)base.GetValue(EventTriggerBehavior.ActionsProperty);
                if (actionCollection == null)
                {
                    actionCollection = new ActionCollection();
                    base.SetValue(EventTriggerBehavior.ActionsProperty, actionCollection);
                }
                return actionCollection;
            }
        }
        public string EventName
        {
            get
            {
                return (string)base.GetValue(EventTriggerBehavior.EventNameProperty);
            }
            set
            {
                base.SetValue(EventTriggerBehavior.EventNameProperty, value);
            }
        }
        public object SourceObject
        {
            get
            {
                return base.GetValue(EventTriggerBehavior.SourceObjectProperty);
            }
            set
            {
                base.SetValue(EventTriggerBehavior.SourceObjectProperty, value);
            }
        }
        public DependencyObject AssociatedObject
        {
            get
            {
                return this.associatedObject;
            }
        }
        public void Attach(DependencyObject associatedObject)
        {
            if (associatedObject == this.associatedObject || DesignMode.DesignModeEnabled)
            {
                return;
            }
            if (this.associatedObject != null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "CannotAttachBehaviorMultipleTimesExceptionMessage", new object[]
                {
                    associatedObject,
                    this.associatedObject
                }));
            }
            this.associatedObject = associatedObject;
            this.SetResolvedSource(this.ComputeResolvedSource());
        }
        public void Detach()
        {
            this.SetResolvedSource(null);
            this.associatedObject = null;
        }
        private void SetResolvedSource(object newSource)
        {
            if (this.AssociatedObject == null || this.resolvedSource == newSource)
            {
                return;
            }
            if (this.resolvedSource != null)
            {
                this.UnregisterEvent(this.EventName);
            }
            this.resolvedSource = newSource;
            if (this.resolvedSource != null)
            {
                this.RegisterEvent(this.EventName);
            }
        }
        private object ComputeResolvedSource()
        {
            if (base.ReadLocalValue(EventTriggerBehavior.SourceObjectProperty) != DependencyProperty.UnsetValue)
            {
                return this.SourceObject;
            }
            return this.AssociatedObject;
        }
        private void RegisterEvent(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return;
            }
            if (!(eventName != "Loaded"))
            {
                if (!this.isLoadedEventRegistered)
                {
                    FrameworkElement frameworkElement = this.resolvedSource as FrameworkElement;
                    if (frameworkElement != null && !EventTriggerBehavior.IsElementLoaded(frameworkElement))
                    {
                        this.isLoadedEventRegistered = true;
                        frameworkElement.Loaded += this.OnEvent;
                    }
                }
                return;
            }
            Type type = this.resolvedSource.GetType();
            EventInfo info = type.GetRuntimeEvent(this.EventName);
            if (info == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "CannotFindEventNameExceptionMessage", new object[]
                {
                    this.EventName,
                    type.Name
                }));
            }
            MethodInfo declaredMethod = typeof(EventTriggerBehavior).GetTypeInfo().GetDeclaredMethod("OnEvent");
            this.eventHandler = declaredMethod.CreateDelegate(info.EventHandlerType, this);
            this.isWindowsRuntimeEvent = EventTriggerBehavior.IsWindowsRuntimeType(info.EventHandlerType);
            if (this.isWindowsRuntimeEvent)
            {
                WindowsRuntimeMarshal.AddEventHandler<Delegate>((Delegate add) => (EventRegistrationToken)info.AddMethod.Invoke(this.resolvedSource, new object[]
                {
                    add
                }), delegate (EventRegistrationToken token)
                {
                    info.RemoveMethod.Invoke(this.resolvedSource, new object[]
                    {
                        token
                    });
                }, this.eventHandler);
                return;
            }
            info.AddEventHandler(this.resolvedSource, this.eventHandler);
        }
        private void UnregisterEvent(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return;
            }
            if (!(eventName != "Loaded"))
            {
                if (this.isLoadedEventRegistered)
                {
                    this.isLoadedEventRegistered = false;
                    FrameworkElement frameworkElement = (FrameworkElement)this.resolvedSource;
                    frameworkElement.Loaded -= this.OnEvent;
                }
                return;
            }
            if (this.eventHandler == null)
            {
                return;
            }
            EventInfo info = this.resolvedSource.GetType().GetRuntimeEvent(eventName);
            if (this.isWindowsRuntimeEvent)
            {
                WindowsRuntimeMarshal.RemoveEventHandler<Delegate>(delegate (EventRegistrationToken token)
                {
                    info.RemoveMethod.Invoke(this.resolvedSource, new object[]
                    {
                        token
                    });
                }, this.eventHandler);
            }
            else
            {
                info.RemoveEventHandler(this.resolvedSource, this.eventHandler);
            }
            this.eventHandler = null;
        }
        private void OnEvent(object sender, object eventArgs)
        {
            Interaction.ExecuteActions(this.resolvedSource, this.Actions, eventArgs);
        }
        private static void OnSourceObjectChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            EventTriggerBehavior eventTriggerBehavior = (EventTriggerBehavior)dependencyObject;
            eventTriggerBehavior.SetResolvedSource(eventTriggerBehavior.ComputeResolvedSource());
        }
        private static void OnEventNameChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            EventTriggerBehavior eventTriggerBehavior = (EventTriggerBehavior)dependencyObject;
            if (eventTriggerBehavior.AssociatedObject == null || eventTriggerBehavior.resolvedSource == null)
            {
                return;
            }
            string eventName = (string)args.OldValue;
            string eventName2 = (string)args.NewValue;
            eventTriggerBehavior.UnregisterEvent(eventName);
            eventTriggerBehavior.RegisterEvent(eventName2);
        }
        internal static bool IsElementLoaded(FrameworkElement element)
        {
            if (element == null)
            {
                return false;
            }
            UIElement content = Window.Current.Content;
            DependencyObject parent = element.Parent;
            if (parent == null)
            {
                parent = VisualTreeHelper.GetParent(element);
            }
            return parent != null || (content != null && element == content);
        }
        private static bool IsWindowsRuntimeType(Type type)
        {
            return type != null && type.AssemblyQualifiedName.EndsWith("ContentType=WindowsRuntime", StringComparison.Ordinal);
        }
    }
}
