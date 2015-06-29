using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;
using ModernHttpClient;
using Newtonsoft.Json.Linq;
using PMA.Helper;
using PMA.Model;

namespace PMA.Services
{
    public class AppointmentService
    {
        private string Token { get; }
        private HttpClient HttpClient { get; }
        public DateTime DateOfAppointment { private get; set; }
        private const string UrlCriarApontamentoDiario = "https://dextranet.dextra.com.br/pma/services/criar_apontamento_diario";
        private const string UrlListarApontamentosDiarios = "https://dextranet.dextra.com.br/pma/services/listar_apontamentos_diarios";

        public AppointmentService(string token)
        {
            Token = token;
            DateOfAppointment = DateTime.Now;
            HttpClient = new HttpClient(new NativeMessageHandler());
        }

        public string StartAppointment(TimeSpan startTime)
        {
            var dailyAppointment = new
            {
                token = Token,
                data = $"{DateOfAppointment:yyyy-MM-dd}",
                inicio = $"{startTime.RoundToNearest(5):HH:mm}",
                intervalo = "00:00",
                fim = "21:00"
            };

            return CreateDailyAppointment(dailyAppointment);
        }

        public string IntervalAppointment(TimeSpan intervalTime)
        {
            var appointment = FindDailyAppointment();

            var totalIntervalTime = intervalTime + appointment.IntervalTime;

            var dailyAppointment = new
            {
                token = Token,
                data = $"{DateOfAppointment:yyyy-MM-dd}",
                inicio = appointment.StartTime.ToString(),
                intervalo = $"{totalIntervalTime.RoundToNearest(5):HH:mm}",
                fim = "21:00"
            };

            return CreateDailyAppointment(dailyAppointment);
        }

        public string EndAppointment(TimeSpan endTime)
        {
            var appointment = FindDailyAppointment();

            var dailyAppointment = new
            {
                token = Token,
                data = $"{DateOfAppointment:yyyy-MM-dd}",
                inicio = appointment.StartTime.ToString(),
                intervalo = appointment.IntervalTime.ToString(),
                fim = $"{endTime.RoundToNearest(5):HH:mm}"
            };

            return CreateDailyAppointment(dailyAppointment);
        }

        private string CreateDailyAppointment(object dailyAppointment)
        {
            var jsonAppointment = JObject.FromObject(dailyAppointment).ToString();

            var response = HttpClient.PostAsync(UrlCriarApontamentoDiario,
                new StringContent(jsonAppointment, Encoding.UTF8, "application/json")).Result;

            return response.Content.ReadAsStringAsync().Result;
        }

        private DailyAppointment FindDailyAppointment()
        {
            var dailyAppointment = new
            {
                token = Token,
                dataInicial = $"{DateOfAppointment:yyyy-MM-dd}",
                dataFinal = $"{DateOfAppointment:yyyy-MM-dd}"
            };

            var jsonAppointment = JObject.FromObject(dailyAppointment).ToString();

            var response = HttpClient.PostAsync(UrlListarApontamentosDiarios,
                new StringContent(jsonAppointment, Encoding.UTF8, "application/json")).Result;

            var responseString = response.Content.ReadAsStringAsync().Result;

            var populatedList = PopulateDailyAppointment(responseString);

            return populatedList?[0];
        }

        private static List<DailyAppointment> PopulateDailyAppointment(string response)
        {
            var entry = XDocument.Parse(response);
            var dailyAppointments = new List<DailyAppointment>();

            entry.Descendants("apontamentoDiario").ToList().ForEach(xElement =>
            {
                dailyAppointments.Add(DailyAppointment.CreateDailyAppointment(xElement));
            });

            return dailyAppointments;
        }
    }
}