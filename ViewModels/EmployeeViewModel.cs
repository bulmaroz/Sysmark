using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SysMarkModerno.Models;
using SysMarkModerno.Services;

namespace SysMarkModerno.ViewModels
{
    public partial class EmployeeViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;
        private readonly NotificationService _notificationService;

        [ObservableProperty]
        private ObservableCollection<Empleado> empleados = new();

        [ObservableProperty]
        private ObservableCollection<EstatusEmpleado> listaEstatus = new();

        [ObservableProperty]
        private Empleado? empleadoSeleccionado;

        [ObservableProperty]
        private bool estaCargando;

        public EmployeeViewModel(DatabaseService databaseService, NotificationService notificationService)
        {
            _databaseService = databaseService;
            _notificationService = notificationService;
            _ = CargarDatosAsync();
        }

        private async Task CargarDatosAsync()
        {
            try
            {
                EstaCargando = true;
                var empleadosList = await _databaseService.ObtenerTodosEmpleadosAsync();
                Empleados = new ObservableCollection<Empleado>(empleadosList);

                var estatusList = await _databaseService.ObtenerEstatusEmpleadosAsync();
                ListaEstatus = new ObservableCollection<EstatusEmpleado>(estatusList);
            }
            catch (Exception ex)
            {
                _notificationService.MostrarError($"Error al cargar empleados: {ex.Message}");
            }
            finally
            {
                EstaCargando = false;
            }
        }

        [RelayCommand]
        private void NuevoEmpleado()
        {
            EmpleadoSeleccionado = new Empleado();
        }

        [RelayCommand]
        private async Task GuardarAsync()
        {
            if (EmpleadoSeleccionado == null)
            {
                _notificationService.MostrarAdvertencia("Seleccione o cree un empleado primero.");
                return;
            }

            if (string.IsNullOrWhiteSpace(EmpleadoSeleccionado.NombreCompleto))
            {
                _notificationService.MostrarAdvertencia("El nombre es requerido.");
                return;
            }

            if (string.IsNullOrWhiteSpace(EmpleadoSeleccionado.ApellidoPaterno))
            {
                _notificationService.MostrarAdvertencia("El apellido paterno es requerido.");
                return;
            }

            if (EmpleadoSeleccionado.IdEstatus == null)
            {
                _notificationService.MostrarAdvertencia("Seleccione un estatus para el empleado.");
                return;
            }

            try
            {
                EstaCargando = true;
                var resultado = await _databaseService.GuardarEmpleadoAsync(EmpleadoSeleccionado);
                if (resultado)
                {
                    _notificationService.MostrarExito("Empleado guardado correctamente.");
                    await CargarDatosAsync();
                }
                else
                {
                    _notificationService.MostrarError("Error al guardar el empleado.");
                }
            }
            catch (Exception ex)
            {
                _notificationService.MostrarError($"Error: {ex.Message}");
            }
            finally
            {
                EstaCargando = false;
            }
        }

        [RelayCommand]
        private async Task EliminarAsync()
        {
            if (EmpleadoSeleccionado == null || EmpleadoSeleccionado.IdEmpleado == 0)
            {
                _notificationService.MostrarAdvertencia("Seleccione un empleado para eliminar.");
                return;
            }

            try
            {
                EstaCargando = true;
                var resultado = await _databaseService.EliminarEmpleadoAsync(EmpleadoSeleccionado.IdEmpleado);
                if (resultado)
                {
                    _notificationService.MostrarExito("Empleado eliminado correctamente.");
                    EmpleadoSeleccionado = null;
                    await CargarDatosAsync();
                }
                else
                {
                    _notificationService.MostrarError("Error al eliminar el empleado.");
                }
            }
            catch (Exception ex)
            {
                _notificationService.MostrarError($"Error: {ex.Message}");
            }
            finally
            {
                EstaCargando = false;
            }
        }
    }
}
