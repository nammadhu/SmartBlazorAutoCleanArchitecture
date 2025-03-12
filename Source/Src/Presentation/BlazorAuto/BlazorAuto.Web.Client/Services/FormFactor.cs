using BASE;
using BlazorAuto.Shared.Services;

namespace BlazorAuto.Web.Client.Services;

public class FormFactor : IFormFactor
    {
    public bool IsClientRender => true;
    public bool IsServerRender => false;

    public bool IsMAUI => false;
    public string GetFormFactor()
        {
        return CONSTANTS.WebAssembly;
        }

    public string GetPlatform()
        {
        return Environment.OSVersion.ToString();
        }
    }
