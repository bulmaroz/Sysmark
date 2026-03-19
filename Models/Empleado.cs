using CommunityToolkit.Mvvm.ComponentModel;

namespace SysMarkModerno.Models
{
    public partial class Empleado : ObservableObject
    {
        [ObservableProperty]
        private int idEmpleado;

        [ObservableProperty]
        private string nombreCompleto = string.Empty;

        [ObservableProperty]
        private string apellidoPaterno = string.Empty;

        [ObservableProperty]
        private string apellidoMaterno = string.Empty;

        [ObservableProperty]
        private string correoElectronico = string.Empty;

        [ObservableProperty]
        private int? idEstatus;

        [ObservableProperty]
        private string estatusDescripcion = string.Empty;

        public string NombreDeMuestra => $"{NombreCompleto} {ApellidoPaterno} {ApellidoMaterno}".Trim();
    }
}
