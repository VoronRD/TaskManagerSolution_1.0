using System.Collections.ObjectModel;
using System.Windows.Controls;
using TaskManager.Core.Models;
using TaskManager.TasksModule.ViewModels;

namespace TaskManager.TasksModule.Views
{
    /// <summary>
    /// Логика взаимодействия для TaskListView.xaml
    /// </summary>
    public partial class TaskListView : UserControl
    {
        public TaskListView()
        {
            InitializeComponent();
            TagsListBox.SelectionChanged += TagsListBox_SelectionChanged;
        }

        private void TagsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is TaskListViewModel vm)
            {
                vm.SelectedTags = new ObservableCollection<Tags>(TagsListBox.SelectedItems.Cast<Tags>());
            }
        }
    }
}
