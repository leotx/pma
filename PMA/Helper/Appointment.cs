using System;
using PMA.Model;

namespace PMA.Helper
{
    public class Appointment
    {
        public AppointmentType AppointmentType { get; set; }

        public void ValidateAppointment()
        {
            var dateOfAppointment = new DateTime(Preferences.Shared.GetLong(Preferences.DateOfAppointment, 0));

            if (dateOfAppointment.Date == DateTime.Now.Date)
            {
                AppointmentType = (AppointmentType)Preferences.Shared.GetInt(Preferences.TypeOfAppointment, 0);
            }
            else
            {
                AppointmentType = AppointmentType.Cheguei;
                SavePreferences();
            }
        }

        public void SavePreferences()
        {
            var prefEditor = Preferences.Shared.Edit();
            prefEditor.PutInt(Preferences.TypeOfAppointment, (int)AppointmentType);
            prefEditor.PutLong(Preferences.DateOfAppointment, DateTime.Now.Ticks);
            prefEditor.Commit();
        }

        public void NextAppointment()
        {
            var arrayList = (AppointmentType[])Enum.GetValues(typeof(AppointmentType));
            var indexArray = Array.IndexOf(arrayList, AppointmentType) + 1;
            AppointmentType = (arrayList.Length == indexArray) ? arrayList[0] : arrayList[indexArray];

            SavePreferences();
        }
    }
}