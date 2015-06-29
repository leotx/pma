using Android.App;
using Android.Content;
using PMA.Helper.Notification;

namespace PMA.Helper.Broadcast
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