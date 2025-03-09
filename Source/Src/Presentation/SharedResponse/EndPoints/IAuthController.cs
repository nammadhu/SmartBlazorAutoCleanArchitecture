
using SharedResponse;
using SharedResponse.Wrappers;

public interface IAuthController
    {
    Task<BaseResult<AuthenticationResponse>> ValidateG(ValidateAppRequest request);
    }
