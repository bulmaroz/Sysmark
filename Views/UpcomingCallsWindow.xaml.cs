using System.Windows;
using SysMarkModerno.ViewModels;

namespace SysMarkModerno.Views
{
    public partial class UpcomingCallsWindow : Window
    {
        public UpcomingCallsWindow(UpcomingCallsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Item_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = ((System.Windows.Controls.ListViewItem)sender).DataContext as SysMarkModerno.Models.Marketing;
            if (item != null)
            {
                var vm = DataContext as UpcomingCallsViewModel;
                if (vm != null) vm.Seleccionado = item;
                DialogResult = true;
                Close();
            }
        }
    }
}
