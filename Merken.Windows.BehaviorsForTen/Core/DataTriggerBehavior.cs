using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace Merken.Windev.BehaviorsForTen.Core
{
    [ContentProperty(Name = "Actions")]
    public sealed class DataTriggerBehavior : DependencyObject, IBehavior
    {
        public static readonly DependencyProperty ActionsProperty = DependencyProperty.Register("Actions", typeof(ActionCollection), typeof(DataTriggerBehavior), new PropertyMetadata(null));
        public static readonly DependencyProperty BindingProperty = DependencyProperty.Register("Binding", typeof(object), typeof(DataTriggerBehavior), new PropertyMetadata(null, new PropertyChangedCallback(DataTriggerBehavior.OnValueChanged)));
        public static readonly DependencyProperty ComparisonConditionProperty = DependencyProperty.Register("ComparisonCondition", typeof(ComparisonConditionType), typeof(DataTriggerBehavior), new PropertyMetadata(ComparisonConditionType.Equal, new PropertyChangedCallback(DataTriggerBehavior.OnValueChanged)));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(DataTriggerBehavior), new PropertyMetadata(null, new PropertyChangedCallback(DataTriggerBehavior.OnValueChanged)));
        private DependencyObject associatedObject;
        public ActionCollection Actions
        {
            get
            {
                ActionCollection actionCollection = (ActionCollection)base.GetValue(DataTriggerBehavior.ActionsProperty);
                if (actionCollection == null)
                {
                    actionCollection = new ActionCollection();
                    base.SetValue(DataTriggerBehavior.ActionsProperty, actionCollection);
                }
                return actionCollection;
            }
        }
        public object Binding
        {
            get
            {
                return base.GetValue(DataTriggerBehavior.BindingProperty);
            }
            set
            {
                base.SetValue(DataTriggerBehavior.BindingProperty, value);
            }
        }
        public ComparisonConditionType ComparisonCondition
        {
            get
            {
                return (ComparisonConditionType)base.GetValue(DataTriggerBehavior.ComparisonConditionProperty);
            }
            set
            {
                base.SetValue(DataTriggerBehavior.ComparisonConditionProperty, value);
            }
        }
        public object Value
        {
            get
            {
                return base.GetValue(DataTriggerBehavior.ValueProperty);
            }
            set
            {
                base.SetValue(DataTriggerBehavior.ValueProperty, value);
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
        }
        public void Detach()
        {
            this.associatedObject = null;
        }
        private static bool Compare(object leftOperand, ComparisonConditionType operatorType, object rightOperand)
        {
            if (leftOperand != null && rightOperand != null)
            {
                rightOperand = TypeConverterHelper.Convert(rightOperand.ToString(), leftOperand.GetType().FullName);
            }
            IComparable comparable = leftOperand as IComparable;
            IComparable comparable2 = rightOperand as IComparable;
            if (comparable != null && comparable2 != null)
            {
                return DataTriggerBehavior.EvaluateComparable(comparable, operatorType, comparable2);
            }
            switch (operatorType)
            {
                case ComparisonConditionType.Equal:
                    return object.Equals(leftOperand, rightOperand);
                case ComparisonConditionType.NotEqual:
                    return !object.Equals(leftOperand, rightOperand);
                case ComparisonConditionType.LessThan:
                case ComparisonConditionType.LessThanOrEqual:
                case ComparisonConditionType.GreaterThan:
                case ComparisonConditionType.GreaterThanOrEqual:
                    if (comparable == null && comparable2 == null)
                    {
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "InvalidOperands", new object[]
                        {
                        (leftOperand != null) ? leftOperand.GetType().Name : "null",
                        (rightOperand != null) ? rightOperand.GetType().Name : "null",
                        operatorType.ToString()
                        }));
                    }
                    if (comparable == null)
                    {
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "InvalidLeftOperand", new object[]
                        {
                        (leftOperand != null) ? leftOperand.GetType().Name : "null",
                        operatorType.ToString()
                        }));
                    }
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "InvalidRightOperand", new object[]
                    {
                    (rightOperand != null) ? rightOperand.GetType().Name : "null",
                    operatorType.ToString()
                    }));
                default:
                    return false;
            }
        }
        private static bool EvaluateComparable(IComparable leftOperand, ComparisonConditionType operatorType, IComparable rightOperand)
        {
            object obj = null;
            try
            {
                obj = Convert.ChangeType(rightOperand, leftOperand.GetType(), CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
            }
            catch (InvalidCastException)
            {
            }
            if (obj == null)
            {
                return operatorType == ComparisonConditionType.NotEqual;
            }
            int num = leftOperand.CompareTo((IComparable)obj);
            switch (operatorType)
            {
                case ComparisonConditionType.Equal:
                    return num == 0;
                case ComparisonConditionType.NotEqual:
                    return num != 0;
                case ComparisonConditionType.LessThan:
                    return num < 0;
                case ComparisonConditionType.LessThanOrEqual:
                    return num <= 0;
                case ComparisonConditionType.GreaterThan:
                    return num > 0;
                case ComparisonConditionType.GreaterThanOrEqual:
                    return num >= 0;
                default:
                    return false;
            }
        }
        private static void OnValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            DataTriggerBehavior dataTriggerBehavior = (DataTriggerBehavior)dependencyObject;
            if (dataTriggerBehavior.AssociatedObject == null)
            {
                return;
            }
            DataBindingHelper.RefreshDataBindingsOnActions(dataTriggerBehavior.Actions);
            if (DataTriggerBehavior.Compare(dataTriggerBehavior.Binding, dataTriggerBehavior.ComparisonCondition, dataTriggerBehavior.Value))
            {
                Interaction.ExecuteActions(dataTriggerBehavior.AssociatedObject, dataTriggerBehavior.Actions, args);
            }
        }
    }
}
