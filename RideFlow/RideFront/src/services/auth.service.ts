const API_BASE = (import.meta.env.VITE_API_BASE_URL || 'https://localhost:7191/api').replace(/\/$/, '')

export type UserRole = 'usuario' | 'conductor' | 'admin'

export type StoredUser = {
    id?: number | string
    name?: string
    fullName?: string
    username?: string
    userName?: string
    email?: string
    role?: string
    [key: string]: unknown
}

type LoginResponse = {
    token: string
    user: StoredUser
}

const TOKEN_KEY = 'rf_token'
const USER_KEY = 'rf_user'
const ROLE_KEY = 'rf_role'

function normalizeRole(role?: string | null): string | null {
    if (!role) return null

    const normalized = role.trim().toLowerCase()

    if (normalized === 'user') return 'usuario'
    if (normalized === 'driver') return 'conductor'
    if (normalized === 'administrator') return 'admin'

    return normalized
}

function resolveUserRole(user?: StoredUser | null, fallbackRole?: string | null): string | null {
    const roleFromUser = typeof user?.role === 'string' ? user.role : null
    return normalizeRole(roleFromUser) || normalizeRole(fallbackRole)
}

function persistSession(token: string, user: StoredUser, role?: string | null): void {
    const resolvedRole = resolveUserRole(user, role)

    localStorage.setItem(TOKEN_KEY, token)
    localStorage.setItem(USER_KEY, JSON.stringify(user))

    if (resolvedRole) {
        localStorage.setItem(ROLE_KEY, resolvedRole)
    } else {
        localStorage.removeItem(ROLE_KEY)
    }
}

function extractValidationError(errors: unknown): string | null {
    if (!errors || typeof errors !== 'object') return null

    const entries = Object.entries(errors as Record<string, unknown>)

    for (const [, value] of entries) {
        if (Array.isArray(value) && value.length > 0) {
            const first = value[0]
            if (typeof first === 'string' && first.trim()) {
                return first
            }
        }

        if (typeof value === 'string' && value.trim()) {
            return value
        }
    }

    return null
}

function mapHttpErrorToMessage(status: number, data: any): string {
    const validationMessage = extractValidationError(data?.errors)
    if (validationMessage) return validationMessage

    if (typeof data?.message === 'string' && data.message.trim()) {
        return data.message
    }

    if (typeof data?.error === 'string' && data.error.trim()) {
        return data.error
    }

    if (status === 400) {
        return 'Los datos enviados no son válidos.'
    }

    if (status === 401) {
        return 'Usuario o contraseña incorrectos.'
    }

    if (status === 403) {
        return 'No tienes permiso para acceder.'
    }

    if (status === 404) {
        return 'No se encontró el servicio de inicio de sesión.'
    }

    if (status === 405) {
        return 'Método no permitido para iniciar sesión.'
    }

    if (status === 408) {
        return 'La solicitud tardó demasiado. Intenta de nuevo.'
    }

    if (status === 409) {
        return 'No se pudo completar el inicio de sesión por un conflicto en la solicitud.'
    }

    if (status === 422) {
        return 'La información enviada no pudo ser procesada.'
    }

    if (status >= 500) {
        return 'Ocurrió un error interno del servidor. Intenta de nuevo.'
    }

    if (typeof data?.title === 'string' && data.title.trim() && data.title.trim().toLowerCase() !== 'unauthorized') {
        return data.title
    }

    return 'No se pudo iniciar sesión.'
}

async function readErrorResponse(response: Response): Promise<string> {
    try {
        const contentType = response.headers.get('content-type') || ''

        if (contentType.includes('application/json')) {
            const errorData = await response.json()
            return mapHttpErrorToMessage(response.status, errorData)
        }

        const errorText = await response.text()

        if (errorText?.trim()) {
            return errorText.trim()
        }
    } catch {
    }

    return mapHttpErrorToMessage(response.status, null)
}

export async function login(
    username: string,
    password: string,
    role: UserRole | string
): Promise<LoginResponse> {
    try {
        const response = await fetch(`${API_BASE}/Auth/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                Accept: 'application/json'
            },
            body: JSON.stringify({
                username,
                password,
                role
            })
        })

        if (!response.ok) {
            const message = await readErrorResponse(response)
            throw new Error(message)
        }

        const data: LoginResponse = await response.json()

        if (!data?.token) {
            throw new Error('La respuesta del servidor no contiene un token válido.')
        }

        persistSession(data.token, data.user, role)

        return data
    } catch (error) {
        if (error instanceof TypeError) {
            throw new Error(`No se pudo conectar con la API en ${API_BASE}. Verifica que el backend esté encendido, la URL sea correcta y CORS esté configurado.`)
        }

        if (error instanceof Error) {
            throw error
        }

        throw new Error('No se pudo iniciar sesión.')
    }
}

export function getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY)
}

export function getUser(): StoredUser | null {
    const rawUser = localStorage.getItem(USER_KEY)

    if (!rawUser) {
        return null
    }

    try {
        return JSON.parse(rawUser) as StoredUser
    } catch {
        localStorage.removeItem(USER_KEY)
        return null
    }
}

export function getRole(): string | null {
    const user = getUser()
    const storedRole = localStorage.getItem(ROLE_KEY)
    const resolvedRole = resolveUserRole(user, storedRole)

    if (resolvedRole && resolvedRole !== storedRole) {
        localStorage.setItem(ROLE_KEY, resolvedRole)
    }

    return resolvedRole
}

export function isAuthenticated(): boolean {
    return !!getToken()
}

export function logout(): void {
    localStorage.removeItem(TOKEN_KEY)
    localStorage.removeItem(USER_KEY)
    localStorage.removeItem(ROLE_KEY)
}