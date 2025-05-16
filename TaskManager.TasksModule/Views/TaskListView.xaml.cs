using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
