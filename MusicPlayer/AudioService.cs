using Plugin.Maui.Audio;
using System.Diagnostics;

namespace MusicPlayer;

public class AudioService(IAudioManager audioManager)
{
    private readonly IAudioManager _audioManager = audioManager;
    private IAudioPlayer? _player;

    public bool IsAudioLoaded { get => GetDuration() != 0; }
    public bool IsAudioPlayerValid { get => _player != null; }

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
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка воспроизведения: {ex.Message}");
            throw;
        }
    }


    public void Stop() => _player?.Stop();
    public void Pause() => _player?.Pause();
    public void Resume() => _player?.Play();

    public void BindToEndedEvent(EventHandler func)
    {
        if (_player == null)
            return;
        _player.PlaybackEnded += func;
    }

    public void UnbindFromEndedEvent(EventHandler func)
    {
        if (_player == null)
            return;
        _player.PlaybackEnded -= func;
    }

    public bool GetIsPlaying()
    {
        return _player != null && _player.IsPlaying;
    }

    public bool GetIsRepeat()
    {
        return _player != null && _player.Loop;
    }

    public void SetIsRepeat(bool value)
    {
        if (_player == null)
            return;
        _player.Loop = value;
    }

    public double GetDuration() => _player == null ? 0.0 : _player.Duration;

    public double GetCurrentPosition() => _player == null ? 0.0 : _player.CurrentPosition;

    public void Seek(double position)
    {
        if (_player == null)
            return;
        if (!_player.CanSeek)
            return;
        _player.Seek(position);
    }

    public void Dispose()
    {
        if (_player == null)
            return;
        _player.Stop();
        _player.Dispose();
        _player = null;
    }

    internal void UnbindFromEndedEvent(Action<object?, EventArgs> onAudioEnded)
    {
        throw new NotImplementedException();
    }
}
