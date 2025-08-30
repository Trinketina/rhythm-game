using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public partial class BeatmapParser : Node
{
    [Export] Beatmaps beatmaps_resource;
    [Export] PackedScene song_element;

    [ExportGroup("Temp")]
    [Export] AudioStreamPlayer audio_player;
    [Export] PackedScene gameplay_scene;
    string beatmaps_directory = "user://beatmaps";
    FileAccess current_file;
    private static readonly int[] empty_beat_value = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    public override void _Ready()
    {
        //ImportZipBeatmap("C:\\Users\\krist\\Downloads\\Kessoku Band - That Band (あのバンド) (HaloMillennium).zip");
        InitializeCurrentBeatmaps();
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
            directory.ChangeDir(beatmaps_directory);
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
            file.Close();
        }
        reader.Close();
        GD.Print("done_importing");

        InitializeSong(directory.GetCurrentDir().PathJoin(files[0]).GetBaseDir());
    }
    #endregion

    public async void InitializeCurrentBeatmaps()
    {
        DirAccess directory = DirAccess.Open(beatmaps_directory);
        var dirs = directory.GetDirectories();
        foreach (var dir in dirs)
        {
            InitializeSong(directory.GetCurrentDir().PathJoin(dir));
        }

        //TEMP:: plays a random song on startup
        Random rand = new();
        int song_at = rand.Next(beatmaps_resource.BeatmapsData.Count);
        Beatmaps.Data song = beatmaps_resource.BeatmapsData.ElementAt(song_at).Value;
        audio_player.Stream = song.song_audio;
        audio_player.Play();

        beatmaps_resource.SelectedSong = song.song_data["name"];
        //TEMP:: after a few seconds jumps to the gameplay scene, for testing
        await ToSignal(GetTree().CreateTimer(2), "timeout");
        GetTree().ChangeSceneToPacked(gameplay_scene);
    }
    public void InitializeSong(string song_filepath)
    {
        DirAccess directory = DirAccess.Open(song_filepath);
        var files = directory.GetFiles();

        string song_name = null;
        Dictionary<string, string> song_data = null;
        Image song_art = null;
        AudioStream song_audio = null;
        VideoStream song_video = null;
        string raw_chart_text = null;

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
                    if (key_value.Length > 2) //in case use of BBCode or HTML
                    {
                        for (int i = 2; i < key_value.Length; i++)
                        {
                            key_value[1] += "=" + key_value[i];
                        }
                    }
                    song_data.Add(key_value[0].Trim(), key_value[1].Trim());
                }
                song_name = song_data["name"];

                if (beatmaps_resource.BeatmapsData.ContainsKey(song_name))
                {
                    GD.Print("Song Previously Added");
                    return;
                }
            }
            else if (filepath.Contains("song"))
            {
                song_audio = new();
                if (filepath.Contains(".ogg"))
                {
                    song_audio = AudioStreamOggVorbis.LoadFromFile(directory.GetCurrentDir().PathJoin(filepath));
                }
                else if (filepath.Contains(".mp3"))
                {
                    song_audio = AudioStreamMP3.LoadFromFile(directory.GetCurrentDir().PathJoin(filepath));
                }
                else if (filepath.Contains(".wav"))
                {
                    song_audio = AudioStreamWav.LoadFromFile(directory.GetCurrentDir().PathJoin(filepath));
                }
                else
                {
                    GD.PrintErr("Unsupported audio format");
                    return;
                    /*song_audio = ResourceImporterOggVorbis.LoadFromFile(directory.GetCurrentDir().PathJoin(filepath))*/
                }
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
                raw_chart_text = file.GetAsText();
            }
        }        
        if (song_name == null || song_audio == null || raw_chart_text == null)
        {
            
            //GD.Print(song_name + song_audio + chart_data);
            GD.PrintErr("failed to import song");
            return;
        }
        var chart_data = ParseChart(raw_chart_text);
        if (song_video == null)
        {
            Beatmaps.Data data = new(song_data, song_art, song_audio, chart_data);
            beatmaps_resource.BeatmapsData.Add(song_name, data);
        }
        else
        {
            Beatmaps.Data data = new(song_data, song_art, song_audio, song_video, chart_data);
            beatmaps_resource.BeatmapsData.Add(song_name, data);
        }
        AddSongToList(song_name);
    }
    public static Dictionary<int, int[]> ParseChart(string raw_chart_text)
    {
        bool note_section = false;
        Dictionary<int, int[]> beats_dictionary = new(); //position in ticks | array of note lengths for their positions

        foreach (var line in raw_chart_text.Split("\n"))
        {
            if (line.Contains("[ExpertSingle]"))
            {
                note_section = true;
                continue;
            }
            if (note_section)
            {
                if (line.Trim().StartsWith("[")) //beatmap parsed
                {
                    break;
                }
                if (line.Contains(" = N "))
                {
                    string[] key_value = line.Split(" = N ");
                    int tick = int.Parse(key_value[0]);
                    string[] note_values = key_value[1].Trim().Split(" ");
                    int note_position = int.Parse(note_values[0]);
                    int note_length = int.Parse(note_values[1]);
                    if (!beats_dictionary.ContainsKey(tick))
                    {
                        int[] blank_values = new int[empty_beat_value.Length];
                        empty_beat_value.CopyTo(blank_values,0);
                        beats_dictionary.Add(tick, blank_values);
                    }
                    if (note_length == 0) //add to beatmap
                    {
                        beats_dictionary[tick][note_position] = 1;
                    }
                    else
                    {
                        beats_dictionary[tick][note_position] = note_length;
                    }
                    GD.Print(tick + " =n " + note_position + " " + note_length );
                }
            }
        }
        GD.Print(beats_dictionary.ToString());
        return beats_dictionary;
    }
    public void AddSongToList(string song_name)
    {
        var beatmap_data = beatmaps_resource.BeatmapsData[song_name];

        string name = beatmap_data.song_data["name"];
        string artist = beatmap_data.song_data["artist"];
        string charter = beatmap_data.song_data["charter"];


        SongElement song = song_element.Instantiate<SongElement>();
        AddChild(song);
        song.Initialize(beatmap_data.song_art, name, artist, charter);
        //song.Position = Vector2.Zero;
    }
}
