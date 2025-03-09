using Shared.Parameters;

namespace Shared.DTOs;

public class GetAllUsersRequest : PaginationRequestParameter
    {
    public string? Name { get; set; }
    public string? Email { get; set; }

    //public Guid RoleId { get; set; }
    public string? Role { get; set; }

    public int TId { get; set; }
    }
