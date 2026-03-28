import { createRouter, createWebHistory } from 'vue-router'
import PublicHome from '../views/PublicHome.vue'
import Login from '../views/Login.vue'
import Dashboard from '../views/Dashboard.vue'
import Routes from '../views/Routes.vue'
import Employees from '../views/Employees.vue'
import Assignments from '../views/Assignments.vue'
import Attendance from '../views/Attendance.vue'
import Reports from '../views/Reports.vue'
import { isAuthenticated } from '../services/auth.service'

const routes = [
    {
        path: '/',
        component: PublicHome
    },
    {
        path: '/login',
        component: Login
    },
    {
        path: '/dashboard',
        component: Dashboard,
        meta: { requiresAuth: true }
    },
    {
        path: '/routes',
        component: Routes,
        meta: { requiresAuth: true }
    },
    {
        path: '/employees',
        component: Employees,
        meta: { requiresAuth: true }
    },
    {
        path: '/assignments',
        component: Assignments,
        meta: { requiresAuth: true }
    },
    {
        path: '/attendance',
        component: Attendance,
        meta: { requiresAuth: true }
    },
    {
        path: '/reports',
        component: Reports,
        meta: { requiresAuth: true }
    }
]

const router = createRouter({
    history: createWebHistory(),
    routes
})

router.beforeEach((to, from, next) => {
    if (to.meta.requiresAuth && !isAuthenticated()) {
        next('/login')
        return
    }

    if (to.path === '/login' && isAuthenticated()) {
        next('/dashboard')
        return
    }

    next()
})

export default router