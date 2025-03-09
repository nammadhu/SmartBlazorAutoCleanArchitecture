namespace SharedResponse.DTOs;

//currently not using,as ADb2C using
public class UserRoleDto
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public int? TID { get; set; }
    public string? UserName { get; set; }
    public string? RoleName { get; set; }
    public string? TName { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastModified { get; set; }
}