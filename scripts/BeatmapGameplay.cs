using Godot;
using Godot.Collections;
using System.Collections.Generic;

public partial class BeatmapGameplay : Node2D
{
    [Signal()] public delegate void HitEndedEventHandler(int score_multiplier, int hit_value);
    [Signal()] public delegate void HoldStartedEventHandler(int index, Vector2 tail_global_position);
    [Signal()] public delegate void HoldEndedEventHandler(int index, Vector2 global_position);
    [Signal()] public delegate void HitEventHandler(int index, Vector2 global_position);
    [Export] float fall_rate;
    [Export] PackedScene beat;
    [Export] Beatmaps beatmaps_resource;

    bool running = false;


    [ExportGroup("temporary")]
    [Export] Vector2[] default_beat_positions;
    [Export] Array<int[]> default_beat_notes;

    public override void _Ready()
    {
        //InitDefaultBeats();
        InitializeBeats(beatmaps_resource.BeatmapsData[beatmaps_resource.SelectedSong].chart_data);
        base._Ready();
    }

    public override void _Process(double delta)
    {
        if (running)
        {
            //TODO:: tie to conductor to offset for audio latency
            //TODO:: add accessibility option for custom latency offsets
            Position += Vector2.Down * (fall_rate * (float)delta);
        } 
        base._Process(delta);
    }


    public void InitializeBeats(System.Collections.Generic.Dictionary<int, int[]> beats)
    {
        foreach (var beat in beats)
        {
            BeatValues values = new(beat.Value, (Vector2.Up * beat.Key)); //NOT FINAL
            //GD.Print(((Vector2.Up * beat.Key)).ToString());
            //GD.Print(beat.Value.GetValue(0));
            SpawnBeat(values);
        }

        //TODO:: make a proper start sequence
        running = true;
    }
    public void SpawnBeat(BeatValues beat_values)
    {
        Beat beat_node = beat.Instantiate<Beat>();
        AddChild(beat_node);
        beat_node.StartBeat(beat_values, this);


        /*beat_node.OnEndNote += EndNote;
        beat_node.OnStartHoldNote += StartHold;
        beat_node.OnEndHoldNote += EndHold;*/
        //GD.Print("spawn note");
    }
    public void HitNote(int index, Vector2 global_position)
    {
        EmitSignal(SignalName.Hit, index, global_position);
    }
    public void EndNote(int score_multiplier, int hit_value)
    {
        EmitSignal(SignalName.HitEnded, score_multiplier, hit_value);
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

    private void InitDefaultBeats()
    {
        BeatValues[] default_beats = new BeatValues[default_beat_positions.Length];
        for(int i = 0; i < default_beat_positions.Length; i++)
        {
            default_beats[i] = new BeatValues(default_beat_notes[i], default_beat_positions[i]);
            
        }
        //InitializeBeats(default_beats);
    }
}
