<template>
    <div class="routes-page" @click="closeUserMenu">
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

        <main class="routes-content">
            <section class="routes-hero">
                <div class="hero-copy">
                    <h1>Gestión de Rutas</h1>
                    <p>Administra las rutas de transporte institucional</p>
                </div>

                <button class="new-route-btn" type="button" @click="openCreateModal">
                    <span class="plus-icon">＋</span>
                    <span>Nueva Ruta</span>
                </button>
            </section>

            <section class="routes-table-card">
                <div class="table-wrap">
                    <table class="routes-table">
                        <thead>
                            <tr>
                                <th>Origen</th>
                                <th>Paradas</th>
                                <th>Destino</th>
                                <th>Horario</th>
                                <th>Conductor</th>
                                <th class="actions-col">Acciones</th>
                            </tr>
                        </thead>

                        <tbody>
                            <tr v-if="loading">
                                <td colspan="6" class="empty-cell">Cargando rutas...</td>
                            </tr>

                            <tr v-else-if="errorMessage">
                                <td colspan="6" class="empty-cell error-cell">{{ errorMessage }}</td>
                            </tr>

                            <tr v-else-if="routes.length === 0">
                                <td colspan="6" class="empty-cell">No hay rutas registradas</td>
                            </tr>

                            <tr v-for="routeItem in routes" :key="routeItem.id">
                                <td>{{ routeItem.origin }}</td>
                                <td>{{ routeItem.stops || '-' }}</td>
                                <td>{{ routeItem.destination }}</td>
                                <td>{{ routeItem.schedule }}</td>
                                <td>{{ routeItem.driverName }}</td>
                                <td class="row-actions">
                                    <button class="icon-btn edit" type="button" @click="editRoute(routeItem)">
                                        ✎
                                    </button>
                                    <button class="icon-btn delete" type="button" @click="deleteRoute(routeItem)" :disabled="deletingId === routeItem.id || saving">
                                        {{ deletingId === routeItem.id ? '...' : '🗑' }}
                                    </button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </section>
        </main>

        <transition name="modal-fade">
            <div v-if="showCreateModal" class="modal-overlay" @click="closeCreateModal">
                <div class="route-modal" @click.stop>
                    <div class="modal-header">
                        <div class="modal-header-top">
                            <h2>{{ isEditing ? 'Editar Ruta' : 'Nueva Ruta' }}</h2>
                            <button class="modal-close" type="button" @click="closeCreateModal">✕</button>
                        </div>

                        <div class="modal-subtitle-row">
                            <span class="header-line"></span>
                            <p>{{ isEditing ? 'Actualiza los datos de la ruta' : 'Complete los datos de ruta de transporte' }}</p>
                            <span class="header-line"></span>
                        </div>
                    </div>

                    <form class="route-form" @submit.prevent="saveRoute">
                        <div class="form-group">
                            <label>Origen <span>*</span></label>
                            <input v-model.trim="form.origin"
                                   type="text"
                                   placeholder="Punto de partida" />
                        </div>

                        <div class="form-group">
                            <label>Paradas</label>
                            <input v-model.trim="form.stops"
                                   type="text"
                                   placeholder="Opcional (ej: Av. Duarte, Lincoln)" />
                        </div>

                        <div class="form-group">
                            <label>Destino <span>*</span></label>
                            <input v-model.trim="form.destination"
                                   type="text"
                                   placeholder="Punto de llegada" />
                        </div>

                        <div class="form-group">
                            <label>Horario de Salida <span>*</span></label>
                            <div class="time-input-wrap">
                                <input v-model="form.schedule"
                                       type="time" />
                                <span class="time-icon">🕘</span>
                            </div>
                        </div>

                        <div class="form-group">
                            <label>Conductor <span>*</span></label>
                            <div class="select-wrap">
                                <select v-model="form.driverId">
                                    <option value="">Seleccione conductor</option>
                                    <option v-for="driver in drivers" :key="driver.id" :value="String(driver.id)">
                                        {{ driver.fullName }}
                                    </option>
                                </select>
                                <span class="select-arrow">⌄</span>
                            </div>
                        </div>

                        <p v-if="formError" class="form-error">{{ formError }}</p>

                        <div class="modal-actions">
                            <button class="btn-cancel" type="button" @click="closeCreateModal">
                                Cancelar
                            </button>
                            <button class="btn-create" type="submit" :disabled="saving">
                                {{ saving ? 'Guardando...' : isEditing ? 'Actualizar' : 'Crear' }}
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </transition>

        <div class="background-overlay">
            <div class="glow glow-1"></div>
            <div class="glow glow-2"></div>
        </div>

        <div class="bottom-scene">
            <div class="bus-illustration">
                <div class="bus-shadow"></div>
                <div class="bus-body">
                    <div class="bus-top"></div>
                    <div class="bus-windows">
                        <span></span>
                        <span></span>
                        <span></span>
                    </div>
                    <div class="bus-door"></div>
                    <div class="bus-stripe"></div>
                    <div class="wheel left"></div>
                    <div class="wheel right"></div>
                </div>
            </div>

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
    import { computed, onMounted, reactive, ref } from 'vue'
    import { useRoute, useRouter } from 'vue-router'
    import { apiFetch } from '../services/api'
    import { getUser, logout } from '../services/auth.service'

    type StoredUser = {
        name?: string
        fullName?: string
        username?: string
        userName?: string
    }

    type DriverItem = {
        id: number | string
        fullName: string
    }

    type RouteItem = {
        id: number | string
        origin: string
        stops?: string | null
        destination: string
        schedule: string
        driverId: number
        driverName: string
        isActive?: boolean
    }

    const route = useRoute()
    const router = useRouter()

    const loading = ref(false)
    const saving = ref(false)
    const deletingId = ref<number | string | null>(null)
    const isUserMenuOpen = ref(false)
    const showCreateModal = ref(false)
    const isEditing = ref(false)
    const editingId = ref<number | string | null>(null)
    const formError = ref('')
    const errorMessage = ref('')
    const user = ref<StoredUser | null>(null)
    const routes = ref<RouteItem[]>([])
    const drivers = ref<DriverItem[]>([])

    const form = reactive({
        origin: '',
        stops: '',
        destination: '',
        schedule: '',
        driverId: ''
    })

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

    const formatSchedule = (value: unknown) => {
        if (!value) return '00:00'

        if (typeof value === 'string') {
            const hhmmss = value.match(/^(\d{2}):(\d{2}):(\d{2})$/)
            if (hhmmss) return `${hhmmss[1]}:${hhmmss[2]}`

            const hhmm = value.match(/^(\d{2}):(\d{2})$/)
            if (hhmm) return `${hhmm[1]}:${hhmm[2]}`
        }

        return String(value)
    }

    const toTimeSpan = (value: string) => {
        if (!value) return '00:00:00'
        return /^\d{2}:\d{2}$/.test(value) ? `${value}:00` : value
    }

    const normalizeRoute = (item: any, index = 0): RouteItem => {
        return {
            id: item.id ?? item.Id ?? index + 1,
            origin: item.origin ?? item.Origin ?? 'Sin origen',
            stops: item.stops ?? item.Stops ?? null,
            destination: item.destination ?? item.Destination ?? 'Sin destino',
            schedule: formatSchedule(item.departureTime ?? item.DepartureTime),
            driverId: item.driverId ?? item.DriverId ?? 0,
            driverName: item.driverName ?? item.DriverName ?? 'Sin conductor',
            isActive: item.isActive ?? item.IsActive ?? true
        }
    }

    const normalizeRoutes = (data: any): RouteItem[] => {
        const list = Array.isArray(data)
            ? data
            : Array.isArray(data?.items)
                ? data.items
                : Array.isArray(data?.data)
                    ? data.data
                    : []

        return list.map((item: any, index: number) => normalizeRoute(item, index))
    }

    const normalizeDrivers = (data: any): DriverItem[] => {
        const list = Array.isArray(data)
            ? data
            : Array.isArray(data?.items)
                ? data.items
                : Array.isArray(data?.data)
                    ? data.data
                    : []

        return list.map((item: any, index: number) => ({
            id: item.id ?? item.driverId ?? item.Id ?? index + 1,
            fullName: item.fullName ?? item.FullName ?? item.name ?? 'Sin nombre'
        }))
    }

    const resetForm = () => {
        form.origin = ''
        form.stops = ''
        form.destination = ''
        form.schedule = ''
        form.driverId = ''
        formError.value = ''
    }

    const openCreateModal = () => {
        resetForm()
        isEditing.value = false
        editingId.value = null
        showCreateModal.value = true
    }

    const closeCreateModal = () => {
        showCreateModal.value = false
        isEditing.value = false
        editingId.value = null
        formError.value = ''
        resetForm()
    }

    const parseErrorMessage = async (response: Response) => {
        try {
            const contentType = response.headers.get('content-type') || ''

            if (contentType.includes('application/json')) {
                const data = await response.json()

                if (data?.errors) {
                    const messages = Object.values(data.errors).flat().join(' ')
                    if (messages) return messages
                }

                return data?.message || data?.title || data?.error || null
            }

            const text = await response.text()
            return text || null
        } catch {
            return null
        }
    }

    const loadRoutes = async () => {
        loading.value = true
        errorMessage.value = ''

        try {
            user.value = getUser()
            const response = await apiFetch('/Routes')

            if (!response.ok) {
                const serverMessage = await parseErrorMessage(response)
                errorMessage.value = serverMessage || 'Error cargando rutas'
                routes.value = []
                return
            }

            const data = await response.json()
            routes.value = normalizeRoutes(data)
        } catch {
            errorMessage.value = 'Error cargando rutas'
            routes.value = []
        } finally {
            loading.value = false
        }
    }

    const loadDrivers = async () => {
        try {
            const response = await apiFetch('/Drivers')

            if (!response.ok) {
                drivers.value = []
                return
            }

            const data = await response.json()
            drivers.value = normalizeDrivers(data)
        } catch {
            drivers.value = []
        }
    }

    const saveRoute = async () => {
        formError.value = ''

        if (!form.origin || !form.destination || !form.schedule || !form.driverId) {
            formError.value = 'Completa todos los campos obligatorios.'
            return
        }

        saving.value = true

        const payload = {
            origin: form.origin.trim(),
            stops: form.stops?.trim() || null,
            destination: form.destination.trim(),
            departureTime: toTimeSpan(form.schedule),
            driverId: Number(form.driverId),
            isActive: true
        }

        try {
            let response: Response

            if (isEditing.value && editingId.value !== null) {
                response = await apiFetch(`/Routes/${editingId.value}`, {
                    method: 'PUT',
                    body: JSON.stringify(payload)
                })
            } else {
                response = await apiFetch('/Routes', {
                    method: 'POST',
                    body: JSON.stringify(payload)
                })
            }

            if (!response.ok) {
                const serverMessage = await parseErrorMessage(response)
                formError.value = serverMessage || 'No se pudo guardar la ruta.'
                return
            }

            await loadRoutes()
            closeCreateModal()
        } catch {
            formError.value = 'No se pudo guardar la ruta.'
        } finally {
            saving.value = false
        }
    }

    const editRoute = (routeItem: RouteItem) => {
        form.origin = routeItem.origin
        form.stops = routeItem.stops || ''
        form.destination = routeItem.destination
        form.schedule = routeItem.schedule
        form.driverId = String(routeItem.driverId)

        isEditing.value = true
        editingId.value = routeItem.id
        formError.value = ''
        showCreateModal.value = true
    }

    const deleteRoute = async (routeItem: RouteItem) => {
        const confirmed = window.confirm(`¿Deseas eliminar la ruta ${routeItem.origin} → ${routeItem.destination}?`)
        if (!confirmed) return

        deletingId.value = routeItem.id

        try {
            const response = await apiFetch(`/Routes/${routeItem.id}`, {
                method: 'DELETE'
            })

            if (!response.ok) {
                return
            }

            await loadRoutes()
        } finally {
            deletingId.value = null
        }
    }

    onMounted(() => {
        loadRoutes()
        loadDrivers()
    })
</script>

<style scoped>
    .routes-page {
        min-height: 100vh;
        position: relative;
        overflow-x: hidden;
        overflow-y: auto;
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

    .routes-content {
        position: relative;
        z-index: 8;
        width: min(100%, 1280px);
        margin: 0 auto;
        padding: 24px 24px 120px;
    }

    .routes-hero {
        display: flex;
        align-items: flex-start;
        justify-content: space-between;
        gap: 20px;
        margin-bottom: 28px;
    }

    .hero-copy {
        min-width: 0;
    }

        .hero-copy h1 {
            margin: 0 0 8px;
            font-size: 46px;
            line-height: 1.02;
            font-weight: 800;
            letter-spacing: -0.03em;
            color: #253f82;
        }

        .hero-copy p {
            margin: 0;
            font-size: 18px;
            color: #617097;
            font-weight: 500;
        }

    .new-route-btn {
        height: 50px;
        min-width: 165px;
        border: none;
        border-radius: 12px;
        padding: 0 18px;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        gap: 8px;
        cursor: pointer;
        font-size: 16px;
        font-weight: 700;
        color: #fff;
        background: linear-gradient(90deg, #2449c6 0%, #3f7df0 72%, #9ac861 100%);
        box-shadow: 0 10px 22px rgba(53, 86, 181, 0.22);
        transition: transform 0.2s ease, box-shadow 0.2s ease;
        flex-shrink: 0;
    }

        .new-route-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 14px 28px rgba(53, 86, 181, 0.28);
        }

    .plus-icon {
        font-size: 18px;
        line-height: 1;
    }

    .routes-table-card {
        position: relative;
        width: 100%;
        border-radius: 18px;
        background: rgba(255, 255, 255, 0.26);
        backdrop-filter: blur(4px);
    }

    .table-wrap {
        width: 100%;
        overflow-x: auto;
        overflow-y: hidden;
        border-radius: 16px;
        border: 1px solid rgba(198, 209, 242, 0.85);
        box-shadow: 0 14px 30px rgba(54, 72, 126, 0.14);
        background: linear-gradient(180deg, rgba(255, 255, 255, 0.9) 0%, rgba(245, 247, 255, 0.88) 100%);
        -webkit-overflow-scrolling: touch;
    }

    .routes-table {
        width: 100%;
        min-width: 1100px;
        border-collapse: separate;
        border-spacing: 0;
        table-layout: auto;
    }

        .routes-table thead th {
            position: relative;
            padding: 14px 18px;
            text-align: left;
            font-size: 14px;
            font-weight: 700;
            color: rgba(255, 255, 255, 0.97);
            background: linear-gradient(180deg, rgba(138, 176, 255, 0.16) 0%, rgba(138, 176, 255, 0.02) 24%, rgba(138, 176, 255, 0) 40%), linear-gradient(90deg, #3159d2 0%, #426be0 30%, #527bea 65%, #618af1 100%);
            box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.16), inset 0 -1px 0 rgba(34, 63, 148, 0.18);
            white-space: nowrap;
        }

            .routes-table thead th:first-child {
                border-top-left-radius: 16px;
            }

            .routes-table thead th:last-child {
                border-top-right-radius: 16px;
            }

            .routes-table thead th:not(:last-child)::after {
                content: '';
                position: absolute;
                top: 12px;
                right: 0;
                width: 1px;
                height: calc(100% - 24px);
                background: linear-gradient(180deg, rgba(255, 255, 255, 0.05) 0%, rgba(255, 255, 255, 0.26) 20%, rgba(255, 255, 255, 0.32) 50%, rgba(255, 255, 255, 0.18) 80%, rgba(255, 255, 255, 0.04) 100%);
            }

        .routes-table tbody td {
            padding: 14px 16px;
            font-size: 16px;
            color: #42527e;
            background: rgba(255, 255, 255, 0.72);
            border-bottom: 1px solid rgba(209, 218, 241, 0.8);
            vertical-align: middle;
        }

            .routes-table tbody td:nth-child(1),
            .routes-table tbody td:nth-child(2),
            .routes-table tbody td:nth-child(3),
            .routes-table tbody td:nth-child(5) {
                white-space: normal;
                word-break: break-word;
            }

            .routes-table thead th:nth-child(1),
            .routes-table tbody td:nth-child(1) {
                width: 20%;
                min-width: 180px;
            }

            .routes-table thead th:nth-child(2),
            .routes-table tbody td:nth-child(2) {
                width: 18%;
                min-width: 160px;
            }

            .routes-table thead th:nth-child(3),
            .routes-table tbody td:nth-child(3) {
                width: 20%;
                min-width: 180px;
            }

            .routes-table thead th:nth-child(4),
            .routes-table tbody td:nth-child(4) {
                width: 12%;
                min-width: 110px;
                white-space: nowrap;
            }

            .routes-table thead th:nth-child(5),
            .routes-table tbody td:nth-child(5) {
                width: 20%;
                min-width: 180px;
            }

            .routes-table thead th:nth-child(6),
            .routes-table tbody td:nth-child(6) {
                width: 10%;
                min-width: 120px;
            }

        .routes-table tbody tr:last-child td:first-child {
            border-bottom-left-radius: 14px;
        }

        .routes-table tbody tr:last-child td:last-child {
            border-bottom-right-radius: 14px;
        }

        .routes-table tbody tr:hover td {
            background: rgba(245, 248, 255, 0.98);
        }

    .actions-col {
        text-align: center;
    }

    .row-actions {
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 10px;
        white-space: nowrap;
    }

    .icon-btn {
        width: 34px;
        height: 34px;
        border: none;
        border-radius: 10px;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        cursor: pointer;
        font-size: 15px;
        box-shadow: 0 6px 12px rgba(61, 79, 133, 0.18);
        transition: transform 0.2s ease, box-shadow 0.2s ease, opacity 0.2s ease;
        flex-shrink: 0;
    }

        .icon-btn:hover {
            transform: translateY(-1px);
        }

        .icon-btn:disabled {
            opacity: 0.6;
            cursor: not-allowed;
        }

        .icon-btn.edit {
            background: linear-gradient(180deg, #ffffff 0%, #edf1ff 100%);
            color: #29448e;
        }

        .icon-btn.delete {
            background: linear-gradient(180deg, #ef6172 0%, #d9485e 100%);
            color: #fff;
        }

    .empty-cell {
        text-align: center;
        color: #667596;
        font-weight: 600;
        padding: 22px 16px !important;
    }

    .error-cell {
        color: #7a87a8;
    }

    .modal-overlay {
        position: fixed;
        inset: 0;
        background: rgba(20, 32, 70, 0.16);
        backdrop-filter: blur(6px);
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 80;
        padding: 18px;
    }

    .route-modal {
        width: 100%;
        max-width: 500px;
        border-radius: 24px;
        background: radial-gradient(circle at top left, rgba(255, 255, 255, 0.98), rgba(249, 246, 252, 0.98) 45%, rgba(244, 246, 255, 0.98) 100%);
        border: 1px solid rgba(215, 221, 242, 0.95);
        box-shadow: 0 22px 46px rgba(36, 54, 108, 0.18);
        overflow: hidden;
        position: relative;
    }

        .route-modal::before {
            content: '';
            position: absolute;
            inset: 0;
            background: radial-gradient(circle at 20% 0%, rgba(255, 255, 255, 0.9), transparent 28%), radial-gradient(circle at 100% 20%, rgba(231, 240, 255, 0.55), transparent 22%);
            pointer-events: none;
        }

    .modal-header {
        padding: 18px 22px 8px;
        position: relative;
        z-index: 1;
    }

    .modal-header-top {
        display: flex;
        align-items: flex-start;
        justify-content: space-between;
        gap: 12px;
    }

    .modal-header h2 {
        margin: 0;
        flex: 1;
        text-align: center;
        font-size: 24px;
        line-height: 1.05;
        font-weight: 800;
        letter-spacing: -0.03em;
        color: #243a7a;
    }

    .modal-close {
        width: 34px;
        height: 34px;
        border: none;
        background: transparent;
        color: #44558b;
        font-size: 22px;
        line-height: 1;
        cursor: pointer;
        border-radius: 10px;
        transition: background 0.2s ease, transform 0.2s ease;
    }

        .modal-close:hover {
            background: rgba(233, 239, 255, 0.8);
            transform: rotate(90deg);
        }

    .modal-subtitle-row {
        margin-top: 4px;
        display: flex;
        align-items: center;
        gap: 10px;
    }

        .modal-subtitle-row p {
            margin: 0;
            white-space: nowrap;
            font-size: 14px;
            color: #5f6d96;
            font-weight: 500;
        }

    .header-line {
        flex: 1;
        height: 1px;
        background: linear-gradient(90deg, transparent 0%, rgba(206, 213, 235, 0.85) 30%, rgba(206, 213, 235, 0.85) 70%, transparent 100%);
    }

    .route-form {
        padding: 8px 22px 22px;
        position: relative;
        z-index: 1;
    }

    .form-group + .form-group {
        margin-top: 12px;
    }

    .form-group label {
        display: inline-block;
        margin-bottom: 6px;
        font-size: 14px;
        font-weight: 800;
        color: #2e3f75;
    }

        .form-group label span {
            color: #eb5569;
        }

    .form-group input,
    .form-group select {
        width: 100%;
        height: 42px;
        border-radius: 11px;
        border: 1px solid rgba(192, 203, 236, 0.95);
        background: linear-gradient(180deg, rgba(255, 255, 255, 0.9) 0%, rgba(245, 247, 255, 0.98) 100%);
        padding: 0 14px;
        font-size: 15px;
        color: #32467c;
        outline: none;
        transition: border-color 0.2s ease, box-shadow 0.2s ease;
        box-shadow: inset 0 1px 1px rgba(255, 255, 255, 0.8);
    }

    .form-group select {
        appearance: none;
        padding-right: 40px;
    }

    .form-group input::placeholder {
        color: #7c87a9;
    }

    .form-group input:focus,
    .form-group select:focus {
        border-color: #6d92f7;
        box-shadow: 0 0 0 4px rgba(101, 137, 242, 0.14);
    }

    .time-input-wrap,
    .select-wrap {
        position: relative;
    }

        .time-input-wrap input {
            padding-right: 46px;
        }

    .time-icon,
    .select-arrow {
        position: absolute;
        right: 14px;
        top: 50%;
        transform: translateY(-50%);
        font-size: 16px;
        pointer-events: none;
        opacity: 0.75;
        color: #7c87a9;
    }

    .form-error {
        margin: 12px 0 0;
        color: #d74b60;
        font-size: 13px;
        font-weight: 700;
    }

    .modal-actions {
        margin-top: 16px;
        padding-top: 14px;
        border-top: 1px solid rgba(219, 225, 243, 0.95);
        display: flex;
        justify-content: flex-end;
        gap: 10px;
    }

    .btn-cancel,
    .btn-create {
        min-width: 116px;
        height: 40px;
        border-radius: 11px;
        padding: 0 18px;
        font-size: 15px;
        font-weight: 800;
        cursor: pointer;
        transition: transform 0.2s ease, box-shadow 0.2s ease, opacity 0.2s ease;
    }

    .btn-cancel {
        border: 1px solid rgba(216, 222, 242, 0.95);
        color: #2d3f74;
        background: linear-gradient(180deg, #ffffff 0%, #f3f5ff 100%);
        box-shadow: 0 8px 18px rgba(76, 92, 143, 0.08);
    }

    .btn-create {
        border: none;
        color: #fff;
        background: linear-gradient(90deg, #1d49cf 0%, #1a7ef0 72%, #2eea59 100%);
        box-shadow: 0 14px 26px rgba(46, 96, 213, 0.24);
    }

        .btn-cancel:hover,
        .btn-create:hover {
            transform: translateY(-1px);
        }

        .btn-create:disabled {
            opacity: 0.7;
            cursor: not-allowed;
        }

    .modal-fade-enter-active,
    .modal-fade-leave-active {
        transition: opacity 0.22s ease, transform 0.22s ease;
    }

    .modal-fade-enter-from,
    .modal-fade-leave-to {
        opacity: 0;
    }

    .background-overlay {
        position: absolute;
        inset: 72px 0 0 0;
        z-index: 1;
        pointer-events: none;
        background: radial-gradient(circle at 8% 18%, rgba(255, 255, 255, 0.9) 0%, rgba(255, 255, 255, 0.08) 18%, transparent 32%), radial-gradient(circle at 88% 28%, rgba(255, 255, 255, 0.65) 0%, rgba(255, 255, 255, 0.05) 14%, transparent 28%);
    }

    .glow {
        position: absolute;
        border-radius: 999px;
        filter: blur(12px);
        opacity: 0.45;
    }

    .glow-1 {
        width: 260px;
        height: 260px;
        top: 18px;
        left: -30px;
        background: radial-gradient(circle, rgba(255, 255, 255, 0.7) 0%, rgba(155, 183, 255, 0.06) 55%, transparent 72%);
    }

    .glow-2 {
        width: 220px;
        height: 220px;
        top: 70px;
        right: 10px;
        background: radial-gradient(circle, rgba(255, 255, 255, 0.55) 0%, rgba(145, 175, 255, 0.05) 58%, transparent 76%);
    }

    .bottom-scene {
        position: absolute;
        inset: auto 0 0 0;
        height: 260px;
        z-index: 2;
        pointer-events: none;
    }

    .bus-illustration {
        position: absolute;
        left: 18px;
        bottom: 110px;
        width: 92px;
        height: 54px;
        opacity: 0.72;
    }

    .bus-shadow {
        position: absolute;
        left: 8px;
        right: 10px;
        bottom: 2px;
        height: 8px;
        border-radius: 50%;
        background: rgba(54, 71, 118, 0.18);
        filter: blur(3px);
    }

    .bus-body {
        position: absolute;
        inset: 0;
        border-radius: 14px 14px 10px 10px;
        background: linear-gradient(180deg, #ffffff 0%, #f3f7ff 100%);
        border: 2px solid rgba(78, 111, 190, 0.15);
        box-shadow: 0 10px 20px rgba(39, 58, 115, 0.12);
    }

    .bus-top {
        position: absolute;
        left: 10px;
        right: 8px;
        top: 6px;
        height: 12px;
        border-radius: 8px 10px 5px 5px;
        background: linear-gradient(90deg, #4ba6ff 0%, #74c1ff 55%, #ffffff 100%);
    }

    .bus-windows {
        position: absolute;
        left: 12px;
        right: 18px;
        top: 22px;
        display: flex;
        gap: 4px;
    }

        .bus-windows span {
            flex: 1;
            height: 10px;
            border-radius: 4px;
            background: linear-gradient(180deg, #83b9ff 0%, #dcecff 100%);
        }

    .bus-door {
        position: absolute;
        right: 8px;
        top: 20px;
        width: 9px;
        height: 18px;
        border-radius: 4px;
        background: linear-gradient(180deg, #163d99 0%, #2c63dc 100%);
    }

    .bus-stripe {
        position: absolute;
        left: 8px;
        right: 8px;
        bottom: 10px;
        height: 6px;
        border-radius: 999px;
        background: linear-gradient(90deg, #0f3dab 0%, #2672f1 58%, #f0c91f 100%);
    }

    .wheel {
        position: absolute;
        bottom: -5px;
        width: 14px;
        height: 14px;
        border-radius: 50%;
        background: #1f2c57;
        box-shadow: inset 0 0 0 3px #485988, inset 0 0 0 6px #d9e3ff;
    }

        .wheel.left {
            left: 18px;
        }

        .wheel.right {
            right: 16px;
        }

    .sparkles-layer {
        position: absolute;
        left: 0;
        right: 0;
        bottom: 60px;
        height: 110px;
        background-image: radial-gradient(circle, rgba(255, 247, 198, 0.82) 0 1.1px, transparent 1.4px), radial-gradient(circle, rgba(255, 251, 220, 0.62) 0 0.9px, transparent 1.3px), radial-gradient(circle, rgba(255, 244, 191, 0.45) 0 0.8px, transparent 1.1px);
        background-size: 18px 18px, 26px 26px, 34px 34px;
        background-position: 0 0, 12px 8px, 30px 16px;
        mask-image: linear-gradient(to top, rgba(0, 0, 0, 1), rgba(0, 0, 0, 0.15) 78%, transparent 100%);
        opacity: 0.55;
    }

    .wave-stack {
        position: absolute;
        inset: auto 0 0 0;
        height: 180px;
    }

    .wave {
        position: absolute;
        left: -8%;
        right: -8%;
        border-radius: 50%;
    }

    .wave-1 {
        bottom: 92px;
        height: 90px;
        background: rgba(255, 255, 255, 0.26);
        clip-path: ellipse(58% 42% at 50% 56%);
    }

    .wave-2 {
        bottom: 62px;
        height: 96px;
        background: rgba(215, 225, 254, 0.42);
        clip-path: ellipse(60% 44% at 50% 56%);
    }

    .wave-3 {
        bottom: 34px;
        height: 108px;
        background: linear-gradient(90deg, rgba(212, 222, 253, 0.82) 0%, rgba(242, 245, 255, 0.56) 44%, rgba(212, 222, 253, 0.82) 100%);
        clip-path: ellipse(61% 45% at 50% 56%);
    }

    .wave-4 {
        bottom: 6px;
        height: 118px;
        background: linear-gradient(90deg, #dde6fb 0%, #f9fbff 42%, #d9e4fb 100%);
        clip-path: ellipse(62% 46% at 50% 56%);
    }

    .wave-5 {
        bottom: -54px;
        height: 142px;
        background: linear-gradient(90deg, #183aa6 0%, #1f49bf 24%, #2d67df 55%, #6297e4 76%, #d0c34b 100%);
        clip-path: ellipse(64% 49% at 50% 56%);
    }

    @media (max-width: 1100px) {
        .routes-hero {
            flex-direction: column;
            align-items: flex-start;
        }

        .hero-copy h1 {
            font-size: 38px;
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

        .routes-content {
            padding: 22px 16px 105px;
        }

        .bottom-scene {
            height: 220px;
        }

        .bus-illustration {
            transform: scale(0.9);
            transform-origin: left bottom;
            bottom: 92px;
        }

        .route-modal {
            max-width: 100%;
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
            font-size: 31px;
        }

        .hero-copy p {
            font-size: 15px;
        }

        .new-route-btn {
            width: 100%;
        }

        .routes-content {
            padding-bottom: 95px;
        }

        .routes-table {
            min-width: 920px;
        }

        .bus-illustration {
            left: 8px;
            bottom: 86px;
            transform: scale(0.72);
        }

        .modal-overlay {
            padding: 10px;
            align-items: center;
        }

        .modal-header,
        .route-form {
            padding-left: 16px;
            padding-right: 16px;
        }

        .modal-header {
            padding-top: 14px;
            padding-bottom: 6px;
        }

        .route-form {
            padding-top: 6px;
            padding-bottom: 16px;
        }

        .modal-header h2 {
            font-size: 22px;
        }

        .modal-subtitle-row p {
            white-space: normal;
            text-align: center;
            font-size: 13px;
        }

        .form-group label {
            font-size: 13px;
        }

        .form-group input,
        .form-group select {
            height: 40px;
            font-size: 14px;
        }

        .modal-actions {
            flex-direction: column;
            margin-top: 14px;
            padding-top: 12px;
        }

        .btn-cancel,
        .btn-create {
            width: 100%;
            min-width: 100%;
            height: 38px;
            font-size: 14px;
        }
    }
</style>