using System.Net.Http;
using System.Text;
using ModernHttpClient;
using Newtonsoft.Json.Linq;

namespace PMA.Services
{
    public class LoginService
    {
        private const string UrlLogin = "https://dextranet.dextra.com.br/pma/services/obter_token";
        private HttpClient HttpClient { get; }

        public LoginService()
        {
            HttpClient = new HttpClient(new NativeMessageHandler());
        }

        public string Login(string username, string password)
        {
            var loginData = new
            {
                username,
                password
            };

            var jsonLogin = JObject.FromObject(loginData).ToString();

            var response =
                HttpClient.PostAsync(UrlLogin, new StringContent(jsonLogin, Encoding.UTF8, "application/json")).Result;

            return response.Content.ReadAsStringAsync().Result;
        }
    }
}