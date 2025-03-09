using MyTown.SharedModels.Features.Cards.Commands;

namespace CleanArchitecture.Application.Features.Cards.Commands
    {
    public class CreateCardCommandHandler(ITownCardRepository townCardRepo, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateCardCommand, BaseResult<int>>
        {
        public async Task<BaseResult<int>> Handle(CreateCardCommand request, CancellationToken cancellationToken)
            {
            var obj = mapper.Map<Card>(request); //product.To<TownCard, TownCardDto>();
            //var obj = request.To<CreateTownCardCommand, TownCard>();
            //todo should modify above 
            //var product = new TownCard(request.Name, request.Price, request.BarCode);

            await townCardRepo.AddAsync(obj, cancellationToken);
            bool success = await unitOfWork.SaveChangesAsync(cancellationToken);

            return new BaseResult<int>(obj.IdCard) { Success = success };
            }
        }
    }