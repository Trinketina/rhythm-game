using Godot;
using System;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;

public partial class ScoreHandler : Control
{
    [ExportGroup("Node Connections")]
    [Export] Label score_label;
    [Export] TempLabel value_label;
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
        }
        else
        {
            Current_Combo = 0;
        }
        UpdateScoreText();
        UpdateHitText(hit_value);
        UpdateComboText();
    }
    public void OnBeatmapHoldStarted(int index, Vector2 global_position)
    {
        held_indexes.Add(index);
        HoldingNote(index);
    }
    public void OnBeatmapHoldEnded(int index, Vector2 global_position)
    {
        held_indexes.Remove(index);
    }

    private async void HoldingNote(int index)
    {
        while (held_indexes.Contains(index))
        {
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
                value_label.ClearAfter("MISS", 1.5f);
                break;
            case 1:
                Current_Combo++;
                value_label.ClearAfter("Okay...", 1.5f);
                break;
            case 2:
                Current_Combo++;
                value_label.ClearAfter("Good!", 1.5f);
                break;
            case 3:
                Current_Combo++;
                value_label.ClearAfter("PERFECT", 1.5f);
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
