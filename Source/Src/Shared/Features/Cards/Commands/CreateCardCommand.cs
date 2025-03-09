using FluentValidation;
using SharedResponse;

namespace Shared.Features.Cards.Commands;

public class CreateCardCommand : Card_Draft,//later should remove this domain type
    IRequest<BaseResult<int>>
{
    //public int MyProperty { get; set; }
}

public class CreateCardCommandValidator : AbstractValidator<CreateCardCommand>
{
    public CreateCardCommandValidator(ITranslator translator)
    {
        //RuleFor(p => p.MyProperty)
        //    .NotNull()
        //    .WithName(p => translator[nameof(p.MyProperty)]);
    }
}
