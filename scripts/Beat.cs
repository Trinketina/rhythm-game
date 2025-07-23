using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Beat : Node2D
{
    bool ready = false;
    public Action<int, int> OnEndNote; //score, value //TODO:: connect to resource and send its signal instead


    [Export] Sprite2D[] note_symbols;
    [Export] Area2D collider;

    int current_score = -1;
    List<int> note_scores;
    List<int> current_note_indexes;


    public override void _Ready()
    {
        collider.AreaEntered += EnterCollider;
        collider.AreaExited += ExitCollider;
        Input.ArrowKeyPressed += PressNote;

        /*TEMP_note_indexes = temp_indexes.ToList();

        StartBeat(TEMP_note_indexes.ToArray());*/
        base._Ready();
    }
    public void StartBeat(int[] note_indexes, Label temp_label)
    {
        /*temporary_label = temp_label;
        current_note_indexes = note_indexes.ToList();
        Position = Vector2.Zero;
        foreach (int note_index in note_indexes) {
            note_symbols[note_index].Show();
        }
        note_scores = new();*/
    }
    public void StartBeat(BeatValues beat)
    {
        current_note_indexes = beat.note_indexes.ToList();
        Position = beat.position;

        foreach (int note_index in beat.note_indexes)
        {
            note_symbols[note_index].Show();
        }
        note_scores = new();

        ready = true;
    }


    public void PressNote(int note_index)
    {
        if (!ready) return;

        if (current_score >= 0)
        {
            if (current_note_indexes.Contains(note_index) && current_score > 0)
            {
                current_note_indexes.Remove(note_index);
                note_scores.Add(current_score);
                GD.Print("Key Pressed, score:" + current_score);
                note_symbols[note_index].Hide();
                if (current_note_indexes.Count == 0)
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
        if (!ready) return;

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
            /*switch (full_score / note_scores.Count)
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
            }*/
            OnEndNote?.Invoke(full_score, full_score / note_scores.Count);
        }
        else
        {
            //temporary_label.Text = "MISS";
            OnEndNote?.Invoke(0, 0);
        }
        OnEndNote = null;
        QueueFree();
    }

    public void EnterCollider(Area2D area)
    {
        if (!ready) return;

        GD.Print("enter");

        current_score += 1;
    }
    public void ExitCollider(Area2D area)
    {
        if (!ready) return;

        GD.Print("exit");
        current_score -= 1;
        if (current_score < 0)
        {
            EndBeat(false);
        }
    }
}
public class BeatValues(int[] _note_indexes, Vector2 _position)
{
    public readonly int[] note_indexes = _note_indexes;
    public readonly Vector2 position = _position;
}