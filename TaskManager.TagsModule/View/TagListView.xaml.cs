using System.Windows.Controls;
using System.Windows.Input;
using TaskManager.TagsModule.ViewModels;

namespace TaskManager.TagsModule.View
{
    public partial class TagListView : UserControl
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
