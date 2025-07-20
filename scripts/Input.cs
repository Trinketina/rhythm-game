using Godot;
using System;

public partial class Input : Node
{
    [Export] Resource note_resource;

    [Export] Sprite2D arrow_up;
    [Export] Sprite2D arrow_left;
    [Export] Sprite2D arrow_right;
    [Export] Sprite2D arrow_down;

    public static Action<int> ArrowKeyPressed;

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("arrow-up"))
        {
            ScaleButton(arrow_up);
            ArrowKeyPressed?.Invoke(0);
        }
        else if (@event.IsActionPressed("arrow-left"))
        {
            ScaleButton(arrow_left);
            ArrowKeyPressed?.Invoke(1);
        }
        else if (@event.IsActionPressed("arrow-right"))
        {
            ScaleButton(arrow_right);
            ArrowKeyPressed?.Invoke(2);
        }
        else if (@event.IsActionPressed("arrow-down"))
        {
            ScaleButton(arrow_down);
            ArrowKeyPressed?.Invoke(3);
        }
        base._Input(@event);
    }

    public async void ScaleButton(Sprite2D arrow_key)
    {
        Vector2 big_size = Vector2.One * 2f;
        arrow_key.Scale = big_size;
        float time = 0;

        while (time <= .1)
        {
            time += (float)GetProcessDeltaTime();
            arrow_key.Scale = big_size.Lerp(Vector2.One, time * 10);
            await ToSignal(GetTree().CreateTimer(GetProcessDeltaTime()), "timeout");
        }
    }
}
