using System;
using System.Xml.Linq;

namespace PMA
{
    public class DailyAppointment
    {
        public DateTime? Data { get; internal set; }
        public TimeSpan? Inicio { get; internal set; }
        public TimeSpan? Fim { get; internal set; }
        public TimeSpan? Intervalo { get; internal set; }

        public static DailyAppointment CreateDailyAppointment(XElement xElement)
        {
            var dateOfAppointment = xElement.Element("data");
            var startHour = xElement.Element("inicio");
            var endHour = xElement.Element("fim");
            var restHour = xElement.Element("intervalo");

            return new DailyAppointment
            {
                Data = dateOfAppointment == null ? (DateTime?)null : DateTime.Parse(dateOfAppointment.Value),
                Inicio = startHour == null ? (TimeSpan?)null : TimeSpan.Parse(startHour.Value),
                Fim = endHour == null ? (TimeSpan?)null : TimeSpan.Parse(endHour.Value),
                Intervalo = restHour == null ? (TimeSpan?)null : TimeSpan.Parse(restHour.Value)
            };
        }
    }
}