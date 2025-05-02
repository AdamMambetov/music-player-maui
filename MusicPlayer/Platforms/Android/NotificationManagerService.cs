using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Media.Session;

namespace MusicPlayer.Platforms.Android;

public class NotificationManagerService : INotificationManagerService
{
    const string channelId = "default";
    const string channelName = "Default";
    const string channelDescription = "The default channel for notifications.";

    public const string TitleKey = "title";
    public const string MessageKey = "message";

    bool channelInitialized = false;
    int messageId = 0;
    int pendingIntentId = 0;

    NotificationManagerCompat compatManager;

    public event EventHandler NotificationReceived;

    public static NotificationManagerService Instance { get; private set; }

    public NotificationManagerService()
    {
        if (Instance == null)
        {
            CreateNotificationChannel();
            compatManager = NotificationManagerCompat.From(Platform.AppContext);
            Instance = this;
        }
    }

    public void SendNotification(string title, string message, DateTime? notifyTime = null)
    {
        if (!channelInitialized)
        {
            CreateNotificationChannel();
        }

        if (notifyTime != null)
        {
            Intent intent = new Intent(Platform.AppContext, typeof(AlarmHandler));
            intent.PutExtra(TitleKey, title);
            intent.PutExtra(MessageKey, message);
            intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

            var pendingIntentFlags = (Build.VERSION.SdkInt >= BuildVersionCodes.S)
                ? PendingIntentFlags.CancelCurrent | PendingIntentFlags.Immutable
                : PendingIntentFlags.CancelCurrent;

            PendingIntent pendingIntent = PendingIntent.GetBroadcast(Platform.AppContext, pendingIntentId++, intent, pendingIntentFlags);
            long triggerTime = GetNotifyTime(notifyTime.Value);
            AlarmManager alarmManager = Platform.AppContext.GetSystemService(Context.AlarmService) as AlarmManager;
            alarmManager.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
        }
        else
        {
            Show(title, message);
        }
    }

    public void ReceiveNotification(string title, string message)
    {
        var args = new NotificationEventArgs()
        {
            Title = title,
            Message = message,
        };
        NotificationReceived?.Invoke(null, args);
    }

    public void Show(string title, string message)
    {
        var context = Platform.AppContext;
        Intent intent = new Intent(context, typeof(MainActivity));
        intent.PutExtra(TitleKey, title);
        intent.PutExtra(MessageKey, message);
        intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

        var pendingIntentFlags = (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            ? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
            : PendingIntentFlags.UpdateCurrent;

        // Intent для кнопок
        var playIntent = new Intent(context, typeof(MediaButtonReceiver));
        playIntent.SetAction("PLAY");
        var playPendingIntent = PendingIntent.GetBroadcast(context, 0, playIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

        var previousIntent = new Intent(context, typeof(MediaButtonReceiver));
        previousIntent.SetAction("PREVIOUS");
        var previousPendingIntent = PendingIntent.GetBroadcast(context, 1, previousIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

        var nextIntent = new Intent(context, typeof(MediaButtonReceiver));
        nextIntent.SetAction("NEXT");
        var nextPendingIntent = PendingIntent.GetBroadcast(context, 2, nextIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);


        PendingIntent pendingIntent = PendingIntent.GetActivity(Platform.AppContext, pendingIntentId++, intent, pendingIntentFlags);
        NotificationCompat.Builder builder = new NotificationCompat.Builder(Platform.AppContext, channelId)
            .SetContentIntent(pendingIntent)
            .SetContentTitle(title)
            .SetContentText(message)
            .SetLargeIcon(BitmapFactory.DecodeResource(Platform.AppContext.Resources, Resource.Drawable.dotnet_bot))
            .SetStyle(new AndroidX.Media.App.NotificationCompat.MediaStyle().SetShowActionsInCompactView(0, 1, 2))
            .AddAction(Resource.Drawable.dotnet_bot, "Previous", previousPendingIntent)
            .AddAction(Resource.Drawable.dotnet_bot, "Pause", playPendingIntent)
            .AddAction(Resource.Drawable.dotnet_bot, "Next", nextPendingIntent)
            .SetOngoing(true)
            .SetAutoCancel(false)
            .SetOnlyAlertOnce(true)
            .SetVisibility(NotificationCompat.VisibilityPublic);

        Notification notification = builder.Build();
        compatManager.Notify(messageId++, notification);
    }

    public void DeleteNotification(int id)
    {
        Intent intent = new Intent(Platform.AppContext, typeof(AlarmHandler));

        var pendingIntentFlags = (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            ? PendingIntentFlags.CancelCurrent | PendingIntentFlags.Immutable
            : PendingIntentFlags.CancelCurrent;

        PendingIntent pendingIntent = PendingIntent.GetBroadcast(Platform.AppContext, id, intent, pendingIntentFlags);
        AlarmManager alarmManager = Platform.AppContext.GetSystemService(Context.AlarmService) as AlarmManager;
        alarmManager.Cancel(pendingIntent);
    }

    void CreateNotificationChannel()
    {
        // Create the notification channel, but only on API 26+.
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channelNameJava = new Java.Lang.String(channelName);
            var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Default)
            {
                Description = channelDescription
            };
            // Register the channel
            NotificationManager manager = (NotificationManager)Platform.AppContext.GetSystemService(Context.NotificationService);
            manager.CreateNotificationChannel(channel);
            channelInitialized = true;
        }
    }

    long GetNotifyTime(DateTime notifyTime)
    {
        DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
        double epochDiff = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;
        long utcAlarmTime = utcTime.AddSeconds(-epochDiff).Ticks / 10000;
        return utcAlarmTime; // milliseconds
    }
}
