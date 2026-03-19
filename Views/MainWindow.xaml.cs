using System.Windows;
using SysMarkModerno.ViewModels;

namespace SysMarkModerno.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
