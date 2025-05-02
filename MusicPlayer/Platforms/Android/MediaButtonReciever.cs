using Android.Content;
using MusicPlayer.ViewModel;

namespace MusicPlayer.Platforms.Android;

[BroadcastReceiver(Enabled = true, Exported = false)]
public class MediaButtonReceiver : BroadcastReceiver
{
    public override void OnReceive(Context context, Intent intent)
    {
        var action = intent.Action;
        var player = MauiApplication.Current.Services.GetService<MusicFileInfoViewModel>();

        switch (action)
        {
            case "PLAY":
                player.PlayCommand.Execute(null);
                break;
            case "NEXT":
                player.NextCommand.Execute(null);
                break;
            case "PREVIOUS":
                player.PrevCommand.Execute(null);
                break;
        }
    }
}
