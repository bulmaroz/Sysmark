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
    public partial class UpcomingCallsViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private ObservableCollection<Marketing> _upcomingCalls = new();

        [ObservableProperty]
        private bool _isBusy;

        public UpcomingCallsViewModel(DatabaseService databaseService)
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
                var calls = await _databaseService.ObtenerLlamadasProximasAsync(7);
                UpcomingCalls = new ObservableCollection<Marketing>(calls);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar llamadas próximas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
