namespace LINQ_To_Twitter_OAuth2_Sample.Shared.Models
{
    public class L2TBase
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; } = false;
    }
}
