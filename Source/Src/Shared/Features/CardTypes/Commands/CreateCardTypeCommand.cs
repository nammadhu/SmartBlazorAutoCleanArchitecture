using FluentValidation;

namespace SHARED.Features.CardTypes.Commands;

//not using this,instead using CreateUpdate
public class CreateCardTypeCommand : CardType,//later should remove this domain type
    IRequest<BaseResult<int>>
    {
    //public int MyProperty { get; set; }
    private class Mapping : AutoMapper.Profile
        {
        public Mapping()
            {
            CreateMap<CreateCardTypeCommand, CardType>().ReverseMap();
            }
        }
    }

public class CreateCardTypeCommandValidator : AbstractValidator<CreateCardTypeCommand>
    {
    public CreateCardTypeCommandValidator(ITranslator translator)
        {
        //RuleFor(p => p.MyProperty)
        //    .NotNull()
        //    .WithName(p => translator[nameof(p.MyProperty)]);
        }
    }
