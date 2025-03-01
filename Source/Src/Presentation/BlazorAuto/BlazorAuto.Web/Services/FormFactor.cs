using BlazorAuto.Shared.Services;
using PublicCommon;

namespace BlazorAuto.Web.Services;

public class FormFactor : IFormFactor
{
    public string GetFormFactor()
    {
        return CONSTANTS.Web;
    }

    public string GetPlatform()
    {
        return Environment.OSVersion.ToString();
    }
}
