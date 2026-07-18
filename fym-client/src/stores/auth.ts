import { defineStore } from 'pinia';
import api from '../services/api';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('fym_token') || null,
    user: null as any,
  }),
  actions: {
    async login(credentials: any) {
      const response = await api.post('/auth/login', credentials);
      this.token = response.data.token;
      localStorage.setItem('fym_token', this.token!);
      api.defaults.headers.common['Authorization'] = `Bearer ${this.token}`;
    },
    logout() {
      this.token = null;
      localStorage.removeItem('fym_token');
      delete api.defaults.headers.common['Authorization'];
    }
  }
});