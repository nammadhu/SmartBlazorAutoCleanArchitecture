using AutoMapper;
using FluentValidation;

namespace Shared.Features.Cards.Commands;

public class UpdateCardCommand : Card_Draft,//later should remove this domain type
    IRequest<BaseResult>
{
    //public int MyProperty { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Card_Draft, UpdateCardCommand>().ReverseMap();
        }
    }
}

public class UpdateCardCommandValidator : AbstractValidator<UpdateCardCommand>
{
    public UpdateCardCommandValidator()//ITranslator translator)
    {
        //    RuleFor(p => p.MyProperty)
        //        .NotNull()
        //        .WithName(p => translator[nameof(p.MyProperty)]);
    }
}
