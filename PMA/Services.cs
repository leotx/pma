using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;
using ModernHttpClient;
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
        private readonly HttpClient _httpClient;

        public Services()
        {
            _httpClient = new HttpClient(new NativeMessageHandler());
        }

        public Services(string token)
        {
            _token = token;
            _httpClient = new HttpClient(new NativeMessageHandler());
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

            return response.Content.ReadAsStringAsync().Result;
        }

        public string StartAppointment(TimeSpan startTime)
        {
            var dailyAppointment = new
            {
                token = _token,
                data = string.Format("{0:yyyy-MM-dd}", DateTime.Now),
                inicio = string.Format("{0:HH:mm}", startTime.RoundToNearest(5)),
                intervalo = "00:00",
                fim = "21:00"
            };

            return CreateDailyAppointment(dailyAppointment);
        }

        public string IntervalAppointment(TimeSpan intervalTime)
        {
            var appointment = FindDailyAppointment();

            var dailyAppointment = new
            {
                token = _token,
                data = string.Format("{0:yyyy-MM-dd}", DateTime.Now),
                inicio = appointment.StartTime.ToString(),
                intervalo = string.Format("{0:HH:mm}", intervalTime.RoundToNearest(5)),
                fim = "21:00"
            };

            return CreateDailyAppointment(dailyAppointment);
        }

        public string EndAppointment(TimeSpan endTime)
        {
            var appointment = FindDailyAppointment();

            var dailyAppointment = new
            {
                token = _token,
                data = string.Format("{0:yyyy-MM-dd}", DateTime.Now),
                inicio = appointment.StartTime.ToString(),
                intervalo = appointment.IntervalTime.ToString(),
                fim = string.Format("{0:HH:mm}", endTime.RoundToNearest(5))
            };

            return CreateDailyAppointment(dailyAppointment);
        }

        private string CreateDailyAppointment(object dayAppointment)
        {
            var response = _httpClient.PostAsync(UrlCriarApontamentoDiario,
                new StringContent(dayAppointment.ToString(), Encoding.UTF8, "application/json")).Result;

            return response.Content.ReadAsStringAsync().Result;
        }

        private DailyAppointment FindDailyAppointment()
        {
            var dailyAppointment = new
            {
                token = _token,
                dataInicial = string.Format("{0:yyyy-MM-dd}", DateTime.Now),
                dataFinal = string.Format("{0:yyyy-MM-dd}", DateTime.Now)
            };

            var response = _httpClient.PostAsync(UrlListarApontamentosDiarios,
                new StringContent(dailyAppointment.ToString(), Encoding.UTF8, "application/json")).Result;

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