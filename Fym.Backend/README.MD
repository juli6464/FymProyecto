# Fym.Backend - API de Autenticación y Usuarios

Este proyecto es el backend para la plataforma **Fym**, desarrollado utilizando una arquitectura limpia y desacoplada, garantizando un manejo seguro de identidades y una persistencia de datos robusta.

El sistema cuenta con un Middleware global de manejo de excepciones estandarizado bajo el formato **RFC 7807 (Problem Details)** para garantizar respuestas limpias, estructuradas y controladas ante cualquier fallo del servidor.

---

## 🛠️ Tecnologías Usadas

*   **Framework Principal:** .NET 9 / .NET 8 (ASP.NET Core Web API)
*   **Persistencia y ORM:** Entity Framework Core (Code-First)
*   **Base de Datos:** SQL Server (Compatible con instancias locales o contenedores Docker)
*   **Seguridad y Autenticación:** 
    *   **JWT (JSON Web Tokens):** Firma digital y manejo de roles mediante Claims para la protección de rutas.
    *   **BCrypt.Net-Next:** Encriptación y hashing seguro de contraseñas.
*   **Documentación:** Swagger / OpenAPI (Configurado con soporte para esquemas de seguridad Bearer).

---

## 🚀 URLs Principales del Proyecto

*   **Servidor Base de la API:** `http://localhost:5252`
*   **Documentación Interactiva (Swagger UI):** `http://localhost:5252/swagger/index.html`

---

## 💻 Requisitos Previos

*   **.NET SDK** (Versión 8.0 o superior instalado en la máquina)
*   **SQL Server** activo (Local o mediante un contenedor Docker)
*   **Herramienta de EF Core instalada:** Si nunca has ejecutado migraciones en tu equipo, instala las herramientas globales ejecutando:
    ```bash
    dotnet tool install --global dotnet-ef
    ```

---

## ⚡ Pasos para Instalar y Ejecutar el Proyecto

Sigue estos pasos en tu terminal posicionándote en la raíz del proyecto backend (`C:\xampp\htdocs\FymProyecto\Fym.Backend\Fym.Api`):

### 1. Restaurar dependencias
Descarga todos los paquetes NuGet requeridos por la solución:
```bash
dotnet restore
2. Configurar la Base de Datos
Abre el archivo appsettings.json en la raíz del proyecto y asegúrate de modificar la propiedad DefaultConnection dentro de ConnectionStrings con las credenciales de tu instancia de SQL Server:

JSON
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=FymDb;User Id=sa;Password=TuPasswordSeguro123*;TrustServerCertificate=True;"
}
3. Crear la Base de Datos y Aplicar Migraciones
Ejecuta el siguiente comando para leer el contexto de Entity Framework, estructurar las tablas automáticamente en tu servidor de base de datos y correr el Data Seeder que inyectará el usuario administrador inicial:

Bash
dotnet ef database update
4. Compilar y Levantar la API
Una vez que las tablas y el seeder se hayan procesado con éxito, inicia el servidor de desarrollo local de .NET:

Bash
dotnet run
🔒 Documentación de Endpoints (Rutas Principales)
1. Módulo de Autenticación (Acceso Público)
POST /api/auth/login

Descripción: Autentica a un usuario del sistema y genera un token JWT válido.

Cuerpo de la Petición (JSON):

JSON
{
  "usernameOrEmail": "superadmin",
  "password": "Admin123*"
}
Respuesta Exitosa (200 OK): Retorna el token de acceso (accessToken) y la información básica del usuario autenticado.

2. Módulo de Usuarios (Protegidos por JWT)
GET /api/usuarios

Descripción: Recupera la lista asíncrona de los usuarios registrados en el sistema. Omite los campos sensibles de seguridad (PasswordHash).

Seguridad: Requiere cabecera HTTP Authorization: Bearer {token}.

GET /api/usuarios/perfil

Descripción: Extrae y expone los claims de identidad (ID de usuario, Username) contenidos directamente en la carga útil del Token JWT enviado por el cliente.

Seguridad: Requiere cabecera HTTP Authorization: Bearer {token}.

🧪 Pruebas a través de Swagger UI
Ingresa a http://localhost:5252/swagger/index.html desde tu navegador.

Despliega el endpoint POST /api/auth/login y haz clic en Try it out. Envía las credenciales por defecto (superadmin / Admin123*) para generar un token.

Copia el string largo del campo token que te devuelve la respuesta (sin las comillas).

Sube al inicio de la página de Swagger y haz clic en el botón "Authorize" (icono de candado verde).

Pega directamente el token copiado en el campo de texto (el esquema HTTP Bearer ya se encuentra preconfigurado y se inyectará automáticamente por debajo).

Haz clic en Authorize y cierra la ventana. Ahora todos los candados de las rutas protegidas se mostrarán cerrados y podrás probar los métodos GET sin recibir un error 401 Unauthorized.

🌐 Configuración de CORS para el Frontend
La API restringe el acceso externo pero tiene habilitada una política específica denominada VueClientPolicy para dar soporte al entorno de desarrollo del cliente en Vue 3 + Vite:

Origen permitido: http://localhost:5173