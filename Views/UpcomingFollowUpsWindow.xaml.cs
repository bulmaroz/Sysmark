using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using SysMarkModerno.ViewModels;

namespace SysMarkModerno.Views
{
    public partial class UpcomingFollowUpsWindow : Window
    {
        public UpcomingFollowUpsWindow(UpcomingFollowUpsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Item_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = ((System.Windows.Controls.ListViewItem)sender).DataContext as SysMarkModerno.Models.Marketing;
            if (item != null)
            {
                var vm = DataContext as UpcomingFollowUpsViewModel;
                if (vm != null) vm.Seleccionado = item;
                DialogResult = true;
                Close();
            }
        }
    }

    public class DatePassedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
            {
                return dt.Date < DateTime.Today;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
