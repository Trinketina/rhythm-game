using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Beat : Node2D
{
    bool ready = false;
    /*public Action<int, int> OnEndNote; //score_multiplier, hit_value
    public Action<int> OnStartHoldNote; //index
    public Action<int> OnEndHoldNote; //index*/

    
    [Export] Note[] notes;
    [Export] Area2D collider;

    public int current_score { get; private set; } = -1;
    List<int> note_scores;
    List<int> current_note_indexes;
    List<int> held_note_indexes;

    BeatmapGameplay beatmap;

    public override void _Ready()
    {
        collider.AreaEntered += EnterCollider;
        collider.AreaExited += ExitCollider;

        Input.NoteKeyPressed += PressNote;


        /*TEMP_note_indexes = temp_indexes.ToList();

        StartBeat(TEMP_note_indexes.ToArray());*/
        base._Ready();
    }
    public void StartBeat(BeatValues beat, BeatmapGameplay _beatmap)
    {
        beatmap = _beatmap;
        //current_note_indexes = beat.note_indexes.ToList();
        current_note_indexes = new();
        held_note_indexes = new();
        Position = beat.position;

        for (int i = 0; i < beat.note_lengths.Length; i++)
        {
            if (beat.note_lengths[i] > 0 && notes.Length > i)
            {
                notes[i].EnableNote(i, beat.note_lengths[i] / Scale.Length());
                current_note_indexes.Add(i);

                if (beat.note_lengths[i] > 1)
                {
                    notes[i].OnReleaseNote += NoteReleased;
                    held_note_indexes.Add(i);
                }
            }
            
        }
        if (current_note_indexes.Count == 0)
        {
            QueueFree();
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
                notes[note_index].Frame = 2;

                //maybe temp??
                beatmap.HitNote(note_index, notes[note_index].GlobalPosition);
                notes[note_index].Hide();

                if (held_note_indexes.Contains(note_index))
                {
                    notes[note_index].StartHold(current_score);
                    //OnStartHoldNote?.Invoke(note_index);
                    beatmap.StartHold(note_index, notes[note_index].GetEndPosition());
                }
                else
                {
                    //beatmap.HitNote(note_index, notes[note_index].GlobalPosition);
                    notes[note_index].Hide();
                }
                if (current_note_indexes.Count == 0)
                {
                    EndHit(true);
                }
            }
            else
            {
                // NOTE WAS MISSED
                //GD.Print("Key Missed");
                EndHit(false);
            }
        }
    }
    public void NoteReleased(int index)
    {
        Note note = notes[index];
        //OnEndNote?.Invoke(note.held_score, note.held_score);
        beatmap.EndNote(note.held_score, note.held_score);
        //OnEndHoldNote?.Invoke(index);
        beatmap.EndHold(index, note.GlobalPosition);
        notes[index].Hide();
        held_note_indexes.Remove(index);
        //GD.Print("Released, score: " + note.held_score);
    }

    public void EndHit(bool success)
    {
        if (!ready) return;

        collider.AreaEntered -= EnterCollider;
        collider.AreaExited -= ExitCollider;
        Input.NoteKeyPressed -= PressNote;

        if (success)
        {
            int full_score = note_scores.Sum();
            //OnEndNote?.Invoke(full_score, full_score / note_scores.Count);
            beatmap.EndNote(full_score, full_score / note_scores.Count);
        }
        else
        {
            foreach (var note in notes)
            {
                note.Hide();
            }
            //OnEndNote?.Invoke(0, 0);
            beatmap.EndNote(0,0);
        }
        //OnEndNote = null;
        ready = false;
        //await ToSignal(GetTree().CreateTimer(.5d), "timeout");
        //QueueFree();
    }

    public void EnterCollider(Area2D area)
    {
        if (!ready) return;

        current_score += 1;
    }
    public void ExitCollider(Area2D area)
    {
        if (!ready) return;

        current_score -= 1;
        if (current_score < 0 && current_note_indexes.Count > 0)
        {
            EndHit(false);
        }
    }
}
public class BeatValues(int[] _note_lengths, Vector2 _position)
{

    public readonly int[] note_lengths = _note_lengths; //0 or null == no note, 1 == hit, longer means held
    public readonly Vector2 position = _position;
}