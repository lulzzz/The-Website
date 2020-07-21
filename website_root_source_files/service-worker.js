self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => event.respondWith(onFetch(event)));

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [/\.dll$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.txt$/, /\.css$/, /\.woff2$/, /\.svg$/, /\.webm$/, /\.webp$/ ];
const offlineAssetsExclude = [ /^service-worker\.js$/ ];

async function onInstall(event) {
    const assetsRequests = self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url) && !asset.url.includes("service-worker.js")))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
        .map(asset => new Request(asset.url, { integrity: asset.hash }));
    await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));
}

async function onActivate(event) {
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));
}

async function onFetch(event) {
    try {
        let cachedResponse = null;
        if (event.request.method === 'GET') {
            const shouldServeIndexHtml = event.request.mode === 'navigate';
    
            const request = shouldServeIndexHtml ? 'index.html' : event.request;
            const cache = await caches.open(cacheName);
            cachedResponse = await cache.match(request);
        }
        return cachedResponse || fetch(event.request);
    } catch (error) {
        return fetch(event.request);
    }
}

async function onSync(event) {
    if (event.tag == 'initSync') {
        event.waitUntil(async () =>
        {

        });
    }
}

/*
self.addEventListener('push', event => event.respondWith(onPush(event)));

async function onPush(event) {
    const payload = event.data.json();
    event.waitUntil(
        self.registration.showNotification('Blazing Pizza', {
            body: payload.message,
            icon: 'pwa-512.png',
            vibrate: [100, 50, 100],
            data: { url: payload.url }
        })
    );
}*/