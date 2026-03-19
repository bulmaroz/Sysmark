GUIA DE ARQUITECTURA TECNICA SYSMARKMODERNO

Este documento describirá la estructura técnica del proyecto SysMarkModerno detallando la implementación del patrón MVVM y la organización de las clases principales. Todos los componentes se diseñarán para garantizar la escalabilidad y el mantenimiento del sistema.

1. ARQUITECTURA MVVM MODEL VIEW VIEWMODEL

El sistema operará bajo el patrón de arquitectura MVVM lo que permitirá una separación clara entre la interfaz de usuario y la lógica de negocio.

El Modelo representará los datos y las reglas de negocio.
La Vista definirá la apariencia visual mediante XAML.
El ViewModel actuará como intermediario gestionando los comandos y el estado de la vista.

2. MODELOS DE DATOS MODELS

Los modelos se ubicarán en el espacio de nombres SysMarkModerno Models y representarán las entidades de la base de datos SQL Server.

Clase Marketing. La clase Marketing representará a las empresas y prospectos. Implementará la interfaz INotifyPropertyChanged para reflejar cambios en la interfaz de usuario. Contendrá propiedades críticas como IdEmpresa que será el identificador único y Empresa que indicará el nombre comercial. El campo Estatus mostrará el estado actual del prospecto y la propiedad TieneLlamada funcionará como una bandera para identificar registros con seguimientos pendientes.

Clase Empleado. La clase Empleado heredará de ObservableObject. El sistema la utilizará para gestionar al personal interno. Incluirá campos como NombreCompleto y CorreoElectronico los cuales serán esenciales para el sistema de notificaciones automáticas.

3. LOGICA DE NAVEGACION Y DATOS VIEWMODELS

Los ViewModels gestionarán la interacción del usuario y la persistencia de datos a través de servicios especializados.

MainViewModel. Este componente será el núcleo de la aplicación. El MainViewModel se encargará de la carga de datos mediante el método CargarDatosAsync el cual recuperará la información desde el servicio de base de datos. La propiedad RegistrosFiltrados mostrará dinámicamente los prospectos según el texto de búsqueda. El objeto expondrá comandos como GuardarCommand utilizando la librería RelayCommand.

EmployeeViewModel. Esta clase se especializará en la administración del catálogo de empleados. El desarrollador encontrará métodos para agregar o eliminar usuarios de manera asíncrona.

4. INTERFAZ DE USUARIO VIEWS

Las vistas se definirán en archivos XAML y utilizarán el framework Material Design para proporcionar una estética moderna.

MainWindow. Constituirá la ventana principal que integrará el panel de control los filtros y la tabla de registros.
EmployeeManagementView. Ofrecerá la interfaz dedicada a la gestión de recursos humanos.
UpcomingCallsWindow. Funcionará como el centro de alertas para llamadas programadas para el día.

5. SERVICIOS DE SOPORTE SERVICES

El sistema se apoyará en servicios especializados para realizar las tareas más importantes del negocio.

DatabaseService. El servicio DatabaseService centralizará todas las peticiones a la base de datos SQL Server utilizando la librería Dapper para optimizar el rendimiento. El programador utilizará el método ObtenerTodoMarketingAsync para traer la lista completa de registros. El sistema ejecutará sentencias SQL asíncronas para insertar o actualizar información mediante los métodos GuardarMarketingAsync y GuardarEmpleadoAsync. La clase gestionará la apertura y cierre de conexiones mediante objetos SqlConnection garantizando la integridad de la información.

EmailService. El servicio EmailService se encargará del envío automático de correos electrónicos. La aplicación utilizará la clase MailMessage de System Net Mail para enviar mensajes en formato HTML. El sistema generará archivos dinámicos con extensión ICS mediante el método GenerateIcsContent para agendar recordatorios directamente en el calendario del usuario. El servicio podrá enviar notificaciones tanto al contacto principal como a los contactos extras definidos en el sistema.

NotificationService. El componente NotificationService gestionará las alertas visuales en el escritorio. El sistema iniciará un monitoreo constante mediante un objeto Timer que ejecutará el método VerificarLlamadasPendientesAsync cada hora. El servicio consultará la base de datos para identificar llamadas programadas para el día actual y utilizará la librería Notifications Wpf para mostrar ventanas emergentes informativas. El sistema permitirá al usuario visualizar el número de tareas pendientes directamente sobre un icono de campana en la interfaz.

Esta estructura técnica asegurará que SysMarkModerno mantendrá un alto estándar de rendimiento y facilidad de desarrollo para futuras integraciones.
