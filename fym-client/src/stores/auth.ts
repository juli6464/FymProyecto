import { defineStore } from 'pinia';
import api from '../services/api';

export const useAuthStore = defineStore('auth', {
  // 1. EL ESTADO: Esto permite que los componentes accedan a estos datos
  state: () => ({
    token: localStorage.getItem('token') || null,
    userRoles: JSON.parse(localStorage.getItem('userRoles') || '[]')
  }),

  actions: {
    async login(credentials: any) {
      const response = await api.post('/auth/login', credentials);
      
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('userRoles', JSON.stringify(response.data.roles));
      
      // Actualizamos el estado de Pinia
      this.token = response.data.token;
      this.userRoles = response.data.roles;
    },
    
    logout() {
      localStorage.removeItem('token');
      localStorage.removeItem('userRoles');
      this.token = null;
      this.userRoles = [];
    }
  }
});