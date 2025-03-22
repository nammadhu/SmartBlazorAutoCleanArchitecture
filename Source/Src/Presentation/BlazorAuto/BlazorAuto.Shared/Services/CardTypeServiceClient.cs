using BASE;
using SHARED.DTOs;
using SHARED.Features.CardTypes.Commands;
using SHARED.Features.CardTypes.Queries;
using SHARED.Interfaces;
using SHARED.Wrappers;
using System.Net.Http.Json;

namespace BlazorAuto.Shared.Services;
public class CardTypeServiceClient(IHttpClientFactory httpClientFactory, ICachingServiceClient<CardTypeDto, int> clientCachingService) : ICardTypeController
    {
    const string endPoint = "api/v1/CardType/";
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(CONSTANTS.ClientAnonymous);

    public async Task<BaseResult<CardTypeDto>> Create(CU_CardTypeCommand model, CancellationToken cancellationToken = default)
        {
        var response = await _httpClient.PostAsJsonAsync($"{endPoint}{nameof(ICardTypeController.Create)}", model, cancellationToken);
        return await response.Content.ReadFromJsonAsync<BaseResult<CardTypeDto>>();
        }

    public async Task<BaseResult<CardTypeDto>> CreateUpdate(CU_CardTypeCommand model, CancellationToken cancellationToken = default)
        {
        var response = await _httpClient.PutAsJsonAsync($"{endPoint}{nameof(ICardTypeController.CreateUpdate)}", model, cancellationToken);
        return await response.Content.ReadFromJsonAsync<BaseResult<CardTypeDto>>();
        }

    public async Task<BaseResult> Delete(int id, CancellationToken cancellationToken = default)
        {
        var response = await _httpClient.DeleteAsync($"{endPoint}{nameof(ICardTypeController.Delete)}", cancellationToken);
        return await response.Content.ReadFromJsonAsync<BaseResult>();
        }

    public async Task<BaseResult<List<CardTypeDto>>> GetAll(CancellationToken cancellationToken = default)
        {
        await clientCachingService.InitializeStoreAsync();
        // First, try to retrieve data from IndexedDB
        var cachedData = await clientCachingService.GetAllAsync();
        if (cachedData?.Count > 0)
            {
            return new BaseResult<List<CardTypeDto>> { Data = cachedData, Success = true };
            }

        // Fallback to API if IndexedDB is empty
        var response = await _httpClient.GetFromJsonAsync<BaseResult<List<CardTypeDto>>>($"{endPoint}{nameof(ICardTypeController.GetAll)}", cancellationToken);
        if (response?.Success == true && response.Data != null)
            {
            //await clientCachingService.AddOrUpdateBulkAsync(response.Data);
            //addOrUpdateBulk works but key should be proper. currently its giving undefined,so will be using addOrUpdate
            foreach (var item in response.Data)
                {
                await clientCachingService.AddOrUpdateAsync(item.Id.ToString(), item);
                }
            }
        return response!;
        }

    public async Task<BaseResult<CardTypeDto>> GetById(int id, CancellationToken cancellationToken = default)
        {
        // Attempt to fetch from IndexedDB first
        var cachedItem = await clientCachingService.GetAsync(id.ToString());
        if (cachedItem != null)
            {
            return new BaseResult<CardTypeDto> { Data = cachedItem, Success = true };
            }

        // Fallback to API
        return await _httpClient.GetFromJsonAsync<BaseResult<CardTypeDto>>($"{endPoint}{nameof(ICardTypeController.GetById)}/{id}", cancellationToken);
        }

    public async Task<PagedResponse<CardTypeDto>> GetPagedList(GetCardTypesPagedListQuery model, CancellationToken cancellationToken = default)
        {
        //todo add name,all 
        var response = await _httpClient.PostAsJsonAsync($"{endPoint}{nameof(ICardTypeController.GetPagedList)}" +
            $"/{(model.All ? nameof(GetCardTypesPagedListQuery.All) == "true" : string.Empty)}" +
            $"/{(!string.IsNullOrEmpty(model.Name) ? nameof(GetCardTypesPagedListQuery.Name) == model.Name : string.Empty)}", model, cancellationToken);
        return await response.Content.ReadFromJsonAsync<PagedResponse<CardTypeDto>>();
        }

    public async Task<BaseResult<CardTypeDto>> Update(CU_CardTypeCommand model, CancellationToken cancellationToken = default)
        {
        var response = await _httpClient.PutAsJsonAsync($"{endPoint}{nameof(ICardTypeController.Update)}", model, cancellationToken);

        if (response != null && response.IsSuccessStatusCode)
            {
            var result = await response.Content.ReadFromJsonAsync<BaseResult<CardTypeDto>>(cancellationToken: cancellationToken);
            if (result?.Data != null)
                await clientCachingService.AddOrUpdateAsync(result.Data.Id.ToString(), result.Data);
            return result;
            }

        return null;
        //on success should update client cache as well
        }
    }
