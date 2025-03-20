using Microsoft.Extensions.Caching.Memory;

namespace CleanArchitecture.Application;

public class ServerCachingTownCards(IMemoryCache memoryCache) : ServerCachingBase(memoryCache) 
    {
    //  private static ConcurrentDictionary<int, TownDto> _townsDictionary = new ConcurrentDictionary<int, TownDto>();
    //static bool _isCacheLoaded = false;

    /// <summary>
    /// for removal card will be null,idCardToRemove some value
    public void AddOrUpdateCardInTown(int townId, CardDto? card = null, CardData? cardData = null, CardDetailDto? cardDetail = null, int idCardToRemove = 0, bool isVerified = true, DateTime townCardsModifiedTime = default)
        {
        var (town, cacheSetTime) = Get<TownCardsDto>(ConstantsCachingServer.CacheCardsOfTownIdKey(townId));

        if (town == null || card == null && cardData == null && cardDetail == null && idCardToRemove <= 0)
            {
            return; // Town not found or nothing to update
            }

        bool isUpdated = false;
        var targetList = isVerified ? town.VerifiedCards : town.DraftCards;

        if (targetList != null)
            {
            if (idCardToRemove > 0 && targetList.Count > 0)
                {
                var indexToRemove = targetList.FindIndex(c => c.Id == idCardToRemove);
                if (indexToRemove >= 0)
                    {
                    targetList.RemoveAt(indexToRemove);
                    isUpdated = true;
                    }
                }

            if (card != null)
                {
                EnsureNoDuplicatesBetweenCards(town, card.Id, isVerified);
                var indexToReplace = targetList.FindIndex(c => c.Id == card.Id);

                if (indexToReplace >= 0)
                    {
                    targetList[indexToReplace] = card;
                    }
                else
                    {
                    targetList.Add(card);
                    }
                isUpdated = true;
                }

            if (cardData != null)
                {
                var indexToReplace = targetList.FindIndex(c => c.Id == cardData.Id);
                if (indexToReplace >= 0)
                    {
                    targetList[indexToReplace].CardData = cardData;
                    isUpdated = true;
                    }
                }

            if (cardDetail != null)
                {
                var indexToReplace = targetList.FindIndex(c => c.Id == cardDetail.Id);
                if (indexToReplace >= 0)
                    {
                    targetList[indexToReplace].CardDetail = cardDetail;
                    isUpdated = true;
                    }
                }
            }
        else if (card != null)
            {
            EnsureNoDuplicatesBetweenCards(town, card.Id, isVerified);
            targetList = new List<CardDto> { card };
            isUpdated = true;
            }

        if (isUpdated)
            {
            town.LastCardUpdateTime = townCardsModifiedTime == default ? DateTimeExtension.CurrentTime : townCardsModifiedTime;
            Set<TownCardsDto>(ConstantsCachingServer.CacheCardsOfTownIdKey(townId), town);
            //UpdateTownsListTownCardUpdatedTimeStamp(townId, town.LastCardUpdateTime );
            }
        }

    public void EnsureNoDuplicatesBetweenCards(TownCardsDto town, int cardId, bool isVerified)
        {
        if (isVerified && town != null && town.DraftCards?.Count > 0)
            {
            town.DraftCards.RemoveAll(c => c.Id == cardId);
            }
        }
    }
