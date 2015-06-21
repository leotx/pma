using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace PMA
{
    public class Services
    {
        private const string UrlLogin = "https://dextranet.dextra.com.br/pma/services/obter_token";
        private const string UrlCriarApontamentoDiario = "https://dextranet.dextra.com.br/pma/services/criar_apontamento_diario";
        private const string UrlListarApontamentosDiarios = "https://dextranet.dextra.com.br/pma/services/listar_apontamentos_diarios";

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

        public string CreateDailyAppointment(string token, DateTime dateAppointment, TimeSpan startHour)
        {
            var dailyAppointment = new
            {
                token,
                data = string.Format("{0:yyyy-MM-dd}", dateAppointment),
                inicio = startHour.ToString(),
                intervalo = "0:00",
                fim = "0:00"
            };

            return DailyAppointment(dailyAppointment);
        }

        public string CreateDailyAppointment(string token, DateTime dateAppointment, TimeSpan startHour, TimeSpan restHour)
        {
            var dailyAppointment = new
            {
                token,
                data = string.Format("{0:yyyy-MM-dd}", dateAppointment),
                inicio = startHour.ToString(),
                intervalo = restHour.ToString(),
                fim = "00:00"
            };

            return DailyAppointment(dailyAppointment);
        }

        public string CreateDailyAppointment(string token, DateTime dateAppointment, TimeSpan startHour, TimeSpan restHour, TimeSpan endHour)
        {
            var dailyAppointment = new
            {
                token,
                data = string.Format("{0:yyyy-MM-dd}", dateAppointment),
                inicio = startHour.ToString(),
                intervalo = restHour.ToString(),
                fim = endHour.ToString()
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

        public List<DailyAppointment> FindDailyAppointments(string token, DateTime startDate, DateTime endDate)
        {
            var dailyAppointment = new
            {
                token,
                dataInicial = string.Format("{0:yyyy-MM-dd}", startDate),
                dataFinal = string.Format("{0:yyyy-MM-dd}", endDate)
            };

            var httpClient = new HttpClient();
            var response = httpClient.PostAsync(UrlCriarApontamentoDiario,
                new StringContent(dailyAppointment.ToString(), Encoding.UTF8, "application/json")).Result;
            response.EnsureSuccessStatusCode();

            var responseString = response.Content.ReadAsStringAsync().Result;

            return PopulateDailyAppointment(responseString);
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