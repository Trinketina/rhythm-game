using Godot;
using System;

public partial class TempLabel : Label
{
    public override void _Ready()
    {
        ClearAfter(1.5f);
    }

    public async void ClearAfter(float seconds)
    {
        while (true)
        {
            Text = "";
            await ToSignal(GetTree().CreateTimer(seconds), "timeout");
        }
    }
}
