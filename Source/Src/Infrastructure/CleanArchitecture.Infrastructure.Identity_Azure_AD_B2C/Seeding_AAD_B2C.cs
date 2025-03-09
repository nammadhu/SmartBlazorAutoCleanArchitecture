using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using PublicCommon;

namespace CleanArchitecture.Infrastructure.Identity_Azure_AD_B2C;

public class Seeding_AAD_B2C//(IConfiguration configuration)
{
    //seeding is inserting but not identifying when they loggedin ,so need to check the reasons
    public static async Task AddUser(IConfiguration configurationNew, List<UserDetailBase> userInfos)
    {
        var configuration = configurationNew;
        string roleAttribute = $"extension_{configuration["GraphApi:AzureAdExtensionsAppId"]}_Roles";

        GraphServiceClient graphClient = new GraphServiceClient(new ClientSecretCredential(
    configuration["GraphApi:AzureAdTenantId"],//"44445572-4530-41b5-a916-9f63a6b5ca98"
    configuration["GraphApi:AzureAdClientId"],//"33335572-4530-41b5-a916-9f63a6b5ca98"
    configuration["GraphApi:AzureAdClientSecret"]//"Ocn8Q~8NpumKbT~4JGMpGcMnKADSFDSDBpq2H2qcki"
    ));
        foreach (var userInfo in userInfos)
        {
            var user = new User
            {
                //AccountEnabled = true,
                DisplayName = userInfo.Name,

                //MailNickname = "testuser",
                UserPrincipalName = "cpim_" + Guid.NewGuid().ToString() + "@" + configuration["AzureAdB2C:Domain"],
                //UserPrincipalName = userInfo.Email?.Replace("@", "_") + "@" + configuration["AzureAdB2C:Domain"],
                Identities = new List<ObjectIdentity>
            {
                new ObjectIdentity
                {
                    SignInType = "federated",
                    Issuer = "google.com",//userInfo.Email!.Substring(userInfo.Email.IndexOf('@')),  //"google.com",
                    IssuerAssignedId = userInfo.Email
                }
            },
                AdditionalData = new Dictionary<string, object>
                {
                    { roleAttribute, userInfo.Roles.ToCsv() }
                }
            };

            try
            {
                var result = await graphClient.Users.PostAsync(user);
                Console.WriteLine("User created successfully");
            }
            catch (ODataError odataError)
            {
                Console.WriteLine($"Error: {odataError.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"CommonError: {e.ToString()}");
            }
        }
    }
}