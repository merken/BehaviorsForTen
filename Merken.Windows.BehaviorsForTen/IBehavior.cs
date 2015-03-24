using Windows.UI.Xaml;

namespace Merken.Windev.BehaviorsForTen
{
    public interface IBehavior
    {
        DependencyObject AssociatedObject { get; }
        void Attach(DependencyObject associatedObject);
        void Detach();
    }
}
