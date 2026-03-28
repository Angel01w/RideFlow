# RideFlow

# Descripción del proyecto
RideFlow es una aplicación web diseñada para la gestión de rutas de transporte institucional para colaboradores de una empresa. Su propósito es automatizar el manejo de rutas, horarios, asignaciones y asistencia, reemplazando el proceso manual que genera confusiones y desorganización.

El sistema permite registrar rutas de transporte con información como origen, destino, horario de salida y conductor asignado. También permite registrar los colaboradores que harán uso del transporte, almacenando sus datos básicos como nombre, departamento y contacto.

Además, RideFlow facilita la asignación de colaboradores a una o varias rutas según sus necesidades y disponibilidad, permitiendo un mejor control de la ocupación de cada ruta. De igual forma, incorpora un módulo de asistencia para registrar diariamente qué colaboradores abordaron una ruta, ofreciendo un resumen por día y por ruta.

El sistema también incluye reportes básicos que permiten consultar la cantidad de colaboradores asignados por ruta, la asistencia diaria o mensual y estadísticas simples como las rutas más utilizadas y los colaboradores con mayor frecuencia de uso.

La aplicación cuenta con una interfaz amigable pensada para el administrador, quien tiene acceso a la gestión de rutas, colaboradores, asignaciones, asistencia y reportes dentro del sistema.

# Tecnologías utilizadas

Backend

* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* JWT (JSON Web Token) para autenticación
* Swagger para documentación y pruebas de endpoints
* Arquitectura en capas

Frontend

* Vue 3 + Vite
* Vue Router
* Fetch API / Axios
* Bootstrap 5
* Composition API
* Diseño responsivo

# Instrucciones para correr la aplicación

Backend (.NET API)

1. Abrir el proyecto en Visual Studio.
2. Verificar que el archivo appsettings.json tenga configurada correctamente la cadena de conexión DefaultConnection.
3. Asegurarse de que la base de datos esté creada y actualizada.
4. Ejecutar la API presionando F5 o desde terminal: dotnet run
5. Verificar que Swagger esté disponible en: https://localhost:7191/swagger

Frontend (Vue)

1. Entrar a la carpeta del frontend: cd RideFront
2. Instalar dependencias: npm install
3. Ejecutar el servidor de desarrollo: npm run dev
4. Abrir el navegador en la URL generada por Vite, normalmente: http://localhost:5173
