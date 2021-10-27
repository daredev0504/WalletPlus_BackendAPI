using System.Text.Json.Serialization;

namespace WalletPlusIncAPI.Services.AuthManager
{
    public class JwtAuthResult
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expiration")]
        public string Expiration { get; set; }
    }
}
