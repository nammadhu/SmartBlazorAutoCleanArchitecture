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


public class IndexedDbService<T>(IJSRuntime jsRuntime, string storeName) where T : class
    {
    private readonly IJSRuntime _jsRuntime = jsRuntime;
    private readonly string _storeName = storeName;

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
        var json = await _jsRuntime.InvokeAsync<string>("indexedDbHelpers.getAll", _storeName);
        return string.IsNullOrEmpty(json) ? new List<T>() : JsonSerializer.Deserialize<List<T>>(json)!;
        }

    public async Task DeleteAsync(string key)
        {
        await _jsRuntime.InvokeVoidAsync("indexedDbHelpers.delete", _storeName, key);
        }
    }



