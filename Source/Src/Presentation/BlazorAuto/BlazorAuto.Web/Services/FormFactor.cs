using BlazorAuto.Shared.Services;
using PublicCommon;

namespace BlazorAuto.Web.Services;

public class FormFactor : IFormFactor
{
    public bool IsClientRender { get { return false; }  }
    public bool IsServerRender { get { return true; } }
    public string GetFormFactor()
    {
        return CONSTANTS.Web;
    }

    public string GetPlatform()
    {
        return Environment.OSVersion.ToString();
    }
}
