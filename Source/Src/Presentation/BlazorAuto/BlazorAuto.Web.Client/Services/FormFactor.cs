using BlazorAuto.Shared.Services;
using PublicCommon;

namespace BlazorAuto.Web.Client.Services;

public class FormFactor : IFormFactor
{
    public string GetFormFactor()
    {
        return CONSTANTS.WebAssembly;
    }

    public string GetPlatform()
    {
        return Environment.OSVersion.ToString();
    }
}
