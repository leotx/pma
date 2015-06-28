using System;
using Android.App;
using Android.Content;
using PMA.Model;
using PMA.Services;

namespace PMA.Helper
{
    public class Appointment
    {
        public AppointmentType AppointmentType { get; private set; }
        private ISharedPreferences _sharedPreferences;
        private PmaService _pmaService;
        const string TypeOfAppointment = "TYPE_APPOINTMENT";
        const string DateOfAppointment = "DATE_APPOINTMENT";
        const string IntervalTime = "INTERVAL_TIME";

        public Appointment(string token)
        {
            _sharedPreferences = Application.Context.GetSharedPreferences("PMA", FileCreationMode.Private);
            _pmaService = new PmaService(token);
        }

        public void ValidateAppointment()
        {
            var dateOfAppointment = new DateTime(_sharedPreferences.GetLong(DateOfAppointment, 0));

            if (dateOfAppointment.Date == DateTime.Now.Date)
            {
                AppointmentType = (AppointmentType)_sharedPreferences.GetInt(TypeOfAppointment, 0);
            }
            else
            {
                AppointmentType = AppointmentType.Cheguei;
                SavePreferences();
            }
        }

        private void SavePreferences()
        {
            var prefEditor = _sharedPreferences.Edit();
            prefEditor.PutInt(TypeOfAppointment, (int)AppointmentType);
            prefEditor.PutLong(DateOfAppointment, DateTime.Now.Ticks);
            prefEditor.Commit();
        }

        public string PointTime(TimeSpan currentTimeSpan)
        {
            var responseResult = string.Empty;

            switch (AppointmentType)
            {
                case AppointmentType.Cheguei:
                    responseResult = _pmaService.StartAppointment(currentTimeSpan);
                    break;
                case AppointmentType.Intervalo:
                    SaveInterval(currentTimeSpan);
                    break;
                case AppointmentType.Voltei:
                    var intervalTime = GetInterval(currentTimeSpan);
                    responseResult = _pmaService.IntervalAppointment(intervalTime);
                    break;
                case AppointmentType.Fui:
                    responseResult = _pmaService.EndAppointment(currentTimeSpan);
                    break;
            }

            return responseResult;
        }

        private TimeSpan GetInterval(TimeSpan currentTimeSpan)
        {
            var intervalTime = TimeSpan.Parse(_sharedPreferences.GetString(IntervalTime, null));
            return currentTimeSpan - intervalTime;
        }

        private void SaveInterval(TimeSpan currentTimeSpan)
        {
            var prefEditor = _sharedPreferences.Edit();
            prefEditor.PutString(IntervalTime, currentTimeSpan.ToString());
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