using BlazorAuto.Shared.Services;

namespace BlazorAuto.Services;

public class FormFactor : IFormFactor
{
    public bool IsClientRender =>  true;
    public bool IsMAUI => true;
    public bool IsServerRender => false;


    public string GetFormFactor()
    {
        return DeviceInfo.Idiom.ToString();
    }

    public string GetPlatform()
    {
        return DeviceInfo.Platform.ToString() + " - " + DeviceInfo.VersionString;
    }
}
