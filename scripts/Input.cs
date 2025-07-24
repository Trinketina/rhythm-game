using Godot;
using System;

public partial class Input : Node
{
    [Export] Vector2 scale_factor;

    [ExportGroup("Input Sprites")]
    [Export] Sprite2D arrow_left;
    [Export] Sprite2D arrow_up;
    [Export] Sprite2D arrow_down;
    [Export] Sprite2D arrow_right;


    public static Action<int> NoteKeyPressed;
    public static Action<int> NoteKeyReleased;

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("arrow-left"))
        {
            ScaleButton(arrow_left);
            NoteKeyPressed?.Invoke(0);
        }
        else if (@event.IsActionReleased("arrow-left"))
        {
            DeScaleButton(arrow_left);
            NoteKeyReleased?.Invoke(0);
        }
        else if (@event.IsActionPressed("arrow-up"))
        {
            ScaleButton(arrow_up);
            NoteKeyPressed?.Invoke(1);
        }
        else if (@event.IsActionReleased("arrow-up"))
        {
            DeScaleButton(arrow_up);
            NoteKeyReleased?.Invoke(1);
        }
        else if (@event.IsActionPressed("arrow-down"))
        {
            ScaleButton(arrow_down);
            NoteKeyPressed?.Invoke(2);
        }
        else if (@event.IsActionReleased("arrow-down"))
        {
            DeScaleButton(arrow_down);
            NoteKeyReleased?.Invoke(2);
        }
        else if (@event.IsActionPressed("arrow-right"))
        {
            ScaleButton(arrow_right);
            NoteKeyPressed?.Invoke(3);
        }
        else if (@event.IsActionReleased("arrow-right"))
        {
            DeScaleButton(arrow_right);
            NoteKeyReleased?.Invoke(3);
        }
        base._Input(@event);
    }

    public async void ScaleButton(Sprite2D arrow_key)
    {
        //Vector2 big_size = Vector2.One * scale_factor;
        //arrow_key.Scale = scale_factor;

        arrow_key.Frame = 1;
        await ToSignal(GetTree().CreateTimer(.05), "timeout");
        arrow_key.Frame = 2;
    }
    public async void DeScaleButton(Sprite2D arrow_key)
    {
        /*Vector2 big_size = Vector2.One * scale_factor;
        arrow_key.Scale = big_size;*/
        float time = 0;

        while (time <= .1)
        {
            time += (float)GetProcessDeltaTime();
            //arrow_key.Scale = scale_factor.Lerp(Vector2.One, time * 10);
            switch (time)
            {
                case < .05f:
                    arrow_key.Frame = 2;
                    break;
                case < .1f:
                    arrow_key.Frame = 1;
                    break;
                default:
                    arrow_key.Frame = 0;
                    break;
            }
            await ToSignal(GetTree().CreateTimer(GetProcessDeltaTime()), "timeout");
        }
    }
}
