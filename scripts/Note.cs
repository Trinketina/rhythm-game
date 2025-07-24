using Godot;
using System;

public partial class Note : Sprite2D
{
    [Export] Area2D collider;
    [Export] Sprite2D held_note;
    [Export] int texture_width = 36;

    public Action<int> OnReleaseNote; //index, sends up to beat
    public int length { get; private set; }

    bool holding = false;
    public int press_score { get; private set; } = -1;
    public int held_score { get; private set; } = -1;
    int index = -1;

    public void EnableNote(int note_index, int note_length)
    {
        Show();
        length = note_length;
        index = note_index;

        if (length > 1)
        {
            //TODO:: implement held notes
            Input.NoteKeyReleased += ReleaseNote;

            collider.AreaEntered += EnterCollider;
            collider.AreaExited += ExitCollider;

            held_note.Position = Vector2.Up * length;
            held_note.Offset = Vector2.Down * length / 2;
            held_note.RegionRect = new(Vector2.Zero, new Vector2(texture_width, length));
            
            held_note.Show();
        }
    }

    public void StartHold(int press_value)
    {
        holding = true;
        press_score = press_value;
    }
    public void ReleaseNote(int note_index)
    {
        if (note_index == index && holding)
        {
            Input.NoteKeyReleased -= ReleaseNote;
            collider.AreaEntered -= EnterCollider;
            collider.AreaExited -= ExitCollider;

            if (held_score < 0)
                held_score = 0;

            OnReleaseNote.Invoke(index);
            holding = false;
        }
    }

    public void EnterCollider(Area2D area)
    {
        held_score += 1;
    }
    public void ExitCollider(Area2D area)
    {
        held_score -= 1;
        if (held_score < 0)
        {
            ReleaseNote(index);
        }
    }
}
