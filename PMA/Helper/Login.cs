using System.Linq;
using System.Xml.Linq;

namespace PMA.Helper
{
    public static class Login
    {
        public static string GetToken(this string response)
        {
            var token = XDocument.Parse(response).Descendants("token").FirstOrDefault();
            return token?.Value;
        }
    }
}