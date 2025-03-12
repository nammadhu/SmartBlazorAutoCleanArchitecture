using BASE;
using BlazorAuto.Shared.Services;

namespace BlazorAuto.Web.Services;

public class FormFactor : IFormFactor
    {
    public bool IsServerRender => true;
    public bool IsClientRender => false;

    public bool IsMAUI => false;
    public string GetFormFactor()
        {
        return CONSTANTS.Web;
        }

    public string GetPlatform()
        {
        return Environment.OSVersion.ToString();
        }
    }
