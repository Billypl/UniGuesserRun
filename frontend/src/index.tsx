import React from 'react'
import ReactDOM from 'react-dom/client'
import './Index.scss'
import App from './App'
import * as serviceWorkerRegistration from './serviceWorkerRegistration'

const root = ReactDOM.createRoot(document.getElementById('root') as HTMLElement)
root.render(
	<React.StrictMode>
		<App />
	</React.StrictMode>
)

// Register service worker for PWA support
serviceWorkerRegistration.register()
