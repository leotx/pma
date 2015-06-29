using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;

namespace PMA.Helper.Notification
{
    [Service]
    public class NotificationService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            throw new System.NotImplementedException();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            new Task(() =>
            {
                var networkVerify = new NetworkVerify();
                networkVerify.Start();
            }).Start();

            return StartCommandResult.Sticky;
        }
    }
}