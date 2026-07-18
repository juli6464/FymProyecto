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
      path: '/registrar',
      name: 'registrar',
      component: () => import('../views/RegisterUserView.vue'),
      meta: { requiresAuth: true, roles: ['SuperAdmin'] } // Opcional: proteger aquí también
    },
    {
      path: '/usuarios/:id', // :id es el parámetro dinámico
      name: 'usuario-detalle',
      component: () => import('../views/DataView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/',
      redirect: '/login'
    }
  ]
});

router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token');
  const userRoles = JSON.parse(localStorage.getItem('userRoles') || '[]');

  console.log("Intentando ir a:", to.path);
  console.log("Token existente:", !!token);
  console.log("Roles del usuario:", userRoles);

  if (to.meta.requiresAuth && !token) {
    next('/login');
  } else if (to.meta.roles && !to.meta.roles.some(role => userRoles.includes(role))) {
    next('/usuarios'); 
  } else {
    next();
  }
});

export default router;