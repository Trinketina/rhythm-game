using Godot;
using System.Buffers;

//using Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class BeatmapGameplay : Node2D
{
    [Signal()] public delegate void HitEndedEventHandler(int score_multiplier, int hit_value);
    [Signal()] public delegate void HoldStartedEventHandler(int index, Vector2 tail_global_position);
    [Signal()] public delegate void HoldEndedEventHandler(int index, Vector2 global_position);
    [Signal()] public delegate void HitEventHandler(int index, Vector2 global_position);
    //[Export] float fall_rate;
    [Export] PackedScene beat;
    [Export] int beat_pool_size = 30;
    [Export] Conductor conductor;
    [Export] Beatmaps beatmaps_resource;

    [ExportGroup("temporary")]
    [Export] Vector2[] default_beat_positions;
    //[Export] Array<int[]> default_beat_notes;

    Dictionary<int, int[]> beats;
    List<int> beat_keys;
    Beat[] beats_pool;
    bool running = false;

    public override void _Ready()
    {
        //InitDefaultBeats();
        InitializeBeats(beatmaps_resource.SelectedSong.chart_data);
        Start();
        base._Ready();
    }

    public override void _Process(double delta)
    {
        if (running)
        {
            //TODO:: tie to conductor to offset for audio latency
            //TODO:: add accessibility option for custom latency offsets
            //Position += Vector2.Down * (conductor.TicksToSeconds((float)delta) /** (float)delta*/);

            //current_time += delta;
            Position = Vector2.Down * (conductor.TimeToTicks(conductor.GetSongPlaytime()));
        } 
        base._Process(delta);
    }
    private async void Start()
    {

        await ToSignal(GetTree().CreateTimer(3), "timeout");
        running = true;
        //current_time = 0;
        conductor.StartSong();
    }

    #region Beat Handling
    public void InitializeBeats(Dictionary<int, int[]> _beats)
    {
        beats = _beats;

        beat_keys = beats.Keys.ToList();
        beat_keys.Sort();

        beats_pool = new Beat[beat_pool_size];

        GD.Print("beat count:" + beats.Count);

        for (int i = 0; i < beat_pool_size; i++)
        {
            BeatValues values = new(beats[beat_keys[i]], (Vector2.Up * beat_keys[i]));
            beats_pool[i] = SpawnBeat(values);

            beat_keys.Remove(i);
        }
        

        //TODO:: make a proper start sequence
        Start();
    }
    public Beat SpawnBeat(BeatValues beat_values)
    {
        Beat beat_node = beat.Instantiate<Beat>();
        AddChild(beat_node);
        beat_node.StartBeat(beat_values, this);


        /*beat_node.OnEndNote += EndNote;
        beat_node.OnStartHoldNote += StartHold;
        beat_node.OnEndHoldNote += EndHold;*/

        return beat_node;
    }
    public void HitNote(int index, Vector2 global_position)
    {
        EmitSignal(SignalName.Hit, index, global_position);
    }
    public void EndNote(Beat beat, int score_multiplier, int hit_value)
    {
        EmitSignal(SignalName.HitEnded, score_multiplier, hit_value);

        if (beat_keys.Count > 0)
        {
            BeatValues values = new(beats[beat_keys[0]], (Vector2.Up * beat_keys[0]));
            beat.StartBeat(values, this);
        }

    }

    public void StartHold(int index, Vector2 global_position)
    {
        GD.Print("start hold");
        EmitSignal(SignalName.HoldStarted, index, global_position);
    }
    public void EndHold(int index, Vector2 global_position)
    {
        GD.Print("end hold");
        EmitSignal(SignalName.HoldEnded, index, global_position);
    }

    /*private void InitDefaultBeats()
    {
        BeatValues[] default_beats = new BeatValues[default_beat_positions.Length];
        for (int i = 0; i < default_beat_positions.Length; i++)
        {
            default_beats[i] = new BeatValues(default_beat_notes[i], default_beat_positions[i]);

        }
        //InitializeBeats(default_beats);
    }*/
    #endregion
}
