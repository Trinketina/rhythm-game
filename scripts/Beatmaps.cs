using Godot;
using System.Collections.Generic;

public partial class Beatmaps : Resource
{
    public Dictionary<string, Data> BeatmapsData = new();
    //int[][] beats; //array of time positions, array of activated notes in each beat


    public class Data
    {
        public readonly Dictionary<string, string> song_data;
        public readonly Image song_art;
        public readonly AudioStream song_audio;
        public readonly VideoStream song_video;

        public readonly string chart_data; //TODO:: separate by difficulty and such

        public Data(Dictionary<string, string> _song_data, Image _song_art, AudioStream _song_audio, VideoStream _song_video, string _chart_data)
        {
            song_data = _song_data;
            song_art = _song_art;

            song_audio = _song_audio;
            song_video = _song_video;

            chart_data = _chart_data;
        }
        public Data(Dictionary<string, string> _song_data, Image _song_art, AudioStream _song_audio, string _chart_data)
        {
            song_data = _song_data;
            song_art = _song_art;
            song_audio = _song_audio;

            chart_data = _chart_data;
        }
    }
}

