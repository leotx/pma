using Android.App;
using Android.Content;
using Android.Support.V4.App;

namespace PMA
{
    public class Notification
    {
        public Notification()
        {
        }

        public void Notify()
        {
            var intent = new Intent(Application.Context, typeof(LoginActivity));

            var pendingIntent =
                PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.OneShot);

            var builder = new NotificationCompat.Builder(Application.Context)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent)
                .SetContentTitle("Você está na Dextra!")
                .SetSmallIcon(Resource.Drawable.logo_dextra)
                .SetContentText("Não esqueça de apontar! :)");

            var notificationManager = (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);
            notificationManager.Notify(0, builder.Build());
        }
    }
}