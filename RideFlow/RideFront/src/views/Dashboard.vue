<template>
    <div class="dashboard-page" @click="closeUserMenu">
        <header class="topbar">
            <div class="topbar-left">
                <img src="../assets/images/RF.png" width="110" height="110" />
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

        <main class="dashboard-content">
            <section class="stats-grid">
                <article class="stat-card accent-yellow">
                    <div class="stat-card-top">
                        <div class="stat-copy">
                            <h3>Rutas Activas</h3>
                            <p class="stat-number">{{ rutas }}</p>
                        </div>
                        <div class="stat-icon yellow">
                            <span>⌖</span>
                        </div>
                    </div>

                    <div class="stat-card-bottom">
                        <span class="stat-trend">Datos reales</span>
                        <span class="stat-dots">••••</span>
                    </div>

                    <div class="stat-wave"></div>
                </article>

                <article class="stat-card accent-blue">
                    <div class="stat-card-top">
                        <div class="stat-copy">
                            <h3>Colaboradores</h3>
                            <p class="stat-number">{{ colaboradores }}</p>
                        </div>
                        <div class="stat-icon blue">
                            <span>👤</span>
                        </div>
                    </div>

                    <div class="stat-card-bottom">
                        <span class="stat-trend">Datos reales</span>
                        <span class="stat-dots">••••</span>
                    </div>

                    <div class="stat-wave"></div>
                </article>

                <article class="stat-card accent-blue">
                    <div class="stat-card-top">
                        <div class="stat-copy">
                            <h3>Asignaciones</h3>
                            <p class="stat-number">{{ asignaciones }}</p>
                        </div>
                        <div class="stat-icon blue">
                            <span>▣</span>
                        </div>
                    </div>

                    <div class="stat-card-bottom">
                        <span class="stat-trend">Datos reales</span>
                        <span class="stat-dots">••••</span>
                    </div>

                    <div class="stat-wave"></div>
                </article>

                <article class="stat-card accent-blue">
                    <div class="stat-card-top">
                        <div class="stat-copy">
                            <h3>Asistencia Hoy</h3>
                            <p class="stat-number">{{ asistencia }}</p>
                        </div>
                        <div class="stat-icon blue">
                            <span>☐</span>
                        </div>
                    </div>

                    <div class="stat-card-bottom">
                        <span class="stat-trend">{{ asistenciaResumen }}</span>
                        <span class="stat-dots">••••</span>
                    </div>

                    <div class="stat-wave"></div>
                </article>
            </section>

            <section class="hero-grid">
                <div class="hero-main">
                    <section class="welcome-card">
                        <div class="welcome-content">
                            <h1>Bienvenido a RideFlow</h1>
                            <p>
                                Gestiona las rutas de transporte de tus colaboradores de manera
                                eficiente con el sistema <strong>RideFlow</strong>.
                            </p>
                        </div>
                    </section>

                    <section class="quick-actions-card">
                        <div class="section-header">
                            <h2>Acciones Rápidas</h2>
                        </div>

                        <button class="action-btn primary" type="button" @click="goToRoutes">
                            <div class="action-left">
                                <div class="action-icon yellow">
                                    <span>⌖</span>
                                </div>
                                <div class="action-copy">
                                    <span class="action-title">Crear Ruta</span>
                                    <span class="action-subtitle">Crea una nueva ruta ahora mismo</span>
                                </div>
                            </div>
                            <span class="action-arrow">›</span>
                        </button>

                        <button class="action-btn secondary" type="button" @click="goToAttendance">
                            <div class="action-left">
                                <div class="action-icon blue">
                                    <span>☰</span>
                                </div>
                                <div class="action-copy">
                                    <span class="action-title dark">Registrar Asistencia</span>
                                    <span class="action-subtitle dark-sub">Registra la asistencia de hoy</span>
                                </div>
                            </div>
                            <span class="action-arrow blue-arrow">›</span>
                        </button>
                    </section>
                </div>

                <aside class="activity-card">
                    <div class="section-header with-menu">
                        <h2>Actividad Reciente</h2>
                        <span class="menu-dots">•••</span>
                    </div>

                    <div class="activity-list" v-if="recentActivity">
                        <div class="activity-item">
                            <img :src="activityAvatarUrl" :alt="recentActivity.title" class="activity-avatar" />
                            <div class="activity-text">
                                <p>
                                    <strong>{{ recentActivity.title }}</strong>
                                    {{ recentActivity.description }}
                                </p>
                                <span>{{ recentActivity.meta }}</span>
                            </div>
                        </div>
                    </div>

                    <div class="activity-list" v-else>
                        <div class="activity-item">
                            <img :src="avatarUrl" :alt="displayName" class="activity-avatar" />
                            <div class="activity-text">
                                <p><strong>{{ displayName }}</strong> aún no tiene actividad reciente</p>
                                <span>Empieza creando rutas, asignaciones o asistencias</span>
                            </div>
                        </div>
                    </div>
                </aside>
            </section>
        </main>

        <div class="bottom-waves">
            <div class="wave layer-1"></div>
            <div class="wave layer-2"></div>
            <div class="wave layer-3"></div>
            <div class="wave layer-4"></div>
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
        username?: string
        userName?: string
        fullName?: string
    }

    type RouteItem = {
        id: number | string
        origin: string
        destination: string
        schedule?: string
    }

    type AssignmentItem = {
        id: number | string
        employeeId: number | string
        employeeName: string
        routeId: number | string
        routeName: string
        assignedDate: string
        isActive: boolean
    }

    type AttendanceItem = {
        id: number | string
        employeeId: number | string
        routeId: number | string
        attendanceDate: string
        status: string
        markedAt?: string
    }

    type RecentActivityItem = {
        type: 'attendance' | 'assignment'
        title: string
        description: string
        meta: string
        when: Date
    }

    const route = useRoute()
    const router = useRouter()

    const rutas = ref(0)
    const colaboradores = ref(0)
    const asignaciones = ref(0)
    const asistencia = ref(0)
    const user = ref<StoredUser | null>(null)
    const isUserMenuOpen = ref(false)
    const assignments = ref<AssignmentItem[]>([])
    const attendances = ref<AttendanceItem[]>([])
    const routesList = ref<RouteItem[]>([])

    const today = new Date().toISOString().slice(0, 10)

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

    const asistenciaResumen = computed(() => {
        const absentToday = attendances.value.filter(x => {
            const dateOnly = String(x.attendanceDate).slice(0, 10)
            return dateOnly === today && String(x.status).toLowerCase() === 'absent'
        }).length

        if (asistencia.value === 0 && absentToday === 0) return 'Sin registros hoy'
        return `${asistencia.value} presentes · ${absentToday} ausentes`
    })

    const recentActivity = computed<RecentActivityItem | null>(() => {
        const lastAttendance = [...attendances.value]
            .filter(x => x.markedAt || x.attendanceDate)
            .sort((a, b) => {
                const da = new Date(a.markedAt || a.attendanceDate).getTime()
                const db = new Date(b.markedAt || b.attendanceDate).getTime()
                return db - da
            })[0]

        const lastAssignment = [...assignments.value]
            .filter(x => x.assignedDate)
            .sort((a, b) => {
                const da = new Date(a.assignedDate).getTime()
                const db = new Date(b.assignedDate).getTime()
                return db - da
            })[0]

        const attendanceDate = lastAttendance ? new Date(lastAttendance.markedAt || lastAttendance.attendanceDate) : null
        const assignmentDate = lastAssignment ? new Date(lastAssignment.assignedDate) : null

        if (attendanceDate && (!assignmentDate || attendanceDate >= assignmentDate)) {
            const routeItem = routesList.value.find(x => String(x.id) === String(lastAttendance.routeId))
            const routeName = routeItem
                ? `${routeItem.origin} → ${routeItem.destination}`
                : `Ruta #${lastAttendance.routeId}`

            return {
                type: 'attendance',
                title: 'Asistencia registrada',
                description: `en ${routeName}`,
                meta: `${capitalizeStatus(lastAttendance.status)} · ${formatRelativeTime(attendanceDate)}`
                    .replace(/^/, '')
                    .trim(),
                when: attendanceDate
            }
        }

        if (assignmentDate && lastAssignment) {
            return {
                type: 'assignment',
                title: lastAssignment.employeeName,
                description: `asignado a ${lastAssignment.routeName}`,
                meta: `${formatRelativeTime(assignmentDate)}`,
                when: assignmentDate
            }
        }

        return null
    })

    const activityAvatarUrl = computed(() => {
        if (!recentActivity.value) return avatarUrl.value
        const name = encodeURIComponent(recentActivity.value.title)
        return `https://ui-avatars.com/api/?name=${name}&background=EAF0FF&color=183A8F&bold=true`
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

    const goToRoutes = () => {
        router.push('/routes')
    }

    const goToAttendance = () => {
        router.push('/attendance')
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
            origin: item.origin ?? item.Origin ?? 'Sin origen',
            destination: item.destination ?? item.Destination ?? 'Sin destino',
            schedule: item.schedule ?? item.departureTime ?? item.DepartureTime ?? ''
        }))
    }

    const normalizeAssignments = (data: any): AssignmentItem[] => {
        const list = Array.isArray(data)
            ? data
            : Array.isArray(data?.items)
                ? data.items
                : Array.isArray(data?.data)
                    ? data.data
                    : []

        return list.map((item: any, index: number) => ({
            id: item.id ?? item.assignmentId ?? item.Id ?? index + 1,
            employeeId: item.employeeId ?? item.EmployeeId ?? '',
            employeeName: item.employeeName ?? item.EmployeeName ?? 'Sin colaborador',
            routeId: item.routeId ?? item.RouteId ?? '',
            routeName: item.routeName ?? item.RouteName ?? 'Sin ruta',
            assignedDate: item.assignedDate ?? item.AssignedDate ?? '',
            isActive: item.isActive ?? item.IsActive ?? true
        }))
    }

    const normalizeAttendances = (data: any): AttendanceItem[] => {
        const list = Array.isArray(data)
            ? data
            : Array.isArray(data?.items)
                ? data.items
                : Array.isArray(data?.data)
                    ? data.data
                    : []

        return list.map((item: any, index: number) => ({
            id: item.id ?? item.attendanceId ?? item.Id ?? index + 1,
            employeeId: item.employeeId ?? item.EmployeeId ?? '',
            routeId: item.routeId ?? item.RouteId ?? '',
            attendanceDate: item.attendanceDate ?? item.AttendanceDate ?? '',
            status: item.status ?? item.Status ?? '',
            markedAt: item.markedAt ?? item.MarkedAt ?? ''
        }))
    }

    const formatRelativeTime = (date: Date) => {
        const diffMs = Date.now() - date.getTime()
        const diffMin = Math.max(1, Math.floor(diffMs / 60000))

        if (diffMin < 60) return `hace ${diffMin} min`

        const diffHours = Math.floor(diffMin / 60)
        if (diffHours < 24) return `hace ${diffHours} h`

        const diffDays = Math.floor(diffHours / 24)
        return `hace ${diffDays} d`
    }

    const capitalizeStatus = (status: string) => {
        const normalized = String(status).toLowerCase()
        if (normalized === 'present') return 'Presente'
        if (normalized === 'absent') return 'Ausente'
        return status
    }

    const loadData = async () => {
        try {
            user.value = getUser()

            const [resRutas, resColab, resAsign, resAttendance] = await Promise.all([
                apiFetch('/Routes'),
                apiFetch('/Employees'),
                apiFetch('/Assignments'),
                apiFetch('/Attendances')
            ])

            if (resRutas?.ok) {
                const data = await resRutas.json()
                const normalized = normalizeRoutes(data)
                routesList.value = normalized
                rutas.value = normalized.length
            } else {
                routesList.value = []
                rutas.value = 0
            }

            if (resColab?.ok) {
                const data = await resColab.json()
                colaboradores.value = Array.isArray(data)
                    ? data.length
                    : Array.isArray(data?.data)
                        ? data.data.length
                        : Array.isArray(data?.items)
                            ? data.items.length
                            : 0
            } else {
                colaboradores.value = 0
            }

            if (resAsign?.ok) {
                const data = await resAsign.json()
                const normalized = normalizeAssignments(data).filter(x => x.isActive)
                assignments.value = normalized
                asignaciones.value = normalized.length
            } else {
                assignments.value = []
                asignaciones.value = 0
            }

            if (resAttendance?.ok) {
                const data = await resAttendance.json()
                const normalized = normalizeAttendances(data)
                attendances.value = normalized
                asistencia.value = normalized.filter(x => {
                    const dateOnly = String(x.attendanceDate).slice(0, 10)
                    return dateOnly === today && String(x.status).toLowerCase() === 'present'
                }).length
            } else {
                attendances.value = []
                asistencia.value = 0
            }
        } catch (error) {
            console.error('Error cargando dashboard:', error)
            rutas.value = 0
            colaboradores.value = 0
            asignaciones.value = 0
            asistencia.value = 0
            assignments.value = []
            attendances.value = []
            routesList.value = []
        }
    }

    onMounted(loadData)
</script>

<style scoped>
    .dashboard-page {
        min-height: 100vh;
        position: relative;
        overflow: hidden;
        background: radial-gradient(circle at top left, rgba(255, 255, 255, 0.98), rgba(244, 245, 251, 0.96) 42%, rgba(234, 237, 246, 0.98) 100%);
        color: #24345f;
    }

    .topbar {
        height: 72px;
        background: rgba(255, 255, 255, 0.72);
        backdrop-filter: blur(14px);
        box-shadow: 0 10px 30px rgba(42, 63, 122, 0.08);
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 0 28px;
        position: relative;
        z-index: 10;
        border-bottom: 1px solid rgba(220, 227, 244, 0.9);
    }

    .topbar-left {
        display: flex;
        align-items: center;
        min-width: 140px;
    }

    .brand-logo {
        height: 40px;
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
        max-width: 90px;
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

    .dashboard-content {
        position: relative;
        z-index: 3;
        max-width: 1180px;
        margin: 0 auto;
        padding: 28px 24px 220px;
    }

    .stats-grid {
        display: grid;
        grid-template-columns: repeat(4, minmax(0, 1fr));
        gap: 18px;
        margin-bottom: 28px;
    }

    .stat-card {
        position: relative;
        min-height: 110px;
        border-radius: 16px;
        overflow: hidden;
        background: linear-gradient(180deg, rgba(255, 255, 255, 0.92) 0%, rgba(249, 250, 255, 0.98) 100%);
        border: 1px solid rgba(218, 225, 242, 0.95);
        box-shadow: 0 12px 28px rgba(63, 82, 138, 0.12);
        transition: transform 0.2s ease, box-shadow 0.2s ease;
    }

        .stat-card:hover {
            transform: translateY(-3px);
            box-shadow: 0 16px 34px rgba(63, 82, 138, 0.16);
        }

    .stat-card-top {
        display: flex;
        justify-content: space-between;
        align-items: flex-start;
        padding: 18px 18px 0;
        position: relative;
        z-index: 2;
    }

    .stat-copy h3 {
        margin: 0;
        font-size: 15px;
        font-weight: 600;
        color: #47598c;
    }

    .stat-number {
        margin: 6px 0 0;
        font-size: 36px;
        line-height: 1;
        font-weight: 800;
        letter-spacing: -0.02em;
        color: #203766;
    }

    .stat-icon {
        width: 42px;
        height: 42px;
        border-radius: 12px;
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 18px;
        color: #fff;
        box-shadow: 0 10px 18px rgba(68, 99, 180, 0.18);
        flex-shrink: 0;
    }

        .stat-icon.blue {
            background: linear-gradient(180deg, #4f7fe9 0%, #3f67d6 100%);
        }

        .stat-icon.yellow {
            background: linear-gradient(180deg, #f5cc42 0%, #efb70b 100%);
        }

    .stat-card-bottom {
        height: 36px;
        margin-top: 10px;
        padding: 0 18px;
        display: flex;
        align-items: center;
        justify-content: space-between;
        position: relative;
        z-index: 2;
    }

    .stat-trend {
        font-size: 12px;
        font-weight: 700;
        color: #5d6d97;
    }

    .stat-dots {
        font-size: 13px;
        letter-spacing: 1px;
        color: #a7b1cb;
    }

    .stat-wave {
        position: absolute;
        left: -5%;
        right: -5%;
        bottom: -8px;
        height: 54px;
        background: radial-gradient(120% 110% at 16% 0%, rgba(209, 222, 254, 0.92) 0%, rgba(209, 222, 254, 0.92) 30%, transparent 31%), radial-gradient(120% 110% at 50% 0%, rgba(189, 208, 251, 0.82) 0%, rgba(189, 208, 251, 0.82) 28%, transparent 29%), radial-gradient(120% 110% at 84% 0%, rgba(223, 232, 255, 0.98) 0%, rgba(223, 232, 255, 0.98) 30%, transparent 31%);
        opacity: 0.95;
    }

    .hero-grid {
        display: grid;
        grid-template-columns: minmax(0, 1fr) 320px;
        gap: 22px;
        align-items: start;
    }

    .hero-main {
        display: grid;
        gap: 18px;
    }

    .welcome-card {
        padding: 6px 2px 0;
    }

    .welcome-content h1 {
        margin: 0 0 14px;
        font-size: 40px;
        line-height: 1.05;
        font-weight: 700;
        letter-spacing: -0.03em;
        color: #263f81;
    }

    .welcome-content p {
        max-width: 650px;
        margin: 0;
        font-size: 22px;
        line-height: 1.7;
        color: #5a688c;
    }

    .quick-actions-card,
    .activity-card {
        background: linear-gradient(180deg, rgba(255, 255, 255, 0.88) 0%, rgba(248, 249, 253, 0.96) 100%);
        border: 1px solid rgba(217, 224, 240, 0.95);
        border-radius: 18px;
        box-shadow: 0 14px 28px rgba(62, 82, 138, 0.1);
    }

    .quick-actions-card {
        max-width: 650px;
        padding: 14px;
    }

    .section-header {
        padding: 2px 4px 14px;
    }

        .section-header h2 {
            margin: 0;
            font-size: 18px;
            font-weight: 700;
            color: #33477e;
        }

        .section-header.with-menu {
            padding: 16px 18px 12px;
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

    .menu-dots {
        font-size: 14px;
        letter-spacing: 1px;
        color: #a4aec6;
    }

    .action-btn {
        width: 100%;
        min-height: 74px;
        border: none;
        border-radius: 14px;
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 0 18px;
        cursor: pointer;
        transition: transform 0.2s ease, box-shadow 0.2s ease;
    }

        .action-btn:hover {
            transform: translateY(-2px);
        }

        .action-btn + .action-btn {
            margin-top: 12px;
        }

        .action-btn.primary {
            background: linear-gradient(90deg, #254fc8 0%, #68c0f4 100%);
            box-shadow: 0 16px 28px rgba(45, 93, 207, 0.2);
        }

        .action-btn.secondary {
            background: linear-gradient(180deg, rgba(255, 255, 255, 0.96) 0%, rgba(247, 249, 255, 0.98) 100%);
            border: 1px solid rgba(218, 224, 240, 0.95);
            box-shadow: 0 8px 18px rgba(67, 88, 143, 0.08);
        }

    .action-left {
        display: flex;
        align-items: center;
        gap: 14px;
    }

    .action-copy {
        display: flex;
        flex-direction: column;
        align-items: flex-start;
    }

    .action-icon {
        width: 42px;
        height: 42px;
        border-radius: 12px;
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 19px;
        flex-shrink: 0;
        color: #fff;
    }

        .action-icon.yellow {
            background: linear-gradient(180deg, #f5cc42 0%, #eab512 100%);
            box-shadow: 0 10px 18px rgba(233, 184, 35, 0.2);
        }

        .action-icon.blue {
            background: linear-gradient(180deg, #5b80ea 0%, #4570da 100%);
            box-shadow: 0 10px 18px rgba(78, 114, 223, 0.18);
        }

    .action-title {
        display: block;
        text-align: left;
        font-size: 16px;
        font-weight: 800;
        color: #fff;
    }

        .action-title.dark {
            color: #31477f;
        }

    .action-subtitle {
        display: block;
        text-align: left;
        margin-top: 4px;
        font-size: 12px;
        color: rgba(255, 255, 255, 0.92);
    }

        .action-subtitle.dark-sub {
            color: #6c7898;
        }

    .action-arrow {
        font-size: 34px;
        line-height: 1;
        color: rgba(255, 255, 255, 0.96);
        flex-shrink: 0;
    }

    .blue-arrow {
        color: #4670dd;
    }

    .activity-card {
        padding-bottom: 14px;
        min-height: 176px;
    }

    .activity-list {
        padding: 0 18px;
    }

    .activity-item {
        display: flex;
        align-items: center;
        gap: 12px;
        padding: 10px 0 0;
    }

    .activity-avatar {
        width: 46px;
        height: 46px;
        border-radius: 50%;
        display: block;
        flex-shrink: 0;
        box-shadow: 0 6px 14px rgba(74, 94, 143, 0.14);
    }

    .activity-text p {
        margin: 0 0 4px;
        font-size: 15px;
        line-height: 1.4;
        color: #425480;
    }

    .activity-text span {
        font-size: 13px;
        color: #6e7a99;
        line-height: 1.4;
    }

    .bottom-waves {
        position: absolute;
        left: 0;
        right: 0;
        bottom: 0;
        height: 260px;
        z-index: 1;
        pointer-events: none;
    }

    .wave {
        position: absolute;
        left: -10%;
        right: -10%;
        border-radius: 50%;
    }

    .layer-1 {
        bottom: -118px;
        height: 248px;
        background: linear-gradient(90deg, #f6f7fc 0%, #eef1fa 45%, #e9edf8 100%);
    }

    .layer-2 {
        bottom: -132px;
        height: 220px;
        background: linear-gradient(90deg, rgba(223, 229, 248, 0.95) 0%, rgba(210, 220, 248, 0.92) 42%, rgba(227, 233, 251, 0.96) 100%);
        clip-path: ellipse(60% 46% at 50% 54%);
    }

    .layer-3 {
        bottom: -148px;
        height: 212px;
        background: linear-gradient(90deg, #0b297a 0%, #123fb0 24%, #2567ea 58%, #d4c11c 86%, #f0c612 100%);
        clip-path: ellipse(60% 46% at 50% 54%);
    }

    .layer-4 {
        bottom: -172px;
        height: 208px;
        background: linear-gradient(90deg, #0a246b 0%, #0d3aa0 28%, #1457d6 57%, #6cb96e 78%, #f0c612 100%);
        clip-path: ellipse(60% 44% at 50% 54%);
        opacity: 0.98;
    }

    @media (max-width: 1150px) {
        .stats-grid {
            grid-template-columns: repeat(2, minmax(0, 1fr));
        }

        .hero-grid {
            grid-template-columns: 1fr;
        }

        .quick-actions-card {
            max-width: 100%;
        }

        .activity-card {
            max-width: 650px;
        }

        .welcome-content h1 {
            font-size: 34px;
        }

        .welcome-content p {
            font-size: 19px;
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

        .dashboard-content {
            padding: 20px 16px 210px;
        }
    }

    @media (max-width: 640px) {
        .stats-grid {
            grid-template-columns: 1fr;
            gap: 14px;
        }

        .user-name {
            display: none;
        }

        .user-chip {
            min-width: auto;
            padding-right: 10px;
        }

        .welcome-content h1 {
            font-size: 28px;
        }

        .welcome-content p {
            font-size: 16px;
            line-height: 1.65;
        }

        .action-btn {
            min-height: 70px;
            padding: 0 14px;
        }

        .activity-card {
            min-height: auto;
        }
    }
</style>