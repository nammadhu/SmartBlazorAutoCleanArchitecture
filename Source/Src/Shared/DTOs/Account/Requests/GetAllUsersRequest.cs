using SHARED.Parameters;

namespace SHARED.DTOs.Account.Requests;

public class GetAllUsersRequest : PaginationRequestParameter
    {
    public string Name { get; set; }
    }
