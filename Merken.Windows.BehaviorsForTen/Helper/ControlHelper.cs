using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Merken.Windev.BehaviorsForTen.Helper
{
    internal static class ControlHelper
    {
        internal static Page GetParentPage(FrameworkElement elem)
        {
            var parent = elem.Parent;
            var parentPage = (parent as Page);
            while (parentPage == null && parent != null)
            {
                parent = (parent as FrameworkElement).Parent;
                parentPage = (parent as Page);
            }
            return parentPage;
        }

        internal static T FindParent<T>(DependencyObject element) where T : FrameworkElement
        {
            FrameworkElement parent = VisualTreeHelper.GetParent(element) as FrameworkElement;

            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                    return correctlyTyped;
                else
                    return FindParent<T>(parent);
            }

            return null;
        }

        internal static T FindChild<T>(FrameworkElement element) where T : FrameworkElement
        {
            int childCount = VisualTreeHelper.GetChildrenCount(element);
            for (int c = 0; c < childCount; c++)
            {
                FrameworkElement child = VisualTreeHelper.GetChild(element, c) as FrameworkElement;
                if (child != null)
                {
                    T correctlyTyped = child as T;
                    if (correctlyTyped != null)
                        return correctlyTyped;
                    else
                        return FindChild<T>(child);
                }
            }
            return null;
        }
    }
}
