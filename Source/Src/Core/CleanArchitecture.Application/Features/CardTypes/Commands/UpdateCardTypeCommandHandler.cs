
namespace CleanArchitecture.Application.Features.CardTypes.Commands
    {
    //not using this,instead using CreateUpdate
    public class UpdateCardTypeCommandHandler(ICardTypeRepository repository, IUnitOfWork unitOfWork
        , ITranslator translator, IMapper mapper) : IRequestHandler<UpdateCardTypeCommand, BaseResult>
        {
        public async Task<BaseResult> Handle(UpdateCardTypeCommand request, CancellationToken cancellationToken)
            {
            var data = await repository.GetByIdIntAsync(request.IdCardType, cancellationToken);
            if (data is null)
                {
                return new BaseResult()
                    {
                    Success = false,
                    Errors = [new(ErrorCode.NotFound, translator.GetString(TranslatorMessages.ProductMessages.Product_NotFound_with_id(request.IdCardType)), nameof(request.IdCardType))]
                    };
                }

            //todo must modify all specific properties here with validation on validator
            data = mapper.Map(request, data);
            data.IdCardType = request.IdCardType;

            repository.Update(data);

            return new BaseResult() { Success = await unitOfWork.SaveChangesAsync(cancellationToken) };
            }
        }
    }
