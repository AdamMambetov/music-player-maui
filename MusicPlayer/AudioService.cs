using Plugin.Maui.Audio;
using System.Diagnostics;

namespace MusicPlayer;

public class AudioService
{
    private readonly IAudioManager _audioManager;
    private IAudioPlayer? _player;

    public AudioService(IAudioManager audioManager)
    {
        this._audioManager = audioManager;
    }

    public void PlayFromStream(Stream audioStream)
    {
        try
        {
            if (_player != null)
            {
                _player.Stop();
                _player.Dispose();
            }

            _player = _audioManager.CreatePlayer(audioStream);
            _player.Play();
        }
        catch (System.Exception ex)
        {
            Debug.WriteLine($"Ошибка воспроизведения: {ex.Message}");
            throw;
        }
    }


    public void Stop()
    {
        _player?.Stop();
    }

    public void Pause()
    {
        _player?.Pause();
    }

    public void Resume()
    {
        _player?.Play();
    }

    public bool GetIsPlaying()
    {
        return _player != null && _player.IsPlaying;
    }

    public bool GetIsRepeat()
    {
        return _player != null && _player.Loop;
    }

    public void SetIsRepeat(in bool value)
    {
        if (_player == null)
            return;
        _player.Loop = value;
    }

    public double GetDuration()
    {
        if (_player == null)
            return 0.0;
        return _player.Duration;
    }
}
