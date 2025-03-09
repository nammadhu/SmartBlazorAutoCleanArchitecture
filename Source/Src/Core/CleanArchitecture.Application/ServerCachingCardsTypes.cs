using Microsoft.Extensions.Caching.Memory;

namespace CleanArchitecture.Application;

public class ServerCachingCardTypes : ServerCachingBase
    {
    //  private static ConcurrentDictionary<int, TownDto> _townsDictionary = new ConcurrentDictionary<int, TownDto>();
    public static bool _isCacheLoaded = false;

    private IMemoryCache memoryCache;

    public ServerCachingCardTypes(IMemoryCache memoryCache) : base(memoryCache)
        {
        this.memoryCache = memoryCache;
        }

    //usually below wont be called,so no concurrent dictionary nothing,instead simplified direct handling
    public void AddOrReplaceCardTypeInCardTypes(CardTypeDto cardTypeDto)
        {
        if (cardTypeDto != null && cardTypeDto.Id > 0)
            {
            (List<CardTypeDto>? cardTypes, DateTime? cacheSetTime) = Get<List<CardTypeDto>>(ConstantsCachingServer.CacheCardTypesListKey);
            if (cardTypes != null)
                {
                var indexToReplace = cardTypes.FindIndex(c => c.Id == cardTypeDto.Id);
                if (indexToReplace > 0)
                    {
                    cardTypes[indexToReplace] = cardTypeDto;
                    }
                else cardTypes.Add(cardTypeDto);
                Set<List<CardTypeDto>>(ConstantsCachingServer.CacheCardTypesListKey, cardTypes);
                }
            }//cardTypes itself not exists in cache,so no need to replace... let it load on next fetch
        }

    public void RemoveCardTypeInCardTypes(int cardTypeId)
        {
        (List<CardTypeDto>? cardTypes, DateTime? cacheSetTime) = Get<List<CardTypeDto>>(ConstantsCachingServer.CacheCardTypesListKey);
        if (cardTypes != null)
            {
            var indexToReplace = cardTypes.FindIndex(c => c.Id == cardTypeId);
            if (indexToReplace > 0)
                {
                cardTypes.RemoveAt(indexToReplace);
                Set<List<CardTypeDto>>(ConstantsCachingServer.CacheCardTypesListKey, cardTypes);
                }
            }
        }
    }
