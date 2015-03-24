using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;

namespace Merken.Windev.BehaviorsForTen
{
    public sealed class ActionCollection : DependencyObjectCollection
    {
        public ActionCollection()
        {
            this.VectorChanged += ActionCollection_VectorChanged;
        }

        private void ActionCollection_VectorChanged(IObservableVector<DependencyObject> sender, IVectorChangedEventArgs eventArgs)
        {
            CollectionChange collectionChange = eventArgs.CollectionChange;
            if (collectionChange == null)
            {
                foreach (DependencyObject current in this)
                    ActionCollection.VerifyType(current);
            }
            if (collectionChange == CollectionChange.ItemInserted || collectionChange == CollectionChange.ItemChanged)
            {
                DependencyObject item = this[(int)eventArgs.Index];
                ActionCollection.VerifyType(item);
            }
        }
        private static void VerifyType(DependencyObject item)
        {
            if (!(item is IAction))
            {
                throw new InvalidOperationException("NonActionAddedToActionCollectionExceptionMessage");
            }
        }
    }
}
