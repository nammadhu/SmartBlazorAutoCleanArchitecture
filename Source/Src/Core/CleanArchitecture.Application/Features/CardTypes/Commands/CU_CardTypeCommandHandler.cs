namespace CleanArchitecture.Application.Features.CardTypes.Commands;

public class CU_CardTypeCommandHandler(ICardTypeRepository repository, IUnitOfWork unitOfWork,
    ITranslator translator, IMapper mapper, IIDGenerator<CardType> idNextGenerator, ServerCachingCardTypes _cachingServiceTown, ILogger<CU_CardTypeCommandHandler> logger) : IRequestHandler<CU_CardTypeCommand, BaseResult<CardTypeDto>>
    {
    public async Task<BaseResult<CardTypeDto>> Handle(CU_CardTypeCommand request, CancellationToken cancellationToken)
        {
        try
            {
            if (request.Id > 0)//UPDATE
                {
                var existingData = await repository.GetByIdAsync(request.Id, cancellationToken);
                if (existingData is null)
                    {
                    return new Error(ErrorCode.NotFound, $"Cant update Non-Existing cardtype with id {request.Id}", nameof(request.Id));
                    }
                var isDuplicateResult = await NameExists(repository, translator, request, existingData, cancellationToken);
                if (isDuplicateResult != null) return isDuplicateResult;

                //todo must modify all specific properties here with validation on validator
                existingData = mapper.Map(request, existingData);
                existingData.Id = request.Id;

                repository.Update(existingData);
                var success = await unitOfWork.SaveChangesAsync(cancellationToken);
                //return new BaseResult<CardTypeDto>() { Success = await unitOfWork.SaveChangesAsync(cancellationToken), Data = mapper.Map<CardTypeDto>(existingData) };
                if (success)
                    {
                    var data = mapper.Map<CardTypeDto>(existingData);
                    _cachingServiceTown.AddOrReplaceCardTypeInCardTypes(data);
                    //return data;
                    return BaseResult<CardTypeDto>.OkNoClientCaching(data);
                    }
                return new Error(ErrorCode.Exception, $"Some issue in {nameof(CU_CardTypeCommandHandler)}-UpdateCard");
                }
            else//create
                {
                var isDuplicateResult = await NameExists(repository, translator, request, null, cancellationToken);
                if (isDuplicateResult != null) return isDuplicateResult;

                var obj = mapper.Map<CardType>(request);
                //var obj = request.To<CreateUpdateCardTypeCommand, CardType>();
                //todo should modify above
                //var product = new CardType(request.Name, request.Price, request.BarCode);

                obj.Id = idNextGenerator.GetNextID();//we can take of this as not necessary now
                var result = await repository.AddAsync(obj, cancellationToken);
                var success = await unitOfWork.SaveChangesAsync(cancellationToken);
                //return new BaseResult<CardTypeDto>() { Success = await unitOfWork.SaveChangesAsync(cancellationToken), Data = mapper.Map<CardTypeDto>(result) };
                if (success)
                    {
                    var data = mapper.Map<CardTypeDto>(result);
                    _cachingServiceTown.AddOrReplaceCardTypeInCardTypes(data);
                    return BaseResult<CardTypeDto>.OkNoClientCaching(data);
                    }
                return new Error(ErrorCode.Exception, $"Some issue in {nameof(CU_CardTypeCommandHandler)}-CreateCardWithData");
                }
            }
        catch (Exception ex)
            {
            logger.LogError(ex, "Failed");
            return new Error(ErrorCode.Exception, translator.GetString("Exception in iCardType Creation/UpdateCard"));
            }
        }

    private static async Task<BaseResult<CardTypeDto>?> NameExists(ICardTypeRepository repository, ITranslator translator, CU_CardTypeCommand request, CardType? existingData, CancellationToken cancellationToken)
        {
        if (existingData is null && request.Id > 0)//exisitng shuld have matching record
            {
            return new Error(ErrorCode.ModelIsNull, translator.GetString($"Data Mapping Is Null"), nameof(request.Id));
            }
        if ((existingData == null || !request.Name.Equals(existingData.Name, StringComparison.CurrentCultureIgnoreCase))
        && await repository.IsNameExistsAsync(request.Name, cancellationToken))//if different then check other shouldnot exists
            {
            return new Error(ErrorCode.DuplicateData, translator.GetString($"Name({request.Name}) Already exists"), nameof(request.Name));
            }
        return null;
        }
    }
