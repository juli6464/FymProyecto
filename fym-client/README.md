# Fym.Client - Interfaz de Gestión de Usuarios

Este proyecto es la aplicación cliente para la plataforma **Fym**, desarrollada con **Vue 3** y **TypeScript**. La interfaz está diseñada para consumir de forma segura la API del backend mediante un sistema de autenticación basado en **JWT (JSON Web Tokens)**.

---

## 🛠️ Tecnologías y Herramientas

*   **Framework:** Vue 3 (Composition API + TypeScript)
*   **Gestión de Estado:** Pinia
*   **Enrutamiento:** Vue Router 4
*   **Comunicación API:** Axios (con interceptores de seguridad para tokens Bearer)
*   **UI/Estilos:** Bootstrap 5
*   **Herramienta de Construcción:** Vite

---

## 🚀 Requisitos Previos

*   **Node.js:** Versión 18.0.0 o superior instalada.
*   **npm o yarn:** Gestor de paquetes configurado.
*   **Backend Activo:** El proyecto [Fym.Backend](https://github.com/juli6464/FymProyecto/tree/main/Fym.Backend) debe estar ejecutándose previamente para poder realizar la autenticación.

---

## ⚡ Instalación y Ejecución

Sigue estos pasos en tu terminal posicionándote en la raíz del proyecto frontend (`C:\xampp\htdocs\FymProyecto\fym-client`):

### 1. Instalar dependencias
Instala todos los paquetes necesarios definidos en el `package.json`:
```bash
npm install
