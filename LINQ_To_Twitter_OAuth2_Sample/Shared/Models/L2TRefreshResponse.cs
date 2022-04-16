using System.Text.Json.Serialization;

namespace LINQ_To_Twitter_OAuth2_Sample.Shared.Models;
public record L2TRefreshResponse(
	[property: JsonPropertyName("token_type")] string TokenType,
	[property: JsonPropertyName("expires_in")] int ExpiresIn,
	[property: JsonPropertyName("access_token")] string AccessToken,
	[property: JsonPropertyName("scope")] string Scope,
	[property: JsonPropertyName("refresh_token")] string RefreshToken
);
