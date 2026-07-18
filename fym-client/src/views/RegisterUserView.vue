<template>
  <div class="container mt-5">      
    <div class="card shadow p-4" style="max-width: 500px; margin: auto;">
        <button class="btn btn-outline-danger" @click="logout">Cerrar Sesión</button>
        <h3 class="mb-4 pt-2">Registrar Nuevo Usuario</h3>
      
      <form @submit.prevent="registerUser">
        <div class="mb-3">
            <label>Nombre de Usuario</label>
            <!-- Usa form.Username en lugar de form.username -->
            <input v-model="form.Username" class="form-control" required />
        </div>
        <div class="mb-3">
            <label>Correo Electrónico</label>
            <input v-model="form.Email" type="email" class="form-control" required />
        </div>
        <div class="mb-3">
            <label>Contraseña</label>
            <input v-model="form.Password" type="password" class="form-control" required />
        </div>
        <button type="submit" class="btn btn-primary w-100">Registrar Usuario</button>
      </form>
      
      <div v-if="message" class="alert mt-3" :class="isError ? 'alert-danger' : 'alert-success'">
        {{ message }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router'; // Importar router
import api from '../services/api';
import { useAuthStore } from '../stores/auth';

const router = useRouter(); // Inicializar router
const form = ref({
  Username: '',
  Email: '',
  Password: ''
});

const message = ref('');
const isError = ref(false);

const authStore = useAuthStore();
const logout = () => {
  authStore.logout();
  router.push('/login');
};

const registerUser = async () => {
  try {
    // La petición debe coincidir con el DTO (Username, Email, Password)
    await api.post('/usuarios/registrar', form.value);
    
    message.value = 'Usuario registrado correctamente. Redirigiendo...';
    isError.value = false;

    // Redirección automática a la lista de usuarios tras 1.5 segundos
    setTimeout(() => {
      router.push('/usuarios');
    }, 1500);
    
  } catch (error: any) {
    console.log(error.response?.data);
    message.value = 'Error al registrar usuario. Verifica los campos.';
    isError.value = true;
  }
};
</script>