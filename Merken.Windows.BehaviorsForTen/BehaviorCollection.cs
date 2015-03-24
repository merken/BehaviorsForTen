using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;

namespace Merken.Windev.BehaviorsForTen
{
    public sealed class BehaviorCollection : DependencyObjectCollection
    {
        private readonly List<IBehavior> oldCollection = new List<IBehavior>();
        public DependencyObject AssociatedObject
        {
            get;
            private set;
        }
        public BehaviorCollection()
        {
            this.VectorChanged += BehaviorCollection_VectorChanged;
        }

        public void Attach(DependencyObject associatedObject)
        {
            if (associatedObject == this.AssociatedObject)
            {
                return;
            }
            if (DesignMode.DesignModeEnabled)
            {
                return;
            }
            if (this.AssociatedObject != null)
            {
                throw new InvalidOperationException("Error attaching behavior");
            }
            this.AssociatedObject = associatedObject;
            foreach (IBehavior behavior in this)
            {
                behavior.Attach(this.AssociatedObject);
            }
        }
        public void Detach()
        {
            foreach (IBehavior behavior in this)
                if (behavior.AssociatedObject != null)
                {
                    behavior.Detach();
                }

            this.AssociatedObject = null;
            this.oldCollection.Clear();
        }
        private void BehaviorCollection_VectorChanged(IObservableVector<DependencyObject> sender, IVectorChangedEventArgs eventArgs)
        {
            if (eventArgs.CollectionChange == null)
            {
                foreach (IBehavior current in this.oldCollection)
                {
                    if (current.AssociatedObject != null)
                    {
                        current.Detach();
                    }
                }
                this.oldCollection.Clear();
                foreach (DependencyObject dependencyObject in this.AsEnumerable())
                    this.oldCollection.Add(this.VerifiedAttach(dependencyObject));
                return;
            }

            int index = (int)eventArgs.Index;
            DependencyObject item = this[index];
            switch (eventArgs.CollectionChange)
            {
                case CollectionChange.ItemInserted:
                    this.oldCollection.Insert(index, this.VerifiedAttach(item));
                    return;
                case CollectionChange.ItemRemoved:
                    {
                        IBehavior behavior = this.oldCollection[index];
                        if (behavior.AssociatedObject != null)
                        {
                            behavior.Detach();
                        }
                        this.oldCollection.RemoveAt(index);
                        return;
                    }
                case CollectionChange.ItemChanged:
                    {
                        IBehavior behavior = this.oldCollection[index];
                        if (behavior.AssociatedObject != null)
                        {
                            behavior.Detach();
                        }
                        this.oldCollection[index] = this.VerifiedAttach(item);
                        return;
                    }
                default:
                    return;
            }
        }
        private IBehavior VerifiedAttach(DependencyObject item)
        {
            IBehavior behavior = item as IBehavior;
            if (behavior == null)
            {
                throw new InvalidOperationException("NonBehaviorAddedToBehaviorCollectionExceptionMessage");
            }
            if (this.oldCollection.Contains(behavior))
            {
                throw new InvalidOperationException("DuplicateBehaviorInCollectionExceptionMessage");
            }
            if (this.AssociatedObject != null)
            {
                behavior.Attach(this.AssociatedObject);
            }
            return behavior;
        }
        [Conditional("DEBUG")]
        private void VerifyOldCollectionIntegrity()
        {
            bool flag = this.Count == this.oldCollection.Count;
            if (flag)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i] != this.oldCollection[i])
                    {
                        return;
                    }
                }
            }
        }
    }
}
