using BlazorAuto.Shared.Services;
using PublicCommon;

namespace BlazorAuto.Web.Client.Services;

public class FormFactor : IFormFactor
{
    public bool IsClientRender { get { return true; } }
    public bool IsServerRender { get { return false; } }
    public string GetFormFactor()
    {
        return CONSTANTS.WebAssembly;
    }

    public string GetPlatform()
    {
        return Environment.OSVersion.ToString();
    }
}
