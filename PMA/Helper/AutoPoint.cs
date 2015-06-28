using System;
using PMA.Model;
using PMA.Services;

namespace PMA.Helper
{
    public class AutoPoint
    {
        private const string Username = "USERNAME";
        private const string Password = "PASSWORD";
        private const string AutoPointActivated = "AUTO_POINT";
        private const string LastNotification = "LAST_NOTIFICATION";
        private readonly Appointment _appointment;
        private PmaService _pmaService;

        public AutoPoint()
        {
            _appointment = new Appointment();
            Login();
        }

        private void Login()
        {
            var username = _appointment.SharedPreferences.GetString(Username, null);
            var password = _appointment.SharedPreferences.GetString(Password, null);
            var response = _pmaService.Login(username.Trim(), password.Trim());
            _pmaService = new PmaService(response.GetToken());
        }

        public void VerifyAppointment()
        {
            var autoPointActivated = _appointment.SharedPreferences.GetBoolean(AutoPointActivated, false);
            if (!autoPointActivated) return;

            MakeAppointment();
        }

        private void MakeAppointment()
        {
            var appointment = new Appointment();
            appointment.ValidateAppointment();
            if (appointment.AppointmentType != AppointmentType.Cheguei) return;

            StartAppointment();
            EndAppointment();

            appointment.NextAppointment();
        }

        private void StartAppointment()
        {
            var currentTimeSpan = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
            _pmaService.StartAppointment(currentTimeSpan);
        }

        private void EndAppointment()
        {
            var lastNotification = _appointment.SharedPreferences.GetLong(LastNotification, 0);
            if (lastNotification <= 0) return;

            var dateOfAppointment = new DateTime(lastNotification);
            var currentTimeSpan = new TimeSpan(dateOfAppointment.Hour, dateOfAppointment.Minute, 0);
            _pmaService.DateOfAppointment = dateOfAppointment;
            _pmaService.EndAppointment(currentTimeSpan);
        }

        public void SaveLastNotification()
        {
            var prefEditor = _appointment.SharedPreferences.Edit();
            prefEditor.PutLong(LastNotification, DateTime.Now.Ticks);
            prefEditor.Commit();
        }
    }
}