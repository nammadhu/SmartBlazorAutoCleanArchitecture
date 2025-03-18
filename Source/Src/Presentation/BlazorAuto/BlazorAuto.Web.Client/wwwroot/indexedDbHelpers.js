const version = 2; // Define the database version
let dbConnection = null; // Shared reference to the opened database

window.indexedDbHelpers = {
    ensureStoreExists: function (storeName) {
        return new Promise((resolve, reject) => {
            if (dbConnection) {
                // If the database connection is already open, resolve immediately
                resolve();
                return;
            }

            const dbRequest = indexedDB.open('MyDatabase', version); // Use the version constant

            dbRequest.onupgradeneeded = (event) => {
                const db = event.target.result;

                // Create the object store if it doesn't exist
                if (!db.objectStoreNames.contains(storeName)) {
                    db.createObjectStore(storeName, { keyPath: 'key' });
                    console.log(`Object store '${storeName}' created.`);
                }
            };

            dbRequest.onsuccess = (event) => {
                dbConnection = event.target.result;
                console.log(`Database opened successfully.`);
                resolve();
            };

            dbRequest.onerror = (event) => {
                console.error(`Database error during ensureStoreExists: ${event.target.error}`);
                reject(event.target.error);
            };
        });
    },

    addOrUpdate: async function (storeName, key, value) {
        console.log('addOrUpdate called: ' + storeName);

        return new Promise((resolve, reject) => {
            const dbRequest = indexedDB.open('MyDatabase', version); // Use the version constant

            dbRequest.onupgradeneeded = (event) => {
                const db = event.target.result;

                // Ensure the store exists
                if (!db.objectStoreNames.contains(storeName)) {
                    db.createObjectStore(storeName, { keyPath: 'key' }); // Create store if it doesn’t exist
                }
            };

            dbRequest.onsuccess = () => {
                const db = dbRequest.result;
                try {
                    const tx = db.transaction(storeName, 'readwrite'); // Must match the storeName
                    const store = tx.objectStore(storeName);
                    store.put({ key, value });

                    tx.oncomplete = () => {
                        console.log(`Data added/updated in store: '${storeName}'`);
                        resolve();
                    };
                    tx.onerror = (event) => reject(event.target.error);
                } catch (err) {
                    console.error(`Transaction error: ${err.message}`);
                    reject(err);
                }
            };

            dbRequest.onerror = (event) => {
                console.error('Database error:', event.target.error);
                reject(event.target.error);
            };
        });
    },

    get: async function (storeName, key) {
        console.log('get called: ' + storeName);

        return new Promise((resolve, reject) => {
            const dbRequest = indexedDB.open('MyDatabase', version); // Use the version constant

            dbRequest.onsuccess = () => {
                const db = dbRequest.result;

                // Ensure the object store exists
                if (!db.objectStoreNames.contains(storeName)) {
                    reject(`Object store '${storeName}' not found`);
                    return;
                }

                try {
                    const tx = db.transaction(storeName, 'readonly');
                    const store = tx.objectStore(storeName);
                    const request = store.get(key);

                    request.onsuccess = () =>
                        resolve(request.result ? request.result.value : null);
                    request.onerror = () => reject(request.error);
                } catch (err) {
                    console.error(`Transaction error: ${err.message}`);
                    reject(err);
                }
            };

            dbRequest.onerror = (event) => {
                console.error(`Database error: ${event.target.error}`);
                reject(event.target.error);
            };
        });
    },

    getAll: async function (storeName) {
        console.log('getAll called: ' + storeName);

        return new Promise((resolve, reject) => {
            if (!dbConnection) {
                reject('Database is not initialized. Call ensureStoreExists first.');
                return;
            }

            try {
                const tx = dbConnection.transaction(storeName, 'readonly');
                const store = tx.objectStore(storeName);
                const request = store.getAll();

                request.onsuccess = () =>
                    resolve(request.result.map((item) => item.value));
                request.onerror = () => reject(request.error);
            } catch (err) {
                console.error(`Transaction error: ${err.message}`);
                reject(err);
            }
        });
    },

    delete: async function (storeName, key) {
        console.log('delete called: ' + storeName);

        return new Promise((resolve, reject) => {
            const dbRequest = indexedDB.open('MyDatabase', version); // Use the version constant

            dbRequest.onsuccess = () => {
                const db = dbRequest.result;

                // Ensure the object store exists
                if (!db.objectStoreNames.contains(storeName)) {
                    reject(`Object store '${storeName}' not found`);
                    return;
                }

                try {
                    const tx = db.transaction(storeName, 'readwrite');
                    const store = tx.objectStore(storeName);
                    store.delete(key);

                    tx.oncomplete = () => {
                        console.log(`Data deleted from store: '${storeName}'`);
                        resolve();
                    };
                    tx.onerror = (event) => reject(event.target.error);
                } catch (err) {
                    console.error(`Transaction error: ${err.message}`);
                    reject(err);
                }
            };

            dbRequest.onerror = (event) => {
                console.error('Database error:', event.target.error);
                reject(event.target.error);
            };
        });
    }
};
