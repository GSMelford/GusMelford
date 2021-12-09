namespace GusMelfordBot.Core
{
    public static class Constants
    {
        public const string UserAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/537.36";

        public static readonly string[] TikTokDomains = {TikTokDomain, TikTokMobileDomain};

        public const string TikTokDomain = "https://vm.tiktok.com/";
        public const string TikTokMobileDomain = "https://m.tiktok.com/";
        public const string SetCommand = "###";
    }
}