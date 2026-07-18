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
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <!-- El v-for crea una fila por cada usuario -->
        <tr v-for="user in usuarios" :key="user.id">
            <td>{{ user.username }}</td>
            <td>
            <!-- Redirige a /usuarios/1, /usuarios/2, etc. -->
            <router-link :to="{ name: 'usuario-detalle', params: { id: user.id } }" 
                        class="btn btn-sm btn-info">
                Ver
            </router-link>
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
// Ejecutar al montar el componente
onMounted(() => {
  fetchUsuarios();
});

const logout = () => {
  authStore.logout();
  router.push('/login');
};
</script>