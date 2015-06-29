using Android.Content;
using Android.App;
using PMA.Notification;

namespace PMA.Helper
{
    [BroadcastReceiver]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class BroadcastBootCompleted : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == Intent.ActionBootCompleted)
            {
                context.ApplicationContext.StartService(new Intent(context, typeof(NotificationService)));
            }
        }
    }
}