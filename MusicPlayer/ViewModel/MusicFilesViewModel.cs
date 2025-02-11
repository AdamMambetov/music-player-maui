﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MusicPlayer.ViewModel;

public partial class MusicFilesViewModel : ObservableObject
{
    public MusicFilesViewModel()
    {
        Items = new ObservableCollection<string>();
    }

    [ObservableProperty]
    ObservableCollection<string> items;

    public void RescanMusic()
    {
        var musicPath = Preferences.Get("music_player.music_note_path", "");
        if (!Path.Exists(musicPath))
            return;

        Items.Clear();
        foreach (var item in Directory.GetFiles(musicPath))
            Items.Add(Path.GetFileName(item));
    }

    [RelayCommand]
    async Task Tap(string s)
    {
        await Shell.Current.GoToAsync($"{nameof(MusicFileInfoPage)}?FileName={s}");
    }
}
