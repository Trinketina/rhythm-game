using Godot;
using System;

public partial class TempLabel : Label
{
    public override void _Ready()
    {
        Text = "";
        base._Ready();
    }

    public async void ClearAfter(string text, float seconds)
    {
        Text = text;

        await ToSignal(GetTree().CreateTimer(seconds), "timeout");

        Text = "";
    }
}
