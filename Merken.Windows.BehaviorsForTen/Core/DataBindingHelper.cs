using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Merken.Windev.BehaviorsForTen.Core
{
    internal static class DataBindingHelper
    {
        private static readonly Dictionary<Type, List<DependencyProperty>> DependenciesPropertyCache = new Dictionary<Type, List<DependencyProperty>>();
        public static void RefreshDataBindingsOnActions(ActionCollection actions)
        {
            foreach (DependencyObject current in actions)
            {
                foreach (DependencyProperty current2 in DataBindingHelper.GetDependencyProperties(current.GetType()))
                    DataBindingHelper.RefreshBinding(current, current2);
            }
        }
        private static IEnumerable<DependencyProperty> GetDependencyProperties(Type type)
        {
            List<DependencyProperty> list = null;
            if (!DataBindingHelper.DependenciesPropertyCache.TryGetValue(type, out list))
            {
                list = new List<DependencyProperty>();
                while (type != null && type != typeof(DependencyObject))
                {
                    foreach (FieldInfo current in type.GetRuntimeFields())
                        if (current.IsPublic && current.FieldType == typeof(DependencyProperty))
                        {
                            DependencyProperty dependencyProperty = current.GetValue(null) as DependencyProperty;
                            if (dependencyProperty != null)
                            {
                                list.Add(dependencyProperty);
                            }
                        }
                }
                type = type.GetTypeInfo().BaseType;
            }
            DataBindingHelper.DependenciesPropertyCache[type] = list;
            return list;
        }
        private static void RefreshBinding(DependencyObject target, DependencyProperty property)
        {
            BindingExpression bindingExpression = target.ReadLocalValue(property) as BindingExpression;
            if (bindingExpression != null && bindingExpression.ParentBinding != null)
            {
                BindingOperations.SetBinding(target, property, bindingExpression.ParentBinding);
            }
        }
    }
}
