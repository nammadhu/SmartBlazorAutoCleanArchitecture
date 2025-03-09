namespace CleanArchitecture.Application.Interfaces.Repositories
    {
    public interface ICardApprovalRepository : IGenericRepository<CardApproval>
        {
        Task<bool> SetApproverCardOfDraftCard(ApprovalCardSetRequestCommand request, CancellationToken cancellationToken);

        Task<List<Domain.CardApproval>> GetCardApprovalsOfApprover(List<int> verifiedCardIdsForApproval, CancellationToken cancellationToken, bool? isVerified = null);

        Task<List<Domain.CardApproval>> GetCardApprovals(int cardId, CancellationToken cancellationToken, bool? isVerified = null);

        Task<int> DeleteCardApprovals(int cardId, List<Domain.CardApproval>? approvers, CancellationToken cancellationToken);
        };

    public interface ICard_AdditionalTownRepository : IGenericRepository<Card_AdditionalTown>
        {
        Task<List<iCardDto>> GetTownAdditionalVerifiedCardsOfType(GetVerifiedCardsOfTypeInTownQuery query, CancellationToken cancellationToken);

        Task<List<iCardDto>> SearchTownAdditionalVerifiedCards(int townId, string name,
        bool includeNonSensitiveData = false, bool includeDetails = false);
        }

    public interface ICardDataRepository : IGenericRepository<CardData>
        { }

    public interface ICardDetailRepository : IGenericRepository<CardDetail>
        {
        Task<int> UpdateOpenClose(int cardId, bool openClose, Guid updatedBy, CancellationToken cancellationToken);
        }

    public interface ICard_DraftChangesRepository : IGenericRepository<Card_DraftChanges>
        {
        /*
        Task GetCardDraft(int iCardId,CancellationToken cancellationToken);

        Task<List<iCardDto>> GetTownVerifiedCardsAsync(int townId, string name, CancellationToken cancellationToken);

        Task<PagedResponse<iCardDto>> GetTownVerifiedCardsPagedListAsync(int townId, int pageNumber, int pageSize, CancellationToken cancellationToken, string name);
        */

        Task<List<iCardDto>> GetVerifiedCardsOfTypeInTown(GetVerifiedCardsOfTypeInTownQuery query, CancellationToken cancellationToken);
        }

    public interface ICardRepository : IGenericRepository<Card>
        {
        Task<List<iCardDto>> GetUserCards(Guid userId, CancellationToken cancellationToken);
        Task<(bool approvedResult, int townIdRefreshRequired)> ApproveCardAsync(ApproveCardCommand request, CancellationToken cancellationToken);

        Task<bool?> ApproveCardByAdmin(ApproveCardCommand request, Card card, CancellationToken cancellationToken);

        Task<Card> GetByIdIntAsync(int id, CancellationToken cancellationToken);

        Task<IList<iCardDto>> GetByNameAsync(string name, CancellationToken cancellationToken);

        /// this wont fetch town,instead only town cards. And drafts are only 100-verified
        Task<TownCardsDto> GetCardsOfTown(int townId, CancellationToken cancellationToken);

        //ideally below 2 should be avoided from outside //todo
        //Task<bool> ApproveCardByAdmin(int cardId, IList<string> adminRoles, CancellationToken cancellationToken, bool? IsVerifiedEntryExists = true, string noteMessage = null);
        //Task<bool> ApproveCardByPeer(int cardId, IList<string> userRoles, CancellationToken cancellationToken, bool? IsVerifiedEntryExists = true, string noteMessage = null);
        //ideally above 2 should be avoided from outside //todo
        }
    }
