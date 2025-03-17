window.indexedDbHelpers = {
    ensureStoreExists: function (storeName) {
        const dbRequest = indexedDB.open('MyDatabase', 1); // Increment version for schema changes

        dbRequest.onupgradeneeded = (event) => {
            const db = event.target.result;

            // Create the store if it doesn't exist
            if (!db.objectStoreNames.contains(storeName)) {
                db.createObjectStore(storeName, { keyPath: 'key' });
                console.log(`Object store '${storeName}' created.`);
            }
        };

        dbRequest.onerror = (event) => {
            console.error('Database error during ensureStoreExists:', event.target.error);
        };
    },
    addOrUpdate: async function (storeName, key, value) {
        const dbRequest = indexedDB.open('MyDatabase', 1);
        console.log('addOrUpdate called'); 
        console.log('addOrUpdate called:' + storeName); 
        dbRequest.onupgradeneeded = (event) => {
            const db = event.target.result;
            if (!db.objectStoreNames.contains(storeName)) {
                db.createObjectStore(storeName, { keyPath: 'key' }); // Create store if it doesn’t exist
            }
        };

        dbRequest.onsuccess = () => {
            const db = dbRequest.result;
            const tx = db.transaction(storeName, 'readwrite'); // Must match the storeName
            const store = tx.objectStore(storeName);
            store.put({ key, value });
        };

        dbRequest.onerror = (event) => {
            console.error('Database error:', event.target.error);
        };
    },

    get: async function (storeName, key) {
        console.log('get called:' + storeName); 
        return new Promise((resolve, reject) => {
            const dbRequest = indexedDB.open('MyDatabase', 1);

            dbRequest.onsuccess = () => {
                const db = dbRequest.result;

                // Ensure the object store exists
                if (!db.objectStoreNames.contains(storeName)) {
                    reject(`Object store '${storeName}' not found`);
                    return;
                }

                const tx = db.transaction(storeName, 'readonly');
                const store = tx.objectStore(storeName);
                const request = store.get(key);

                request.onsuccess = () => resolve(request.result ? request.result.value : null);
                request.onerror = () => reject(request.error);
            };

            dbRequest.onerror = (event) => {
                reject(event.target.error);
            };
        });
    },

    getAll: async function (storeName) {
        console.log('getAll called:' + storeName); 
        return new Promise((resolve, reject) => {
            const dbRequest = indexedDB.open('MyDatabase', 1);

            dbRequest.onsuccess = () => {
                const db = dbRequest.result;

                // Ensure the object store exists
                if (!db.objectStoreNames.contains(storeName)) {
                    reject(`Object store '${storeName}' not found`);
                    return;
                }

                const tx = db.transaction(storeName, 'readonly');
                const store = tx.objectStore(storeName);
                const request = store.getAll();

                request.onsuccess = () => resolve(request.result.map(item => item.value));
                request.onerror = () => reject(request.error);
            };

            dbRequest.onerror = (event) => {
                reject(event.target.error);
            };
        });
    },

    delete: async function (storeName, key) {
        const dbRequest = indexedDB.open('MyDatabase', 1);

        dbRequest.onsuccess = () => {
            const db = dbRequest.result;

            // Ensure the object store exists
            if (!db.objectStoreNames.contains(storeName)) {
                console.error(`Object store '${storeName}' not found`);
                return;
            }

            const tx = db.transaction(storeName, 'readwrite');
            const store = tx.objectStore(storeName);
            store.delete(key);
        };

        dbRequest.onerror = (event) => {
            console.error('Database error:', event.target.error);
        };
    }
};
