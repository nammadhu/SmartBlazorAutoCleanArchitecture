﻿using MyTown.SharedModels.Features.Towns.Commands;
using SharedResponse;

namespace CleanArchitecture.Application.Features.Towns.Commands
    {
    //not using this,instead using CreateUpdate
    public class UpdateTownCommandHandler(ITownRepository repository, IUnitOfWork unitOfWork
        , ITranslator translator, IMapper mapper) : IRequestHandler<UpdateTownCommand, BaseResult>
        {
        public async Task<BaseResult> Handle(UpdateTownCommand request, CancellationToken cancellationToken)
            {
            var data = await repository.GetByIdIntAsync(request.Id);
            if (data is null)
                {
                return new BaseResult()
                    {
                    Success = false,
                    Errors = [new(ErrorCode.NotFound, translator.GetString(TranslatorMessages.ProductMessages.Product_notfound_with_id(request.Id)), nameof(request.Id))]
                    };
                }

            //todo must modify all specific properties here with validation on validator
            data = mapper.Map(request, data);
            data.Id = request.Id;

            repository.Update(data);

            return new BaseResult() { Success = await unitOfWork.SaveChangesAsync() };
            }
        }
    }