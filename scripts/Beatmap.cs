using Godot;
using Godot.Collections;

public partial class Beatmap : Node2D
{
    [Signal()] public delegate void BeatEndedEventHandler(int score, int hit_value);


    [Export] float fall_rate;
    [Export] PackedScene beat;

    bool running = false;

    [ExportGroup("temporary")]
    [Export] Vector2[] default_beat_positions;
    [Export] Array<int[]> default_beat_notes;

    public override void _Ready()
    {
        InitDefaultBeats();
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


    public void InitializeBeats(BeatValues[] beats)
    {
        foreach (var beat in beats)
        {
            SpawnBeat(beat);
        }

        //TODO:: make a proper start sequence
        running = true;
    }
    public void SpawnBeat(BeatValues beat_values)
    {
        Beat beat_script = beat.Instantiate<Beat>();
        AddChild(beat_script);
        beat_script.StartBeat(beat_values);

        beat_script.OnEndNote += EndNote;
    }
    private void EndNote(int score, int hit_value)
    {
        EmitSignal(SignalName.BeatEnded);
    }

    private void InitDefaultBeats()
    {
        BeatValues[] default_beats = new BeatValues[default_beat_positions.Length];
        for(int i = 0; i < default_beat_positions.Length; i++)
        {
            default_beats[i] = new BeatValues(default_beat_notes[i], default_beat_positions[i]);
            
        }
        InitializeBeats(default_beats);
    }
}
