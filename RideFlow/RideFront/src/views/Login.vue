<template>
    <div class="login-page">
        <div class="login-overlay"></div>

        <div class="login-card">
            <div class="card-glow card-glow--blue"></div>
            <div class="card-glow card-glow--gold"></div>

            <div class="logo-wrap">
                <img src="../assets/images/RF.png" alt="RideFlow" class="logo" />
            </div>

            <div class="header-copy">
                <h1 class="title">Bienvenido a RideFlow</h1>
                <p class="subtitle">Inicia sesión para gestionar rutas, colaboradores, asistencia y reportes.</p>
            </div>

            <form class="login-form" @submit.prevent="handleLogin">
                <div class="field-block">
                    <label class="field-label">Usuario</label>
                    <div class="input-wrap">
                        <span class="input-icon">👤</span>
                        <input v-model="username"
                               type="text"
                               placeholder="Ingrese su usuario"
                               autocomplete="username"
                               required />
                    </div>
                </div>

                <div class="field-block">
                    <label class="field-label">Contraseña</label>
                    <div class="input-wrap">
                        <span class="input-icon">🔒</span>
                        <input v-model="password"
                               :type="showPassword ? 'text' : 'password'"
                               placeholder="Ingrese su contraseña"
                               autocomplete="current-password"
                               required />
                        <button type="button"
                                class="toggle-password"
                                @click="showPassword = !showPassword">
                            {{ showPassword ? '🙈' : '👁️' }}
                        </button>
                    </div>
                </div>

                <div class="actions">
                    <button type="submit" class="submit-btn" :disabled="loading">
                        <span class="submit-icon">↪</span>
                        <span>{{ loading ? 'Iniciando...' : 'Iniciar Sesión' }}</span>
                    </button>

                    <button type="button" class="back-btn" @click="goToPublicHome">
                        <span class="back-icon">←</span>
                        <span>Volver al inicio</span>
                    </button>
                </div>

                <p v-if="error" class="error-text">{{ error }}</p>
            </form>

            <div class="login-footer">
                <span class="footer-pill footer-pill--blue"></span>
                <span class="footer-pill footer-pill--gold"></span>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
    import { ref } from 'vue'
    import { useRouter } from 'vue-router'
    import { login } from '../services/auth.service'

    const router = useRouter()

    const username = ref('')
    const password = ref('')
    const showPassword = ref(false)
    const loading = ref(false)
    const error = ref('')

    function getFriendlyErrorMessage(err: unknown): string {
        if (err instanceof Error && typeof err.message === 'string' && err.message.trim()) {
            const raw = err.message.trim()

            try {
                const parsed = JSON.parse(raw)

                if (parsed && typeof parsed === 'object') {
                    if (parsed.status === 401) return 'Usuario o contraseña incorrectos.'
                    if (parsed.status === 403) return 'No tienes permiso para acceder.'
                    if (parsed.status === 404) return 'No se encontró el servicio de inicio de sesión.'
                    if (parsed.status >= 500) return 'Ocurrió un error interno del servidor. Intenta de nuevo.'

                    if (typeof parsed.message === 'string' && parsed.message.trim()) {
                        return parsed.message.trim()
                    }

                    if (typeof parsed.error === 'string' && parsed.error.trim()) {
                        return parsed.error.trim()
                    }

                    if (typeof parsed.title === 'string' && parsed.title.trim()) {
                        const title = parsed.title.trim().toLowerCase()

                        if (title === 'unauthorized') return 'Usuario o contraseña incorrectos.'
                        if (title === 'forbidden') return 'No tienes permiso para acceder.'
                        if (title === 'not found') return 'No se encontró el servicio de inicio de sesión.'

                        return parsed.title.trim()
                    }
                }
            } catch {
            }

            if (raw.includes('"status":401') || raw.toLowerCase() === 'unauthorized') {
                return 'Usuario o contraseña incorrectos.'
            }

            if (raw.includes('"status":403') || raw.toLowerCase() === 'forbidden') {
                return 'No tienes permiso para acceder.'
            }

            if (raw.includes('"status":404')) {
                return 'No se encontró el servicio de inicio de sesión.'
            }

            if (raw.includes('"status":500')) {
                return 'Ocurrió un error interno del servidor. Intenta de nuevo.'
            }

            return raw
        }

        if (typeof err === 'string' && err.trim()) {
            const raw = err.trim()

            try {
                const parsed = JSON.parse(raw)

                if (parsed?.status === 401) return 'Usuario o contraseña incorrectos.'
                if (parsed?.status === 403) return 'No tienes permiso para acceder.'
                if (parsed?.status === 404) return 'No se encontró el servicio de inicio de sesión.'
                if (parsed?.status >= 500) return 'Ocurrió un error interno del servidor. Intenta de nuevo.'

                if (typeof parsed?.message === 'string' && parsed.message.trim()) {
                    return parsed.message.trim()
                }

                if (typeof parsed?.title === 'string' && parsed.title.trim()) {
                    const title = parsed.title.trim().toLowerCase()

                    if (title === 'unauthorized') return 'Usuario o contraseña incorrectos.'
                    if (title === 'forbidden') return 'No tienes permiso para acceder.'

                    return parsed.title.trim()
                }
            } catch {
            }

            return raw
        }

        if (err && typeof err === 'object') {
            const maybeMessage = (err as { message?: unknown }).message
            const maybeStatus = (err as { status?: unknown }).status
            const maybeTitle = (err as { title?: unknown }).title

            if (maybeStatus === 401) return 'Usuario o contraseña incorrectos.'
            if (maybeStatus === 403) return 'No tienes permiso para acceder.'
            if (maybeStatus === 404) return 'No se encontró el servicio de inicio de sesión.'
            if (typeof maybeStatus === 'number' && maybeStatus >= 500) {
                return 'Ocurrió un error interno del servidor. Intenta de nuevo.'
            }

            if (typeof maybeTitle === 'string' && maybeTitle.trim().toLowerCase() === 'unauthorized') {
                return 'Usuario o contraseña incorrectos.'
            }

            if (typeof maybeMessage === 'string' && maybeMessage.trim()) {
                return maybeMessage.trim()
            }
        }

        return 'No se pudo iniciar sesión.'
    }

    const handleLogin = async () => {
        error.value = ''
        loading.value = true

        try {
            await login(username.value, password.value, 'admin')
            router.push('/dashboard')
        } catch (err: unknown) {
            error.value = getFriendlyErrorMessage(err)
        } finally {
            loading.value = false
        }
    }

    const goToPublicHome = () => {
        router.push('/')
    }
</script>

<style scoped>
    .login-page {
        min-height: 100vh;
        position: relative;
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 24px;
        overflow: hidden;
        background: url('../assets/images/FondoLogin.png') center center / cover no-repeat;
    }

    .login-overlay {
        position: absolute;
        inset: 0;
        background: linear-gradient(135deg, rgba(9, 26, 63, 0.55) 0%, rgba(15, 42, 68, 0.35) 35%, rgba(255, 255, 255, 0.08) 100%);
        backdrop-filter: blur(2px);
    }

    .login-card {
        position: relative;
        z-index: 2;
        width: 100%;
        max-width: 430px;
        padding: 30px 26px 22px;
        border-radius: 30px;
        background: rgba(255, 255, 255, 0.90);
        border: 1px solid rgba(255, 255, 255, 0.55);
        box-shadow: 0 25px 60px rgba(8, 29, 74, 0.25), 0 10px 24px rgba(8, 29, 74, 0.12);
        backdrop-filter: blur(12px);
        overflow: hidden;
    }

    .card-glow {
        position: absolute;
        border-radius: 50%;
        filter: blur(40px);
        opacity: 0.22;
        pointer-events: none;
    }

    .card-glow--blue {
        width: 180px;
        height: 180px;
        top: -70px;
        right: -50px;
        background: #2b7cff;
    }

    .card-glow--gold {
        width: 140px;
        height: 140px;
        bottom: -50px;
        left: -30px;
        background: #f4cb3c;
    }

    .logo-wrap {
        display: flex;
        justify-content: center;
        margin-bottom: 10px;
    }

    .logo {
        width: 150px;
        max-width: 100%;
        object-fit: contain;
        display: block;
    }

    .header-copy {
        text-align: center;
        margin-bottom: 22px;
    }

    .title {
        margin: 0;
        font-size: 28px;
        font-weight: 900;
        line-height: 1.1;
        color: #142b57;
        letter-spacing: -0.02em;
    }

    .subtitle {
        margin: 10px 0 0;
        font-size: 14px;
        line-height: 1.6;
        color: #6d7b94;
    }

    .login-form {
        display: flex;
        flex-direction: column;
        gap: 14px;
    }

    .field-block {
        display: flex;
        flex-direction: column;
        gap: 7px;
    }

    .field-label {
        font-size: 13px;
        font-weight: 800;
        color: #21385f;
    }

    .input-wrap {
        position: relative;
        display: flex;
        align-items: center;
    }

    .input-icon {
        position: absolute;
        left: 14px;
        font-size: 14px;
        opacity: 0.78;
        pointer-events: none;
    }

    .input-wrap input {
        width: 100%;
        height: 52px;
        border: 1px solid #d9e2f0;
        border-radius: 14px;
        background: rgba(255, 255, 255, 0.88);
        padding: 0 46px 0 42px;
        font-size: 14px;
        color: #22324d;
        outline: none;
        box-sizing: border-box;
        transition: border-color 0.2s ease, box-shadow 0.2s ease, transform 0.2s ease;
    }

        .input-wrap input:focus {
            border-color: #2b7cff;
            background: #ffffff;
            box-shadow: 0 0 0 4px rgba(43, 124, 255, 0.10);
        }

        .input-wrap input::placeholder {
            color: #9aa5b6;
        }

    .toggle-password {
        position: absolute;
        right: 14px;
        border: none;
        background: transparent;
        cursor: pointer;
        font-size: 15px;
        padding: 0;
        line-height: 1;
    }

    .actions {
        display: flex;
        flex-direction: column;
        gap: 10px;
        margin-top: 8px;
    }

    .submit-btn {
        height: 54px;
        border: none;
        border-radius: 16px;
        background: linear-gradient(90deg, #0f49c7 0%, #1b7df2 55%, #13b5f7 100%);
        color: #ffffff;
        font-size: 15px;
        font-weight: 900;
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 10px;
        cursor: pointer;
        box-shadow: 0 16px 30px rgba(18, 130, 255, 0.26);
        transition: transform 0.18s ease, box-shadow 0.18s ease, opacity 0.18s ease;
    }

        .submit-btn:hover {
            transform: translateY(-1px);
            box-shadow: 0 18px 34px rgba(18, 130, 255, 0.30);
        }

        .submit-btn:disabled {
            opacity: 0.82;
            cursor: not-allowed;
            transform: none;
        }

    .back-btn {
        height: 48px;
        border: 1px solid #d7e1f2;
        border-radius: 14px;
        background: rgba(255, 255, 255, 0.82);
        color: #28457b;
        font-size: 14px;
        font-weight: 800;
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 8px;
        cursor: pointer;
        transition: transform 0.18s ease, border-color 0.18s ease, background 0.18s ease, box-shadow 0.18s ease;
    }

        .back-btn:hover {
            transform: translateY(-1px);
            border-color: #bcd0f5;
            background: #ffffff;
            box-shadow: 0 10px 22px rgba(63, 98, 173, 0.10);
        }

    .back-icon,
    .submit-icon {
        font-size: 15px;
        line-height: 1;
    }

    .error-text {
        margin: 2px 0 0;
        padding: 10px 12px;
        text-align: center;
        font-size: 13px;
        font-weight: 800;
        color: #d92d20;
        background: rgba(255, 240, 240, 0.95);
        border: 1px solid rgba(217, 45, 32, 0.18);
        border-radius: 12px;
    }

    .login-footer {
        margin-top: 20px;
        display: flex;
        justify-content: space-between;
        align-items: center;
    }

    .footer-pill {
        display: block;
        width: 86px;
        height: 6px;
        border-radius: 999px 999px 0 0;
    }

    .footer-pill--blue {
        background: linear-gradient(90deg, #0c3ba8 0%, #1b67f0 100%);
    }

    .footer-pill--gold {
        background: linear-gradient(90deg, #f4cb3c 0%, #6dcc57 100%);
    }

    @media (max-width: 520px) {
        .login-page {
            padding: 16px;
        }

        .login-card {
            max-width: 100%;
            padding: 24px 18px 18px;
            border-radius: 24px;
        }

        .logo {
            width: 132px;
        }

        .title {
            font-size: 24px;
        }

        .subtitle {
            font-size: 13px;
        }

        .input-wrap input {
            height: 48px;
            font-size: 13px;
        }

        .submit-btn {
            height: 50px;
            font-size: 14px;
        }

        .back-btn {
            height: 46px;
            font-size: 13px;
        }

        .footer-pill {
            width: 72px;
        }
    }
</style>