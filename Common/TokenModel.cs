using System;
using System.Text.Json.Serialization;

namespace Common;

public class TokenModel
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonIgnore]
    public DateTime ExpiresAt { get; set; }

    public void SetExpiration() => ExpiresAt = DateTime.UtcNow.AddSeconds(ExpiresIn);
}
