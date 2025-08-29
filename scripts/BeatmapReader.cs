using Godot;
using System.Collections.Generic;


public partial class BeatmapReader : Node
{
    [Export] Beatmaps beatmaps;
    [Export] PackedScene song_element;
    string beatmaps_directory = "user://beatmaps";
    FileAccess current_file;

    public override void _Ready()
    {
        ImportZipBeatmap("C:\\Users\\krist\\Downloads\\Jamie Paige - BIRDBRAIN (w／OK Glass) (Downbad).zip");
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

        Dictionary<string, string> song_data;
        Image song_art;
        AudioStream song_audio;
        VideoStream song_video;
        string chart_data;

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

            if (filepath.Contains("song.ini"))
            {
                string[] data_text = file.GetAsText().Split("\n");
                song_data = new();
                foreach (var line in data_text)
                {
                    if (!line.Contains("=")) { continue; }
                    string[] key_value = line.Split("=");
                    song_data.Add(key_value[0].Trim(), key_value[1].Trim());
                }
            }
            else if (filepath.Contains("song"))
            {
                song_audio = new();
                song_audio = AudioStreamOggVorbis.LoadFromBuffer(buffer);
            }
            else if (filepath.Contains("video"))
            {
                song_video = new();
                song_video.File = filepath;
            }
            else if (filepath.Contains("album"))
            {
                song_art = new();
                song_art.Load(filepath);
            }
            else if (filepath.Contains(".chart"))
            {
                chart_data = file.GetAsText();
            }
        }
        GD.Print("done_importing");
        AddSongToList(directory.GetCurrentDir().PathJoin(files[0].Substring(0, files[0].LastIndexOf("/"))));
    }
    #endregion

    public void InitializeSongs()
    {

    }
    public void AddSongToList(string song_dir)
    {
        DirAccess dir = DirAccess.Open(song_dir);
        var files = dir.GetFiles();
        string art_filepath = null, data_filepath = null;
        foreach (var filepath in files)
        {
            if (data_filepath != null && art_filepath != null)
            {
                break;
            }
            if (filepath.Contains("album") && !filepath.Contains("import"))
            {
                art_filepath = filepath; continue;
            }
            if (filepath.Contains("song.ini"))
            {
                data_filepath = filepath; continue;
            }
        }
        if (art_filepath.Equals(null) || data_filepath.Equals(null))
        {
            GD.PrintErr("failed to find song information");
            return;
        }
        GD.Print(dir.GetCurrentDir().PathJoin(art_filepath));
        //Image art_image = Image.LoadFromFile(dir.GetCurrentDir().PathJoin(art_filepath));
        Image art_image = new();
        art_image.Load(art_filepath);

        /*FileAccess song_data_file = FileAccess.Open(dir.GetCurrentDir().PathJoin(data_filepath), FileAccess.ModeFlags.Read);
        string[] data_text = song_data_file.GetAsText().Split("\n");
        Dictionary<string, string> song_data = new();
        foreach (var line in data_text)
        {
            if (!line.Contains("=")) { continue; }
            string[] key_value = line.Split("=");
            song_data.Add(key_value[0].Trim(), key_value[1].Trim());
        }*/
        string name = song_data["name"];
        string artist = song_data["artist"];
        string charter = song_data["charter"];


        SongElement song = song_element.Instantiate<SongElement>();
        song.Initialize(art_image, name, artist, charter);
    }
}
