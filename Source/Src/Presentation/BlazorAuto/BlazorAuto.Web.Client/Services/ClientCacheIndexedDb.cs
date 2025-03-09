using Blazor.IndexedDB;
using Microsoft.JSInterop;
using Shared.DTOs;

namespace BlazorAuto.Web.Client.Services;
//https://github.com/brianly1003/Blazor.IndexedDB
//https://www.syncfusion.com/faq/blazor/general/how-do-i-use-indexeddb-in-blazor-webassembly
public class ClientCacheIndexedDb(IJSRuntime jSRuntime, string name, int version) : IndexedDb(jSRuntime, name, version)
    {
    public IndexedSet<ProductDto> Products { get; set; }
    //these are just like tables, add whatever required for clientside and use it.
    }


