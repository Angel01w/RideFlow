<template>
	<div class="reports-page" @click="closeUserMenu">
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

		<main class="reports-content">
			<section class="reports-hero">
				<div class="hero-copy">
					<h1>Reportes</h1>
					<p>Análisis de ocupación, asistencia y estadísticas</p>
				</div>
			</section>

			<section class="tabs-row">
				<button v-for="tab in tabs"
						:key="tab.key"
						type="button"
						class="report-tab"
						:class="{ active: activeTab === tab.key }"
						@click="activeTab = tab.key">
					{{ tab.label }}
				</button>
			</section>

			<section class="filters-card">
				<div class="filters-grid">
					<div class="filter-field">
						<label>Desde</label>
						<input v-model="dateFrom" type="date" />
					</div>

					<div class="filter-field">
						<label>Hasta</label>
						<input v-model="dateTo" type="date" />
					</div>

					<div class="filter-actions">
						<button type="button" class="secondary-btn" @click="clearDateFilters">
							Limpiar filtros
						</button>
					</div>
				</div>
			</section>

			<section class="report-card">
				<div class="report-card-header">
					<div class="report-title-wrap">
						<span class="report-icon">
							<svg viewBox="0 0 24 24" fill="none" aria-hidden="true">
								<path d="M12 21C16.4183 21 20 17.4183 20 13C20 8.58172 16.4183 5 12 5C7.58172 5 4 8.58172 4 13C4 17.4183 7.58172 21 12 21Z" stroke="currentColor" stroke-width="2" />
								<path d="M12 13L15 10" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
								<path d="M12 21V17" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
								<path d="M8 3H16" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
							</svg>
						</span>
						<h2>{{ reportTitle }}</h2>
					</div>
				</div>

				<div class="table-wrap" v-if="activeTab === 'ocupacion'">
					<table class="reports-table">
						<thead>
							<tr>
								<th>Ruta</th>
								<th>Horario</th>
								<th>Asignados</th>
								<th>Ocupación</th>
								<th>Estado</th>
							</tr>
						</thead>

						<tbody>
							<tr v-if="loading">
								<td colspan="5" class="empty-cell">Cargando reporte...</td>
							</tr>

							<tr v-else-if="occupancyRows.length === 0">
								<td colspan="5" class="empty-cell">No hay datos para mostrar</td>
							</tr>

							<tr v-for="row in occupancyRows" :key="row.id">
								<td>{{ row.route }}</td>
								<td>{{ row.schedule }}</td>
								<td>{{ row.assigned }}</td>
								<td>
									<div class="occupancy-cell">
										<div class="occupancy-bar">
											<span class="occupancy-fill" :style="{ width: row.occupancyPercent + '%' }"></span>
										</div>
										<span>{{ row.occupancyPercent }}%</span>
									</div>
								</td>
								<td>
									<span class="status-badge">
										{{ row.status }}
									</span>
								</td>
							</tr>
						</tbody>
					</table>
				</div>

				<div class="table-wrap" v-else-if="activeTab === 'asistencia'">
					<table class="reports-table">
						<thead>
							<tr>
								<th>Ruta</th>
								<th>Asignados</th>
								<th>Presentes</th>
								<th>Ausentes</th>
								<th>Total Marcados</th>
								<th>% Asistencia</th>
							</tr>
						</thead>

						<tbody>
							<tr v-if="loading">
								<td colspan="6" class="empty-cell">Cargando reporte...</td>
							</tr>

							<tr v-else-if="attendanceRows.length === 0">
								<td colspan="6" class="empty-cell">No hay datos para mostrar</td>
							</tr>

							<tr v-for="row in attendanceRows" :key="row.id">
								<td>{{ row.route }}</td>
								<td>{{ row.assigned }}</td>
								<td>{{ row.present }}</td>
								<td>{{ row.absent }}</td>
								<td>{{ row.total }}</td>
								<td>{{ row.percent }}%</td>
							</tr>
						</tbody>
					</table>
				</div>

				<div class="table-wrap" v-else-if="activeTab === 'mas-utilizadas'">
					<table class="reports-table">
						<thead>
							<tr>
								<th>Ruta</th>
								<th>Asignaciones</th>
								<th>Ranking</th>
							</tr>
						</thead>

						<tbody>
							<tr v-if="loading">
								<td colspan="3" class="empty-cell">Cargando reporte...</td>
							</tr>

							<tr v-else-if="mostUsedRows.length === 0">
								<td colspan="3" class="empty-cell">No hay datos para mostrar</td>
							</tr>

							<tr v-for="row in mostUsedRows" :key="row.id">
								<td>{{ row.route }}</td>
								<td>{{ row.count }}</td>
								<td>#{{ row.rank }}</td>
							</tr>
						</tbody>
					</table>
				</div>

				<div class="table-wrap" v-else>
					<table class="reports-table">
						<thead>
							<tr>
								<th>Fecha</th>
								<th>Asignados</th>
								<th>Presentes</th>
								<th>Ausentes</th>
								<th>Total Marcados</th>
								<th>% Asistencia</th>
							</tr>
						</thead>

						<tbody>
							<tr v-if="loading">
								<td colspan="6" class="empty-cell">Cargando reporte...</td>
							</tr>

							<tr v-else-if="dailyRows.length === 0">
								<td colspan="6" class="empty-cell">No hay datos para mostrar</td>
							</tr>

							<tr v-for="row in dailyRows" :key="row.date">
								<td>{{ row.date }}</td>
								<td>{{ row.assigned }}</td>
								<td>{{ row.present }}</td>
								<td>{{ row.absent }}</td>
								<td>{{ row.total }}</td>
								<td>{{ row.percent }}%</td>
							</tr>
						</tbody>
					</table>
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
		schedule: string
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

	type OccupancyRow = {
		id: number | string
		route: string
		schedule: string
		assigned: number
		occupancyPercent: number
		status: string
	}

	const route = useRoute()
	const router = useRouter()

	const isUserMenuOpen = ref(false)
	const user = ref<StoredUser | null>(null)
	const loading = ref(false)
	const activeTab = ref('ocupacion')
	const dateFrom = ref('')
	const dateTo = ref('')

	const routes = ref<RouteItem[]>([])
	const assignments = ref<AssignmentItem[]>([])
	const attendances = ref<AttendanceItem[]>([])

	const tabs = [
		{ key: 'ocupacion', label: 'Ocupación de Rutas' },
		{ key: 'asistencia', label: 'Asistencia' },
		{ key: 'mas-utilizadas', label: 'Rutas Más Utilizadas' },
		{ key: 'asistencia-diaria', label: 'Asistencia Diaria' }
	]

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

	const reportTitle = computed(() => {
		if (activeTab.value === 'ocupacion') return 'Reporte de Ocupación por Ruta'
		if (activeTab.value === 'asistencia') return 'Reporte de Asistencia por Ruta'
		if (activeTab.value === 'mas-utilizadas') return 'Reporte de Rutas Más Utilizadas'
		return 'Reporte de Asistencia Diaria'
	})

	const isPresentStatus = (value: unknown) => {
		const status = String(value ?? '').trim().toLowerCase()
		return status === 'present' || status === 'presente'
	}

	const isAbsentStatus = (value: unknown) => {
		const status = String(value ?? '').trim().toLowerCase()
		return status === 'absent' || status === 'ausente'
	}

	const normalizeDate = (value: unknown) => {
		const raw = String(value ?? '').trim()
		if (!raw) return ''
		return raw.slice(0, 10)
	}

	const isDateInRange = (date: string) => {
		if (!date) return false
		if (dateFrom.value && date < dateFrom.value) return false
		if (dateTo.value && date > dateTo.value) return false
		return true
	}

	const clearDateFilters = () => {
		dateFrom.value = ''
		dateTo.value = ''
	}

	const filteredAttendances = computed(() => {
		return attendances.value.filter(item => {
			const date = normalizeDate(item.attendanceDate)
			const validStatus = isPresentStatus(item.status) || isAbsentStatus(item.status)
			return !!date && validStatus && isDateInRange(date)
		})
	})

	const activeAssignmentsForRange = computed(() => {
		if (!dateFrom.value && !dateTo.value) {
			return assignments.value.filter(x => x.isActive)
		}

		return assignments.value.filter(item => {
			if (!item.isActive) return false

			const assignedDate = normalizeDate(item.assignedDate) || '0000-00-00'

			if (dateTo.value && assignedDate > dateTo.value) return false

			return true
		})
	})

	const occupancyRows = computed<OccupancyRow[]>(() => {
		return routes.value.map(routeItem => {
			const assigned = activeAssignmentsForRange.value.filter(
				x => String(x.routeId) === String(routeItem.id)
			).length

			const occupancyPercent = assigned > 0 ? 100 : 0

			return {
				id: routeItem.id,
				route: `${routeItem.origin} → ${routeItem.destination}`,
				schedule: routeItem.schedule,
				assigned,
				occupancyPercent,
				status: assigned > 0 ? 'Con asignaciones' : 'Sin asignaciones'
			}
		})
	})

	const attendanceRows = computed(() => {
		return routes.value
			.map(routeItem => {
				const assignedEmployees = activeAssignmentsForRange.value.filter(
					x => String(x.routeId) === String(routeItem.id)
				)

				const assignedEmployeeIds = new Set(
					assignedEmployees.map(x => String(x.employeeId))
				)

				const presentKeys = new Set<string>()
				const markedKeys = new Set<string>()

				for (const item of filteredAttendances.value) {
					const routeId = String(item.routeId ?? '').trim()
					const employeeId = String(item.employeeId ?? '').trim()
					const key = `${routeId}|${employeeId}`

					if (routeId !== String(routeItem.id)) continue
					if (!assignedEmployeeIds.has(employeeId)) continue

					markedKeys.add(key)

					if (isPresentStatus(item.status)) {
						presentKeys.add(key)
					}
				}

				const assigned = assignedEmployees.length
				const present = presentKeys.size
				const absent = assigned > present ? assigned - present : 0
				const total = markedKeys.size
				const percent = assigned > 0 ? Math.round((present / assigned) * 100) : 0

				return {
					id: routeItem.id,
					route: `${routeItem.origin} → ${routeItem.destination}`,
					assigned,
					present,
					absent,
					total,
					percent
				}
			})
			.filter(x => x.assigned > 0 || x.total > 0)
	})

	const mostUsedRows = computed(() => {
		return [...routes.value]
			.map(routeItem => {
				const count = activeAssignmentsForRange.value.filter(
					x => String(x.routeId) === String(routeItem.id)
				).length

				return {
					id: routeItem.id,
					route: `${routeItem.origin} → ${routeItem.destination}`,
					count
				}
			})
			.filter(x => x.count > 0)
			.sort((a, b) => b.count - a.count)
			.map((item, index) => ({
				...item,
				rank: index + 1
			}))
	})

	const dailyRows = computed(() => {
		const dates = [...new Set(filteredAttendances.value.map(x => normalizeDate(x.attendanceDate)))]
			.filter(Boolean)
			.sort((a, b) => a.localeCompare(b))

		return dates
			.map(date => {
				const assignmentsForDate = assignments.value.filter(item => {
					if (!item.isActive) return false
					const assignedDate = normalizeDate(item.assignedDate) || '0000-00-00'
					return assignedDate <= date
				})

				const assignedKeys = new Set(
					assignmentsForDate.map(x => `${String(x.routeId)}|${String(x.employeeId)}`)
				)

				const presentKeys = new Set<string>()
				const markedKeys = new Set<string>()

				for (const item of filteredAttendances.value) {
					const itemDate = normalizeDate(item.attendanceDate)
					if (itemDate !== date) continue

					const routeId = String(item.routeId ?? '').trim()
					const employeeId = String(item.employeeId ?? '').trim()
					const key = `${routeId}|${employeeId}`

					if (!assignedKeys.has(key)) continue

					markedKeys.add(key)

					if (isPresentStatus(item.status)) {
						presentKeys.add(key)
					}
				}

				const assigned = assignedKeys.size
				const present = presentKeys.size
				const absent = assigned > present ? assigned - present : 0
				const total = markedKeys.size
				const percent = assigned > 0 ? Math.round((present / assigned) * 100) : 0

				return {
					date,
					assigned,
					present,
					absent,
					total,
					percent
				}
			})
			.filter(x => x.assigned > 0 || x.total > 0)
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
			routeName: item.routeName ?? item.RouteName ?? '',
			assignedDate: item.assignedDate ?? item.AssignedDate ?? '',
			isActive: Boolean(item.isActive ?? item.IsActive ?? true)
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

		return list
			.map((item: any, index: number) => ({
				id: item.id ?? item.attendanceId ?? item.Id ?? index + 1,
				employeeId: item.employeeId ?? item.EmployeeId ?? '',
				routeId: item.routeId ?? item.RouteId ?? '',
				attendanceDate: item.attendanceDate ?? item.AttendanceDate ?? '',
				status: item.status ?? item.Status ?? '',
				markedAt: item.markedAt ?? item.MarkedAt ?? ''
			}))
			.filter(item => {
				const hasDate = normalizeDate(item.attendanceDate).length > 0
				const validStatus = isPresentStatus(item.status) || isAbsentStatus(item.status)
				return hasDate && validStatus
			})
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

	const loadReport = async () => {
		loading.value = true

		try {
			user.value = getUser()

			const [routesResponse, assignmentsResponse, attendancesResponse] = await Promise.all([
				apiFetch('/Routes'),
				apiFetch('/Assignments'),
				apiFetch('/Attendances')
			])

			if (routesResponse?.ok) {
				const data = await routesResponse.json()
				routes.value = normalizeRoutes(data)
			} else {
				routes.value = []
			}

			if (assignmentsResponse?.ok) {
				const data = await assignmentsResponse.json()
				assignments.value = normalizeAssignments(data)
			} else {
				assignments.value = []
			}

			if (attendancesResponse?.ok) {
				const data = await attendancesResponse.json()
				attendances.value = normalizeAttendances(data)
			} else {
				attendances.value = []
			}
		} catch (error) {
			console.error('Error cargando reportes:', error)
			routes.value = []
			assignments.value = []
			attendances.value = []
		} finally {
			loading.value = false
		}
	}

	onMounted(loadReport)
</script>

<style scoped>
	.reports-page {
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

	.reports-content {
		position: relative;
		z-index: 8;
		max-width: 1180px;
		margin: 0 auto;
		padding: 28px 28px 110px;
	}

	.reports-hero {
		margin-bottom: 18px;
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

	.tabs-row {
		display: inline-flex;
		align-items: center;
		gap: 2px;
		padding: 4px;
		margin-bottom: 14px;
		border-radius: 999px;
		background: rgba(229, 232, 249, 0.86);
		border: 1px solid rgba(211, 217, 239, 0.9);
	}

	.report-tab {
		height: 32px;
		padding: 0 14px;
		border: none;
		border-radius: 999px;
		background: transparent;
		color: #34497f;
		font-size: 13px;
		font-weight: 700;
		cursor: pointer;
		transition: background 0.2s ease, color 0.2s ease;
	}

		.report-tab.active {
			background: linear-gradient(180deg, #ffffff 0%, #f6f8ff 100%);
			box-shadow: 0 4px 12px rgba(88, 103, 149, 0.08);
		}

	.filters-card {
		margin-bottom: 14px;
		padding: 16px 18px;
		border-radius: 16px;
		border: 1px solid rgba(205, 213, 239, 0.88);
		background: linear-gradient(180deg, rgba(255, 255, 255, 0.92) 0%, rgba(246, 248, 255, 0.9) 100%);
		box-shadow: 0 12px 24px rgba(59, 76, 132, 0.08);
	}

	.filters-grid {
		display: grid;
		grid-template-columns: repeat(3, minmax(0, 1fr));
		gap: 14px;
		align-items: end;
	}

	.filter-field {
		display: flex;
		flex-direction: column;
		gap: 6px;
	}

		.filter-field label {
			font-size: 13px;
			font-weight: 800;
			color: #2d4177;
		}

		.filter-field input {
			height: 44px;
			border: 1px solid rgba(205, 213, 239, 0.95);
			border-radius: 12px;
			padding: 0 12px;
			font-size: 14px;
			color: #34497d;
			background: #fff;
			outline: none;
		}

			.filter-field input:focus {
				border-color: #4f8ef7;
				box-shadow: 0 0 0 4px rgba(79, 142, 247, 0.12);
			}

	.filter-actions {
		display: flex;
		align-items: end;
	}

	.secondary-btn {
		height: 44px;
		padding: 0 16px;
		border: 1px solid rgba(205, 213, 239, 0.95);
		border-radius: 12px;
		background: #fff;
		color: #2d4177;
		font-size: 14px;
		font-weight: 800;
		cursor: pointer;
		transition: all 0.2s ease;
	}

		.secondary-btn:hover {
			background: #f6f8ff;
			border-color: #b9c8f3;
		}

	.report-card {
		border-radius: 16px;
		border: 1px solid rgba(205, 213, 239, 0.88);
		background: linear-gradient(180deg, rgba(255, 255, 255, 0.92) 0%, rgba(246, 248, 255, 0.9) 100%);
		box-shadow: 0 12px 24px rgba(59, 76, 132, 0.08);
		padding: 16px 18px 12px;
	}

	.report-card-header {
		margin-bottom: 14px;
	}

	.report-title-wrap {
		display: flex;
		align-items: center;
		gap: 8px;
	}

	.report-icon {
		width: 14px;
		height: 14px;
		color: #243a7a;
		display: inline-flex;
	}

		.report-icon svg {
			width: 100%;
			height: 100%;
			display: block;
		}

	.report-title-wrap h2 {
		margin: 0;
		font-size: 14px;
		font-weight: 800;
		color: #243a7a;
	}

	.table-wrap {
		overflow-x: auto;
	}

	.reports-table {
		width: 100%;
		border-collapse: separate;
		border-spacing: 0;
		min-width: 900px;
	}

		.reports-table thead th {
			padding: 12px 8px 10px;
			text-align: left;
			font-size: 13px;
			font-weight: 800;
			color: #2d4177;
			border-bottom: 1px solid rgba(217, 223, 241, 0.95);
		}

		.reports-table tbody td {
			padding: 12px 8px;
			font-size: 14px;
			color: #34497d;
			border-bottom: 1px solid rgba(229, 233, 247, 0.88);
		}

		.reports-table tbody tr:last-child td {
			border-bottom: none;
		}

	.occupancy-cell {
		display: flex;
		align-items: center;
		gap: 8px;
	}

	.occupancy-bar {
		width: 52px;
		height: 6px;
		border-radius: 999px;
		background: #ddd8ef;
		overflow: hidden;
		flex-shrink: 0;
	}

	.occupancy-fill {
		display: block;
		height: 100%;
		border-radius: 999px;
		background: linear-gradient(90deg, #b7b6ea 0%, #c7c2f0 100%);
	}

	.status-badge {
		height: 24px;
		padding: 0 10px;
		border-radius: 8px;
		background: #1a2f7a;
		color: #fff;
		font-size: 12px;
		font-weight: 800;
		display: inline-flex;
		align-items: center;
		justify-content: center;
	}

	.empty-cell {
		text-align: center;
		color: #667596;
		font-weight: 700;
		padding: 22px 16px !important;
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

		.reports-content {
			padding: 22px 16px 100px;
		}

		.tabs-row {
			width: 100%;
			overflow-x: auto;
			display: flex;
		}

		.filters-grid {
			grid-template-columns: 1fr;
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
	}
</style>