using System;
using System.Collections.Generic;
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
using TaskManager.TagsModule.ViewModels;

namespace TaskManager.TagsModule.View
{
    /// <summary>
    /// Логика взаимодействия для TagListView.xaml
    /// </summary>
    public partial class TagListView : Window
    {
        public TagListView()
        {
            InitializeComponent();
        }

        private void ListViewItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is TagListViewModel vm && vm.SelectedTag != null)
            {
                vm.SelectTagCommand.Execute(vm.SelectedTag);
            }
        }
    }
}
