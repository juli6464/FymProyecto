import { createRouter, createWebHistory } from 'vue-router';
import LoginView from '../views/LoginView.vue';

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: LoginView
    },
    {
      path: '/usuarios',
      name: 'usuarios',
      // Aquí cargaremos después la vista de lista de usuarios
      component: () => import('../views/UsuariosView.vue') 
    },
    {
      path: '/',
      redirect: '/login'
    }
  ]
});

export default router;