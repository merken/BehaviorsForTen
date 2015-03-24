using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Merken.Windev.BehaviorsForTen.Core
{
    internal static class TypeConverterHelper
    {
        private const string ContentControlFormatString = "<ContentControl xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:c='using:{0}'><c:{1}>{2}</c:{1}></ContentControl>";
        public static object Convert(string value, string destinationTypeFullName)
        {
            if (string.IsNullOrEmpty(destinationTypeFullName))
            {
                throw new ArgumentNullException("destinationTypeFullName");
            }
            string scope = TypeConverterHelper.GetScope(destinationTypeFullName);
            if (string.Equals(scope, "System", StringComparison.Ordinal))
            {
                if (string.Equals(destinationTypeFullName, typeof(string).FullName, StringComparison.Ordinal))
                {
                    return value;
                }
                if (string.Equals(destinationTypeFullName, typeof(bool).FullName, StringComparison.Ordinal))
                {
                    return bool.Parse(value);
                }
                if (string.Equals(destinationTypeFullName, typeof(int).FullName, StringComparison.Ordinal))
                {
                    return int.Parse(value, CultureInfo.InvariantCulture);
                }
                if (string.Equals(destinationTypeFullName, typeof(double).FullName, StringComparison.Ordinal))
                {
                    return double.Parse(value, CultureInfo.CurrentCulture);
                }
            }
            string type = TypeConverterHelper.GetType(destinationTypeFullName);
            string text = string.Format(CultureInfo.InvariantCulture, "<ContentControl xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:c='using:{0}'><c:{1}>{2}</c:{1}></ContentControl>", new object[]
            {
                scope,
                type,
                value
            });
            ContentControl contentControl = XamlReader.Load(text) as ContentControl;
            if (contentControl != null)
            {
                return contentControl.Content;
            }
            return null;
        }
        private static string GetScope(string name)
        {
            int num = name.LastIndexOf('.');
            if (num != name.Length - 1)
            {
                return name.Substring(0, num);
            }
            return name;
        }
        private static string GetType(string name)
        {
            int num = name.LastIndexOf('.');
            if (num != name.Length - 1)
            {
                return name.Substring(num + 1);
            }
            return name;
        }
    }

}
