using System;
using System.Xml.Linq;

namespace PMA
{
    public class DailyAppointment
    {
        public DateTime? DateOfAppointment { get; internal set; }
        public TimeSpan StartHour { get; internal set; }
        public TimeSpan EndHour { get; internal set; }
        public TimeSpan RestHour { get; internal set; }

        public static DailyAppointment CreateDailyAppointment(XElement xElement)
        {
            var dateOfAppointment = xElement.Element("data");
            var startHour = xElement.Element("inicio");
            var endHour = xElement.Element("fim");
            var restHour = xElement.Element("intervalo");

            return new DailyAppointment
            {
                DateOfAppointment = dateOfAppointment == null ? (DateTime?)null : DateTime.Parse(dateOfAppointment.Value),
                StartHour = startHour == null ? new TimeSpan() : TimeSpan.Parse(startHour.Value),
                EndHour = endHour == null ? new TimeSpan() : TimeSpan.Parse(endHour.Value),
                RestHour = restHour == null ? new TimeSpan() : TimeSpan.Parse(restHour.Value)
            };
        }
    }
}