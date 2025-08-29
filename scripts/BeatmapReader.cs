using Godot;
using System;
using System.Collections.Generic;


public partial class BeatmapReader : Node
{
    [Export] Beatmaps beatmaps;
    [Export] PackedScene song_element;
    string beatmaps_directory = "user://beatmaps";
    FileAccess current_file;

    public override void _Ready()
    {
        //ImportZipBeatmap("C:\\Users\\krist\\Downloads\\Jamie Paige - BIRDBRAIN (w／OK Glass) (Downbad).zip");
        ImportSong("user://beatmaps/Jamie Paige - BIRDBRAIN (w／OK Glass) (Downbad)/");
    }
    #region import
    public void ImportZipBeatmap(string zip_file_path)
    {
        ZipReader reader = new();
        //current_file = FileAccess.Open(filename, FileAccess.ModeFlags.Read);
        var result = reader.Open(zip_file_path);
        if (result != Error.Ok)
        {
            GD.PrintErr(result);
            return;
        }
        //string beatmap_name = zip_file_path.Substring(zip_file_path.LastIndexOf("\\")).Replace(".zip", "");

        DirAccess directory = DirAccess.Open(beatmaps_directory);
        if (directory == null)
        {
            directory = DirAccess.Open("user://");
            directory.MakeDirRecursive(beatmaps_directory);
            directory = DirAccess.Open(beatmaps_directory);
        }
        //GD.Print(OS.GetDataDir());
        /*directory.MakeDirRecursive(directory.GetCurrentDir().PathJoin(beatmap_name));
        directory = DirAccess.Open(directory.GetCurrentDir().PathJoin(beatmap_name));*/

        var files = reader.GetFiles();

        foreach (var filepath in files) 
        {
            if (filepath.EndsWith("/"))
            {
                directory.MakeDirRecursive(filepath);
                continue;
            }

            directory.MakeDirRecursive(directory.GetCurrentDir().PathJoin(filepath).GetBaseDir());
            var file = FileAccess.Open(directory.GetCurrentDir().PathJoin(filepath), FileAccess.ModeFlags.Write);
            var buffer = reader.ReadFile(filepath);
            file.StoreBuffer(buffer);

        }

        string song_filepath = directory.GetCurrentDir().PathJoin(files[0]).GetBaseDir();
        GD.Print(song_filepath);
        ImportSong(song_filepath);

        

        GD.Print("done_importing");
    }
    #endregion

    public void ImportSong(string song_filepath)
    {
        DirAccess directory = DirAccess.Open(song_filepath);
        var files = directory.GetFiles();

        string song_name = null;
        Dictionary<string, string> song_data = null;
        Image song_art = null;
        AudioStream song_audio = null;
        VideoStream song_video = null;
        string chart_data = null;

        foreach (var filepath in files)
        {
            if (filepath.Contains("song.ini"))
            {
                var file = FileAccess.Open(directory.GetCurrentDir().PathJoin(filepath), FileAccess.ModeFlags.Read);
                string[] data_text = file.GetAsText(true).Split("\n");
                song_data = new();
                foreach (var line in data_text)
                {
                    if (!line.Contains("=")) { continue; }
                    string[] key_value = line.Split("=");
                    song_data.Add(key_value[0].Trim(), key_value[1].Trim());
                }
                song_name = song_data["name"];
            }
            else if (filepath.Contains("song"))
            {
                song_audio = new();
                //song_audio = AudioStreamOggVorbis.LoadFromFile(directory.GetCurrentDir().PathJoin(filepath));
            }
            else if (filepath.Contains("video"))
            {
                song_video = new();
                song_video.File = directory.GetCurrentDir().PathJoin(filepath);
            }
            else if (filepath.Contains("album"))
            {
                song_art = new();
                GD.Print(song_art.Load(directory.GetCurrentDir().PathJoin(filepath)));
            }
            else if (filepath.Contains(".chart"))
            {
                var file = FileAccess.Open(directory.GetCurrentDir().PathJoin(filepath), FileAccess.ModeFlags.Read);
                chart_data = file.GetAsText();
            }
        }        
        if (song_name == null || /*song_audio == null ||*/ chart_data == null)
        {
            GD.PrintErr("failed to import song");
            return;
        }
        if (song_video == null)
        {
            Beatmaps.Data data = new(song_data, song_art, song_audio, chart_data);
            beatmaps.BeatmapsData.Add(song_name, data);
        }
        else
        {
            Beatmaps.Data data = new(song_data, song_art, song_audio, song_video, chart_data);
            beatmaps.BeatmapsData.Add(song_name, data);
        }
        AddSongToList(song_name);
    }
    public void AddSongToList(string song_name)
    {
        var beatmap_data = beatmaps.BeatmapsData[song_name];

        string name = beatmap_data.song_data["name"];
        string artist = beatmap_data.song_data["artist"];
        string charter = beatmap_data.song_data["charter"];


        SongElement song = song_element.Instantiate<SongElement>();
        AddChild(song);
        song.Initialize(beatmap_data.song_art, name, artist, charter);
        //song.Position = Vector2.Zero;
    }
}
