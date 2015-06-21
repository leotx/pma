using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using PMA.Helper;

namespace PMA
{
    public class Services
    {
        private readonly string _token;
        private const string UrlLogin = "https://dextranet.dextra.com.br/pma/services/obter_token";
        private const string UrlCriarApontamentoDiario = "https://dextranet.dextra.com.br/pma/services/criar_apontamento_diario";
        private const string UrlListarApontamentosDiarios = "https://dextranet.dextra.com.br/pma/services/listar_apontamentos_diarios";

        public Services()
        {
        }

        public Services(string token)
        {
            _token = token;
        }

        public string Login(string username, string password)
        {
            var loginData = new
            {
                username,
                password
            };

            var httpClient = new HttpClient();

            var jsonLogin = JObject.FromObject(loginData).ToString();

            var response = httpClient.PostAsync(UrlLogin,
                new StringContent(jsonLogin, Encoding.UTF8, "application/json")).Result;

            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsStringAsync().Result;
        }

        public string CreateDailyAppointment(TimeSpan startHour)
        {
            var dailyAppointment = new
            {
                token = _token,
                data = string.Format("{0:yyyy-MM-dd}", DateTime.Now),
                inicio = string.Format("{0:HH:mm}", startHour.RoundToNearest(5)),
                intervalo = "00:00",
                fim = "21:00"
            };

            return DailyAppointment(dailyAppointment);
        }

        public string CreateDailyAppointment(TimeSpan startHour, TimeSpan restHour)
        {
            var dailyAppointment = new
            {
                token = _token,
                data = string.Format("{0:yyyy-MM-dd}", DateTime.Now),
                inicio = string.Format("{0:HH:mm}", startHour.RoundToNearest(5)),
                intervalo = string.Format("{0:HH:mm}", restHour.RoundToNearest(5)),
                fim = "21:00"
            };

            return DailyAppointment(dailyAppointment);
        }

        public string CreateDailyAppointment(TimeSpan startHour, TimeSpan restHour, TimeSpan endHour)
        {
            var dailyAppointment = new
            {
                token = _token,
                data = string.Format("{0:yyyy-MM-dd}", DateTime.Now),
                inicio =  string.Format("{0:HH:mm}", startHour.RoundToNearest(5)),
                intervalo = string.Format("{0:HH:mm}", restHour.RoundToNearest(5)),
                fim = string.Format("{0:HH:mm}", endHour.RoundToNearest(5))
            };

            return DailyAppointment(dailyAppointment);
        }

        private static string DailyAppointment(object dayAppointment)
        {
            var httpClient = new HttpClient();

            var response = httpClient.PostAsync(UrlCriarApontamentoDiario,
                new StringContent(dayAppointment.ToString(), Encoding.UTF8, "application/json")).Result;

            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsStringAsync().Result;
        }

        public DailyAppointment FindDailyAppointment()
        {
            var dailyAppointment = new
            {
                token = _token,
                dataInicial = string.Format("{0:yyyy-MM-dd}", DateTime.Now),
                dataFinal = string.Format("{0:yyyy-MM-dd}", DateTime.Now)
            };

            var httpClient = new HttpClient();
            var response = httpClient.PostAsync(UrlListarApontamentosDiarios,
                new StringContent(dailyAppointment.ToString(), Encoding.UTF8, "application/json")).Result;
            response.EnsureSuccessStatusCode();

            var responseString = response.Content.ReadAsStringAsync().Result;

            var populatedList = PopulateDailyAppointment(responseString);

            return populatedList != null ? populatedList[0] : null;
        }

        private static List<DailyAppointment> PopulateDailyAppointment(string response)
        {
            var entry = XDocument.Parse(response);
            var dailyAppointments = new List<DailyAppointment>();

            entry.Descendants("apontamentoDiario").ToList().ForEach(xElement =>
            {
                dailyAppointments.Add(PMA.DailyAppointment.CreateDailyAppointment(xElement));
            });

            return dailyAppointments;
        }
    }
}