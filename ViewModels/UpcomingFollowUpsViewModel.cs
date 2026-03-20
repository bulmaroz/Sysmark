using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SysMarkModerno.Models;
using SysMarkModerno.Services;

namespace SysMarkModerno.ViewModels
{
    public partial class UpcomingFollowUpsViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private ObservableCollection<Marketing> _upcomingFollowUps = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private Marketing _seleccionado;

        public UpcomingFollowUpsViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _ = LoadDataAsync();
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            IsBusy = true;
            try
            {
                var seguimientos = await _databaseService.ObtenerSeguimientosProximosAsync(7);
                UpcomingFollowUps = new ObservableCollection<Marketing>(seguimientos);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar seguimientos próximos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
