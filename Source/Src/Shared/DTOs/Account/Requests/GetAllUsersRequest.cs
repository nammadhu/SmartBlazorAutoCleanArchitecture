using Shared.Parameters;

namespace Shared.DTOs.Account.Requests;

public class GetAllUsersRequest : PaginationRequestParameter
    {
    public string Name { get; set; }
    }
