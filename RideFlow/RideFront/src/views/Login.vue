<template>
    <div class="login-page">
        <div class="login-card">
            <div class="logo-wrap">
                <img src="../assets/images/RF.png" alt="RideFlow" class="logo" />
            </div>

            <h1 class="title">Sistema de Gestión de Transporte</h1>
            <p class="subtitle">Acceso Administrativo</p>

            <div class="role-switch">
                <button type="button"
                        class="role-btn"
                        :class="{ active: selectedRole === 'usuario' }"
                        @click="selectedRole = 'usuario'">
                    <span class="role-left">
                        <span class="role-icon">👤</span>
                        <span>Usuario</span>
                    </span>
                    <span class="role-radio" :class="{ checked: selectedRole === 'usuario' }"></span>
                </button>

                <button type="button"
                        class="role-btn"
                        :class="{ active: selectedRole === 'conductor' }"
                        @click="selectedRole = 'conductor'">
                    <span class="role-left">
                        <span class="role-icon">🚌</span>
                        <span>Conductor</span>
                    </span>
                    <span class="role-radio" :class="{ checked: selectedRole === 'conductor' }"></span>
                </button>

                <button type="button"
                        class="role-btn"
                        :class="{ active: selectedRole === 'admin' }"
                        @click="selectedRole = 'admin'">
                    <span class="role-left">
                        <span class="role-icon">🛡️</span>
                        <span>Admin</span>
                    </span>
                    <span class="role-radio" :class="{ checked: selectedRole === 'admin' }"></span>
                </button>
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

                <button type="submit" class="submit-btn" :disabled="loading">
                    <span class="submit-icon">↪</span>
                    <span>{{ loading ? 'Iniciando...' : 'Iniciar Sesión' }}</span>
                </button>

                <p v-if="error" class="error-text">{{ error }}</p>
            </form>

            <div class="card-accent left"></div>
            <div class="card-accent right"></div>
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
    const selectedRole = ref<'usuario' | 'conductor' | 'admin'>('usuario')

    const handleLogin = async () => {
        error.value = ''
        loading.value = true

        try {
            await login(username.value, password.value, selectedRole.value)
            router.push('/dashboard')
        } catch (err: any) {
            error.value = err?.message || 'No se pudo iniciar sesión'
        } finally {
            loading.value = false
        }
    }
</script>

<style scoped>
    .login-page {
        min-height: 100vh;
        background: linear-gradient(rgba(255, 255, 255, 0.1), rgba(255, 255, 255, 0.1)), url('../assets/images/FondoLogin.png') center center / cover no-repeat;
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 24px;
    }

    .login-card {
        position: relative;
        width: 100%;
        max-width: 360px;
        background: rgba(255, 255, 255, 0.9);
        border-radius: 26px;
        padding: 18px 18px 16px;
        box-shadow: 0 20px 45px rgba(15, 42, 68, 0.18), 0 8px 24px rgba(15, 42, 68, 0.1);
        backdrop-filter: blur(6px);
        overflow: hidden;
    }

    .logo-wrap {
        display: flex;
        justify-content: center;
        margin-bottom: 6px;
    }

    .logo {
        width: 150px;
        max-width: 100%;
        object-fit: contain;
        display: block;
    }

    .title {
        margin: 0;
        text-align: center;
        font-size: 15px;
        font-weight: 800;
        line-height: 1.2;
        color: #1b2b53;
    }

    .subtitle {
        margin: 2px 0 10px;
        text-align: center;
        font-size: 12px;
        font-weight: 500;
        color: #7d8797;
    }

    .role-switch {
        display: grid;
        grid-template-columns: repeat(3, 1fr);
        gap: 6px;
        background: #eef2f7;
        border-radius: 999px;
        padding: 4px;
        margin-bottom: 12px;
    }

    .role-btn {
        height: 34px;
        border: none;
        border-radius: 999px;
        background: transparent;
        color: #3f4d67;
        font-size: 11px;
        font-weight: 700;
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 4px;
        padding: 0 10px;
        cursor: pointer;
        transition: 0.2s ease;
    }

        .role-btn.active {
            background: linear-gradient(90deg, #08255c 0%, #1f79ff 100%);
            color: #ffffff;
            box-shadow: 0 8px 18px rgba(17, 113, 255, 0.22);
        }

    .role-left {
        display: flex;
        align-items: center;
        gap: 6px;
    }

    .role-icon {
        font-size: 11px;
        line-height: 1;
    }

    .role-radio {
        width: 12px;
        height: 12px;
        border-radius: 50%;
        border: 1.6px solid rgba(70, 84, 108, 0.4);
        background: #fff;
        flex-shrink: 0;
    }

    .role-btn.active .role-radio {
        border-color: rgba(255, 255, 255, 0.9);
        background: rgba(255, 255, 255, 0.25);
        position: relative;
    }

        .role-btn.active .role-radio.checked::after {
            content: '';
            position: absolute;
            inset: 2.2px;
            border-radius: 50%;
            background: #ffffff;
        }

    .login-form {
        display: flex;
        flex-direction: column;
    }

    .field-block {
        margin-bottom: 10px;
    }

    .field-label {
        display: block;
        margin-bottom: 4px;
        font-size: 12px;
        font-weight: 700;
        color: #24364e;
    }

    .input-wrap {
        position: relative;
        display: flex;
        align-items: center;
    }

    .input-icon {
        position: absolute;
        left: 12px;
        font-size: 12px;
        opacity: 0.75;
    }

    .input-wrap input {
        width: 100%;
        height: 38px;
        border: 1px solid #d8e0eb;
        border-radius: 8px;
        background: rgba(255, 255, 255, 0.75);
        padding: 0 38px 0 32px;
        font-size: 12px;
        color: #22324d;
        outline: none;
        transition: 0.2s ease;
        box-sizing: border-box;
    }

        .input-wrap input:focus {
            border-color: #2b7cff;
            background: #ffffff;
            box-shadow: 0 0 0 4px rgba(43, 124, 255, 0.08);
        }

        .input-wrap input::placeholder {
            color: #9aa5b6;
        }

    .toggle-password {
        position: absolute;
        right: 10px;
        border: none;
        background: transparent;
        cursor: pointer;
        font-size: 13px;
        padding: 0;
        line-height: 1;
    }

    .submit-btn {
        margin-top: 8px;
        height: 40px;
        border: none;
        border-radius: 8px;
        background: linear-gradient(90deg, #0f49c7 0%, #13b5f7 100%);
        color: #ffffff;
        font-size: 13px;
        font-weight: 800;
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 8px;
        cursor: pointer;
        box-shadow: 0 12px 24px rgba(18, 130, 255, 0.25);
        transition: transform 0.15s ease;
    }

        .submit-btn:hover {
            transform: translateY(-1px);
        }

        .submit-btn:disabled {
            opacity: 0.8;
            cursor: not-allowed;
        }

    .submit-icon {
        font-size: 14px;
        line-height: 1;
    }

    .error-text {
        margin: 8px 0 0;
        text-align: center;
        font-size: 12px;
        font-weight: 700;
        color: #d92d20;
    }

    .card-accent {
        position: absolute;
        bottom: 0;
        width: 74px;
        height: 6px;
        border-radius: 999px 999px 0 0;
    }

        .card-accent.left {
            left: 0;
            background: linear-gradient(90deg, #0c3ba8 0%, #1b67f0 100%);
        }

        .card-accent.right {
            right: 0;
            background: linear-gradient(90deg, #f4cb3c 0%, #6dcc57 100%);
        }

    @media (max-width: 480px) {
        .login-page {
            padding: 14px;
        }

        .login-card {
            max-width: 100%;
            padding: 18px 14px 14px;
        }

        .title {
            font-size: 14px;
        }

        .logo {
            width: 138px;
        }

        .role-switch {
            grid-template-columns: 1fr;
            border-radius: 18px;
        }

        .role-btn {
            border-radius: 14px;
        }
    }
</style>