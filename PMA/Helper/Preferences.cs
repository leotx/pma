using System.Reflection;
using Android.App;
using Android.Content;

namespace PMA.Helper
{
    public static class Preferences
    {
        public const string Username = "USERNAME";
        public const string Password = "PASSWORD";
        public const string TypeOfAppointment = "TYPE_APPOINTMENT";
        public const string DateOfAppointment = "DATE_APPOINTMENT";
        public const string AutoPointActivated = "AUTO_POINT";
        public const string LastNotification = "LAST_NOTIFICATION";
        public const string IntervalTime = "INTERVAL_TIME";
        public static ISharedPreferences Shared { get; private set; }

        static Preferences()
        {
            Shared = Application.Context.GetSharedPreferences(Assembly.GetExecutingAssembly().GetName().Name, FileCreationMode.Private);
        }
    }
}