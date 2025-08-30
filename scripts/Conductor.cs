using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class Conductor : AudioStreamPlayer
{
    [Export] Beatmaps beatmaps_resource;
    List<int> bpm_change_positions;
    public float current_song_playtime { get; private set; } = 0;

    public float bpm_start_tick = 0;
    public float bpm_start_second = 0;
    public float bpm = 160;
    public float resolution = 192;
    //TODO:: once music playback is implemented, ensure audio latency is accounteed for
    // to allow for scripts like Beatmap to adjust positioning data accordingly
    public override void _Ready()
    {
        bpm_change_positions = beatmaps_resource.SelectedSong.bpm_data.Keys.ToList();
        base._Ready();
    }
    public override void _Process(double delta)
    {
        //GetSongPlaytime();
        if (bpm_change_positions.Count > 0 && TimeToTicks(GetSongPlaytime()) >= bpm_change_positions.First())
        {
            var new_bpm_position = bpm_change_positions.First();
            bpm_change_positions.Remove(new_bpm_position);
            UpdateBPM(new_bpm_position, beatmaps_resource.SelectedSong.bpm_data[new_bpm_position]);
        }
        else
        {
            TimeToTicks(GetSongPlaytime());
        }

        base._Process(delta);
    }
    public float GetSongPlaytime()
    {
        current_song_playtime = GetPlaybackPosition() 
            + (float)AudioServer.GetTimeSinceLastMix() 
            - (float)AudioServer.GetOutputLatency();

        return current_song_playtime;
    }
    public float GetCurrentAudioOffset()
    {
        return (float)AudioServer.GetTimeSinceLastMix() - (float)AudioServer.GetOutputLatency();
    }
    public void StartSong()
    {
        Stream = beatmaps_resource.SelectedSong.song_audio;
        Play();
    }

    public void UpdateBPM(int starting_tick, float new_bpm)
    {
        bpm_start_tick = starting_tick;
        bpm = new_bpm;

        bpm_start_second = TicksToTime(bpm_start_tick);
    }

    public float TimeToTicks(float seconds)
    {
        float seconds_per_beat = 60 / bpm;
        float delta_seconds = seconds - bpm_start_second;
        //float delta_beats = delta_seconds / seconds_per_beat;
        //float delta_ticks = delta_beats * resolution;

        return (delta_seconds / seconds_per_beat * resolution) + bpm_start_tick;
    }
    public float TicksToTime(float ticks)
    {
        float seconds_per_beat = 60 / bpm;
        float delta_ticks = ticks - bpm_start_tick;
        float delta_beats = delta_ticks / seconds_per_beat;
        float delta_seconds = delta_beats * resolution;


        // 

        return (seconds_per_beat * delta_ticks / resolution) + bpm_start_second;
    }
}
