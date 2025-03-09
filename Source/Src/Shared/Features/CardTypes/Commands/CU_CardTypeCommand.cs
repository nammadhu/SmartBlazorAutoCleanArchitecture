using AutoMapper;
using FluentValidation;
using Shared.DTOs;

namespace Shared.Features.CardTypes.Commands;

public class CU_CardTypeCommand : CardType, IRequest<BaseResult<CardTypeDto>>
    {
    private class Mapping : Profile
        {
        public Mapping()
            {
            CreateMap<CU_CardTypeCommand, CardType>().ReverseMap();
            }
        }
    }

public class CreateUpdateCardTypeCommandValidator : AbstractValidator<CU_CardTypeCommand>
    {
    public CreateUpdateCardTypeCommandValidator()//(ITranslator translator)
        {
        RuleFor(p => p.Name)
            .NotNull();
        //.WithName(p => translator[nameof(p.Name)]);
        RuleFor(p => p.ShortName)
            .NotNull();
        //.WithName(p => translator[nameof(p.ShortName)]);
        }
    }
