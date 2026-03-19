<<<<<<< HEAD
# Seguimiento de prospectos a clientes - Sistema de Seguimiento de Marketing

## 📋 Descripción

Sistema moderno de gestión de marketing desarrollado en WPF con Material Design. Permite dar seguimiento a prospectos, clientes y proyectos con notificaciones automáticas de llamadas programadas.

## ✨ Características Principales

### ✅ Implementadas en este Prototipo

- **Interfaz Moderna**: Material Design con tema claro/oscuro
- **Gestión de Marketing**: CRUD completo de prospectos y clientes
- **Búsqueda Avanzada**: Filtros inteligentes por múltiples criterios
- **Notificaciones Automáticas**: Sistema de alertas para llamadas programadas
- **Conexión Multiusuario**: Soporte para SQL Server en red local
- **Filtros Dinámicos**: 
  - Búsqueda en tiempo real por texto
  - Filtros por estatus, estado, giro y proyecto
  - Combinación de múltiples filtros

### 🎯 Características del Sistema

1. **Dashboard Principal**
   - Menú lateral navegable
   - Contador de notificaciones en tiempo real
   - Navegación fluida entre módulos

2. **Módulo de Marketing**
   - Visualización en DataGrid con todas las empresas
   - Doble clic para editar
   - Botones de acción rápida (Editar/Eliminar)
   - Formulario completo de edición con:
     - Datos principales de la empresa
     - Ubicación completa
     - Contacto principal y secundario
     - Seguimiento y proyectos
     - Comentarios y notas

3. **Sistema de Notificaciones**
   - Verificación automática cada 5 minutos
   - Badge con contador en la barra superior
   - Ventana de notificaciones con lista de llamadas pendientes
   - Filtrado de llamadas del día actual y siguiente

4. **Configuración**
   - Configuración de conexión a base de datos
   - Soporte para autenticación de Windows o SQL
   - Prueba de conexión
   - Configuración de notificaciones

## 🚀 Requisitos

- Windows 10/11
- .NET 8.0 SDK o superior
- SQL Server 2016 o superior (Express, Standard o Enterprise)
- Visual Studio 2022 (recomendado) o Visual Studio Code

## 📦 Instalación

### 1. Preparar la Base de Datos

Ejecuta el script `schema.sql` incluido en tu SQL Server para crear la base de datos `SysMark` con todas sus tablas y procedimientos almacenados.

```sql
-- En SQL Server Management Studio (SSMS)
-- Abre el archivo script.sql y ejecútalo
```

### 2. Configurar Acceso en Red (Para uso multiusuario)

#### En el servidor donde está SQL Server:

1. **Habilitar conexiones remotas:**
   - SQL Server Configuration Manager
   - SQL Server Network Configuration → Protocols for MSSQLSERVER
   - Habilitar "TCP/IP"
   - Reiniciar el servicio SQL Server

2. **Configurar Firewall:**
   ```powershell
   # Ejecutar como Administrador
   New-NetFirewallRule -DisplayName "SQL Server" -Direction Inbound -Protocol TCP -LocalPort 1433 -Action Allow
   ```

3. **Obtener la IP del servidor:**
   ```cmd
   ipconfig
   # Anota la dirección IPv4 (ejemplo: 192.168.1.100)
   ```

### 3. Compilar el Proyecto

#### Opción A: Visual Studio 2022
1. Abre `SysMarkModerno.csproj`
2. Restaurar paquetes NuGet (automático)
3. Presiona F5 para compilar y ejecutar

#### Opción B: Línea de comandos
```bash
cd SysMarkModerno
dotnet restore
dotnet build
dotnet run
```

### 4. Configurar Conexión en la Aplicación

1. Localiza el archivo `appsettings.json` en la carpeta de la aplicación.
2. Configura los datos de conexión en el campo `SysMarkDB`:
   - **Servidor**: `localhost` o la IP del servidor.
   - **Base de Datos**: `SysMark`
3. Guarda el archivo y reinicia la aplicación.

## 🖥️ Uso del Sistema

### Gestión de Marketing

1. **Crear Nuevo Registro:**
   - Clic en botón "Nuevo Registro"
   - Llenar el formulario
   - Guardar

2. **Búsqueda Rápida:**
   - Escribir en el campo de búsqueda
   - Los resultados se filtran en tiempo real

3. **Filtros Avanzados:**
   - Usar los combos de Estatus, Estado, Giro y Proyecto
   - Los filtros se combinan automáticamente

4. **Editar Registro:**
   - Doble clic en un registro
   - O usar el botón de editar (lápiz)

5. **Programar Llamada:**
   - Al editar, seleccionar fecha en "Próxima Llamada"
   - Marcar checkbox "Agendar Notificación"
   - El sistema lo mostrará en notificaciones

### Notificaciones

- **Ver notificaciones**: Clic en el ícono de campana (esquina superior derecha)
- **Badge**: Muestra el número de llamadas pendientes
- Las notificaciones se actualizan automáticamente cada 5 minutos

## 🔧 Distribución a Otras PCs

### Opción 1: Compilación Release

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

El ejecutable estará en: `bin\Release\net8.0-windows\win-x64\publish\`

### Opción 2: Instalador MSI (Recomendado)

Usa WiX Toolset o Advanced Installer para crear un instalador profesional:

1. Instala WiX Toolset
2. Agrega referencia al proyecto
3. Configura Product.wxs
4. Compila el instalador

### Configuración en cada PC cliente:

1. Copia e instala la aplicación
2. Edita `appsettings.json` con la IP del servidor: `IP_DEL_SERVIDOR` (ejemplo: `192.168.1.100`)
3. Probad y aseguraos que la conexión es exitosa.

## 📊 Estructura del Proyecto

```
SysMarkModerno/
├── App.xaml                    # Aplicación principal
├── App.xaml.cs                 # Código de aplicación
├── MainWindow.xaml             # Ventana principal
├── MainWindow.xaml.cs          # Lógica de ventana principal
├── Models/
│   └── Marketing.cs            # Modelo de datos
├── Services/
│   ├── DatabaseService.cs      # Servicio de base de datos
│   └── NotificationService.cs  # Servicio de notificaciones
└── Views/
    ├── MarketingPage.xaml      # Página de marketing
    ├── MarketingEditWindow.xaml # Ventana de edición
    ├── NotificationsWindow.xaml # Ventana de notificaciones
    ├── ConfiguracionPage.xaml  # Página de configuración
    ├── DirectorioPage.xaml     # Página de directorio (placeholder)
    └── EmpleadosPage.xaml      # Página de empleados (placeholder)
```

## 🔐 Seguridad

- Las contraseñas se pueden almacenar de forma segura usando Windows Credential Manager
- Usa siempre `TrustServerCertificate=true` o configura certificados SSL
- Implementa roles y permisos en SQL Server para cada usuario

## 🚧 Próximas Mejoras

### Para Desarrollo Completo:

1. **Dashboard con Gráficos**
   - Integrar biblioteca de gráficos (LiveCharts o OxyPlot)
   - Métricas de conversión
   - Análisis por período

2. **Módulo de Reportes**
   - Exportación a Excel
   - Generación de PDFs
   - Reportes personalizables

3. **Sistema de Usuarios**
   - Login con autenticación
   - Roles y permisos
   - Auditoría de cambios

4. **Mejoras de UI**
   - Animaciones
   - Temas personalizables
   - Configuración de columnas visibles

5. **Funcionalidades Adicionales**
   - Importación masiva desde Excel
   - Integración con email
   - Backup automático

## 📞 Soporte

Para dudas o problemas:
1. Revisa los logs en la carpeta de la aplicación
2. Verifica la conexión a la base de datos
3. Revisa que el firewall permita el puerto 1433
=======
# Sysmark
>>>>>>> d4838f95c4ccce39674bf7139e54b470c3757cf2
