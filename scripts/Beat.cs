using Godot;
using System.Security.Cryptography;
using System;
using System.Reflection.Metadata;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

public partial class Beat : Node2D
{
    [Export] TextEdit text;
    [Export] Model model;
    [Export] Sprite2D[] note_symbols;
    [Export] Area2D collider;

    int current_score = -1;
    List<int> note_scores;

    [Export] int[] temp_indexes;
    List<int> TEMP_note_indexes = new() { 0,2 };

    public override void _Ready()
    {
        collider.AreaEntered += EnterCollider;
        collider.AreaExited += ExitCollider;
        Input.ArrowKeyPressed += PressNote;

        TEMP_note_indexes = temp_indexes.ToList();

        StartBeat(TEMP_note_indexes.ToArray());
        base._Ready();
    }
    public override void _Process(double delta)
    {
        this.Position += Vector2.Down * (70f * (float)delta);
        base._Process(delta);
    }

    public void StartBeat(int[] note_indexes)
    {
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
            switch (current_score)
            {
                case 1:
                    text.Text = "OKAY";
                    break;
                case 2:
                    text.Text = "GOOD";
                    break;
                case 3:
                    text.Text = "PERFECT";
                    break;
            }
        }
        else
        {
            text.Text = "MISS";
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