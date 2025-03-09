namespace BlazorAuto.Shared.Services
    {
    public interface IFormFactor
        {
        public bool IsClientRender { get; }
        public bool IsServerRender { get; }
        public bool IsMAUI { get; }
        public string GetFormFactor();
        public string GetPlatform();
        }
    }
