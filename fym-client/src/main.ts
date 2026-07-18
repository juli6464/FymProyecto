import 'bootstrap/dist/css/bootstrap.min.css';
import { createApp } from 'vue';
// import './style.css'
import { createPinia } from 'pinia';
import App from './App.vue';
import router from './router'; // Importa el router que acabamos de crear

const app = createApp(App);

app.use(createPinia());
app.use(router); // Registrar el router
app.mount('#app');