<template>
  <div class="container mt-5">
    <div v-if="usuario" class="card p-4 shadow">
      <h1>Detalle de Usuario</h1>
      <hr>
      <p><strong>Nombre de Usuario:</strong> {{ usuario.username || usuario.Username }}</p>
      <p><strong>Correo:</strong> {{ usuario.email || usuario.Email }}</p>
      <p><strong>Roles: </strong> 
         <span class="badge bg-primary">{{ usuario.roles ? usuario.roles.join(', ') : 'Sin roles' }}</span>
      </p>
      
      <button class="btn btn-secondary mt-3" @click="$router.push('/usuarios')">Volver</button>
    </div>
    <div v-else class="text-center mt-5">
      <div class="spinner-border" role="status"></div>
      <p>Cargando información...</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import api from '../services/api';

const route = useRoute();
const usuario = ref(null);

onMounted(async () => {
  const userId = route.params.id;
  console.log("ID capturado de la URL:", userId); // Mira esto en la consola
  
  try {
    const response = await api.get(`/usuarios/${userId}`);
    usuario.value = response.data;
  } catch (error) {
    console.error("Error al cargar usuario:", error);
  }
});
</script>