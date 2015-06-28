using System;
using System.Reflection;
using Android.App;
using Android.Content;
using PMA.Model;

namespace PMA.Helper
{
    public class Appointment
    {
        public AppointmentType AppointmentType { get; private set; }
        public readonly ISharedPreferences SharedPreferences;
        const string TypeOfAppointment = "TYPE_APPOINTMENT";
        const string DateOfAppointment = "DATE_APPOINTMENT";

        public Appointment()
        {
            SharedPreferences = Application.Context.GetSharedPreferences(Assembly.GetExecutingAssembly().GetName().Name, FileCreationMode.Private);
        }

        public void ValidateAppointment()
        {
            var dateOfAppointment = new DateTime(SharedPreferences.GetLong(DateOfAppointment, 0));

            if (dateOfAppointment.Date == DateTime.Now.Date)
            {
                AppointmentType = (AppointmentType)SharedPreferences.GetInt(TypeOfAppointment, 0);
            }
            else
            {
                AppointmentType = AppointmentType.Cheguei;
                SavePreferences();
            }
        }

        private void SavePreferences()
        {
            var prefEditor = SharedPreferences.Edit();
            prefEditor.PutInt(TypeOfAppointment, (int)AppointmentType);
            prefEditor.PutLong(DateOfAppointment, DateTime.Now.Ticks);
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