<template>
  <div class="container mt-5">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>Usuarios</h1>
      <!-- Botón para ir al formulario de registro -->
      <router-link 
        v-if="authStore.userRoles.includes('SuperAdmin')" 
        to="/registrar" 
        class="btn btn-primary">
        Registrar Nuevo Usuario
      </router-link>      
      <button class="btn btn-outline-danger" @click="logout">Cerrar Sesión</button>
    </div>
    
    <table class="table table-hover table-striped border">
        <thead class="table-dark">
            <tr>
            <th>Nombre de Usuario</th>
            <th>Roles</th> <!-- Añadimos columna de roles para ver el estado actual -->
            <th>Acciones</th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="user in usuarios" :key="user.id">
                <td>{{ user.username }}</td>
                <td>{{ user.roles.join(', ') }}</td> <!-- Muestra los roles actuales -->
                <td>
                    <router-link :to="{ name: 'usuario-detalle', params: { id: user.id } }" 
                                class="btn btn-sm btn-info me-2">Ver</router-link>
                    
                    <!-- Botón de Cambiar Rol: Solo si es SuperAdmin y no es ya Admin -->
                    <button 
                        v-if="authStore.userRoles.includes('SuperAdmin') && user.roles.includes('User') && !user.roles.includes('SuperAdmin')"
                        @click="cambiarRol(user.id, 'SuperAdmin')"
                        class="btn btn-sm btn-warning">
                        Cambiar rol a Admin
                    </button>
                </td>
            </tr>
        </tbody>
    </table>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useAuthStore } from '../stores/auth';
import { useRouter } from 'vue-router';
import api from '../services/api'; // Asegúrate de importar tu instancia de axios

const authStore = useAuthStore();
const router = useRouter();

// Variable reactiva para almacenar los usuarios
const usuarios = ref([]);

// Función para obtener usuarios de la API
const fetchUsuarios = async () => {
  try {
    const response = await api.get('/usuarios');
    // MIRA ESTE LOG EN LA CONSOLA (F12)
    console.log("Lo que responde la API:", response.data); 
    
    // Si response.data es algo como [{ "Username": "...", "Id": "..." }] 
    // entonces en el template debes usar user.Username y user.Id (con mayúsculas)
    usuarios.value = response.data;
  } catch (error) {
    console.error('Error al obtener usuarios:', error);
  }
};


const cambiarRol = async (userId: string, nuevoRol: string) => {
  if (!confirm(`¿Cambiar rol a ${nuevoRol}?`)) return;

  try {
    // La ruta debe empezar SIN slash si la baseURL ya lo incluye
    // Axios hará: http://localhost:5252/api + /usuarios/...
    await api.post(`/usuarios/${userId}/asignar-rol`, JSON.stringify(nuevoRol), {
      headers: {
        'Content-Type': 'application/json' // OBLIGATORIO para recibir un string en el Body
      }
    });

    alert('Rol actualizado con éxito');
    await fetchUsuarios(); // Recargamos la tabla
  } catch (error: any) {
    console.error('Detalle del error:', error.response?.data);
    alert('Error al cambiar el rol: ' + (error.response?.data || 'Ver consola'));
  }
};

// Ejecutar al montar el componente
onMounted(() => {
  fetchUsuarios();
});

const logout = () => {
  authStore.logout();
  router.push('/login');
};
</script>