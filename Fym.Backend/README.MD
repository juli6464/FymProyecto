# Fym.Backend - API de Autenticación y Usuarios

Este proyecto es el backend para la plataforma **Fym**, desarrollado bajo una arquitectura desacoplada, garantizando un manejo seguro de identidades y una persistencia de datos robusta.

El sistema implementa un Middleware global de manejo de excepciones para garantizar respuestas estructuradas ante cualquier fallo.

## Tecnologías Usadas
* **Framework:** .NET 9 / .NET 8 (ASP.NET Core Web API)
* **Persistencia:** Entity Framework Core (Code-First)
* **Base de Datos:** SQL Server
* **Seguridad:** JWT (Roles/Claims) y BCrypt.Net-Next (Hashing)
* **Documentación:** Swagger / OpenAPI

## URLs Principales
* **API Base:** `http://localhost:5252`
* **Swagger UI:** `http://localhost:5252/swagger/index.html`

## Requisitos Previos
* .NET SDK 8.0 o superior.
* SQL Server activo.
* Herramientas de EF Core: `dotnet tool install --global dotnet-ef`

## Pasos para Instalar y Ejecutar
1. **Restaurar dependencias:** `dotnet restore`
2. **Configurar BD:** Edita `appsettings.json` con tu cadena de conexión.
3. **Aplicar Migraciones:** `dotnet ef database update`
4. **Ejecutar:** `dotnet run`

## Documentación de Endpoints

### 1. Autenticación (Público)
* `POST /api/auth/login`: Autentica al usuario y retorna un token JWT.

### 2. Gestión de Usuarios (Protegidos)
* `POST /api/usuarios/registrar`: Registra un nuevo usuario (**Solo SuperAdmin**).
* `POST /api/usuarios/{id}/asignar-rol`: Asigna un rol específico, eliminando los anteriores (**Solo SuperAdmin**).
* `GET /api/usuarios`: Obtiene la lista completa de usuarios con sus roles (**Autenticado**).
* `GET /api/usuarios/{id}`: Obtiene los detalles de un usuario específico por su ID (**Autenticado**).

> **Nota:** Todos los endpoints de usuarios requieren el header `Authorization: Bearer {token}`.

## Pruebas con Swagger UI
1. Abre `http://localhost:5252/swagger/index.html`.
2. Llama a `POST /api/auth/login` con:
   ```json
   { "usernameOrEmail": "superadmin", "password": "Admin123*" }