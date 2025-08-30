using Godot;
using System;

public partial class Note : Sprite2D
{
    [Export] Area2D collider;
    [Export] Sprite2D held_note;
    [Export] Sprite2D end_note;
    int texture_frames = 4;
    int texture_width = 42;

    public Action<int> OnReleaseNote; //index, sends up to beat
    public float length { get; private set; }

    bool holding = false;
    public int press_score { get; private set; } = -1;
    public int held_score { get; private set; } = -1;
    int index = -1;

    public void EnableNote(int note_index, float note_length)
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
            held_note.RegionRect = new(Vector2.Right * ((texture_frames - 1) * texture_width), new Vector2(texture_width, length));
            
            held_note.Show();
        }
    }

    public void StartHold(int press_value)
    {
        holding = true;
        press_score = press_value;
        IncreaseSpriteSizes();
    }

    public void IncreaseSpriteSizes()
    {
        //held_note.Frame = 4;
        held_note.RegionRect = new(Vector2.Right * (texture_frames * texture_width), new Vector2(texture_width, length));
        end_note.Frame = 2;
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

    public Vector2 GetEndPosition()
    {
        return end_note.GlobalPosition;
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
