<template>
    <div class="attendance-page" @click="closeUserMenu">
        <header class="topbar">
            <div class="topbar-left">
                <img src="../assets/images/RF.png" alt="RideFlow" class="brand-logo" />
            </div>

            <nav class="topbar-nav">
                <router-link to="/dashboard" class="nav-link" :class="{ active: isActive('/dashboard') }">Dashboard</router-link>
                <router-link to="/routes" class="nav-link" :class="{ active: isActive('/routes') }">Rutas</router-link>
                <router-link to="/employees" class="nav-link" :class="{ active: isActive('/employees') }">Colaboradores</router-link>
                <router-link to="/assignments" class="nav-link" :class="{ active: isActive('/assignments') }">Asignaciones</router-link>
                <router-link to="/attendance" class="nav-link" :class="{ active: isActive('/attendance') }">Asistencia</router-link>
                <router-link to="/reports" class="nav-link" :class="{ active: isActive('/reports') }">Reportes</router-link>
            </nav>

            <div class="user-menu-wrapper" @click.stop>
                <button class="user-chip user-chip-button" type="button" @click="toggleUserMenu">
                    <div class="avatar-wrap">
                        <img :src="avatarUrl" :alt="displayName" class="avatar" />
                    </div>
                    <span class="user-name">{{ displayName }}</span>
                    <span class="user-caret" :class="{ open: isUserMenuOpen }">▼</span>
                </button>

                <transition name="dropdown-fade">
                    <div v-if="isUserMenuOpen" class="user-dropdown">
                        <button class="dropdown-item logout" type="button" @click="handleLogout">
                            Cerrar sesión
                        </button>
                    </div>
                </transition>
            </div>
        </header>

        <main class="attendance-content">
            <section class="attendance-hero">
                <div class="hero-copy">
                    <h1>Control de Asistencia</h1>
                    <p>Registre la asistencia diaria de colaboradores por ruta</p>
                </div>
            </section>

            <section class="filters-grid">
                <article class="filter-card">
                    <h3>Seleccionar Ruta</h3>
                    <div class="select-wrap">
                        <select v-model="selectedRouteId">
                            <option value="">Seleccione una ruta</option>
                            <option v-for="routeItem in routes"
                                    :key="routeItem.id"
                                    :value="String(routeItem.id)">
                                {{ routeItem.origin }} → {{ routeItem.destination }}
                            </option>
                        </select>
                        <span class="select-arrow">⌄</span>
                    </div>
                </article>

                <article class="filter-card">
                    <h3>Fecha</h3>
                    <div class="date-wrap">
                        <input v-model="selectedDate" type="date" />
                        <span class="date-icon">
                            <svg viewBox="0 0 24 24" fill="none" aria-hidden="true">
                                <rect x="4" y="5" width="16" height="15" rx="2" stroke="currentColor" stroke-width="2" />
                                <path d="M8 3V7" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
                                <path d="M16 3V7" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
                                <path d="M4 10H20" stroke="currentColor" stroke-width="2" />
                            </svg>
                        </span>
                    </div>
                </article>

                <article class="filter-card info-card">
                    <h3>Información</h3>
                    <p>{{ infoText }}</p>
                </article>
            </section>

            <section class="attendance-panel">
                <div v-if="!selectedRouteId" class="empty-state">
                    <div class="empty-icon">
                        <svg viewBox="0 0 64 64" fill="none" aria-hidden="true">
                            <rect x="16" y="18" width="32" height="30" rx="4" stroke="currentColor" stroke-width="4" />
                            <path d="M16 28H48" stroke="currentColor" stroke-width="4" />
                            <path d="M24 12V22" stroke="currentColor" stroke-width="4" stroke-linecap="round" />
                            <path d="M40 12V22" stroke="currentColor" stroke-width="4" stroke-linecap="round" />
                        </svg>
                    </div>
                    <p>Seleccione una ruta para registrar asistencia</p>
                </div>

                <div v-else class="attendance-list">
                    <div class="attendance-list-header">
                        <div>
                            <h3>{{ selectedRouteLabel }}</h3>
                            <p>{{ formattedDateLabel }}</p>
                        </div>
                    </div>

                    <div v-if="loading" class="attendance-loading">
                        Cargando asistencia...
                    </div>

                    <div v-else-if="employeesForSelectedRoute.length === 0" class="attendance-empty">
                        No hay colaboradores asignados a esta ruta
                    </div>

                    <div v-else class="attendance-items">
                        <article v-for="employee in employeesForSelectedRoute"
                                 :key="employee.id"
                                 class="attendance-item">
                            <div class="attendance-employee">
                                <div class="employee-avatar">
                                    {{ initials(employee.name) }}
                                </div>
                                <div class="employee-copy">
                                    <h4>{{ employee.name }}</h4>
                                    <p>{{ employee.department || 'Sin departamento' }}</p>
                                </div>
                            </div>

                            <label class="attendance-switch">
                                <input type="checkbox"
                                       :checked="attendanceMap[employee.id] === true"
                                       @change="toggleAttendance(employee.id, ($event.target as HTMLInputElement).checked)" />
                                <span class="slider"></span>
                                <span class="switch-label">
                                    {{ attendanceMap[employee.id] ? 'Presente' : 'Ausente' }}
                                </span>
                            </label>
                        </article>
                    </div>
                </div>
            </section>
        </main>

        <div class="background-overlay">
            <div class="glow glow-1"></div>
            <div class="glow glow-2"></div>
        </div>

        <div class="bottom-scene">
            <div class="sparkles-layer"></div>

            <div class="wave-stack">
                <div class="wave wave-1"></div>
                <div class="wave wave-2"></div>
                <div class="wave wave-3"></div>
                <div class="wave wave-4"></div>
                <div class="wave wave-5"></div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { apiFetch } from '../services/api'
import { getUser, logout } from '../services/auth.service'

type StoredUser = {
    name?: string
    fullName?: string
    username?: string
    userName?: string
}

type RouteItem = {
    id: number | string
    origin: string
    destination: string
    schedule?: string
    driver?: string
}

type EmployeeItem = {
    id: number | string
    name: string
    department?: string
    routeId?: number | string | null
}

const route = useRoute()
const router = useRouter()

const isUserMenuOpen = ref(false)
const user = ref<StoredUser | null>(null)
const loading = ref(false)

const routes = ref<RouteItem[]>([])
const employees = ref<EmployeeItem[]>([])

const selectedRouteId = ref('')
const selectedDate = ref(new Date().toISOString().slice(0, 10))
const attendanceMap = ref<Record<string | number, boolean>>({})

const displayName = computed(() => {
    return (
        user.value?.name ||
        user.value?.fullName ||
        user.value?.username ||
        user.value?.userName ||
        'Usuario'
    )
})

const avatarUrl = computed(() => {
    const name = encodeURIComponent(displayName.value)
    return `https://ui-avatars.com/api/?name=${name}&background=EAF0FF&color=183A8F&bold=true`
})

const infoText = computed(() => {
    if (!selectedRouteId.value) {
        return 'Seleccione una ruta para ver detalles'
    }

    return `Fecha seleccionada: ${formattedDateLabel.value}`
})

const selectedRoute = computed(() => {
    return routes.value.find(x => String(x.id) === String(selectedRouteId.value)) || null
})

const selectedRouteLabel = computed(() => {
    if (!selectedRoute.value) return ''
    return `${selectedRoute.value.origin} → ${selectedRoute.value.destination}`
})

const employeesForSelectedRoute = computed(() => {
    if (!selectedRouteId.value) return []
    return employees.value.filter(x => String(x.routeId ?? '') === String(selectedRouteId.value))
})

const formattedDateLabel = computed(() => {
    if (!selectedDate.value) return ''
    const [year, month, day] = selectedDate.value.split('-')
    return `${day}/${month}/${year}`
})

const isActive = (path: string) => route.path === path

const toggleUserMenu = () => {
    isUserMenuOpen.value = !isUserMenuOpen.value
}

const closeUserMenu = () => {
    isUserMenuOpen.value = false
}

const handleLogout = () => {
    logout()
    closeUserMenu()
    router.push('/login')
}

const initials = (name: string) => {
    return name
        .split(' ')
        .filter(Boolean)
        .slice(0, 2)
        .map(part => part[0]?.toUpperCase() || '')
        .join('')
}

const normalizeRoutes = (data: any): RouteItem[] => {
    const list = Array.isArray(data)
        ? data
        : Array.isArray(data?.items)
            ? data.items
            : Array.isArray(data?.data)
                ? data.data
                : []

    return list.map((item: any, index: number) => ({
        id: item.id ?? item.routeId ?? item.Id ?? item.IdRoute ?? index + 1,
        origin: item.origin ?? item.origen ?? item.startLocation ?? item.from ?? 'Sin origen',
        destination: item.destination ?? item.destino ?? item.endLocation ?? item.to ?? 'Sin destino',
        schedule: item.schedule ?? item.horario ?? item.time ?? item.departureTime ?? '',
        driver: item.driverName ?? item.driver ?? item.conductor ?? item.employeeName ?? ''
    }))
}

const normalizeEmployees = (data: any): EmployeeItem[] => {
    const list = Array.isArray(data)
        ? data
        : Array.isArray(data?.items)
            ? data.items
            : Array.isArray(data?.data)
                ? data.data
                : []

    return list.map((item: any, index: number) => ({
        id: item.id ?? item.employeeId ?? item.Id ?? item.IdEmployee ?? index + 1,
        name: item.name ?? item.nombre ?? item.fullName ?? 'Sin nombre',
        department: item.department ?? item.departamento ?? item.area ?? '',
        routeId: item.routeId ?? item.idRoute ?? item.IdRoute ?? item.assignedRouteId ?? null
    }))
}

const loadData = async () => {
    loading.value = true

    try {
        user.value = getUser()

        const [routesResponse, employeesResponse] = await Promise.all([
            apiFetch('/Routes'),
            apiFetch('/Employees')
        ])

        if (routesResponse?.ok) {
            const routesData = await routesResponse.json()
            routes.value = normalizeRoutes(routesData)
        } else {
            routes.value = []
        }

        if (employeesResponse?.ok) {
            const employeesData = await employeesResponse.json()
            employees.value = normalizeEmployees(employeesData)
        } else {
            employees.value = []
        }
    } catch (error) {
        console.error('Error cargando asistencia:', error)
        routes.value = []
        employees.value = []
    } finally {
        loading.value = false
    }
}

const toggleAttendance = async (employeeId: number | string, checked: boolean) => {
    attendanceMap.value = {
        ...attendanceMap.value,
        [employeeId]: checked
    }

    try {
        await apiFetch('/Attendance', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                employeeId,
                routeId: selectedRouteId.value ? Number(selectedRouteId.value) : null,
                date: selectedDate.value,
                present: checked
            })
        }).catch(() => null)
    } catch (error) {
        console.error('Error registrando asistencia:', error)
    }
}

onMounted(loadData)
</script>

<style scoped>
    .attendance-page {
        min-height: 100vh;
        position: relative;
        overflow: hidden;
        background: radial-gradient(circle at top left, rgba(255, 255, 255, 0.98), rgba(244, 245, 251, 0.96) 38%, rgba(232, 237, 248, 0.98) 100%);
        color: #24345f;
    }

    .topbar {
        height: 72px;
        background: rgba(255, 255, 255, 0.74);
        backdrop-filter: blur(14px);
        box-shadow: 0 10px 30px rgba(42, 63, 122, 0.08);
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 0 22px;
        position: relative;
        z-index: 20;
        border-bottom: 1px solid rgba(220, 227, 244, 0.9);
    }

    .topbar-left {
        display: flex;
        align-items: center;
        min-width: 140px;
    }

    .brand-logo {
        height: 42px;
        object-fit: contain;
        display: block;
    }

    .topbar-nav {
        display: flex;
        align-items: center;
        gap: 34px;
    }

    .nav-link {
        position: relative;
        font-size: 15px;
        font-weight: 500;
        color: #3e4f86;
        text-decoration: none;
        transition: color 0.2s ease, transform 0.2s ease;
    }

        .nav-link:hover {
            color: #214ab6;
            transform: translateY(-1px);
        }

        .nav-link.active {
            color: #1f3f98;
            font-weight: 800;
        }

            .nav-link.active::after {
                content: '';
                position: absolute;
                left: 0;
                right: 0;
                bottom: -12px;
                height: 3px;
                border-radius: 999px;
                background: linear-gradient(90deg, #1f41a8 0%, #4f8ef7 100%);
            }

    .user-menu-wrapper {
        position: relative;
        display: flex;
        align-items: center;
    }

    .user-chip {
        height: 48px;
        padding: 0 14px 0 8px;
        border-radius: 999px;
        background: linear-gradient(180deg, #f8f9fe 0%, #eef2fb 100%);
        border: 1px solid rgba(219, 226, 243, 0.95);
        display: flex;
        align-items: center;
        gap: 10px;
        box-shadow: 0 8px 20px rgba(58, 79, 135, 0.08);
        min-width: 170px;
        justify-content: flex-end;
    }

    .user-chip-button {
        cursor: pointer;
        outline: none;
        transition: transform 0.2s ease, box-shadow 0.2s ease;
    }

        .user-chip-button:hover {
            transform: translateY(-1px);
            box-shadow: 0 10px 22px rgba(58, 79, 135, 0.12);
        }

    .avatar-wrap {
        width: 34px;
        height: 34px;
        border-radius: 50%;
        overflow: hidden;
        background: #fff;
        box-shadow: 0 4px 10px rgba(60, 82, 141, 0.12);
        flex-shrink: 0;
    }

    .avatar {
        width: 100%;
        height: 100%;
        display: block;
    }

    .user-name {
        font-size: 14px;
        font-weight: 700;
        color: #33477d;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
        max-width: 92px;
    }

    .user-caret {
        font-size: 10px;
        color: #7d88a8;
        flex-shrink: 0;
        transition: transform 0.2s ease;
    }

        .user-caret.open {
            transform: rotate(180deg);
        }

    .user-dropdown {
        position: absolute;
        top: calc(100% + 10px);
        right: 0;
        width: 190px;
        padding: 8px;
        border-radius: 16px;
        background: rgba(255, 255, 255, 0.96);
        border: 1px solid rgba(219, 226, 243, 0.95);
        box-shadow: 0 18px 34px rgba(52, 72, 127, 0.14);
        backdrop-filter: blur(14px);
        z-index: 30;
    }

    .dropdown-item {
        width: 100%;
        border: none;
        background: transparent;
        border-radius: 12px;
        padding: 12px 14px;
        text-align: left;
        font-size: 14px;
        font-weight: 600;
        color: #33477d;
        cursor: pointer;
        transition: background 0.2s ease, color 0.2s ease;
    }

        .dropdown-item:hover {
            background: #f4f7ff;
            color: #1f3f98;
        }

        .dropdown-item.logout:hover {
            background: #fff4f4;
            color: #d44848;
        }

    .dropdown-fade-enter-active,
    .dropdown-fade-leave-active {
        transition: opacity 0.18s ease, transform 0.18s ease;
    }

    .dropdown-fade-enter-from,
    .dropdown-fade-leave-to {
        opacity: 0;
        transform: translateY(-6px);
    }

    .attendance-content {
        position: relative;
        z-index: 8;
        max-width: 1180px;
        margin: 0 auto;
        padding: 28px 28px 110px;
    }

    .attendance-hero {
        margin-bottom: 26px;
    }

    .hero-copy h1 {
        margin: 0 0 8px;
        font-size: 28px;
        line-height: 1.05;
        font-weight: 800;
        letter-spacing: -0.02em;
        color: #253f82;
    }

    .hero-copy p {
        margin: 0;
        font-size: 16px;
        color: #435786;
        font-weight: 600;
    }

    .filters-grid {
        display: grid;
        grid-template-columns: 1.1fr 1fr 0.95fr;
        gap: 12px;
        margin-bottom: 14px;
    }

    .filter-card {
        min-height: 102px;
        border-radius: 12px;
        border: 1px solid rgba(205, 213, 239, 0.88);
        background: linear-gradient(180deg, rgba(255, 255, 255, 0.92) 0%, rgba(245, 247, 255, 0.9) 100%);
        box-shadow: 0 10px 22px rgba(59, 76, 132, 0.08);
        padding: 16px 18px;
    }

        .filter-card h3 {
            margin: 0 0 18px;
            font-size: 15px;
            font-weight: 800;
            color: #253a72;
        }

    .info-card p {
        margin: 0;
        font-size: 14px;
        color: #62719b;
        line-height: 1.5;
    }

    .select-wrap,
    .date-wrap {
        position: relative;
    }

        .select-wrap select,
        .date-wrap input {
            width: 100%;
            height: 28px;
            border-radius: 6px;
            border: 1px solid rgba(188, 199, 234, 0.94);
            background: linear-gradient(180deg, rgba(247, 247, 255, 0.96) 0%, rgba(242, 244, 255, 0.98) 100%);
            padding: 0 32px 0 12px;
            font-size: 12px;
            color: #4b5c89;
            outline: none;
            appearance: none;
            transition: border-color 0.2s ease, box-shadow 0.2s ease;
        }

            .select-wrap select:focus,
            .date-wrap input:focus {
                border-color: #6d92f7;
                box-shadow: 0 0 0 3px rgba(101, 137, 242, 0.12);
            }

    .select-arrow {
        position: absolute;
        right: 10px;
        top: 50%;
        transform: translateY(-50%);
        color: #a2adc9;
        font-size: 14px;
        pointer-events: none;
    }

    .date-icon {
        position: absolute;
        right: 10px;
        top: 50%;
        transform: translateY(-50%);
        width: 13px;
        height: 13px;
        color: #a2adc9;
        pointer-events: none;
    }

        .date-icon svg {
            width: 100%;
            height: 100%;
            display: block;
        }

    .attendance-panel {
        min-height: 280px;
        border-radius: 14px;
        border: 1px solid rgba(205, 213, 239, 0.88);
        background: linear-gradient(180deg, rgba(255, 255, 255, 0.92) 0%, rgba(246, 248, 255, 0.9) 100%);
        box-shadow: 0 12px 24px rgba(59, 76, 132, 0.08);
        display: flex;
        align-items: center;
        justify-content: center;
        overflow: hidden;
    }

    .empty-state {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        color: #aeb8d8;
        text-align: center;
    }

    .empty-icon {
        width: 54px;
        height: 54px;
        margin-bottom: 14px;
        color: #bcc5df;
    }

        .empty-icon svg {
            width: 100%;
            height: 100%;
            display: block;
        }

    .empty-state p {
        margin: 0;
        font-size: 16px;
        color: #7b88ac;
    }

    .attendance-list {
        width: 100%;
        height: 100%;
        padding: 20px;
    }

    .attendance-list-header {
        margin-bottom: 18px;
    }

        .attendance-list-header h3 {
            margin: 0 0 6px;
            font-size: 18px;
            color: #253a72;
        }

        .attendance-list-header p {
            margin: 0;
            font-size: 13px;
            color: #69789d;
        }

    .attendance-loading,
    .attendance-empty {
        min-height: 140px;
        display: flex;
        align-items: center;
        justify-content: center;
        color: #6f7d9e;
        font-weight: 700;
    }

    .attendance-items {
        display: grid;
        gap: 12px;
    }

    .attendance-item {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 16px;
        padding: 14px 16px;
        border-radius: 12px;
        background: rgba(255, 255, 255, 0.78);
        border: 1px solid rgba(214, 221, 241, 0.9);
    }

    .attendance-employee {
        display: flex;
        align-items: center;
        gap: 12px;
    }

    .employee-avatar {
        width: 42px;
        height: 42px;
        border-radius: 50%;
        background: linear-gradient(180deg, #eaf1ff 0%, #dce7ff 100%);
        color: #23418d;
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 13px;
        font-weight: 800;
    }

    .employee-copy h4 {
        margin: 0 0 4px;
        font-size: 15px;
        color: #2e3f75;
    }

    .employee-copy p {
        margin: 0;
        font-size: 13px;
        color: #6c7b9e;
    }

    .attendance-switch {
        display: inline-flex;
        align-items: center;
        gap: 10px;
        cursor: pointer;
        user-select: none;
    }

        .attendance-switch input {
            display: none;
        }

    .slider {
        position: relative;
        width: 48px;
        height: 26px;
        border-radius: 999px;
        background: #d7def1;
        transition: background 0.2s ease;
    }

        .slider::after {
            content: '';
            position: absolute;
            top: 3px;
            left: 3px;
            width: 20px;
            height: 20px;
            border-radius: 50%;
            background: #fff;
            box-shadow: 0 2px 8px rgba(35, 53, 102, 0.2);
            transition: transform 0.2s ease;
        }

    .attendance-switch input:checked + .slider {
        background: linear-gradient(90deg, #2b5ede 0%, #34ca69 100%);
    }

        .attendance-switch input:checked + .slider::after {
            transform: translateX(22px);
        }

    .switch-label {
        min-width: 58px;
        font-size: 13px;
        font-weight: 700;
        color: #33477d;
    }

    .background-overlay {
        position: absolute;
        inset: 72px 0 0 0;
        z-index: 1;
        pointer-events: none;
        background: radial-gradient(circle at 8% 20%, rgba(255, 255, 255, 0.85) 0%, rgba(255, 255, 255, 0.05) 18%, transparent 36%), radial-gradient(circle at 92% 24%, rgba(255, 255, 255, 0.55) 0%, rgba(255, 255, 255, 0.04) 14%, transparent 30%);
    }

    .glow {
        position: absolute;
        border-radius: 999px;
        filter: blur(12px);
        opacity: 0.36;
    }

    .glow-1 {
        width: 300px;
        height: 300px;
        top: 16px;
        left: -40px;
        background: radial-gradient(circle, rgba(255, 255, 255, 0.72) 0%, rgba(155, 183, 255, 0.06) 55%, transparent 72%);
    }

    .glow-2 {
        width: 240px;
        height: 240px;
        top: 70px;
        right: 10px;
        background: radial-gradient(circle, rgba(255, 255, 255, 0.48) 0%, rgba(145, 175, 255, 0.05) 58%, transparent 76%);
    }

    .bottom-scene {
        position: absolute;
        inset: auto 0 0 0;
        height: 260px;
        z-index: 2;
        pointer-events: none;
    }

    .sparkles-layer {
        position: absolute;
        left: 0;
        right: 0;
        bottom: 58px;
        height: 122px;
        background-image: radial-gradient(circle, rgba(255, 247, 198, 0.82) 0 1.1px, transparent 1.4px), radial-gradient(circle, rgba(255, 251, 220, 0.62) 0 0.9px, transparent 1.3px), radial-gradient(circle, rgba(255, 244, 191, 0.45) 0 0.8px, transparent 1.1px);
        background-size: 18px 18px, 26px 26px, 34px 34px;
        background-position: 0 0, 12px 8px, 30px 16px;
        mask-image: linear-gradient(to top, rgba(0, 0, 0, 1), rgba(0, 0, 0, 0.15) 78%, transparent 100%);
        opacity: 0.56;
    }

    .wave-stack {
        position: absolute;
        inset: auto 0 0 0;
        height: 200px;
    }

    .wave {
        position: absolute;
        left: -8%;
        right: -8%;
        border-radius: 50%;
    }

    .wave-1 {
        bottom: 102px;
        height: 96px;
        background: rgba(255, 255, 255, 0.26);
        clip-path: ellipse(58% 42% at 50% 56%);
    }

    .wave-2 {
        bottom: 70px;
        height: 102px;
        background: rgba(215, 225, 254, 0.44);
        clip-path: ellipse(60% 44% at 50% 56%);
    }

    .wave-3 {
        bottom: 40px;
        height: 112px;
        background: linear-gradient(90deg, rgba(212, 222, 253, 0.82) 0%, rgba(242, 245, 255, 0.56) 44%, rgba(212, 222, 253, 0.82) 100%);
        clip-path: ellipse(61% 45% at 50% 56%);
    }

    .wave-4 {
        bottom: 10px;
        height: 126px;
        background: linear-gradient(90deg, #dde6fb 0%, #f9fbff 42%, #d9e4fb 100%);
        clip-path: ellipse(62% 46% at 50% 56%);
    }

    .wave-5 {
        bottom: -56px;
        height: 150px;
        background: linear-gradient(90deg, #092a97 0%, #1143b8 24%, #1f61e0 58%, #4c95e7 80%, #e0c91b 100%);
        clip-path: ellipse(64% 49% at 50% 56%);
    }

    @media (max-width: 1100px) {
        .filters-grid {
            grid-template-columns: 1fr;
        }
    }

    @media (max-width: 900px) {
        .topbar {
            height: auto;
            padding: 16px 18px;
            gap: 14px;
            flex-wrap: wrap;
        }

        .topbar-nav {
            order: 3;
            width: 100%;
            flex-wrap: wrap;
            gap: 18px;
        }

        .attendance-content {
            padding: 22px 16px 100px;
        }

        .attendance-item {
            flex-direction: column;
            align-items: flex-start;
        }
    }

    @media (max-width: 640px) {
        .user-name {
            display: none;
        }

        .user-chip {
            min-width: auto;
            padding-right: 10px;
        }

        .hero-copy h1 {
            font-size: 26px;
        }

        .hero-copy p {
            font-size: 14px;
        }

        .empty-state p {
            font-size: 14px;
        }
    }
</style>