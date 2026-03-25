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
    const roleFromUser =
        typeof user?.role === 'string'
            ? user.role
            : null

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
            let message = 'Error en login'

            try {
                const contentType = response.headers.get('content-type') || ''

                if (contentType.includes('application/json')) {
                    const errorData = await response.json()
                    message =
                        errorData?.message ||
                        errorData?.error ||
                        errorData?.title ||
                        message
                } else {
                    const errorText = await response.text()
                    message = errorText || message
                }
            } catch {
            }

            throw new Error(message)
        }

        const data: LoginResponse = await response.json()

        persistSession(data.token, data.user, role)

        return data
    } catch (error) {
        if (error instanceof TypeError) {
            throw new Error(`No se pudo conectar con la API en ${API_BASE}. Verifica que el backend esté encendido, la URL sea correcta y CORS esté configurado.`)
        }

        throw error
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