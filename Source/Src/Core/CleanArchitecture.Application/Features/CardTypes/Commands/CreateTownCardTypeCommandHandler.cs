using MyTown.SharedModels.Features.CardTypes.Commands;

namespace CleanArchitecture.Application.Features.CardTypes.Commands
    {
    //not using this,instead using CreateUpdate
    public class CreateTownCardTypeCommandHandler(ITownCardTypeRepository townCardTypeRepo, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateCardTypeCommand, BaseResult<int>>
        {
        public async Task<BaseResult<int>> Handle(CreateCardTypeCommand request, CancellationToken cancellationToken)
            {
            var obj = mapper.Map<CardType>(request);
            //var obj = request.To<CreateTownCardTypeCommand, TownCardType>();
            //todo should modify above 
            //var product = new TownCardType(request.Name, request.Price, request.BarCode);

            await townCardTypeRepo.AddAsync(obj, cancellationToken);
            bool success = await unitOfWork.SaveChangesAsync(cancellationToken);

            return obj.IdCardType;
            }
        }
    }