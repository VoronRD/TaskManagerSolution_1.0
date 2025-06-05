using Microsoft.Xaml.Behaviors;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Core.Models;

namespace TaskManager.Infrastructure.Behaviors
{
    public class ListBoxSelectedItemsBehavior : Behavior<ListBox>
    {
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(
                nameof(SelectedItems),
                typeof(IList),
                typeof(ListBoxSelectedItemsBehavior),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemsChanged));

        public IList SelectedItems
        {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
            {
                AssociatedObject.SelectionChanged += OnSelectionChanged;

                AssociatedObject.Loaded += (s, e) =>
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        // Повторное применение даже если объект SelectedItems не менялся
                        InvokeSelectedItemsChanged(this, SelectedItems);
                    }), System.Windows.Threading.DispatcherPriority.Loaded);
                };
            }
        }


        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
                AssociatedObject.SelectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedItems == null || AssociatedObject == null) return;

            foreach (var item in e.RemovedItems)
            {
                if (item is Tags removedTag)
                {
                    var toRemove = SelectedItems
                        .OfType<Tags>()
                        .FirstOrDefault(t => t.Id == removedTag.Id);

                    if (toRemove != null)
                        SelectedItems.Remove(toRemove);
                }
            }

            foreach (var item in e.AddedItems)
            {
                if (item is Tags addedTag)
                {
                    bool exists = SelectedItems
                        .OfType<Tags>()
                        .Any(t => t.Id == addedTag.Id);

                    if (!exists)
                        SelectedItems.Add(addedTag);
                }
            }
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListBoxSelectedItemsBehavior behavior &&
                behavior.AssociatedObject != null &&
                behavior.AssociatedObject.IsLoaded)
            {
                InvokeSelectedItemsChanged(behavior, e.NewValue as IList);
            }
        }

        public static void InvokeSelectedItemsChanged(ListBoxSelectedItemsBehavior behavior, IList newValue)
        {
            if (behavior?.AssociatedObject == null || newValue == null)
                return;

            behavior.AssociatedObject.SelectedItems.Clear();

            foreach (var newItem in newValue)
            {
                foreach (var item in behavior.AssociatedObject.Items)
                {
                    if (item is Tags candidate && newItem is Tags target)
                    {
                        if (candidate.Id == target.Id)
                        {
                            behavior.AssociatedObject.SelectedItems.Add(candidate);
                            break;
                        }
                    }
                }
            }
        }

    }
}
