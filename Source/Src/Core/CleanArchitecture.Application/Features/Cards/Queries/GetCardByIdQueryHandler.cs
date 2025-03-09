namespace CleanArchitecture.Application.Features.Cards.Queries
    {
    public class GetCardByIdQueryHandler(ICardRepository TownCardRepository,
        ICard_DraftChangesRepository draftChangesRepository,
        ITranslator translator, IMapper mapper) : IRequestHandler<GetCardByIdQuery, BaseResult<iCardDto>>
        {
        public async Task<BaseResult<iCardDto>> Handle(GetCardByIdQuery request, CancellationToken cancellationToken)
            {
            _CardBase? card;

            if (request.IsDraft)
                card = await draftChangesRepository.GetByIdAsync(request.IdCard, cancellationToken);
            else
                card = await TownCardRepository.GetByIdIntAsync(request.IdCard, cancellationToken);

            if (card is null)
                {
                return new Error(ErrorCode.NotFound, translator.GetString("iCard does not exists for provided Id"), nameof(request.IdCard));
                }

            var result = mapper.Map<iCardDto>(card); //product.To<TownCard, TownCardDto>();

            //return result;
            return BaseResult<iCardDto>.Ok(result, ConstantsCachingServer.Card_MinCacheTimeSpan);
            }
        }
    }
