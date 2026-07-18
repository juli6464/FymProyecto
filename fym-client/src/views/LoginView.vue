<template>
  <div class="container d-flex justify-content-center align-items-center min-vh-100">
    <div class="card p-4 shadow" style="width: 100%; max-width: 400px;">
        <h1 class="text-center text-dark mb-4">FYM TECHNOLOGY</h1>
        <h2 class="text-center text-dark mb-4">Iniciar Sesión</h2>
      <form @submit.prevent="handleLogin">
        <div class="mb-3">
          <label class="form-label">Usuario</label>
          <input v-model="form.usernameOrEmail" type="text" class="form-control" required />
        </div>
        <div class="mb-3">
          <label class="form-label">Contraseña</label>
          <input v-model="form.password" type="password" class="form-control" required />
        </div>
        <button type="submit" class="btn btn-primary w-100">Entrar</button>
      </form>
      <p v-if="error" class="text-danger mt-3 text-center">{{ error }}</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import { useAuthStore } from '../stores/auth';
import { useRouter } from 'vue-router';

const authStore = useAuthStore();
const router = useRouter();
const error = ref('');

const form = reactive({
  usernameOrEmail: '',
  password: ''
});

const handleLogin = async () => {
  try {
    error.value = '';
    // Al ejecutar esto, el store ya guarda el token y roles en localStorage
    await authStore.login(form); 
    
    // Ahora, cuando el router haga el check en beforeEach, 
    // encontrará el token y los roles y te permitirá pasar.
    router.push('/usuarios');
  } catch (err: any) {
    error.value = 'Credenciales incorrectas o servidor no responde.';
  }
};
</script>