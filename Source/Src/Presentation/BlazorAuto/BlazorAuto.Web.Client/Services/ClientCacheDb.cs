using CleanArchitecture.Domain.Products.DTOs;
using Blazor.IndexedDB;
using Microsoft.JSInterop;

namespace BlazorAuto.Web.Client.Services;

//todo change this to indexedb
public class ClientCacheDb : IndexedDb
{
    public ClientCacheDb(IJSRuntime jSRuntime, string name, int version) : base(jSRuntime, name, version)
    { }
    public IndexedSet<ProductDto> Products { get; set; }

}


