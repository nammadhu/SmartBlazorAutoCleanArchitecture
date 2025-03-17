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

    public IndexedDbService(IJSRuntime jsRuntime, string storeName)
        {
        _jsRuntime = jsRuntime;
        _storeName = storeName;

        }

    public async Task InitializeStoreAsync()
        {
        try
            {
            await _jsRuntime.InvokeVoidAsync("indexedDbHelpers.ensureStoreExists", _storeName);
            Console.WriteLine($"Object store '{_storeName}' initialized.");
            }
        catch (Exception ex)
            {
            Console.WriteLine($"Failed to initialize object store '{_storeName}': {ex.Message}");
            }
        }

    public async Task AddOrUpdateAsync(string key, T entity)
        {
        var json = JsonSerializer.Serialize(entity);
        await _jsRuntime.InvokeVoidAsync("indexedDbHelpers.addOrUpdate", _storeName, key, json);
        }

    public async Task<T?> GetAsync(string key)
        {
        var json = await _jsRuntime.InvokeAsync<string>("indexedDbHelpers.get", _storeName, key);
        return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<T>(json);
        }

    public async Task<List<T>> GetAllAsync()
        {
        try
            {
            var json = await _jsRuntime.InvokeAsync<string>("indexedDbHelpers.getAll", _storeName);
            return string.IsNullOrEmpty(json) ? new List<T>() : JsonSerializer.Deserialize<List<T>>(json)!;
            }
        catch (JSException ex)
            {
            Console.WriteLine($"GetAllAsync failed for store '{_storeName}': {ex.Message}");
            return new List<T>();
            }
        }

    public async Task DeleteAsync(string key)
        {
        await _jsRuntime.InvokeVoidAsync("indexedDbHelpers.delete", _storeName, key);
        }
    }



