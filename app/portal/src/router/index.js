import { createRouter, createWebHistory } from 'vue-router';
import Login from '../components/Login.vue'; // 导入登录组件
import TodoList from '../components/TodoList.vue'; // 导入待办事项组件
import Register from '../components/Register.vue';
import Profile from '../components/Profile.vue';
import ChangePassword from '../components/ChangePassword.vue'
import CategoryManagement from '../components/CategoryManagement.vue'
import { getAccessToken } from '@/tools/auth';


const routes = [
    {
        path: '/login',
        name: 'Login',
        component: Login
    },
    {
        path: '/register',
        name: 'Register',
        component: Register
    },
    {
        path: '/',
        name: 'TodoList',
        component: TodoList,
        meta: { requiresAuth: true }
    },
    {
        path: '/profile',
        name: 'profile',
        component: Profile,
        meta: { requiresAuth: true }
    },
    {
        path: '/categories',
        name: 'Categories',
        component: CategoryManagement,
        meta: { requiresAuth: true }
    },
    {
        path: '/change-password',
        name: 'ChangePassword',
        component: ChangePassword,
        meta: { requiresAuth: true }
    }
];

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes
});

// 全局路由守卫
router.beforeEach((to, form, next) => {
    if (to.matched.some(record => record.meta.requiresAuth)) {
        const isLoggedIn = getAccessToken();
        if (!isLoggedIn) {
            next({ name: 'Login' });
        }
        else {
            next();
        }
    }
    else {
        next();
    }
}
)

export default router;
