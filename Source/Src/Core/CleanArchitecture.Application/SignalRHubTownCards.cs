using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Timers;

namespace MyTown.Application;

//https://github.com/nammadhu/BlazorWebAppSignalRLiveUpdates
public partial class SignalRHubTownCards : Hub, ISignalRTownCards
    {
    //internal static ConcurrentDictionary<string, List<BusinessCardDto>> _TownCardsDictionary = new ConcurrentDictionary<string, List<BusinessCardDto>>();
    //internal static ConcurrentDictionary<int, (List<iCardDto> VerifiedCardList, List<iCardDto> DraftCardList)> _TownCardsDictionary = new ConcurrentDictionary<int, (List<iCardDto>, List<iCardDto>)>();

    internal static ConcurrentDictionary<int, TownCardsDto> _TownCardsDictionary = new ConcurrentDictionary<int, TownCardsDto>();

    private static readonly System.Timers.Timer cleanupTimer;

    public event Action<CardDto>? OnCardReceived;//no use here ,just for interface purpose //todo need to modify

    static SignalRHubTownCards()//static ctor for default initiation
        {
        cleanupTimer = new System.Timers.Timer(60000); // Check every minute
        cleanupTimer.Elapsed += CleanupExpiredEntries;
        cleanupTimer.Start();
        }

    public async Task JoinGroup(GetCardsOfTownQuery model)//, CancellationToken cancellationToken = default)
        {
        //_logger.LogError($"JoinGroup called for TownId: {model.IdTown}");
        try
            {
            if (model.IdTown > 0)
                {
                var townResult = await _cardController.GetCardsOfTown(model);//, cancellationToken);
                if (townResult.Success && townResult.Data != null)
                    {
                    var item = new TownCardsDto() { Id = townResult.Data.Id, VerifiedCards = townResult.Data.VerifiedCards, DraftCards = townResult.Data.DraftCards, UserCount = 0, LastAccessedTime = DateTimeExtension.CurrentTime };
                    await Groups.AddToGroupAsync(Context.ConnectionId, model.IdTown.ToString());//, cancellationToken);
                    _TownCardsDictionary.AddOrUpdate(model.IdTown,
                        new TownCardsDto() { Id = townResult.Data.Id, VerifiedCards = townResult.Data.VerifiedCards, DraftCards = townResult.Data.DraftCards, UserCount = 0, LastAccessedTime = DateTimeExtension.CurrentTime },
                        (key, oldValue) => new TownCardsDto() { Id = townResult.Data.Id, VerifiedCards = townResult.Data.VerifiedCards, DraftCards = townResult.Data.DraftCards, UserCount = oldValue.UserCount + 1, LastAccessedTime = DateTimeExtension.CurrentTime });

                    //_TownCardsDictionary.AddOrUpdate(model.IdTown,
                    //new TownCardsDto(),
                    //(key, value) => new TownCardsDto() { Id = townResult.Data.Id, VerifiedCards = townResult.Data.VerifiedCards, DraftCards = townResult.Data.DraftCards, UserCount = value.UserCount + 1, LastAccessedTime = DateTimeExtension.CurrentTime });
                    }
                }
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "JoinGroup issue:");
            }
        }

    public async Task LeaveGroup(int townId)//, CancellationToken cancellationToken = default)
        {
        if (townId == 0) return;
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, townId.ToString());//, cancellationToken);

        if (_TownCardsDictionary.TryGetValue(townId, out var entry))
            {
            entry.UserCount -= 1;
            if (entry.UserCount <= 0)//need to more testing
                _TownCardsDictionary.TryRemove(townId, out _);
            else
                _TownCardsDictionary[townId] = entry;
            }
        }

    [Authorize]
    public async Task<BaseResult<CardDto>?> Create(CU_CardCommand model)//, CancellationToken cancellationToken = default)
        {
        UserValidation(model.Operator, model.IsForVerifiedCard);
        var resultCreated = await _cardController.Create(model);//, cancellationToken);

        if (resultCreated?.Success == true && resultCreated.Data != null && resultCreated.Data.IdTown > 0)
            {
            var businessCards = _TownCardsDictionary.GetOrAdd(resultCreated.Data.IdTown,
                new TownCardsDto() { Id = resultCreated.Data.IdTown });
            resultCreated.Data.LastModified ??= DateTimeExtension.CurrentTime;

            var cardList = (resultCreated.Data.IsVerified == true ? businessCards.VerifiedCards : businessCards.DraftCards) ?? new();

            var index = cardList.FindIndex(bc => bc.Id == resultCreated.Data.Id);

            if (index >= 0)
                { //already in list so no more modification required.
                  //Really dont know how new item adding to _TownCardsDictionary
                  //throw new Exception("Repetition in adding created card");
                  //return resultCreated;
                }
            else
                {
                cardList.Add(resultCreated.Data);

                //below is just to ensure ideally it wont happen
                if (resultCreated.Data.IsVerified == true &&
                    businessCards.VerifiedCards?.Exists(c => c.Id == resultCreated.Data.Id) != true)
                    businessCards.VerifiedCards = cardList;
                else if (businessCards.DraftCards?.Exists(c => c.Id == resultCreated.Data.Id) != true)
                    businessCards.DraftCards = cardList;

                businessCards.LastAccessedTime = DateTimeExtension.CurrentTime;
                _TownCardsDictionary[resultCreated.Data.IdTown] = businessCards;
                }
            await BroadcastUpdatedCardToGroup(resultCreated.Data);
            }
        return resultCreated;
        }

    private void UserValidation(Guid operatorId, bool isForVerifiedCard)
        {
        var userId = Context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roles = Context?.User?.FindFirst(ClaimTypes.Role)?.Value;

        if (!Guid.TryParse(userId, out Guid userGuid))
            throw new HubException("userGuId Failed");

        if (operatorId == Guid.Empty)
            throw new HubException($"userGuId Empty");

        if (operatorId != Guid.Empty && operatorId != userGuid)
            throw new HubException($"userGuId Forgery as {operatorId} but actual is {userGuid}");

        if (isForVerifiedCard == true && (roles == null || !roles.Contains("Admin")))
            {
            throw new HubException("Only admins can add verified cards.");
            }
        }

    private async Task BroadcastUpdatedCardToGroup(CardDto iCardDto)
        {
        try
            {
            // Broadcast the new business card to all clients in the group
            if (iCardDto != null)
                // Broadcast the new business card to all clients in the group
                await Clients.Group(iCardDto.IdTown.ToString())
                    .SendAsync(ISignalRTownCards.ReceiveBusinessCard, iCardDto);//, resultCreated.Data.IsVerifiedEntryExists == true);
            }
        catch (Exception ex)
            {
            // Log the exception
            _logger.LogError($"Exception in {nameof(BroadcastUpdatedCardToGroup)}  method: {ex.Message}");
            // Handle the exception as needed
            }
        }

    /*
     Updates are allowed as, For verified any number of times
    SignalR list update,had to do one more check as is selected for today date then only update list otherwise no.
    Everyday list will be refreshed based on daily date selection,so 24hr once refresh of all.
    For draft,allow only as, Data/detail if lastmodified is earlier than 24hrs of now
    For main data allow only if earlier than 48 hrs
    Excluding first 3 days period
     */

    [Authorize]
    public async Task<BaseResult<CardDto>?> UpdateCard(CU_CardCommand model)//, CancellationToken cancellationToken = default)
        {//here only main key info update,no data or detail
        UserValidation(model.Operator, model.IsForVerifiedCard);

        var resultUpdates = await _cardController.UpdateCard(model);//, cancellationToken);

        if (resultUpdates?.Success == true && resultUpdates.Data != null)
            {
            if (_TownCardsDictionary.TryGetValue(model.IdTown, out var businessCards))
                {
                //if isforVerifiedCards then either exists or add now
                //else it can be on verified also draft
                //as previously may be draft now moving to verified

                if (model.IsForVerifiedCard == false && resultUpdates.Data.IsVerified == true &&
                    businessCards.VerifiedCards?.Exists(bc => bc.Id == resultUpdates.Data.Id) == true)
                    {//now its draft, it also  has verified ,so donot go for update of drafts
                    return resultUpdates;
                    }

                var cardList = ((model.IsForVerifiedCard == true || resultUpdates.Data.IsVerified == true) ? businessCards.VerifiedCards : businessCards.DraftCards) ?? new();

                var index = cardList.FindIndex(bc => bc.Id == resultUpdates.Data.Id);

                if (resultUpdates.Data.IsVerified == true
                    && businessCards.DraftCards?.Exists(bc => bc.Id == resultUpdates.Data.Id) == true) //index < 0)
                    {//now its verified,so remove cache of drafts. as this might be previous Draft
                    businessCards.DraftCards.RemoveAll(bc => bc.Id == resultUpdates.Data.Id);
                    }

                if (index >= 0)
                    {
                    resultUpdates.Data.LastModified ??= DateTimeExtension.CurrentTime;
                    cardList[index] = resultUpdates.Data;

                    businessCards.LastAccessedTime = DateTimeExtension.CurrentTime;
                    _TownCardsDictionary[model.IdTown] = businessCards;

                    if (model.IsVerified == true)
                        businessCards.DraftCards?.RemoveAll(d => d.Id == resultUpdates.Data.Id);
                    //above will not be updated for existing users,instead for new Users only
                    await BroadcastUpdatedCardToGroup(resultUpdates.Data);
                    }
                else
                    {
                    //update is wrong,so ignores
                    }
                }
            }
        return resultUpdates;
        }

    [Authorize]
    public async Task<BaseResult<CardData>?> UpdateCardData(CU_CardDataCommand model)//, CancellationToken cancellationToken = default)
        {
        var userId = Context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roles = Context?.User?.FindFirst(ClaimTypes.Role)?.Value;

        var resultCardDataUpdated = await _cardController.UpdateCardData(model);//, cancellationToken);

        if (resultCardDataUpdated?.Success == true && resultCardDataUpdated.Data != null)
            {
            var resultCard = resultCardDataUpdated.Data;
            if (model.IdTown > 0 && _TownCardsDictionary.TryGetValue(model.IdTown, out var businessCards))
                {
                var cardList = ((model.IsVerified == true || resultCard.IsVerified == true) ? businessCards.VerifiedCards : businessCards.DraftCards) ?? new();

                var index = cardList.FindIndex(bc => bc.Id == resultCard.Id);

                if (index >= 0)
                    {
                    resultCard.LastModified ??= DateTimeExtension.CurrentTime;
                    cardList[index].CardData = resultCard;

                    businessCards.LastAccessedTime = DateTimeExtension.CurrentTime;
                    _TownCardsDictionary[model.IdTown] = businessCards;

                    if (model.IsVerified == true)
                        businessCards.DraftCards?.RemoveAll(d => d.Id == resultCard.Id);
                    //above will not be updated for existing users,instead for new Users only
                    await BroadcastUpdatedCardToGroup(cardList[index]);
                    }
                else
                    {
                    //update is wrong,so ignores
                    }
                }
            }
        return resultCardDataUpdated;
        }

    [Authorize]
    public async Task<BaseResult<CardDetailDto>?> UpdateCardDetail(CU_CardDetailCommand model)//, CancellationToken cancellationToken = default)
        {
        var userId = Context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roles = Context?.User?.FindFirst(ClaimTypes.Role)?.Value;

        var resultCardDetailUpdated = await _cardController.UpdateCardDetail(model);//, cancellationToken);

        if (resultCardDetailUpdated?.Success == true && resultCardDetailUpdated.Data != null)
            {
            var resultCard = resultCardDetailUpdated.Data;
            if (model.IdTown > 0 && _TownCardsDictionary.TryGetValue(model.IdTown, out var businessCards))
                {
                var cardList = ((model.IsVerified == true || resultCard.IsVerified == true) ? businessCards.VerifiedCards : businessCards.DraftCards) ?? new();

                var index = cardList.FindIndex(bc => bc.Id == resultCard.Id);

                if (index >= 0)
                    {
                    resultCard.LastModified ??= DateTimeExtension.CurrentTime;
                    cardList[index].CardDetail = resultCard;

                    businessCards.LastAccessedTime = DateTimeExtension.CurrentTime;
                    _TownCardsDictionary[model.IdTown] = businessCards;

                    if (model.IsVerified == true)
                        businessCards.DraftCards?.RemoveAll(d => d.Id == resultCard.Id);
                    //above will not be updated for existing users,instead for new Users only
                    await BroadcastUpdatedCardToGroup(cardList[index]);
                    }
                else
                    {
                    //update is wrong,so ignores
                    }
                }
            }
        return resultCardDetailUpdated;
        }

    [Authorize]
    public async Task<BaseResult<bool?>> UpdateOpenClose(CardDetailOpenCloseUpdateCommand model)//, CancellationToken cancellationToken = default)
        {
        var userId = Context?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roles = Context?.User?.FindFirst(ClaimTypes.Role)?.Value;

        var resultCardDetailUpdated = await _cardController.UpdateOpenClose(model);//, cancellationToken);

        if (resultCardDetailUpdated?.Success == true && resultCardDetailUpdated.Data != null)
            {
            var resultCard = resultCardDetailUpdated.Data;
            if (model.IdTown > 0 && _TownCardsDictionary.TryGetValue(model.IdTown, out var businessCards))
                {
                //TODO may be had to change here,can be draft also ????
                var cardList = (model.IsVerified == true ? businessCards.VerifiedCards : businessCards.DraftCards) ?? new();

                var index = cardList.FindIndex(bc => bc.Id == model.Id);

                if (index >= 0)
                    {
                    if (cardList[index]?.CardDetail != null)
                        {
                        cardList[index].CardDetail!.LastModified = DateTimeExtension.CurrentTime;
                        cardList[index].CardDetail!.IsOpenNow = model.IsOpenClose;
                        cardList[index].CardDetail!.LastModifiedBy = model.Operator;

                        businessCards.LastAccessedTime = DateTimeExtension.CurrentTime;
                        _TownCardsDictionary[model.IdTown] = businessCards;

                        if (model.IsVerified == true)
                            businessCards.DraftCards?.RemoveAll(d => d.Id == model.Id);
                        //above will not be updated for existing users,instead for new Users only
                        await BroadcastUpdatedCardToGroup(cardList[index]);
                        }
                    }
                else
                    {
                    //update is wrong,so ignores
                    }
                }
            }
        return resultCardDetailUpdated;
        }

    public Task Approval(int idTown, int idCard, bool? isApproved)
        {
        throw new NotImplementedException();
        }

    //more methods for Approve,Reject..in this sends to IsVerifiedEntryExists or Draft asUsual
    //more for  Like,Share,Comments..in this case send only response as count instead of full card

    private static void CleanupExpiredEntries(object? sender, ElapsedEventArgs e)
        {
        var expirationTime = DateTime.UtcNow.AddMinutes(-10); // Entries older than 10 minutes will be removed

        foreach (var key in _TownCardsDictionary.Keys)
            {
            if (_TownCardsDictionary.TryGetValue(key, out var entry) && entry.LastAccessedTime < expirationTime)
                {
                _TownCardsDictionary.TryRemove(key, out _);
                }
            }
        }

    public Task<bool> InitializeAsync(int idTown)
        {
        throw new NotImplementedException();
        }

    public ValueTask DisposeAsync()
        {
        throw new NotImplementedException();
        }
    }

/*
public class CacheManager
{
    //Usage
    //         // Add or update entry
    //    cacheManager.AddOrUpdate(townCard.Id, townCard);

    //    // Update entry
    //    townCard.VerifiedCards.Add(new iCardDto { Id = 102, CardName = "Card2" });
    //    cacheManager.AddOrUpdate(townCard.Id, townCard);

    private ConcurrentQueue<Action> _updateQueue = new ConcurrentQueue<Action>();
    private ConcurrentDictionary<int, TownCardsDto> _TownCardsDictionary = new ConcurrentDictionary<int, TownCardsDto>();

    public void AddOrUpdate(int id, TownCardsDto townCard)
    {
        _updateQueue.Enqueue(() => _TownCardsDictionary[id] = townCard);
        ProcessQueue();
    }

    public void Delete(int id)
    {
        _updateQueue.Enqueue(() => _TownCardsDictionary.TryRemove(id, out _));
        ProcessQueue();
    }

    public TownCardsDto Get(int id)
    {
        if (_TownCardsDictionary.TryGetValue(id, out var townCard))
            return townCard;
        else return null;
    }

    private void ProcessQueue()
    {
        while (_updateQueue.TryDequeue(out var updateAction))
        {
            // Execute the update
            updateAction();
        }
    }
}
*/
