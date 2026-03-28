<template>
    <div class="assignments-page" @click="closeUserMenu">
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

        <main class="assignments-content">
            <section class="assignments-hero">
                <div class="hero-copy">
                    <h1>Asignación de Colaboradores</h1>
                    <p>Asigne colaboradores a las rutas disponibles</p>
                </div>

                <button class="new-assignment-btn" type="button" @click="openCreateModal">
                    <span class="plus-icon">＋</span>
                    <span>Nueva Asignación</span>
                </button>
            </section>

            <section v-if="errorMessage" class="feedback-banner error-banner">
                {{ errorMessage }}
            </section>

            <section v-if="successMessage" class="feedback-banner success-banner">
                {{ successMessage }}
            </section>

            <section class="assignments-list">
                <article v-if="loading" class="route-card loading-card">
                    <p>Cargando asignaciones...</p>
                </article>

                <article v-for="routeItem in routeCards"
                         :key="routeItem.id"
                         class="route-card">
                    <div class="route-card-header">
                        <div class="route-card-info">
                            <h3>{{ routeItem.origin }} → {{ routeItem.destination }}</h3>
                            <p>Horario: {{ routeItem.schedule }}</p>
                        </div>

                        <div class="route-badges">
                            <span class="occupied-badge">{{ routeItem.assignedEmployees.length }} asignados</span>
                        </div>
                    </div>

                    <div class="route-card-body">
                        <div v-if="routeItem.assignedEmployees.length === 0" class="empty-state">
                            <div class="empty-icon">
                                <svg viewBox="0 0 64 64" fill="none" aria-hidden="true">
                                    <rect x="10" y="18" width="38" height="18" rx="4" stroke="currentColor" stroke-width="3" />
                                    <path d="M48 24H54C56.7614 24 59 26.2386 59 29V33C59 34.1046 58.1046 35 57 35H48V24Z" stroke="currentColor" stroke-width="3" />
                                    <circle cx="22" cy="42" r="5" stroke="currentColor" stroke-width="3" />
                                    <circle cx="46" cy="42" r="5" stroke="currentColor" stroke-width="3" />
                                    <path d="M15 18V12C15 10.8954 15.8954 10 17 10H41C42.1046 10 43 10.8954 43 12V18" stroke="currentColor" stroke-width="3" />
                                </svg>
                            </div>
                            <p>No hay colaboradores asignados a esta ruta</p>
                        </div>

                        <div v-else class="assigned-list">
                            <div v-for="assignment in routeItem.assignedEmployees"
                                 :key="assignment.id"
                                 class="assigned-item">
                                <div class="assigned-main">
                                    <div class="assigned-avatar">
                                        {{ getInitials(assignment.employeeName) }}
                                    </div>

                                    <div class="assigned-info">
                                        <strong>{{ assignment.employeeName }}</strong>
                                        <span>Asignado: {{ formatDate(assignment.assignedDate) }}</span>
                                    </div>
                                </div>

                                <div class="assigned-actions">
                                    <button class="mini-action edit"
                                            type="button"
                                            @click="editAssignment(assignment)"
                                            :disabled="saving || deletingId === assignment.id">
                                        ✎
                                    </button>
                                    <button class="mini-action delete"
                                            type="button"
                                            @click="deleteAssignment(assignment)"
                                            :disabled="saving || deletingId === assignment.id">
                                        {{ deletingId === assignment.id ? '...' : '🗑' }}
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </article>

                <article v-if="!loading && routeCards.length === 0" class="route-card loading-card">
                    <p>No hay rutas disponibles para asignar.</p>
                </article>
            </section>
        </main>

        <transition name="modal-fade">
            <div v-if="showCreateModal" class="modal-overlay" @click="closeCreateModal">
                <div class="assignment-modal" @click.stop>
                    <div class="modal-header">
                        <div class="modal-header-top">
                            <h2>{{ isEditing ? 'Editar Asignación' : 'Nueva Asignación' }}</h2>
                            <button class="modal-close" type="button" @click="closeCreateModal">✕</button>
                        </div>

                        <p class="modal-subtitle">
                            {{ isEditing ? 'Actualice la asignación del colaborador' : 'Asigne un colaborador a una ruta de transporte' }}
                        </p>
                    </div>

                    <form class="assignment-form" @submit.prevent="saveAssignment">
                        <div class="form-group">
                            <label>Colaborador <span>*</span></label>
                            <div class="select-wrap">
                                <select v-model="createForm.employeeId">
                                    <option value="">Seleccione un colaborador</option>
                                    <option v-for="employee in employees"
                                            :key="employee.id"
                                            :value="String(employee.id)">
                                        {{ employee.name }}
                                    </option>
                                </select>
                                <span class="select-arrow">⌄</span>
                            </div>
                        </div>

                        <div class="form-group">
                            <label>Ruta <span>*</span></label>
                            <div class="select-wrap">
                                <select v-model="createForm.routeId">
                                    <option value="">Seleccione una ruta</option>
                                    <option v-for="routeItem in routes"
                                            :key="routeItem.id"
                                            :value="String(routeItem.id)">
                                        {{ routeItem.origin }} → {{ routeItem.destination }}
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
                                {{ saving ? 'Guardando...' : (isEditing ? 'Actualizar' : 'Asignar') }}
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

    type RouteItem = {
        id: number | string
        origin: string
        destination: string
        schedule: string
    }

    type EmployeeItem = {
        id: number | string
        name: string
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
    const successMessage = ref('')
    const user = ref<StoredUser | null>(null)
    const routes = ref<RouteItem[]>([])
    const employees = ref<EmployeeItem[]>([])
    const assignments = ref<AssignmentItem[]>([])

    const createForm = reactive({
        employeeId: '',
        routeId: ''
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

    const routeCards = computed(() => {
        return routes.value.map(routeItem => {
            const assignedEmployees = assignments.value.filter(
                x => String(x.routeId) === String(routeItem.id) && x.isActive
            )

            return {
                ...routeItem,
                assignedEmployees
            }
        })
    })

    const isActive = (path: string) => route.path === path

    const toggleUserMenu = () => {
        isUserMenuOpen.value = !isUserMenuOpen.value
    }

    const closeUserMenu = () => {
        isUserMenuOpen.value = false
    }

    const clearMessages = () => {
        errorMessage.value = ''
        successMessage.value = ''
    }

    const handleLogout = () => {
        logout()
        closeUserMenu()
        router.push('/login')
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
            origin: item.origin ?? item.origen ?? item.startLocation ?? item.from ?? item.Origin ?? 'Sin origen',
            destination: item.destination ?? item.destino ?? item.endLocation ?? item.to ?? item.Destination ?? 'Sin destino',
            schedule: formatSchedule(item.schedule ?? item.horario ?? item.time ?? item.departureTime ?? item.DepartureTime)
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
            name: item.name ?? item.nombre ?? item.fullName ?? item.FullName ?? 'Sin nombre'
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

    const formatDate = (value: string) => {
        if (!value) return 'Sin fecha'

        const date = new Date(value)

        if (Number.isNaN(date.getTime())) {
            return value
        }

        return date.toLocaleDateString('es-DO', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit'
        })
    }

    const getInitials = (name: string) => {
        return name
            .split(' ')
            .filter(Boolean)
            .slice(0, 2)
            .map(part => part[0]?.toUpperCase() ?? '')
            .join('')
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

    const resetCreateForm = () => {
        createForm.employeeId = ''
        createForm.routeId = ''
        formError.value = ''
    }

    const openCreateModal = () => {
        resetCreateForm()
        clearMessages()
        isEditing.value = false
        editingId.value = null
        showCreateModal.value = true
    }

    const closeCreateModal = () => {
        showCreateModal.value = false
        isEditing.value = false
        editingId.value = null
        formError.value = ''
        resetCreateForm()
    }

    const loadData = async () => {
        loading.value = true
        errorMessage.value = ''

        try {
            user.value = getUser()

            const [routesResponse, employeesResponse, assignmentsResponse] = await Promise.all([
                apiFetch('/Routes'),
                apiFetch('/Employees'),
                apiFetch('/Assignments')
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

            if (assignmentsResponse?.ok) {
                const assignmentsData = await assignmentsResponse.json()
                assignments.value = normalizeAssignments(assignmentsData)
            } else {
                assignments.value = []
            }
        } catch {
            errorMessage.value = 'No se pudieron cargar las asignaciones.'
            routes.value = []
            employees.value = []
            assignments.value = []
        } finally {
            loading.value = false
        }
    }

    const saveAssignment = async () => {
        formError.value = ''
        clearMessages()

        if (!createForm.employeeId || !createForm.routeId) {
            formError.value = 'Completa todos los campos obligatorios.'
            return
        }

        saving.value = true

        const exists = assignments.value.some(x =>
            String(x.employeeId) === String(createForm.employeeId) &&
            String(x.routeId) === String(createForm.routeId) &&
            x.isActive &&
            (!isEditing.value || String(x.id) !== String(editingId.value))
        )

        if (exists) {
            formError.value = 'Este colaborador ya está asignado a esta ruta.'
            saving.value = false
            return
        }

        try {
            let response: Response

            if (isEditing.value && editingId.value !== null) {
                response = await apiFetch(`/Assignments/${editingId.value}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        employeeId: Number(createForm.employeeId),
                        routeId: Number(createForm.routeId),
                        isActive: true
                    })
                })
            } else {
                response = await apiFetch('/Assignments', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        employeeId: Number(createForm.employeeId),
                        routeId: Number(createForm.routeId)
                    })
                })
            }

            if (!response?.ok) {
                const serverMessage = response ? await parseErrorMessage(response) : null
                formError.value = serverMessage || 'No se pudo guardar la asignación.'
                return
            }

            await loadData()
            closeCreateModal()
            successMessage.value = isEditing.value
                ? 'La asignación fue actualizada correctamente.'
                : 'La asignación fue creada correctamente.'
        } catch {
            formError.value = 'No se pudo guardar la asignación.'
        } finally {
            saving.value = false
        }
    }

    const editAssignment = (assignment: AssignmentItem) => {
        clearMessages()
        createForm.employeeId = String(assignment.employeeId)
        createForm.routeId = String(assignment.routeId)
        isEditing.value = true
        editingId.value = assignment.id
        formError.value = ''
        showCreateModal.value = true
    }

    const deleteAssignment = async (assignment: AssignmentItem) => {
        clearMessages()
        deletingId.value = assignment.id

        try {
            const response = await apiFetch(`/Assignments/${assignment.id}`, {
                method: 'DELETE'
            })

            if (!response?.ok) {
                const serverMessage = response ? await parseErrorMessage(response) : null
                errorMessage.value = serverMessage || 'No se pudo eliminar la asignación.'
                return
            }

            assignments.value = assignments.value.filter(x => String(x.id) !== String(assignment.id))
            await loadData()
            successMessage.value = 'La asignación fue eliminada correctamente.'
        } catch {
            errorMessage.value = 'No se pudo eliminar la asignación.'
        } finally {
            deletingId.value = null
        }
    }

    onMounted(loadData)
</script>

<style scoped>
    .assignments-page {
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

    .assignments-content {
        position: relative;
        z-index: 8;
        max-width: 1180px;
        margin: 0 auto;
        padding: 28px 28px 110px;
    }

    .assignments-hero {
        display: flex;
        align-items: flex-start;
        justify-content: space-between;
        gap: 20px;
        margin-bottom: 22px;
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

    .feedback-banner {
        margin-bottom: 16px;
        padding: 14px 16px;
        border-radius: 12px;
        font-size: 14px;
        font-weight: 700;
        border: 1px solid;
        box-shadow: 0 10px 20px rgba(56, 74, 131, 0.08);
    }

    .error-banner {
        color: #a53b4c;
        background: #fff4f6;
        border-color: #f1c7cf;
    }

    .success-banner {
        color: #226347;
        background: #f1fff7;
        border-color: #bfe7cf;
    }

    .new-assignment-btn {
        height: 50px;
        min-width: 190px;
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
        background: linear-gradient(90deg, #1948ce 0%, #2c65df 68%, #36cb69 100%);
        box-shadow: 0 12px 24px rgba(42, 76, 181, 0.24);
        transition: transform 0.2s ease, box-shadow 0.2s ease;
    }

        .new-assignment-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 15px 28px rgba(42, 76, 181, 0.28);
        }

    .plus-icon {
        font-size: 18px;
        line-height: 1;
    }

    .assignments-list {
        display: grid;
        gap: 12px;
    }

    .route-card {
        border-radius: 12px;
        border: 1px solid rgba(201, 210, 240, 0.85);
        background: linear-gradient(180deg, rgba(255, 255, 255, 0.92) 0%, rgba(246, 248, 255, 0.9) 100%);
        box-shadow: 0 12px 24px rgba(56, 74, 131, 0.1);
        overflow: hidden;
    }

    .route-card-header {
        display: flex;
        align-items: flex-start;
        justify-content: space-between;
        gap: 14px;
        padding: 14px 16px 12px;
        border-bottom: 1px solid rgba(217, 224, 243, 0.9);
    }

    .route-card-info h3 {
        margin: 0 0 4px;
        font-size: 15px;
        font-weight: 800;
        color: #283d75;
    }

    .route-card-info p {
        margin: 0;
        font-size: 13px;
        color: #56678f;
        font-weight: 500;
    }

    .route-badges {
        display: flex;
        align-items: center;
        gap: 6px;
        flex-shrink: 0;
    }

    .occupied-badge {
        display: inline-flex;
        align-items: center;
        height: 20px;
        padding: 0 8px;
        border-radius: 6px;
        background: #142b73;
        color: #fff;
        font-size: 11px;
        font-weight: 800;
    }

    .route-card-body {
        min-height: 112px;
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 16px;
    }

    .empty-state {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        color: #7684a7;
        text-align: center;
    }

    .empty-icon {
        width: 34px;
        height: 34px;
        margin-bottom: 10px;
        color: #b2bcdb;
    }

        .empty-icon svg {
            width: 100%;
            height: 100%;
            display: block;
        }

    .empty-state p {
        margin: 0;
        font-size: 14px;
        color: #56678f;
    }

    .assigned-list {
        width: 100%;
        display: grid;
        gap: 10px;
    }

    .assigned-item {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 12px;
        padding: 10px 12px;
        border-radius: 10px;
        background: rgba(243, 246, 255, 0.92);
        border: 1px solid rgba(212, 220, 245, 0.9);
    }

    .assigned-main {
        display: flex;
        align-items: center;
        gap: 12px;
        min-width: 0;
    }

    .assigned-avatar {
        width: 36px;
        height: 36px;
        border-radius: 999px;
        display: flex;
        align-items: center;
        justify-content: center;
        background: linear-gradient(180deg, #1d49cf 0%, #1a7ef0 100%);
        color: #fff;
        font-size: 12px;
        font-weight: 800;
        flex-shrink: 0;
    }

    .assigned-info {
        display: flex;
        flex-direction: column;
        min-width: 0;
    }

        .assigned-info strong {
            color: #283d75;
            font-size: 14px;
            font-weight: 800;
        }

        .assigned-info span {
            color: #667596;
            font-size: 12px;
            font-weight: 600;
        }

    .assigned-actions {
        display: flex;
        align-items: center;
        gap: 8px;
        flex-shrink: 0;
    }

    .mini-action {
        width: 32px;
        height: 32px;
        border: none;
        border-radius: 8px;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        cursor: pointer;
        font-size: 14px;
        box-shadow: 0 6px 12px rgba(61, 79, 133, 0.18);
        transition: transform 0.2s ease, opacity 0.2s ease;
    }

        .mini-action:hover {
            transform: translateY(-1px);
        }

        .mini-action:disabled {
            opacity: 0.6;
            cursor: not-allowed;
        }

        .mini-action.edit {
            background: linear-gradient(180deg, #ffffff 0%, #edf1ff 100%);
            color: #29448e;
        }

        .mini-action.delete {
            background: linear-gradient(180deg, #ef6172 0%, #d9485e 100%);
            color: #fff;
        }

    .loading-card {
        min-height: 84px;
        display: flex;
        align-items: center;
        justify-content: center;
        font-weight: 700;
        color: #667596;
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
        padding: 16px;
    }

    .assignment-modal {
        width: 100%;
        max-width: 500px;
        border-radius: 16px;
        background: linear-gradient(180deg, rgba(255, 255, 255, 0.99) 0%, rgba(247, 246, 252, 0.98) 100%);
        border: 1px solid rgba(216, 220, 242, 0.95);
        box-shadow: 0 20px 40px rgba(36, 54, 108, 0.16);
        overflow: hidden;
    }

    .modal-header {
        padding: 18px 20px 8px;
    }

    .modal-header-top {
        display: flex;
        align-items: flex-start;
        justify-content: space-between;
        gap: 12px;
    }

    .modal-header h2 {
        margin: 0;
        font-size: 18px;
        line-height: 1.1;
        font-weight: 800;
        color: #243a7a;
    }

    .modal-close {
        width: 28px;
        height: 28px;
        border: none;
        background: transparent;
        color: #32467c;
        font-size: 18px;
        line-height: 1;
        cursor: pointer;
        border-radius: 8px;
        transition: background 0.2s ease;
    }

        .modal-close:hover {
            background: rgba(233, 239, 255, 0.8);
        }

    .modal-subtitle {
        margin: 8px 0 0;
        font-size: 14px;
        color: #596890;
        font-weight: 500;
    }

    .assignment-form {
        padding: 8px 20px 18px;
    }

    .form-group + .form-group {
        margin-top: 18px;
    }

    .form-group label {
        display: inline-block;
        margin-bottom: 8px;
        font-size: 14px;
        font-weight: 800;
        color: #253a72;
    }

        .form-group label span {
            color: #eb5569;
        }

    .select-wrap {
        position: relative;
    }

        .select-wrap select {
            width: 100%;
            height: 42px;
            border-radius: 8px;
            border: 1px solid rgba(186, 198, 235, 0.95);
            background: linear-gradient(180deg, rgba(255, 255, 255, 0.95) 0%, rgba(246, 247, 255, 0.98) 100%);
            padding: 0 42px 0 14px;
            font-size: 14px;
            color: #32467c;
            outline: none;
            appearance: none;
            transition: border-color 0.2s ease, box-shadow 0.2s ease;
        }

            .select-wrap select:focus {
                border-color: #6d92f7;
                box-shadow: 0 0 0 3px rgba(101, 137, 242, 0.12);
            }

    .select-arrow {
        position: absolute;
        right: 14px;
        top: 50%;
        transform: translateY(-50%);
        color: #a1accd;
        font-size: 16px;
        pointer-events: none;
    }

    .form-error {
        margin: 12px 0 0;
        color: #d74b60;
        font-size: 12px;
        font-weight: 700;
    }

    .modal-actions {
        margin-top: 22px;
        display: flex;
        justify-content: flex-end;
        gap: 10px;
    }

    .btn-cancel,
    .btn-create {
        min-width: 122px;
        height: 38px;
        border-radius: 8px;
        padding: 0 16px;
        font-size: 13px;
        font-weight: 800;
        cursor: pointer;
        transition: transform 0.2s ease, box-shadow 0.2s ease, opacity 0.2s ease;
    }

    .btn-cancel {
        border: 1px solid rgba(216, 222, 242, 0.95);
        color: #2d3f74;
        background: linear-gradient(180deg, #ffffff 0%, #f3f5ff 100%);
        box-shadow: 0 6px 14px rgba(76, 92, 143, 0.08);
    }

    .btn-create {
        border: none;
        color: #fff;
        background: linear-gradient(90deg, #1d49cf 0%, #1a7ef0 72%, #2eea59 100%);
        box-shadow: 0 12px 22px rgba(46, 96, 213, 0.24);
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
        .assignments-hero {
            flex-direction: column;
            align-items: flex-start;
        }

        .route-card-header {
            flex-direction: column;
            align-items: flex-start;
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

        .assignments-content {
            padding: 22px 16px 100px;
        }

        .assignment-modal {
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
            font-size: 26px;
        }

        .hero-copy p {
            font-size: 14px;
        }

        .new-assignment-btn {
            width: 100%;
        }

        .assigned-item {
            flex-direction: column;
            align-items: flex-start;
        }

        .assigned-actions {
            width: 100%;
            justify-content: flex-end;
        }

        .modal-overlay {
            padding: 10px;
        }

        .modal-actions {
            flex-direction: column;
        }

        .btn-cancel,
        .btn-create {
            width: 100%;
            min-width: 100%;
        }
    }
</style>