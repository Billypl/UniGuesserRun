// This file is now managed by vite-plugin-pwa
// The plugin automatically generates and registers the service worker

type Config = {
	onSuccess?: (registration: ServiceWorkerRegistration) => void
	onUpdate?: (registration: ServiceWorkerRegistration) => void
}

export function register(config?: Config) {
	// Service worker registration is now handled by vite-plugin-pwa
	// See vite.config.ts for PWA configuration
	if (import.meta.env.PROD && 'serviceWorker' in navigator) {
		window.addEventListener('load', () => {
			// The service worker is automatically registered by vite-plugin-pwa
			// You can access it via navigator.serviceWorker.ready
			navigator.serviceWorker.ready.then((registration) => {
				if (config && config.onSuccess) {
					config.onSuccess(registration)
				}
			})
		})
	}
}

export function unregister() {
	if ('serviceWorker' in navigator) {
		navigator.serviceWorker.ready
			.then((registration) => {
				registration.unregister()
			})
			.catch((error) => {
				console.error(error.message)
			})
	}
}
