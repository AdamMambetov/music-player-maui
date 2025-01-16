using Plugin.Maui.Audio;
using System.Diagnostics;

namespace MusicPlayer;

public class AudioService
{
    private readonly IAudioManager audioManager;
    private IAudioPlayer player;

    public AudioService(IAudioManager audioManager)
    {
        this.audioManager = audioManager;
    }

    public void PlayFromStream(Stream audioStream)
    {
        try
        {
            if (player != null)
            {
                player.Stop();
                player.Dispose();
            }

            player = audioManager.CreatePlayer(audioStream);
            player.Play();
        }
        catch (System.Exception ex)
        {
            Debug.WriteLine($"Ошибка воспроизведения: {ex.Message}");
            throw;
        }
    }


    public void Stop()
    {
        player?.Stop();
    }

    public void Pause()
    {
        player?.Pause();
    }

    public void Resume()
    {
        player?.Play();
    }
}
