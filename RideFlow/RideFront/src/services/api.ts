const API_BASE = (import.meta.env.VITE_API_BASE_URL || 'https://localhost:7191/api').replace(/\/$/, '')

export async function apiFetch(endpoint: string, options: RequestInit = {}) {
    const token = localStorage.getItem('rf_token')

    const headers = new Headers(options.headers || {})

    if (!headers.has('Accept')) {
        headers.set('Accept', 'application/json')
    }

    const hasBody = options.body !== undefined && options.body !== null
    const isFormData = typeof FormData !== 'undefined' && options.body instanceof FormData

    if (hasBody && !isFormData && !headers.has('Content-Type')) {
        headers.set('Content-Type', 'application/json')
    }

    if (token && !headers.has('Authorization')) {
        headers.set('Authorization', `Bearer ${token}`)
    }

    const normalizedEndpoint = endpoint.startsWith('/') ? endpoint : `/${endpoint}`

    return fetch(`${API_BASE}${normalizedEndpoint}`, {
        ...options,
        headers
    })
}

export { API_BASE }