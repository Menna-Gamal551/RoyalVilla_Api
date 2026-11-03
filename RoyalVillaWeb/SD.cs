namespace RoyalVillaWeb
{
    public static class SD
    {
        public enum ApiType
        {
            POST,
            GET,
            PUT,
            DELETE,
        }
        public const string SessionToken = "JWToken";
        public const string CurrentAPIVersion = "V2";
        public static string APIBaseUrl { get; set; }

        public static string GetImageUrl(string?imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            { return "/Images/placeholder-villa.jpg"; }
            if (imageUrl.StartsWith("http"))
            { return imageUrl; }

            return $"{APIBaseUrl}{imageUrl}";
        }
    }
}
