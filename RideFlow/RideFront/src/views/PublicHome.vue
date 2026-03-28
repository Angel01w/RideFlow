<template>
	<div class="public-home">
		<header class="topbar">
			<div class="brand">
				<img src="../assets/images/RF.png" alt="RideFlow" class="brand-logo" />
			</div>
		</header>

		<main class="page-content">
			<section class="hero">
				<div class="hero-content">
					<span class="badge">Sistema de Transporte Institucional</span>
					<h1>Gestiona el transporte de tus colaboradores con orden y control</h1>
					<p>
						RideFlow centraliza rutas, colaboradores, asignaciones, asistencia y reportes
						en una sola plataforma moderna, clara y fácil de usar.
					</p>

					<div class="hero-actions">
						<button class="primary-btn" type="button" @click="goToLogin">
							Iniciar sesión
						</button>
					</div>

					<div class="hero-metrics">
						<div class="metric-card">
							<strong>Rutas</strong>
							<span>Organiza recorridos con origen, destino, horario y conductor.</span>
						</div>
						<div class="metric-card">
							<strong>Asistencia</strong>
							<span>Registra el control diario por colaborador y por ruta.</span>
						</div>
						<div class="metric-card">
							<strong>Reportes</strong>
							<span>Obtén visibilidad de ocupación y comportamiento operativo.</span>
						</div>
					</div>
				</div>

				<div class="hero-visual">
					<div class="hero-showcase">
						<div class="showcase-card">
							<div class="showcase-icon blue">⌖</div>
							<div class="showcase-copy">
								<h3>Planificación de rutas</h3>
								<p>Define recorridos y asigna conductores de forma ordenada.</p>
							</div>
						</div>

						<div class="showcase-card">
							<div class="showcase-icon yellow">👥</div>
							<div class="showcase-copy">
								<h3>Gestión de colaboradores</h3>
								<p>Administra quién utiliza el transporte institucional.</p>
							</div>
						</div>

						<div class="showcase-card">
							<div class="showcase-icon green">☑</div>
							<div class="showcase-copy">
								<h3>Control y seguimiento</h3>
								<p>Marca asistencia y consulta reportes para tomar decisiones.</p>
							</div>
						</div>

						<div class="showcase-flow">
							<div class="flow-mini">
								<span class="flow-step">1</span>
								<span>Crear rutas</span>
							</div>
							<div class="flow-line"></div>
							<div class="flow-mini">
								<span class="flow-step">2</span>
								<span>Asignar personal</span>
							</div>
							<div class="flow-line"></div>
							<div class="flow-mini">
								<span class="flow-step">3</span>
								<span>Controlar asistencia</span>
							</div>
						</div>
					</div>
				</div>
			</section>
		</main>

		<div class="waves">
			<div class="wave wave-1"></div>
			<div class="wave wave-2"></div>
			<div class="wave wave-3"></div>
		</div>
	</div>
</template>

<script setup lang="ts">
	import { useRouter } from 'vue-router'

	const router = useRouter()

	const goToLogin = () => {
		router.push('/login')
	}
</script>

<style scoped>
	.public-home {
		min-height: 100vh;
		position: relative;
		overflow-x: hidden;
		background: radial-gradient(circle at top left, rgba(255, 255, 255, 0.98), rgba(244, 245, 251, 0.96) 40%, rgba(232, 237, 248, 0.98) 100%);
		color: #24345f;
	}

	.topbar {
		height: 76px;
		padding: 0 28px;
		display: flex;
		align-items: center;
		justify-content: flex-start;
		background: rgba(255, 255, 255, 0.72);
		backdrop-filter: blur(14px);
		border-bottom: 1px solid rgba(220, 227, 244, 0.9);
		box-shadow: 0 10px 30px rgba(42, 63, 122, 0.08);
		position: relative;
		z-index: 5;
	}

	.brand {
		display: flex;
		align-items: center;
	}

	.brand-logo {
		height: 42px;
		object-fit: contain;
		display: block;
	}

	.page-content {
		position: relative;
		z-index: 3;
		max-width: 1180px;
		margin: 0 auto;
		padding: 36px 28px 180px;
	}

	.hero {
		display: grid;
		grid-template-columns: minmax(0, 1fr) 430px;
		gap: 34px;
		align-items: center;
		min-height: 620px;
	}

	.hero-content {
		max-width: 700px;
	}

	.badge {
		display: inline-flex;
		align-items: center;
		height: 34px;
		padding: 0 14px;
		border-radius: 999px;
		background: rgba(64, 108, 225, 0.1);
		color: #2c4da7;
		font-size: 13px;
		font-weight: 800;
		margin-bottom: 16px;
	}

	.hero-content h1 {
		margin: 0 0 16px;
		font-size: 60px;
		line-height: 1.02;
		letter-spacing: -0.04em;
		color: #253f82;
		max-width: 760px;
	}

	.hero-content p {
		margin: 0;
		font-size: 22px;
		line-height: 1.7;
		color: #5c6a8e;
		max-width: 680px;
	}

	.hero-actions {
		margin-top: 28px;
	}

	.primary-btn {
		height: 50px;
		padding: 0 24px;
		border: none;
		border-radius: 14px;
		color: #fff;
		cursor: pointer;
		font-size: 16px;
		font-weight: 800;
		background: linear-gradient(90deg, #2449c6 0%, #3f7df0 72%, #9ac861 100%);
		box-shadow: 0 14px 26px rgba(53, 86, 181, 0.22);
		transition: transform 0.2s ease, box-shadow 0.2s ease;
	}

		.primary-btn:hover {
			transform: translateY(-2px);
		}

	.hero-metrics {
		display: grid;
		grid-template-columns: repeat(3, minmax(0, 1fr));
		gap: 12px;
		margin-top: 26px;
		max-width: 700px;
	}

	.metric-card {
		padding: 16px;
		border-radius: 16px;
		background: linear-gradient(180deg, rgba(255, 255, 255, 0.88) 0%, rgba(248, 249, 255, 0.96) 100%);
		border: 1px solid rgba(217, 224, 240, 0.95);
		box-shadow: 0 12px 24px rgba(62, 82, 138, 0.08);
	}

		.metric-card strong {
			display: block;
			margin-bottom: 6px;
			font-size: 16px;
			color: #2b447d;
		}

		.metric-card span {
			font-size: 13px;
			color: #6c7898;
			line-height: 1.5;
		}

	.hero-visual {
		display: flex;
		justify-content: center;
	}

	.hero-showcase {
		width: 100%;
		max-width: 430px;
		padding: 18px;
		border-radius: 24px;
		background: linear-gradient(180deg, rgba(255, 255, 255, 0.92) 0%, rgba(247, 249, 255, 0.96) 100%);
		border: 1px solid rgba(215, 222, 242, 0.95);
		box-shadow: 0 20px 40px rgba(51, 72, 127, 0.12);
		display: grid;
		gap: 14px;
	}

	.showcase-card {
		display: flex;
		align-items: flex-start;
		gap: 14px;
		padding: 16px;
		border-radius: 18px;
		background: linear-gradient(180deg, rgba(255, 255, 255, 0.94) 0%, rgba(242, 246, 255, 0.96) 100%);
		border: 1px solid rgba(219, 226, 244, 0.95);
	}

	.showcase-icon {
		width: 46px;
		height: 46px;
		border-radius: 14px;
		display: flex;
		align-items: center;
		justify-content: center;
		font-size: 20px;
		color: #fff;
		flex-shrink: 0;
	}

		.showcase-icon.blue {
			background: linear-gradient(180deg, #5b80ea 0%, #4570da 100%);
		}

		.showcase-icon.yellow {
			background: linear-gradient(180deg, #f5cc42 0%, #eab512 100%);
		}

		.showcase-icon.green {
			background: linear-gradient(180deg, #31c96a 0%, #1cab56 100%);
		}

	.showcase-copy h3 {
		margin: 0 0 6px;
		font-size: 18px;
		color: #30477d;
	}

	.showcase-copy p {
		margin: 0;
		font-size: 14px;
		line-height: 1.6;
		color: #6c7898;
	}

	.showcase-flow {
		margin-top: 4px;
		padding: 16px;
		border-radius: 18px;
		background: linear-gradient(180deg, rgba(255, 255, 255, 0.94) 0%, rgba(242, 246, 255, 0.96) 100%);
		border: 1px solid rgba(219, 226, 244, 0.95);
		display: grid;
		gap: 10px;
	}

	.flow-mini {
		display: flex;
		align-items: center;
		gap: 10px;
		color: #3b5188;
		font-weight: 700;
		font-size: 14px;
	}

	.flow-step {
		width: 30px;
		height: 30px;
		border-radius: 999px;
		display: inline-flex;
		align-items: center;
		justify-content: center;
		background: linear-gradient(180deg, #edf2ff 0%, #dfe9ff 100%);
		color: #2c4da7;
		font-size: 13px;
		font-weight: 800;
		flex-shrink: 0;
	}

	.flow-line {
		width: 2px;
		height: 16px;
		margin-left: 14px;
		background: linear-gradient(180deg, #d8e2fb 0%, #bfd0f7 100%);
		border-radius: 999px;
	}

	.waves {
		position: absolute;
		left: 0;
		right: 0;
		bottom: 0;
		height: 240px;
		pointer-events: none;
	}

	.wave {
		position: absolute;
		left: -8%;
		right: -8%;
		border-radius: 50%;
	}

	.wave-1 {
		bottom: 82px;
		height: 100px;
		background: rgba(255, 255, 255, 0.24);
		clip-path: ellipse(58% 42% at 50% 56%);
	}

	.wave-2 {
		bottom: 38px;
		height: 120px;
		background: linear-gradient(90deg, #dde6fb 0%, #f9fbff 42%, #d9e4fb 100%);
		clip-path: ellipse(62% 46% at 50% 56%);
	}

	.wave-3 {
		bottom: -44px;
		height: 150px;
		background: linear-gradient(90deg, #183aa6 0%, #1f49bf 24%, #2d67df 55%, #6297e4 76%, #d0c34b 100%);
		clip-path: ellipse(64% 49% at 50% 56%);
	}

	@media (max-width: 1080px) {
		.hero {
			grid-template-columns: 1fr;
			min-height: auto;
		}

		.hero-content h1 {
			font-size: 46px;
		}
	}

	@media (max-width: 720px) {
		.topbar {
			padding: 0 16px;
		}

		.page-content {
			padding: 28px 16px 160px;
		}

		.hero-content h1 {
			font-size: 36px;
		}

		.hero-content p {
			font-size: 16px;
		}

		.hero-metrics {
			grid-template-columns: 1fr;
		}

		.hero-showcase {
			max-width: 100%;
		}

		.primary-btn {
			width: 100%;
		}
	}
</style>