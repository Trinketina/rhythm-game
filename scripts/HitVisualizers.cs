using Godot;
using System.Collections.Generic;

public partial class HitVisualizers : Node
{
    [Export] Sprite2D[] visualizer_sprites;
    [Export] Sprite2D[] hold_sprites;
    [Export] float show_time = 1.3f;

    List<int> currently_holding = new();

    public void OnHitNote(int index, Vector2 global_position)
    {
        visualizer_sprites[index].GlobalPosition = global_position;
        visualizer_sprites[index].Show();

        HideAfter(index);
    }
    public void OnStartHold(int index, Vector2 global_position)
    {
        hold_sprites[index].GlobalPosition = global_position;
        visualizer_sprites[index].Show();

        hold_sprites[index].Show();
        currently_holding.Add(index);
        //HideAfter(visualizer_sprites[index]);
    }
    public void OnStopHold(int index, Vector2 global_position) 
    {
        hold_sprites[index].GlobalPosition = global_position;

        currently_holding.Remove(index);
        hold_sprites[index].Hide();

        visualizer_sprites[index].Show();

        HideAfter(index);
    }

    public async void HideAfter(int index)
    {
        
        await ToSignal(GetTree().CreateTimer(show_time), "timeout");
        if (!currently_holding.Contains(index))
            visualizer_sprites[index].Hide();
    }

    public override void _Process(double delta)
    {
        if (currently_holding.Count > 0)
        {
            foreach (var held_index in currently_holding)
            {
                hold_sprites[held_index].Position += Vector2.Down * (250 * (float)delta / hold_sprites[held_index].GlobalScale.Y);
            }
        }
        base._Process(delta);
    }
}
