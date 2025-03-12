using AutoMapper;
using FluentValidation;

namespace SHARED.Features.CardTypes.Commands
    {
    //not using this,instead using CreateUpdate
    public class UpdateCardTypeCommand : CardType,//later should remove this domain type
        IRequest<BaseResult>
        {
        //public int MyProperty { get; set; }

        private class Mapping : Profile
            {
            public Mapping()
                {
                CreateMap<CardType, UpdateCardTypeCommand>().ReverseMap();
                }
            }
        }

    public class UpdateCardTypeCommandValidator : AbstractValidator<UpdateCardTypeCommand>
        {
        public UpdateCardTypeCommandValidator(ITranslator translator)
            {
            //    RuleFor(p => p.MyProperty)
            //        .NotNull()
            //        .WithName(p => translator[nameof(p.MyProperty)]);
            }
        }
    }
