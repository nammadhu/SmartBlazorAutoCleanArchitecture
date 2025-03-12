namespace SHARED.DTOs;

public class RoleDto
    {
    //UserRole is with Aspnet Identity System
    //AzureAd_B2C uses RoleDto of sharedResponse
    public RoleDto()
        {
        }

    public RoleDto(Guid roleId, string name)
        {
        RoleId = roleId;
        RoleName = name;
        }

    public RoleDto(Guid roleId)
        {
        RoleId = roleId;
        if (MasterData.Roles != null && MasterData.Roles.Any())
            RoleName = MasterData.Roles.Find(r => r.RoleId == RoleId)?.RoleName;
        }

    public RoleDto(Guid roleId, int? tID) : this(roleId)
        {
        TID = tID;
        }

    public RoleDto(Guid roleId, int? tID, string? roleName) : this(roleId, tID)
        {
        RoleName = roleName;
        }

    public RoleDto(Guid roleId, int? tID, string? roleName, string? tName) : this(roleId, tID, roleName)
        {
        TName = tName;
        }

    public Guid RoleId { get; set; }
    public int? TID { get; set; }
    public string? RoleName { get; set; }
    public string? TName { get; set; }
    }
