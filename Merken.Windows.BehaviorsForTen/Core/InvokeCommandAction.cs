using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Merken.Windev.BehaviorsForTen.Core
{
    public sealed class InvokeCommandAction : DependencyObject, IAction
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(InvokeCommandAction), new PropertyMetadata(null));
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(InvokeCommandAction), new PropertyMetadata(null));
        public static readonly DependencyProperty InputConverterProperty = DependencyProperty.Register("InputConverter", typeof(IValueConverter), typeof(InvokeCommandAction), new PropertyMetadata(null));
        public static readonly DependencyProperty InputConverterParameterProperty = DependencyProperty.Register("InputConverterParameter", typeof(object), typeof(InvokeCommandAction), new PropertyMetadata(null));
        public static readonly DependencyProperty InputConverterLanguageProperty = DependencyProperty.Register("InputConverterLanguage", typeof(string), typeof(InvokeCommandAction), new PropertyMetadata(string.Empty));
        public ICommand Command
        {
            get
            {
                return (ICommand)base.GetValue(InvokeCommandAction.CommandProperty);
            }
            set
            {
                base.SetValue(InvokeCommandAction.CommandProperty, value);
            }
        }
        public object CommandParameter
        {
            get
            {
                return base.GetValue(InvokeCommandAction.CommandParameterProperty);
            }
            set
            {
                base.SetValue(InvokeCommandAction.CommandParameterProperty, value);
            }
        }
        public IValueConverter InputConverter
        {
            get
            {
                return (IValueConverter)base.GetValue(InvokeCommandAction.InputConverterProperty);
            }
            set
            {
                base.SetValue(InvokeCommandAction.InputConverterProperty, value);
            }
        }
        public object InputConverterParameter
        {
            get
            {
                return base.GetValue(InvokeCommandAction.InputConverterParameterProperty);
            }
            set
            {
                base.SetValue(InvokeCommandAction.InputConverterParameterProperty, value);
            }
        }
        public string InputConverterLanguage
        {
            get
            {
                return (string)base.GetValue(InvokeCommandAction.InputConverterLanguageProperty);
            }
            set
            {
                base.SetValue(InvokeCommandAction.InputConverterLanguageProperty, value);
            }
        }
        public object Execute(object sender, object parameter)
        {
            if (this.Command == null)
            {
                return false;
            }
            object parameter2;
            if (this.CommandParameter != null)
            {
                parameter2 = this.CommandParameter;
            }
            else
            {
                if (this.InputConverter != null)
                {
                    parameter2 = this.InputConverter.Convert(parameter, typeof(object), this.InputConverterParameter, this.InputConverterLanguage);
                }
                else
                {
                    parameter2 = parameter;
                }
            }
            if (!this.Command.CanExecute(parameter2))
            {
                return false;
            }
            this.Command.Execute(parameter2);
            return true;
        }
    }

}
