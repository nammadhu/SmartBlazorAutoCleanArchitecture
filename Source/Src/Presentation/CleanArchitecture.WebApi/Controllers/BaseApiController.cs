using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CleanArchitecture.Application.Wrappers;
using CleanArchitecture.WebApi.Infrastructure.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.WebApi.Controllers;

[ApiController]
[ApiResultFilter]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public abstract class BaseApiController : ControllerBase
{
    private IMediator _mediator;
    private readonly IMemoryCache _cache;
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

    protected BaseApiController()
    {
        _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
    }

    protected BaseApiController(IMediator mediator)
    {
        _mediator ??= mediator;
    }

    protected BaseApiController(IMediator mediator, IMemoryCache cache)
    {
        _mediator ??= mediator;
        _cache = cache;
    }

    protected BaseApiController(IMemoryCache cache)
    {
        _cache = cache;
    }

    // Get data from cache
    protected T GetCachedItem<T>(string cacheKey)
    {
        _cache.TryGetValue(cacheKey, out T cachedItem);
        return cachedItem;
    }
    // Get data from cache
    protected List<T> GetCachedList<T>(string cacheKey)
    {
        _cache.TryGetValue(cacheKey, out List<T> cachedList);
        return cachedList ?? new List<T>();
    }

    // Cache data retrieval and setting with optimization
    protected async Task<T> GetOrSetCachedResponse<T>(string cacheKey, Func<Task<T>> getDataFunc, TimeSpan? expiration = null)
    {
        var cachedItem = GetCachedItem<T>(cacheKey);
        if (cachedItem != null)
        {
            return cachedItem;
        }

        var data = await getDataFunc();
        SetOptimizedCachedItem(cacheKey, data, expiration);
        return data;
    }

    protected async Task<PagedResponse<T>> GetOrSetCachedPagedResponseAsync<T>(string cacheKey, Func<Task<PagedResponse<T>>> getDataFunc, TimeSpan? expiration = null)
    {
        var cachedItem = GetCachedItem<PagedResponse<T>>(cacheKey);
        if (cachedItem != null)
        {
            return cachedItem;
        }

        var data = await getDataFunc();
        SetOptimizedCachedItem(cacheKey, data, expiration);
        return data;
    }
   
    // Cache list retrieval and setting with optimization (async)
    protected async Task<List<T>> GetOrSetCachedListAsync<T>(string cacheKey, Func<Task<List<T>>> getDataFunc, TimeSpan? expiration = null)
    {
        var cachedList = GetCachedList<T>(cacheKey);
        if (cachedList.Any())
        {
            return cachedList;
        }

        var data = await getDataFunc();
        SetOptimizedCachedItem(cacheKey, data, expiration);
        return data;
    }
    // Set data to cache with optimization
    protected void SetOptimizedCachedItem<T>(string cacheKey, T item, TimeSpan? expiration = null)
    {
        var optimizedItem = CacheOptimizationHelper.OptimizeForCache(item);
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
        };
        _cache.Set(cacheKey, optimizedItem, cacheEntryOptions);
    }

    // Invalidate a single item in a cached list
    protected void InvalidateCachedItem<T>(string cacheKey, Func<T, bool> predicate)
    {
        var cachedList = GetCachedItem<List<T>>(cacheKey) ?? new List<T>();
        var existingItem = cachedList.FirstOrDefault(predicate);
        if (existingItem != null)
        {
            cachedList.Remove(existingItem);
            SetOptimizedCachedItem(cacheKey, cachedList);
        }
    }
     

    // Update a single item in a cached list with optimization
    protected void UpdateCachedList<T>(string cacheKey, Func<T, bool> predicate, T updatedItem)
    {
        var cachedList = GetCachedItem<List<T>>(cacheKey) ?? new List<T>();
        var existingItem = cachedList.FirstOrDefault(predicate);
        if (existingItem != null)
        {
            cachedList.Remove(existingItem);
        }

        cachedList.Add(CacheOptimizationHelper.OptimizeForCache(updatedItem));
        SetOptimizedCachedItem(cacheKey, cachedList);
    }
    
    // Remove a single item from a cached list
    protected void RemoveFromCachedList<T>(string cacheKey, Func<T, bool> predicate)
    {
        var cachedList = GetCachedList<T>(cacheKey);
        var existingItem = cachedList.FirstOrDefault(predicate);
        if (existingItem != null)
        {
            cachedList.Remove(existingItem);
            SetOptimizedCachedItem(cacheKey, cachedList);
        }

    }
}

public static class CacheOptimizationHelper
{
    public static T OptimizeForCache<T>(T item)
    {
        var json = JsonSerializer.Serialize(item, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        });
        return JsonSerializer.Deserialize<T>(json);
    }
}
