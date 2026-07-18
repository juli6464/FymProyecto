<template>
  <div class="container mt-5">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>Usuarios</h1>
      <button class="btn btn-outline-danger" @click="logout">Cerrar Sesión</button>
    </div>
    
    <table class="table table-hover table-striped border">
      <thead class="table-dark">
        <tr>
          <th>ID</th>
          <th>Nombre de Usuario</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <!-- Aquí mapearemos los datos después -->
        <tr>
          <td>1</td>
          <td>superadmin</td>
          <td><button class="btn btn-sm btn-info">Ver</button></td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '../stores/auth';
import { useRouter } from 'vue-router';

const authStore = useAuthStore();
const router = useRouter();

const logout = () => {
  authStore.logout();
  router.push('/login');
};
</script>