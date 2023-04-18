namespace JWTAuthProject.AppCode.Data
{
    public class FileDirectories
    {
        public static string Receipt = "wwwroot/receipt/";
        public static string Thumbnail = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Thumbnail/");
        public static string ProfilePic = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Profile/");
        public static string JsonDoc = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/doc/");
        public const string BannerImagePathSuffix = "wwwroot/Img/Banners/";
    }
}
