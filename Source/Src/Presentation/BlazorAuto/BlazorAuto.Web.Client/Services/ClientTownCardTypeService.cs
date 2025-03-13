using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SHARED.DTOs;
using SHARED.Features.CardTypes.Commands;
using SHARED.Features.CardTypes.Queries;
using SHARED.Interfaces;
using SHARED.Wrappers;
using System.Net.Http.Json;

namespace BlazorAuto.Web.Client.Services;
public class ClientTownCardTypeService(HttpClient httpClient, IndexedDbService<CardTypeDto> indexedDbService) : ITownCardTypeController
    {
    private readonly HttpClient _httpClient = httpClient;
    private readonly IndexedDbService<CardTypeDto> _indexedDbService = indexedDbService;

    public async Task<BaseResult<CardTypeDto>> Create(CU_CardTypeCommand model, CancellationToken cancellationToken = default)
        {
        var response = await _httpClient.PostAsJsonAsync("api/cardtypes", model, cancellationToken);
        return await response.Content.ReadFromJsonAsync<BaseResult<CardTypeDto>>();
        }

    public async Task<BaseResult<CardTypeDto>> CreateUpdate(CU_CardTypeCommand model, CancellationToken cancellationToken = default)
        {
        var response = await _httpClient.PutAsJsonAsync($"api/cardtypes/{model.Id}", model, cancellationToken);
        return await response.Content.ReadFromJsonAsync<BaseResult<CardTypeDto>>();
        }

    public async Task<BaseResult> Delete(int id, CancellationToken cancellationToken = default)
        {
        var response = await _httpClient.DeleteAsync($"api/cardtypes/{id}", cancellationToken);
        return await response.Content.ReadFromJsonAsync<BaseResult>();
        }

    public async Task<BaseResult<List<CardTypeDto>>> GetAll(CancellationToken cancellationToken = default)
        {
        // First, try to retrieve data from IndexedDB
        var cachedData = await _indexedDbService.GetAllAsync();
        if (cachedData.Any())
            {
            return new BaseResult<List<CardTypeDto>> { Data = cachedData, Success = true };
            }

        // Fallback to API if IndexedDB is empty
        var response = await _httpClient.GetFromJsonAsync<BaseResult<List<CardTypeDto>>>("api/cardtypes", cancellationToken);
        if (response?.Success == true)
            {
            foreach (var item in response.Data)
                {
                await _indexedDbService.AddOrUpdateAsync(item.Id.ToString(), item);
                }
            }
        return response!;
        }

    public async Task<BaseResult<CardTypeDto>> GetById(int id, CancellationToken cancellationToken = default)
        {
        // Attempt to fetch from IndexedDB first
        var cachedItem = await _indexedDbService.GetAsync(id.ToString());
        if (cachedItem != null)
            {
            return new BaseResult<CardTypeDto> { Data = cachedItem, Success = true };
            }

        // Fallback to API
        return await _httpClient.GetFromJsonAsync<BaseResult<CardTypeDto>>($"api/cardtypes/{id}", cancellationToken);
        }

    public async Task<PagedResponse<CardTypeDto>> GetPagedList(GetCardTypesPagedListQuery model, CancellationToken cancellationToken = default)
        {
        var response = await _httpClient.PostAsJsonAsync("api/cardtypes/paged", model, cancellationToken);
        return await response.Content.ReadFromJsonAsync<PagedResponse<CardTypeDto>>();
        }

    public async Task<BaseResult<CardTypeDto>> Update(CU_CardTypeCommand model, CancellationToken cancellationToken = default)
        {
        var response = await _httpClient.PutAsJsonAsync($"api/cardtypes/{model.Id}", model, cancellationToken);
        return await response.Content.ReadFromJsonAsync<BaseResult<CardTypeDto>>();
        }
    }
