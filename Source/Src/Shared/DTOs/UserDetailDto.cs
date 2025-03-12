using PublicCommon;

namespace Shared.DTOs;

public class UserDetailDto : UserDetail
    {
    public List<RoleDto>? RoleDtos { get; set; }
    public new List<CardDto>? iCards { get; set; }
    public List<CardApproval>? CardApprovals { get; set; }

    public bool CanCreateNewCard(int idTown = 0)
        {
        //all first time guys fell here
        if (iCards?.Count == 0) return true;

        if (Roles?.Count > 0 && Roles.Intersect(CONSTANTS.ROLES.TownAdminWriters(idTown)).Count() > 0)
            return true;

        if (iCards?.Count > 0 &&
            iCards.Count(x => x.IsVerified == true) - iCards.Count(x => x.IsVerified == false) >= 1)
            return false;
        return true;
        }
    }
