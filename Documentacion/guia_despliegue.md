GUIA DE DESPLIEGUE AVANZADO SYSMARKMODERNO

Esta guía estará diseñada para administradores de sistemas que deseen implementar SysMarkModerno en un entorno de producción garantizando seguridad y estabilidad.

1. ESTRATEGIA DE COMPILACION BUILD PIPELINE

Se utilizará el proceso de Publicación de NET para generar binarios optimizados. Este método incluirá el runtime dentro del archivo exe eliminando la necesidad de instalaciones manuales en los clientes.

ReadyToRun. Precompilará el código para reducir el tiempo de inicio.
SingleFile. Generará un solo archivo ejecutable fácil de distribuir.

2. SEGURIDAD DEL SERVIDOR DE BASE DE DATOS

En un entorno de red el servidor SQL deberá estar protegido pero accesible.

Puerto Estático. Se asegurará que SQL Server escuche en el puerto TCP 1433.
Reglas de Firewall. El administrador ejecutará comandos de PowerShell para permitir la conexión de los clientes mediante reglas de entrada.

3. DISTRIBUCION A EQUIPOS CLIENTE

1. El usuario generará el archivo único mediante el comando de publicación.
2. Se copiará la carpeta de publicación que contiene el ejecutable y el archivo appsettings json a cada máquina cliente.
3. En cada cliente el usuario editará la configuración para apuntar a la IP de la Máquina Servidor.
4. Se creará un acceso directo en el escritorio para el usuario final.
5. El administrador agregará la ruta del programa a la lista de exclusiones del antivirus.

4. MANTENIMIENTO Y MONITOREO

La aplicación registrará excepciones en la subcarpeta logs. En caso de fallo el usuario revisará la conectividad mediante el comando ping y consultará el archivo de registro de errores para identificar el origen del problema.

5. DESINSTALACION Y LIMPIEZA

Para remover la aplicación el usuario eliminará la carpeta que contiene el ejecutable y borrará las entradas de configuración local si fuera necesario.
