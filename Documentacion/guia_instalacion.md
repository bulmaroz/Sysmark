GUIA DE INSTALACION DETALLADA SYSMARKMODERNO

Esta guía proporcionará una hoja de ruta exhaustiva para la preparación del entorno la configuración de la infraestructura de datos y la puesta en marcha del proyecto SysMarkModerno. Estará dirigida tanto a desarrolladores como a personal de TI encargado del soporte.

0. ARQUITECTURA DE RED SERVIDOR CLIENTE

Para implementaciones en entornos con múltiples usuarios SysMarkModerno operará bajo un modelo centralizado.

1. Máquina Servidor. Será la encargada de alojar la base de datos mediante SQL Server. En esta máquina se configurará la instancia para permitir conexiones remotas.
2. Máquinas Cliente. Ejecutarán el archivo exe generado. Se conectarán al servidor a través de la red local utilizando la IP de la Máquina Servidor.

1. PREPARACION DEL ENTORNO TECNOLOGICO

Para asegurar un rendimiento óptimo y evitar conflictos de compatibilidad se cumplirán los siguientes requisitos técnicos.

A. Requisitos de Hardware Recomendado
Procesador de 4 núcleos a 2.5 GHz o superior.
Memoria RAM de 8 GB mínimo.
Almacenamiento de 500 MB de espacio libre.

B. Requisitos de Software
Windows 10 o 11 Pro o Enterprise.
NET 8.0 SDK y Runtime. Será el motor principal de la aplicación.
SQL Server 2016 hasta 2022. Se recomendará la edición Express o Standard.

2. DESPLIEGUE DE LA INFRAESTRUCTURA DE DATOS

La base de datos será el corazón de SysMarkModerno. Se seguirán estos pasos para una implementación sin errores.

Ejecución del Script de Estructura
El archivo schema.sql no solo creará tablas sino que establecerá la lógica de integridad referencial.

1. El usuario iniciará SQL Server Management Studio con privilegios de Administrador.
2. El usuario seleccionará el archivo schema.sql mediante el menú de Archivo.
3. Se revisará que no existan bases de datos con el nombre SysMark para evitar colisiones.
4. El usuario presionará la tecla F5 para ejecutar el script. El sistema realizará la verificación de la base de datos la creación del esquema la generación de la tabla MARKETING y el registro de procedimientos almacenados.

Tablas Auxiliares Críticas. Se asegurará la ejecución de los scripts correspondientes para las tablas Empleados y Contactas si la versión del sistema lo requiriera.

3. CONFIGURACION FINA DE APPSETTINGS JSON

El archivo de configuración será el puente entre la interfaz y el servidor. Se editará con precisión.

Server. Se usará la IP Estática del servidor en entornos de producción.
TrustServerCertificate. Será obligatorio marcar esta propiedad en True.
CheckIntervalMinutes. Definirá el tiempo que el sistema esperará para actualizar las notificaciones.

4. COMPILACION Y DEPURACION

El usuario hará clic derecho en la solución dentro de Visual Studio y seleccionará la opción de limpiar solución. El sistema descargará automáticamente las librerías necesarias mediante NuGet. El usuario presionará la combinación de teclas Ctrl Shift B para compilar el proyecto.

5. SOLUCION DE PROBLEMAS DE INSTALACION

En caso de fallo el administrador verificara los permisos en SQL Server y revisará que el servicio de SQL Server esté en ejecución. También se confirmará que los paquetes NuGet se hayan restaurado correctamente.
