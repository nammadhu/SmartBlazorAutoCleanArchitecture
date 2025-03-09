namespace Shared.Features.Towns.Commands;

public class DeleteTownCommand : IRequest<BaseResult>
    {
    public int IdTown { get; set; }
    }
