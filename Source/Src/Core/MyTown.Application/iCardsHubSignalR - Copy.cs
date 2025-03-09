using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MyTown.SharedModels.Interfaces;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Timers;

namespace MyTown.Application;
public partial class iCardsHubSignalR : Hub
{
    //internal static ConcurrentDictionary<string, List<BusinessCardDto>> _businessCardsDictionary = new ConcurrentDictionary<string, List<BusinessCardDto>>();
    //internal static ConcurrentDictionary<int, (List<iCardDto> VerifiedCardList, List<iCardDto> DraftCardList)> _businessCardsDictionary = new ConcurrentDictionary<int, (List<iCardDto>, List<iCardDto>)>();

    internal static ConcurrentDictionary<int, (List<iCardDto> VerifiedCardList, List<iCardDto> DraftCardList, int UserCount, DateTime LastAccessed)> _businessCardsDictionary = new ConcurrentDictionary<int, (List<iCardDto>, List<iCardDto>, int, DateTime)>();

    private static readonly System.Timers.Timer cleanupTimer;
    static iCardsHubSignalR()//static ctor for default initiation
    {
        cleanupTimer = new System.Timers.Timer(60000); // Check every minute
        cleanupTimer.Elapsed += CleanupExpiredEntries;
        cleanupTimer.Start();
    }

    public async Task JoinGroup(int townId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, townId.ToString());

        _businessCardsDictionary.AddOrUpdate(townId,
            (new List<iCardDto>(), new List<iCardDto>(), 1, DateTime.UtcNow),
            (key, value) => (value.VerifiedCardList, value.DraftCardList, value.UserCount + 1, DateTime.UtcNow));
    }

    public async Task LeaveGroup(int townId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, townId.ToString());

        if (_businessCardsDictionary.TryGetValue(townId, out var entry))
        {
            entry.UserCount -= 1;
            if (entry.UserCount == 0)
            {
                _businessCardsDictionary.TryRemove(townId, out _);
            }
            else
            {
                _businessCardsDictionary[townId] = entry;
            }
        }
    }
    //todo add one more endpoint as AddVerifiedCard
    [Authorize]
    public async Task AddBusinessCard(int townId, iCardDto businessCardDto, bool isVerified)
    {
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roles = Context.User.FindFirst(ClaimTypes.Role)?.Value;

        if (isVerified && (roles == null || !roles.Contains("Admin")))
        {
            throw new HubException("Only admins can add verified cards.");
        }

        var businessCards = _businessCardsDictionary.GetOrAdd(townId, (new List<iCardDto>(), new List<iCardDto>(), 0, DateTime.UtcNow));
        businessCardDto.LastModified ??= DateTimeExtension.CurrentTime;

        if (isVerified)
        {
            businessCards.VerifiedCardList.Add(businessCardDto);
        }
        else
        {
            businessCards.DraftCardList.Add(businessCardDto);
        }

        businessCards.LastAccessed = DateTimeExtension.CurrentTime;
        _businessCardsDictionary[townId] = businessCards;

        // Broadcast the new business card to all clients in the group
        await Clients.Group(townId.ToString()).SendAsync("ReceiveBusinessCard", businessCardDto, isVerified);
    }

    [Authorize]
    public async Task UpdateBusinessCard(int townId, iCardDto businessCardDto, bool isVerified)
    {
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roles = Context.User.FindFirst(ClaimTypes.Role)?.Value;

        if (isVerified && (roles == null || !roles.Contains("Admin")))
        {
            throw new HubException("Only admins can add verified cards.");
        }

        if (_businessCardsDictionary.TryGetValue(townId, out var businessCards))
        {
            var cardList = isVerified ? businessCards.VerifiedCardList : businessCards.DraftCardList;
            var index = cardList.FindIndex(bc => bc.Id == businessCardDto.Id);

            if (index >= 0)
            {
                businessCardDto.LastModified ??= DateTimeExtension.CurrentTime;
                cardList[index] = businessCardDto;

                businessCards.LastAccessed = DateTimeExtension.CurrentTime;
                _businessCardsDictionary[townId] = businessCards;

                // Broadcast the updated business card to all clients in the group
                await Clients.Group(townId.ToString()).SendAsync("ReceiveBusinessCard", businessCardDto, isVerified);
            }
        }
    }

    //more methods for Approve,Reject..in this sends to Verified or Draft asusual
    //more for  Like,Share,Comments..in this case send only response as count instead of full card

    private static void CleanupExpiredEntries(object? sender, ElapsedEventArgs e)
    {
        var expirationTime = DateTime.UtcNow.AddMinutes(-10); // Entries older than 10 minutes will be removed

        foreach (var key in _businessCardsDictionary.Keys)
        {
            if (_businessCardsDictionary.TryGetValue(key, out var entry) && entry.LastAccessed < expirationTime)
            {
                _businessCardsDictionary.TryRemove(key, out _);
            }
        }
    }
}