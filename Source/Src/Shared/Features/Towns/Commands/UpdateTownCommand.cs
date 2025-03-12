using AutoMapper;

namespace SHARED.Features.Towns.Commands;
//not using this,instead using CreateUpdate
public class UpdateTownCommand : Town,//later should remove this domain type
    IRequest<BaseResult>
    {
    //public int MyProperty { get; set; }

    private class Mapping : Profile
        {
        public Mapping()
            {
            CreateMap<Town, UpdateTownCommand>().ReverseMap();
            }
        }

    }

