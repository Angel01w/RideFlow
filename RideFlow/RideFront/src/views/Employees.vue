<template>
    <div class="employees-page" @click="closeUserMenu">
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

        <main class="employees-content">
            <section class="employees-hero">
                <div class="hero-copy">
                    <h1>Gestión de Colaboradores</h1>
                    <p>Administra los colaboradores que utilizan el transporte</p>
                </div>

                <button class="new-employee-btn" type="button" @click="openCreateModal">
                    <span class="plus-icon">＋</span>
                    <span>Nuevo Colaborador</span>
                </button>
            </section>

            <section class="employees-table-card">
                <div class="table-wrap">
                    <table class="employees-table">
                        <thead>
                            <tr>
                                <th>Nombre</th>
                                <th>Departamento</th>
                                <th>Contacto</th>
                                <th>Email</th>
                                <th class="actions-col">Acciones</th>
                            </tr>
                        </thead>

                        <tbody>
                            <tr v-if="loading">
                                <td colspan="5" class="empty-cell">Cargando colaboradores...</td>
                            </tr>

                            <tr v-else-if="errorMessage">
                                <td colspan="5" class="empty-cell">{{ errorMessage }}</td>
                            </tr>

                            <tr v-else-if="employees.length === 0">
                                <td colspan="5" class="empty-cell">No hay colaboradores registrados</td>
                            </tr>

                            <tr v-for="employee in employees" :key="employee.id">
                                <td>{{ employee.name }}</td>
                                <td>{{ employee.department }}</td>
                                <td>{{ employee.contact }}</td>
                                <td>{{ employee.email }}</td>
                                <td class="row-actions">
                                    <button class="icon-btn edit" type="button" @click="editEmployee(employee)" :disabled="creating || deletingId === employee.id">
                                        ✎
                                    </button>
                                    <button class="icon-btn delete" type="button" @click="deleteEmployee(employee)" :disabled="creating || deletingId === employee.id">
                                        {{ deletingId === employee.id ? '...' : '🗑' }}
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
                <div class="employee-modal" @click.stop>
                    <div class="modal-header">
                        <div class="modal-header-top">
                            <h2><span>{{ isEditing ? 'Editar' : 'Nuevo' }}</span> Colaborador</h2>
                            <button class="modal-close" type="button" @click="closeCreateModal">✕</button>
                        </div>

                        <div class="modal-subtitle-row">
                            <span class="header-line"></span>
                            <p>{{ isEditing ? 'Actualiza la información del colaborador' : 'Complete la información del colaborador' }}</p>
                            <span class="header-line"></span>
                        </div>
                    </div>

                    <form class="employee-form" @submit.prevent="saveEmployee">
                        <div class="form-group">
                            <label>Nombre Completo <span>*</span></label>
                            <input v-model.trim="createForm.name"
                                   type="text"
                                   placeholder="Nombre del colaborador" />
                        </div>

                        <div class="form-group">
                            <label>Departamento <span>*</span></label>
                            <input v-model.trim="createForm.department"
                                   type="text"
                                   placeholder="Área de trabajo" />
                        </div>

                        <div class="form-group">
                            <label>Teléfono de Contacto <span>*</span></label>
                            <input v-model.trim="createForm.contact"
                                   type="text"
                                   placeholder="555-0000" />
                        </div>

                        <div class="form-group">
                            <label>Correo Electrónico <span>*</span></label>
                            <input v-model.trim="createForm.email"
                                   type="email"
                                   placeholder="correo@empresa.com" />
                        </div>

                        <p v-if="formError" class="form-error">{{ formError }}</p>

                        <div class="modal-actions">
                            <button class="btn-cancel" type="button" @click="closeCreateModal">
                                Cancelar
                            </button>
                            <button class="btn-create" type="submit" :disabled="creating">
                                {{ creating ? 'Guardando...' : (isEditing ? 'Actualizar' : 'Crear') }}
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

    type EmployeeItem = {
        id: number | string
        name: string
        department: string
        contact: string
        email: string
        isActive?: boolean
    }

    const route = useRoute()
    const router = useRouter()

    const loading = ref(false)
    const creating = ref(false)
    const deletingId = ref<number | string | null>(null)
    const isUserMenuOpen = ref(false)
    const showCreateModal = ref(false)
    const isEditing = ref(false)
    const editingId = ref<number | string | null>(null)
    const formError = ref('')
    const errorMessage = ref('')
    const user = ref<StoredUser | null>(null)
    const employees = ref<EmployeeItem[]>([])

    const createForm = reactive({
        name: '',
        department: '',
        contact: '',
        email: ''
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
            name: item.name ?? item.nombre ?? item.fullName ?? item.FullName ?? 'Sin nombre',
            department: item.department ?? item.departamento ?? item.area ?? item.Department ?? 'Sin departamento',
            contact: item.contact ?? item.phone ?? item.telefono ?? item.celular ?? item.Phone ?? 'Sin contacto',
            email: item.email ?? item.correo ?? item.Email ?? 'Sin email',
            isActive: item.isActive ?? item.IsActive ?? true
        }))
    }

    const resetCreateForm = () => {
        createForm.name = ''
        createForm.department = ''
        createForm.contact = ''
        createForm.email = ''
        formError.value = ''
    }

    const openCreateModal = () => {
        resetCreateForm()
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

    const loadEmployees = async () => {
        loading.value = true
        errorMessage.value = ''

        try {
            user.value = getUser()

            const response = await apiFetch('/Employees')

            if (!response?.ok) {
                const serverMessage = response ? await parseErrorMessage(response) : null
                errorMessage.value = serverMessage || 'No se pudieron cargar los colaboradores.'
                employees.value = []
                return
            }

            const data = await response.json()
            employees.value = normalizeEmployees(data)
        } catch {
            errorMessage.value = 'No se pudieron cargar los colaboradores.'
            employees.value = []
        } finally {
            loading.value = false
        }
    }

    const saveEmployee = async () => {
        formError.value = ''

        if (!createForm.name || !createForm.department || !createForm.contact || !createForm.email) {
            formError.value = 'Completa todos los campos obligatorios.'
            return
        }

        creating.value = true

        try {
            let response: Response

            if (isEditing.value && editingId.value !== null) {
                const currentEmployee = employees.value.find(x => String(x.id) === String(editingId.value))

                const payload = {
                    fullName: createForm.name.trim(),
                    department: createForm.department.trim(),
                    phone: createForm.contact.trim(),
                    email: createForm.email.trim(),
                    isActive: currentEmployee?.isActive ?? true
                }

                response = await apiFetch(`/Employees/${editingId.value}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(payload)
                })
            } else {
                const payload = {
                    fullName: createForm.name.trim(),
                    department: createForm.department.trim(),
                    phone: createForm.contact.trim(),
                    email: createForm.email.trim()
                }

                response = await apiFetch('/Employees', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(payload)
                })
            }

            if (!response?.ok) {
                const serverMessage = response ? await parseErrorMessage(response) : null
                formError.value = serverMessage || 'No se pudo guardar el colaborador.'
                return
            }

            await loadEmployees()
            closeCreateModal()
        } catch {
            formError.value = 'No se pudo guardar el colaborador.'
        } finally {
            creating.value = false
        }
    }

    const editEmployee = (employee: EmployeeItem) => {
        createForm.name = employee.name
        createForm.department = employee.department
        createForm.contact = employee.contact
        createForm.email = employee.email

        isEditing.value = true
        editingId.value = employee.id
        formError.value = ''
        showCreateModal.value = true
    }

    const deleteEmployee = async (employee: EmployeeItem) => {
        const confirmed = window.confirm(`¿Deseas eliminar a ${employee.name}?`)
        if (!confirmed) return

        deletingId.value = employee.id

        try {
            const response = await apiFetch(`/Employees/${employee.id}`, {
                method: 'DELETE'
            })

            if (!response?.ok) {
                throw new Error('No se pudo eliminar el colaborador')
            }

            employees.value = employees.value.filter(x => x.id !== employee.id)
        } catch {
            alert('No se pudo eliminar el colaborador')
        } finally {
            deletingId.value = null
        }
    }

    onMounted(loadEmployees)
</script>

<style scoped>
    .employees-page {
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

    .employees-content {
        position: relative;
        z-index: 8;
        max-width: 1180px;
        margin: 0 auto;
        padding: 28px 28px 110px;
    }

    .employees-hero {
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

    .new-employee-btn {
        height: 50px;
        min-width: 196px;
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

        .new-employee-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 15px 28px rgba(42, 76, 181, 0.28);
        }

    .plus-icon {
        font-size: 18px;
        line-height: 1;
    }

    .employees-table-card {
        position: relative;
    }

    .table-wrap {
        overflow-x: auto;
        border-radius: 16px;
        border: 1px solid rgba(198, 209, 242, 0.85);
        box-shadow: 0 14px 28px rgba(54, 72, 126, 0.12);
        background: linear-gradient(180deg, rgba(255, 255, 255, 0.92) 0%, rgba(245, 247, 255, 0.9) 100%);
    }

    .employees-table {
        width: 100%;
        border-collapse: separate;
        border-spacing: 0;
        min-width: 900px;
    }

        .employees-table thead th {
            padding: 14px 16px;
            text-align: left;
            font-size: 14px;
            font-weight: 700;
            color: rgba(255, 255, 255, 0.98);
            background: linear-gradient(90deg, #0c2faa 0%, #1a49c9 35%, #1d60df 70%, #1848c7 100%);
            border-right: 1px solid rgba(255, 255, 255, 0.12);
        }

            .employees-table thead th:first-child {
                border-top-left-radius: 14px;
            }

            .employees-table thead th:last-child {
                border-top-right-radius: 14px;
                border-right: none;
                text-align: center;
            }

        .employees-table tbody td {
            padding: 13px 16px;
            font-size: 15px;
            font-weight: 600;
            color: #2f416f;
            background: rgba(255, 255, 255, 0.74);
            border-bottom: 1px solid rgba(209, 218, 241, 0.85);
        }

        .employees-table tbody tr:last-child td:first-child {
            border-bottom-left-radius: 14px;
        }

        .employees-table tbody tr:last-child td:last-child {
            border-bottom-right-radius: 14px;
        }

        .employees-table tbody tr:hover td {
            background: rgba(247, 249, 255, 0.98);
        }

    .actions-col {
        width: 120px;
    }

    .row-actions {
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 10px;
    }

    .icon-btn {
        width: 32px;
        height: 32px;
        border: none;
        border-radius: 8px;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        cursor: pointer;
        font-size: 15px;
        box-shadow: 0 6px 12px rgba(61, 79, 133, 0.18);
        transition: transform 0.2s ease;
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
        font-weight: 700;
        padding: 22px 16px !important;
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

    .employee-modal {
        width: 100%;
        max-width: 430px;
        border-radius: 18px;
        background: linear-gradient(180deg, rgba(255, 255, 255, 0.99) 0%, rgba(247, 246, 252, 0.98) 100%);
        border: 1px solid rgba(216, 220, 242, 0.95);
        box-shadow: 0 20px 40px rgba(36, 54, 108, 0.16);
        overflow: hidden;
        position: relative;
    }

    .modal-header {
        padding: 14px 16px 8px;
    }

    .modal-header-top {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 12px;
    }

    .modal-header h2 {
        margin: 0;
        flex: 1;
        text-align: center;
        font-size: 16px;
        line-height: 1.1;
        font-weight: 500;
        color: #243a7a;
    }

        .modal-header h2 span {
            font-weight: 800;
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

    .modal-subtitle-row {
        margin-top: 4px;
        display: flex;
        align-items: center;
        gap: 10px;
    }

        .modal-subtitle-row p {
            margin: 0;
            white-space: nowrap;
            font-size: 12px;
            color: #596890;
            font-weight: 500;
        }

    .header-line {
        flex: 1;
        height: 1px;
        background: linear-gradient(90deg, transparent 0%, rgba(206, 213, 235, 0.85) 30%, rgba(206, 213, 235, 0.85) 70%, transparent 100%);
    }

    .employee-form {
        padding: 8px 16px 14px;
    }

    .form-group + .form-group {
        margin-top: 10px;
    }

    .form-group label {
        display: inline-block;
        margin-bottom: 5px;
        font-size: 13px;
        font-weight: 800;
        color: #253a72;
    }

        .form-group label span {
            color: #eb5569;
        }

    .form-group input {
        width: 100%;
        height: 30px;
        border-radius: 6px;
        border: 1px solid rgba(186, 198, 235, 0.95);
        background: linear-gradient(180deg, rgba(255, 255, 255, 0.95) 0%, rgba(246, 247, 255, 0.98) 100%);
        padding: 0 12px;
        font-size: 12px;
        color: #32467c;
        outline: none;
        transition: border-color 0.2s ease, box-shadow 0.2s ease;
    }

        .form-group input::placeholder {
            color: #707da1;
        }

        .form-group input:focus {
            border-color: #6d92f7;
            box-shadow: 0 0 0 3px rgba(101, 137, 242, 0.12);
        }

    .form-error {
        margin: 10px 0 0;
        color: #d74b60;
        font-size: 12px;
        font-weight: 700;
    }

    .modal-actions {
        margin-top: 12px;
        padding-top: 10px;
        border-top: 1px solid rgba(219, 225, 243, 0.95);
        display: flex;
        justify-content: flex-end;
        gap: 10px;
    }

    .btn-cancel,
    .btn-create {
        min-width: 104px;
        height: 34px;
        border-radius: 8px;
        padding: 0 16px;
        font-size: 12px;
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
        background: rgba(215, 225, 254, 0.42);
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
        .employees-hero {
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

        .employees-content {
            padding: 22px 16px 100px;
        }

        .employee-modal {
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

        .new-employee-btn {
            width: 100%;
        }

        .modal-overlay {
            padding: 10px;
        }

        .modal-header,
        .employee-form {
            padding-left: 14px;
            padding-right: 14px;
        }

        .modal-subtitle-row p {
            white-space: normal;
            text-align: center;
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