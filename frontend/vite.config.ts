import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import { VitePWA } from 'vite-plugin-pwa'
import svgr from 'vite-plugin-svgr'
import path from 'path'
import fs from 'fs'
import { fileURLToPath } from 'url'

const __dirname = path.dirname(fileURLToPath(import.meta.url))

// https://vitejs.dev/config/
export default defineConfig({
	plugins: [
		react(),
		svgr(),
		VitePWA({
			registerType: 'autoUpdate',
			includeAssets: ['favicon.ico', 'logo192.png', 'logo512.png'],
			manifest: {
				name: 'Unigueser',
				short_name: 'Unigueser',
				description: 'A fun and engaging guessing game',
				theme_color: '#000000',
				icons: [
					{
						src: 'logo192.png',
						sizes: '192x192',
						type: 'image/png',
					},
					{
						src: 'logo512.png',
						sizes: '512x512',
						type: 'image/png',
					},
				],
			},
			workbox: {
				globPatterns: ['**/*.{js,css,html,ico,png,svg,json}'],
				runtimeCaching: [
					{
						urlPattern: /^https:\/\/.*\.tile\.openstreetmap\.org\/.*/i,
						handler: 'CacheFirst',
						options: {
							cacheName: 'openstreetmap-tiles',
							expiration: {
								maxEntries: 500,
								maxAgeSeconds: 60 * 60 * 24 * 30, // 30 days
							},
							cacheableResponse: {
								statuses: [0, 200],
							},
						},
					},
				],
			},
		}),
	],
	resolve: {
		alias: {
			'@': path.resolve(__dirname, './src'),
		},
	},
	css: {
		preprocessorOptions: {
			scss: {
				silenceDeprecations: ['legacy-js-api'],
			},
		},
	},
	server: {
		port: 3000,
		open: true,
	},
	preview: {
		port: 3000,
	},
	build: {
		outDir: 'build',
		sourcemap: true,
		rollupOptions: {
			output: {
				manualChunks: {
					'react-vendor': ['react', 'react-dom', 'react-router-dom'],
					'map-vendor': ['leaflet', 'react-leaflet'],
					'form-vendor': ['react-hook-form'],
				},
			},
		},
	},
})
