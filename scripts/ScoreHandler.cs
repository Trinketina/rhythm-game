using Godot;
using System;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;

public partial class ScoreHandler : Control
{
    [ExportGroup("Node Connections")]
    [Export] Label score_label;
    [Export] Label value_label;
    [Export] Label combo;

    [ExportGroup("Score Modifiers")]
    [Export] float held_multipler_seconds = .5f;


    //[Signal()] public delegate void HitEventHandler(string hit_value);

    public int Full_Score { get; private set; }
    public int Current_Combo { get; private set; }
    public int Max_Combo { get; private set; }

    public List<int> held_indexes = new();
    public override void _Ready()
    {
        base._Ready();
    }

    public void OnBeatmapHitEnded(int score_multiplier, int hit_value)
    {
        if (hit_value > 0)
        {
            Current_Combo++;
            Full_Score += score_multiplier * Current_Combo;
            UpdateScoreText();
            UpdateComboText();
            UpdateHitText(hit_value);
        }
    }
    public void OnBeatmapHoldStarted(int index)
    {
        held_indexes.Add(index);
        HoldingNote(index);
    }
    public void OnBeatmapHoldEnded(int index)
    {
        held_indexes.Remove(index);
    }

    private async void HoldingNote(int index)
    {
        GD.Print("holding holding note");
        while (held_indexes.Contains(index))
        {
            GD.Print("true true");
            await ToSignal(GetTree().CreateTimer(held_multipler_seconds), "timeout");
            Current_Combo++;
            Full_Score += Current_Combo;
            UpdateComboText();
            UpdateScoreText();
        }
    }

    private void UpdateHitText(int hit_value)
    {
        switch (hit_value)
        {
            case 0:
                Current_Combo = 0;
                value_label.Text = "MISS";
                break;
            case 1:
                Current_Combo++;
                value_label.Text = "Okay...";
                break;
            case 2:
                Current_Combo++;
                value_label.Text = "Good!";
                break;
            case 3:
                Current_Combo++;
                value_label.Text = "PERFECT";
                break;
        }
    }
    private void UpdateScoreText()
    {
        score_label.Text = "Score: " + Full_Score;
    }
    private void UpdateComboText()
    {
        combo.Text = "Combo: " + Current_Combo;
    }
}
