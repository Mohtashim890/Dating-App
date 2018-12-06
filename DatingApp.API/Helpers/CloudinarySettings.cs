namespace DatingApp.API.Helpers
{
    public class CloudinarySettings
    {
        // to access Cloudinary Settings easily
        public string CloudName { get; set; }

        public string ApiKey { get; set; }

        public string ApiSecret { get; set; }

        // head back to startup.cs to tie these props to those in appsettings.json
    }
}