using Godot;
using System.Security.Cryptography;
using System;
using System.Reflection.Metadata;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

public partial class Beat : Node2D
{
    Label temporary_label;
    //[Export] Model model;
    [Export] Sprite2D[] note_symbols;
    [Export] Area2D collider;

    int current_score = -1;
    List<int> note_scores;
    [ExportGroup("Temporary")]
    [Export] int[] temp_indexes;
    [Export] int speed;
    List<int> TEMP_note_indexes;

    public override void _Ready()
    {
        collider.AreaEntered += EnterCollider;
        collider.AreaExited += ExitCollider;
        Input.ArrowKeyPressed += PressNote;

        /*TEMP_note_indexes = temp_indexes.ToList();

        StartBeat(TEMP_note_indexes.ToArray());*/
        base._Ready();
    }
    public override void _Process(double delta)
    {
        this.Position += Vector2.Down * (speed * (float)delta);
        base._Process(delta);
    }

    public void StartBeat(int[] note_indexes, Label temp_label)
    {
        temporary_label = temp_label;
        TEMP_note_indexes = note_indexes.ToList();
        Position = Vector2.Zero;
        foreach (int note_index in note_indexes) {
            note_symbols[note_index].Show();
        }
        note_scores = new();
    }

    public void PressNote(int note_index)
    {
        if (current_score >= 0)
        {
            if (TEMP_note_indexes.Contains(note_index) && current_score > 0)
            {
                TEMP_note_indexes.Remove(note_index);
                note_scores.Add(current_score);
                GD.Print("Key Pressed, score:" + current_score);
                note_symbols[note_index].Hide();
                if (TEMP_note_indexes.Count == 0)
                {
                    EndBeat(true);
                }
            }
            else
            {
                // NOTE WAS MISSED
                GD.Print("Key Missed");
                EndBeat(false);
            }
        }
    }

    public void EndBeat(bool success)
    {
        collider.AreaEntered -= EnterCollider;
        collider.AreaExited -= ExitCollider;
        Input.ArrowKeyPressed -= PressNote;
        /*foreach (var note in note_symbols)
        {
            note.Hide();
        }*/
        // send score to model
        GD.Print("end_beat");
        if (success)
        {
            int full_score = note_scores.Sum();
            // 4
            switch (full_score / note_scores.Count)
            {
                case 1:
                    temporary_label.Text = "OKAY";
                    break;
                case 2:
                    temporary_label.Text = "GOOD";
                    break;
                case 3: 
                    temporary_label.Text = "PERFECT";
                    break;
            }
        }
        else
        {
            temporary_label.Text = "MISS";
        }
        QueueFree();
    }

    public void EnterCollider(Area2D area)
    {
        GD.Print("enter");

        current_score += 1;
    }
    public void ExitCollider(Area2D area)
    {
        GD.Print("exit");
        current_score -= 1;
        if (current_score < 0)
        {
            EndBeat(false);
        }
    }
}