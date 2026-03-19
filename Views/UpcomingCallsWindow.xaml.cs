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
    }
}
