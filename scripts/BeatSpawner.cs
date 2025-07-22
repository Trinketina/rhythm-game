using Godot;
using System;
using System.Collections.Generic;

public partial class BeatSpawner : Node
{
    [Export] PackedScene beat;

    [ExportGroup("Temporary")]
    [Export] Label temp_label;

    List<int[]> beat_types;


    [Signal()] public delegate void BeatSpawnedEventHandler();
    public override void _Ready()
    {
        beat_types = new List<int[]>();
        beat_types.Add([0,1,2,3]);
        beat_types.Add([1, 2]);
        beat_types.Add([0, 3]);
        beat_types.Add([0]);
        beat_types.Add([1]);
        beat_types.Add([2]);
        beat_types.Add([3]);
        TempSpawnConstantNotes();
        base._Ready();
    }
    public async void TempSpawnConstantNotes()
    {
        while (true)
        {
            SpawnBeat();
            await ToSignal(GetTree().CreateTimer(3), "timeout");
        }
    }
    public void SpawnBeat()
    {
        Beat beat_node = beat.Instantiate<Beat>();
        this.AddChild(beat_node);
        GD.Print("spawn beat");
        int rand_beat_type = new Random().Next(beat_types.Count - 1);
        beat_node.StartBeat(beat_types[rand_beat_type], temp_label);

        EmitSignal(SignalName.BeatSpawned);
    }
}
