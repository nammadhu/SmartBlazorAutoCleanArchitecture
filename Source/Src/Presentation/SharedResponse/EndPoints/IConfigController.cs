//using Microsoft.AspNetCore.Components;//dont add this it makes conflict with mvc 

using PublicCommon;

namespace SharedResponse.EndPoints
    {
    public interface IConfigController
        {
        BaseResult<AppSettingsForClient> Fetch();
        }
    }