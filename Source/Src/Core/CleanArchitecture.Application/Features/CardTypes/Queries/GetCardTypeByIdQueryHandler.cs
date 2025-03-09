namespace CleanArchitecture.Application.Features.CardTypes.Queries
    {
    public class GetCardTypeByIdQueryHandler(ICardTypeRepository townCardTypeRepository, //ITranslator translator,
        IMapper mapper) : IRequestHandler<GetCardTypeByIdQuery, BaseResult<CardTypeDto>>
        {
        public async Task<BaseResult<CardTypeDto>> Handle(GetCardTypeByIdQuery request, CancellationToken cancellationToken)
            {
            var cardType = await townCardTypeRepository.GetByIdAsync(request.IdCardType, cancellationToken);

            if (cardType is null)
                {
                return new Error(ErrorCode.NotFound, $"CardType Not Found for id {request.IdCardType}", nameof(request.IdCardType));
                }
            var result = mapper.Map<CardTypeDto>(cardType);
            return result;
            //return BaseResult<CardTypeDto>.Ok(result, ConstantsCaching.Town_MinCacheTimeSpan);
            }
        }
    }
