using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using IdentityModel;
using IdentityModel.Client;

namespace IdentityServer.IntegrationTests;

public static class TokenExtensions
{
    public static Dictionary<string, JsonNode> GetFields(this TokenResponse response)
    {
        return JsonSerializer.Deserialize<Dictionary<string, JsonNode>>(response.Json);
    }

    public static Dictionary<string, JsonNode> GetPayload(this TokenResponse response)
    {
        var token = response.AccessToken
            .Split('.')
            .Skip(1)
            .Take(1)
            .First();

        var dencoded = Encoding.UTF8.GetString(Base64Url.Decode(token));
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, JsonNode>>(dencoded);

        return dictionary;
    }

}
