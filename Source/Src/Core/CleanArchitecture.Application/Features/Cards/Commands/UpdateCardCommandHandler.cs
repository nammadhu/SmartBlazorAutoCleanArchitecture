using MyTown.SharedModels.Features.Cards.Commands;
using SharedResponse;

namespace CleanArchitecture.Application.Features.Cards.Commands
    {
    public class UpdateCardCommandHandler(ITownCardRepository repository, IUnitOfWork unitOfWork
        , ITranslator translator, IMapper mapper) : IRequestHandler<UpdateCardCommand, BaseResult>
        {
        public async Task<BaseResult> Handle(UpdateCardCommand request, CancellationToken cancellationToken)
            {
            var data = await repository.GetByIdIntAsync(request.IdCard, cancellationToken);
            if (data is null)
                {
                return new BaseResult()
                    {
                    Success = false,
                    Errors = [new(ErrorCode.NotFound, translator.GetString(TranslatorMessages.ProductMessages.Product_notfound_with_id(request.IdCard)), nameof(request.IdCard))]
                    };
                }

            //todo must modify all specific properties here with validation on validator
            data = mapper.Map(request, data);
            data.IdCard = request.IdCard;

            repository.Update(data);

            return new BaseResult() { Success = await unitOfWork.SaveChangesAsync(cancellationToken) };
            }
        }
    }