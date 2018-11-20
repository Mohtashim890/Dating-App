using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Helpers
{
    //static class means no instance required for using it methods
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message) {
            // include new headers to our http response
            response.Headers.Add("Application-Error", message);
            // to display above header
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            // to allow CORS
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
}