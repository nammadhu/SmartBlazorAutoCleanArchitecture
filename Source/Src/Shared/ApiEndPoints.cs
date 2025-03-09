namespace Shared;

public class Api
    {
    /*Example of how to use
            ITownController ic;
            Api api = new Api(1, typeof(ITownController));
            var t1=api.UrlOf(nameof(ic.GetById));
            Console.WriteLine(t1);
     */

    public Api(int version, string controller)
        {
        Version = version;
        Controller = controller;
        }

    public Api(int version, Type controller)
        {
        Version = version;
        //Controller = nameof(controller);//this doesnt works bcz it return only current variable name
        Controller = ExtractControllerName(controller.Name);
        }

    public Api(int version, string controller, string action) : this(version, controller)
        {
        Action = action;
        }

    public string UrlOf<T>(Func<T, object> method)
        {//not using currently
        return UrlOf(method.Method.Name);
        }

    public string HealthCheckUrl = "Health";

    public string UrlOf(string action)
        {
        return $"api/v{Version}/{Controller}/{action}";
        }

    public string UrlOf(string action, params string[] parametersKeyEqValues)
        {
        string parameters = "";
        if (parametersKeyEqValues?.Count() > 0)
            {
            bool ampersandRequired = false;
            parametersKeyEqValues.ToList().ForEach(parameterKeyEqValue =>
            {
                parameters += ampersandRequired + parameterKeyEqValue;
                ampersandRequired = true;
            });
            }

        return $"api/v{Version}/{Controller}/{action}?{parameters}";
        }

    public int Version { get; set; }
    public string Controller { get; set; }
    public string? Action { get; set; }

    public static string ExtractControllerName(string interfaceName)
        {
        if (interfaceName.StartsWith("I") && interfaceName.EndsWith("Controller"))
            {
            return interfaceName.Substring(1, interfaceName.Length - "Controller".Length - 1);
            }
        else if (interfaceName.EndsWith("Controller"))
            {
            return interfaceName.Substring(0, interfaceName.Length - "Controller".Length - 1);
            }
        else
            {
            return interfaceName;
            }
        }
    }

/*
public class ApiEndPoints
    {
    //public const string Version = "V1";

    //public const string ClientAnonymous = "AnonymousClient";
    //public const string ClientAuth = "AuthClient";

  //  public const string Prefix = "api";

    //public const string Town = "Town";
    //public const string TownCard = "TownCard";
    //public const string TownCardType = "TownCardType";

    //public static string BaseUrl(string type)
    //    {
    //    return $"{Version}/{type}";
    //    }

    //public const string Config = "Config";
    //public const string Vote = "Vote";
    //public const string Constituency = "Constituency";
    //public const string VoteSupportOppose = "VoteSupportOppose";

    //public const string VoteGetWithMyUserId = $"{Prefix}/Vote";
    //public const string VotePost = $"{Prefix}/Vote";

    //public const string ConstituencyGetAll = $"{Prefix}/Constituency";

    //public const string VoteSupportOpposePost = $"{Prefix}/VoteSupportOppose";

    //[("api/v{version:apiVersion}/[controller]/[action]")]
    // public const string ConfigExtractionGet = "v1/Config/Get";
    //public const string ConfigurationKey = "Configuration";
    //public const string TokenExtractionPost = "v1/AuthenticationConfigurations/Validate";
   // public const string TokenExtractionPostGoogle = "v1/Auth/ValidateG";
    //public const string LocalApiIssuedJwtKey = "ApiIssuedJwtKey";
    //public const string GetAll = "GetAll";
    //public const string GetById = "GetById";
    //public const string GetPagedList = "GetPagedList";
    //public const string Create = "Create";
    //public const string Update = "Update";
    //public const string Delete = "Delete";

    //public const string GetUserCardsMoreDetails = "GetUserCardsMoreDetails";
    }

*/
