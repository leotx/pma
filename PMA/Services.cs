using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PMA
{
    public class Services
    {
        private const string UrlLogin = "https://dextranet.dextra.com.br/pma/services/obter_token";
        private const string UrlCriarApontamentoDiario = "https://dextranet.dextra.com.br/pma/services/criar_apontamento_diario";

        public async Task<string> Login(string username, string password)
        {
            var loginData = new
            {
                username,
                password
            };

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(UrlLogin,
                new StringContent(loginData.ToString(), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
 
        public async Task<string> CreateDayAppointment(string token, string day, string startHour, string endHour, string restHour)
        {
            var dayAppointment = new
            {
                token,
                data = day,
                inicio = startHour,
                intervalo = restHour,
                fim = endHour
            };

            var httpClient = new HttpClient();

            var response = await httpClient.PostAsync(UrlCriarApontamentoDiario, 
                new StringContent(dayAppointment.ToString(), Encoding.UTF8, "application/json"));
            
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}