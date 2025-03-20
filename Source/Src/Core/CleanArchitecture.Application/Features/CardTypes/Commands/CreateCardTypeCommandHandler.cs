
namespace CleanArchitecture.Application.Features.CardTypes.Commands
    {
    //not using this,instead using CreateUpdate
    public class CreateCardTypeCommandHandler(ICardTypeRepository townCardTypeRepo, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateCardTypeCommand, BaseResult<int>>
        {
        public async Task<BaseResult<int>> Handle(CreateCardTypeCommand request, CancellationToken cancellationToken)
            {
            var obj = mapper.Map<CardType>(request);
            //var obj = request.To<CreateCardTypeCommand, CardType>();
            //todo should modify above 
            //var product = new CardType(request.Name, request.Price, request.BarCode);

            await townCardTypeRepo.AddAsync(obj, cancellationToken);
            bool success = await unitOfWork.SaveChangesAsync(cancellationToken);

            return obj.Id;
            }
        }
    }
