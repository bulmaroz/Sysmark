using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SysMarkModerno.Models
{
    public class Marketing : INotifyPropertyChanged
    {
        private int _idEmpresa;
        private string _empresa;
        private string _paginaWeb;
        private string _erpQueManeja;
        private string _contacto;
        private string _puesto;
        private string _telefono;
        private string _extension;
        private string _email;
        private string _giro;
        private string _estado;
        private string _ciudad;
        private string _direccion;
        private int? _codigoPostal;
        private string _contacto2;
        private string _puesto2;
        private string _telefono2;
        private string _email2;
        private DateTime? _proximaLlamada;
        private DateTime _fechaContacto;
        private string _estatus;
        private bool _proyectoEpicor;
        private bool _proyectoOpera;
        private string _comentarios;
        private string _ingresadoPor;
        private string _clienteDe;
        private string _celular;
        private string _celularContacto2;
        private bool _tieneLlamada;
        private DateTime? _seguimiento;
        private bool _seguimientoActivo;

        public int IdEmpresa
        {
            get => _idEmpresa;
            set { _idEmpresa = value; OnPropertyChanged(); }
        }

        public string Empresa
        {
            get => _empresa;
            set { _empresa = value; OnPropertyChanged(); }
        }

        public string PaginaWeb
        {
            get => _paginaWeb;
            set { _paginaWeb = value; OnPropertyChanged(); }
        }

        public string ErpQueManeja
        {
            get => _erpQueManeja;
            set { _erpQueManeja = value; OnPropertyChanged(); }
        }

        public string Contacto
        {
            get => _contacto;
            set { _contacto = value; OnPropertyChanged(); }
        }

        public string Puesto
        {
            get => _puesto;
            set { _puesto = value; OnPropertyChanged(); }
        }

        public string Telefono
        {
            get => _telefono;
            set { _telefono = value; OnPropertyChanged(); }
        }

        public string Extension
        {
            get => _extension;
            set { _extension = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string Giro
        {
            get => _giro;
            set { _giro = value; OnPropertyChanged(); }
        }

        public string Estado
        {
            get => _estado;
            set { _estado = value; OnPropertyChanged(); }
        }

        public string Ciudad
        {
            get => _ciudad;
            set { _ciudad = value; OnPropertyChanged(); }
        }

        public string Direccion
        {
            get => _direccion;
            set { _direccion = value; OnPropertyChanged(); }
        }

        public int? CodigoPostal
        {
            get => _codigoPostal;
            set { _codigoPostal = value; OnPropertyChanged(); }
        }

        public string Contacto2
        {
            get => _contacto2;
            set { _contacto2 = value; OnPropertyChanged(); }
        }

        public string Puesto2
        {
            get => _puesto2;
            set { _puesto2 = value; OnPropertyChanged(); }
        }

        public string Telefono2
        {
            get => _telefono2;
            set { _telefono2 = value; OnPropertyChanged(); }
        }

        public string Email2
        {
            get => _email2;
            set { _email2 = value; OnPropertyChanged(); }
        }

        public DateTime? ProximaLlamada
        {
            get => _proximaLlamada;
            set { _proximaLlamada = value; OnPropertyChanged(); }
        }

        public DateTime FechaContacto
        {
            get => _fechaContacto;
            set { _fechaContacto = value; OnPropertyChanged(); }
        }

        public string Estatus
        {
            get => _estatus;
            set { _estatus = value; OnPropertyChanged(); }
        }

        public bool ProyectoEpicor
        {
            get => _proyectoEpicor;
            set { _proyectoEpicor = value; OnPropertyChanged(); }
        }

        public bool ProyectoOpera
        {
            get => _proyectoOpera;
            set { _proyectoOpera = value; OnPropertyChanged(); }
        }

        public string Comentarios
        {
            get => _comentarios;
            set { _comentarios = value; OnPropertyChanged(); }
        }

        public string IngresadoPor
        {
            get => _ingresadoPor;
            set { _ingresadoPor = value; OnPropertyChanged(); }
        }

        public string ClienteDe
        {
            get => _clienteDe;
            set { _clienteDe = value; OnPropertyChanged(); }
        }

        public string Celular
        {
            get => _celular;
            set { _celular = value; OnPropertyChanged(); }
        }

        public string CelularContacto2
        {
            get => _celularContacto2;
            set { _celularContacto2 = value; OnPropertyChanged(); }
        }

        public bool TieneLlamada
        {
            get => _tieneLlamada;
            set { _tieneLlamada = value; OnPropertyChanged(); }
        }

        public DateTime? Seguimiento
        {
            get => _seguimiento;
            set { _seguimiento = value; OnPropertyChanged(); }
        }

        public bool SeguimientoActivo
        {
            get => _seguimientoActivo;
            set { _seguimientoActivo = value; OnPropertyChanged(); }
        }

        // Propiedades calculadas
        public string EstatusColor
        {
            get
            {
                return Estatus switch
                {
                    "Nuevo" => "#4CAF50",
                    "En Proceso" => "#2196F3",
                    "Contactado" => "#FF9800",
                    "Cerrado" => "#9E9E9E",
                    "Ganado" => "#8BC34A",
                    "Perdido" => "#F44336",
                    _ => "#757575"
                };
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
