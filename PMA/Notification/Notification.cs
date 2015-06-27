using System;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Support.V4.App;

namespace PMA.Notification
{
    public static class Notification
    {
        public static void Notify(string contentTitle)
        {
            var intent = new Intent(Application.Context, typeof(LoginActivity));

            var pendingIntent =
                PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.OneShot);

            var builder = new NotificationCompat.Builder(Application.Context)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent)
                .SetContentTitle(contentTitle)
                .SetSmallIcon(Resource.Drawable.logo_dextra)
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Alarm))
                .SetDefaults((int) (NotificationDefaults.Sound | NotificationDefaults.Vibrate))
                .SetContentText("Não esqueça de apontar! :)");

            var notificationManager = (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);
            notificationManager.Notify(Guid.NewGuid().GetHashCode(), builder.Build());
        }
    }
}