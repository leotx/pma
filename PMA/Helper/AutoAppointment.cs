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
            _pmaService = new PmaService();
        }

        private void Login()
        {
            var username = Preferences.Shared.GetString(Preferences.Username, null);
            var password = Preferences.Shared.GetString(Preferences.Password, null);
            var response = _pmaService.Login(username.Trim(), password.Trim());
            var tokenData = response.GetToken();
            if (tokenData != null)
                _pmaService = new PmaService(tokenData);
        }

        public void VerifyAppointment()
        {
            var autoPointActivated = Preferences.Shared.GetBoolean(Preferences.AutoPointActivated, false);
            if (!autoPointActivated) return;

            Login();
            MakeAppointment();
        }

        private void MakeAppointment()
        {
            if (_pmaService == null) return;

            var appointment = new Appointment();
            appointment.ValidateAppointment();

            switch (appointment.AppointmentType)
            {
                case AppointmentType.Cheguei:
                    StartAppointment();
                    EndAppointment();
                    appointment.AppointmentType = AppointmentType.Intervalo;
                    break;
                case AppointmentType.Intervalo:
                    var intervalAppointed = IntervalAppointment();
                    if (intervalAppointed) appointment.AppointmentType = AppointmentType.Fui;
                    break;
            }

            appointment.SavePreferences();
        }

        private bool IntervalAppointment()
        {
            var dateOfAppointment = GetLastDate();
            if (!dateOfAppointment.HasValue) return false;

            var minimumInterval = Preferences.Shared.GetInt(Preferences.MinimumInterval, 20);

            var differenceTimeSpan = DateTime.Now - dateOfAppointment.Value;
            if (differenceTimeSpan.TotalMinutes < minimumInterval) return false;

            _pmaService.IntervalAppointment(differenceTimeSpan);
            return true;
        }

        private void StartAppointment()
        {
            var currentTimeSpan = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
            _pmaService.StartAppointment(currentTimeSpan);
        }

        private void EndAppointment()
        {
            var dateOfAppointment = GetLastDate();

            if (!dateOfAppointment.HasValue || dateOfAppointment.Value.Date == DateTime.Now.Date) return;

            var currentTimeSpan = new TimeSpan(dateOfAppointment.Value.Hour, dateOfAppointment.Value.Minute, 0);
            _pmaService.DateOfAppointment = dateOfAppointment.Value;
            _pmaService.EndAppointment(currentTimeSpan);
        }

        private static DateTime? GetLastDate()
        {
            var lastNotification = Preferences.Shared.GetLong(Preferences.LastNotification, 0);
            if (lastNotification <= 0) return null;

            return new DateTime(lastNotification);
        }

        public void SaveLastDate()
        {
            var prefEditor = Preferences.Shared.Edit();
            prefEditor.PutLong(Preferences.LastNotification, DateTime.Now.Ticks);
            prefEditor.Commit();
        }
    }
}