using PublicCommon;
using Shared.DTOs;

namespace Shared;

public class MasterData
    {
    //public static  List<RoleDto> Roles = [];//for aspnet identity system

    //below is for azure ad b2c system
    public static List<RoleDto> Roles =
        [new RoleDto(new Guid("B6E2CEDA-E400-4479-B404-08DCDDEFB3CA"), CONSTANTS.ROLES.Role_Admin),
        new RoleDto(new Guid("B1F2758A-2F60-4189-B405-08DCDDEFB3CA"), CONSTANTS.ROLES.Role_InternalAdmin),
        new RoleDto(new Guid("CBE23FCD-9CF8-4E8F-B406-08DCDDEFB3CA"), CONSTANTS.ROLES.Role_InternalViewer),
        new RoleDto(new Guid("B8DAF4AA-D634-4470-B407-08DCDDEFB3CA"), CONSTANTS.ROLES.Role_TownAdmin),

        new RoleDto(new Guid("9EB1F6BA-4C11-42B3-B408-08DCDDEFB3CA"), CONSTANTS.ROLES.Role_TownReviewer),
        new RoleDto(new Guid("EDE976FD-2449-463D-B409-08DCDDEFB3CA"), CONSTANTS.ROLES.Role_CardCreator),
        new RoleDto(new Guid("85CA0096-6FDE-4793-B40A-08DCDDEFB3CA"), CONSTANTS.ROLES.Role_CardOwner),
        new RoleDto(new Guid("59E52575-AFA9-4DE5-B40B-08DCDDEFB3CA"), CONSTANTS.ROLES.Role_CardVerifiedReviewer),
        new RoleDto(new Guid("198663B6-E70F-4A41-B40C-08DCDDEFB3CA"), CONSTANTS.ROLES.Role_CardVerifiedOwner),

        new RoleDto(new Guid("D936A0E0-702F-4C38-B40D-08DCDDEFB3CA"), CONSTANTS.ROLES.Role_Blocked)
        ];
    }
