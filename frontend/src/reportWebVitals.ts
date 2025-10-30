const reportWebVitals = (onPerfEntry?: (metric: any) => void) => {
	if (onPerfEntry && onPerfEntry instanceof Function) {
		import('web-vitals').then((wv) => {
			// web-vitals exports may vary across versions; call available functions defensively
			if ('getCLS' in wv) (wv as any).getCLS(onPerfEntry)
			if ('getFID' in wv) (wv as any).getFID(onPerfEntry)
			if ('getFCP' in wv) (wv as any).getFCP(onPerfEntry)
			if ('getLCP' in wv) (wv as any).getLCP(onPerfEntry)
			if ('getTTFB' in wv) (wv as any).getTTFB(onPerfEntry)
		})
	}
}

export default reportWebVitals
