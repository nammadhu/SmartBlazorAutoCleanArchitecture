using Blazor.IndexedDB;
using Microsoft.JSInterop;
using SHARED.DTOs;
using System.Text.Json;

namespace BlazorAuto.Web.Client.Services;
//https://github.com/brianly1003/Blazor.IndexedDB
//https://www.syncfusion.com/faq/blazor/general/how-do-i-use-indexeddb-in-blazor-webassembly
public class ClientCacheIndexedDb(IJSRuntime jSRuntime, string name, int version) : IndexedDb(jSRuntime, name, version)
    {
    public IndexedSet<ProductDto> Products { get; set; }
    //these are just like tables, add whatever required for clientside and use it.
    }

public class IndexedDbService<T> where T : class
    {
    private readonly IJSRuntime _jsRuntime;
    private readonly string _storeName;
    private bool _isInitialized = false; // Track if the store is initialized

    public IndexedDbService(IJSRuntime jsRuntime, string storeName)
        {
        _jsRuntime = jsRuntime;
        _storeName = storeName;
        }

    /// <summary>
    /// Ensures the object store is initialized by invoking the JavaScript function `ensureStoreExists`.
    /// </summary>
    public async Task InitializeStoreAsync()
        {
        if (_isInitialized)
            {
            Console.WriteLine($"Object store '{_storeName}' is already initialized.");
            return;
            }

        try
            {
            // Call the JavaScript function to ensure the store exists
            await _jsRuntime.InvokeVoidAsync("indexedDbHelpers.ensureStoreExists", _storeName);
            Console.WriteLine($"Object store '{_storeName}' initialized.");
            _isInitialized = true; // Mark as initialized
            }
        catch (Exception ex)
            {
            Console.WriteLine($"Failed to initialize object store '{_storeName}': {ex.Message}");
            throw;
            }
        }

    /// <summary>
    /// Adds or updates an entity in the IndexedDB store.
    /// </summary>
    public async Task AddOrUpdateAsync(string key, T entity)
        {
        try
            {
            // Ensure the store is initialized before performing the operation
            await InitializeStoreAsync();

            var json = JsonSerializer.Serialize(entity);
            await _jsRuntime.InvokeVoidAsync("indexedDbHelpers.addOrUpdate", _storeName, key, json);
            Console.WriteLine($"Data with key '{key}' added/updated in store '{_storeName}'.");
            }
        catch (Exception ex)
            {
            Console.WriteLine($"Failed to add/update key '{key}' in store '{_storeName}': {ex.Message}");
            throw;
            }
        }

    /// <summary>
    /// Retrieves an entity by its key from the IndexedDB store.
    /// </summary>
    public async Task<T?> GetAsync(string key)
        {
        try
            {
            // Ensure the store is initialized before performing the operation
            await InitializeStoreAsync();

            var json = await _jsRuntime.InvokeAsync<string?>("indexedDbHelpers.get", _storeName, key);
            return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<T>(json);
            }
        catch (Exception ex)
            {
            Console.WriteLine($"Failed to retrieve key '{key}' from store '{_storeName}': {ex.Message}");
            //throw;
            return null;
            }
        }

    /// <summary>
    /// Retrieves all entities from the IndexedDB store.
    /// </summary>
    public async Task<List<T>> GetAllAsync()
        {
        try
            {
            // Ensure the store is initialized before performing the operation
            await InitializeStoreAsync();

            var json = await _jsRuntime.InvokeAsync<string?>("indexedDbHelpers.getAll", _storeName);
            return string.IsNullOrEmpty(json) ? new List<T>() : JsonSerializer.Deserialize<List<T>>(json)!;
            }
        catch (Exception ex)
            {
            Console.WriteLine($"Failed to retrieve all data from store '{_storeName}': {ex.Message}");
            //throw;
            return null;
            }
        }

    /// <summary>
    /// Deletes an entity by its key from the IndexedDB store.
    /// </summary>
    public async Task DeleteAsync(string key)
        {
        try
            {
            // Ensure the store is initialized before performing the operation
            await InitializeStoreAsync();

            await _jsRuntime.InvokeVoidAsync("indexedDbHelpers.delete", _storeName, key);
            Console.WriteLine($"Data with key '{key}' deleted from store '{_storeName}'.");
            }
        catch (Exception ex)
            {
            Console.WriteLine($"Failed to delete key '{key}' from store '{_storeName}': {ex.Message}");
            throw;
            }
        }
    }



