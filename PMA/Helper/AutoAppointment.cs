using System;
using PMA.Model;
using PMA.Services;

namespace PMA.Helper
{
    public class AutoAppointment
    {
        private PmaService _pmaService;

        public AutoAppointment()
        {
            Login();
        }

        private void Login()
        {
            var username = Preferences.Shared.GetString(Preferences.Username, null);
            var password = Preferences.Shared.GetString(Preferences.Password, null);
            var response = _pmaService.Login(username.Trim(), password.Trim());
            _pmaService = new PmaService(response.GetToken());
        }

        public void VerifyAppointment()
        {
            var autoPointActivated = Preferences.Shared.GetBoolean(Preferences.AutoPointActivated, false);
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
            var lastNotification = Preferences.Shared.GetLong(Preferences.LastNotification, 0);
            if (lastNotification <= 0) return;

            var dateOfAppointment = new DateTime(lastNotification);
            var currentTimeSpan = new TimeSpan(dateOfAppointment.Hour, dateOfAppointment.Minute, 0);
            _pmaService.DateOfAppointment = dateOfAppointment;
            _pmaService.EndAppointment(currentTimeSpan);
        }

        public void SaveLastNotification()
        {
            var prefEditor = Preferences.Shared.Edit();
            prefEditor.PutLong(Preferences.LastNotification, DateTime.Now.Ticks);
            prefEditor.Commit();
        }
    }
}