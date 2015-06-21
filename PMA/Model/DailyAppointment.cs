using System;
using System.Xml.Linq;

namespace PMA.Model
{
    public class DailyAppointment
    {
        public DateTime? DateOfAppointment { get; internal set; }
        public TimeSpan StartTime { get; internal set; }
        public TimeSpan EndTime { get; internal set; }
        public TimeSpan IntervalTime { get; internal set; }

        public static DailyAppointment CreateDailyAppointment(XElement xElement)
        {
            var dateOfAppointment = xElement.Element("data");
            var startTime = xElement.Element("inicio");
            var endTime = xElement.Element("fim");
            var restTime = xElement.Element("intervalo");

            return new DailyAppointment
            {
                DateOfAppointment = dateOfAppointment == null ? (DateTime?)null : DateTime.Parse(dateOfAppointment.Value),
                StartTime = startTime == null ? new TimeSpan() : TimeSpan.Parse(startTime.Value),
                EndTime = endTime == null ? new TimeSpan() : TimeSpan.Parse(endTime.Value),
                IntervalTime = restTime == null ? new TimeSpan() : TimeSpan.Parse(restTime.Value)
            };
        }
    }
}