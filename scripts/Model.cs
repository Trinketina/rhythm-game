using Godot;
using System.Collections.Generic;

public partial class Model : Resource
{
	Stack<BeatValue> all_beats = new Stack<BeatValue>();
    public BeatValue current_beat;

    public void AddBeat()
    {
        //todo
        //all_beats.Push(BEAT HERE);
    }
    public void HitNote(int index)
    {

    }
    public void PopBeat()
    {
        current_beat = all_beats.Pop();
    }
}

public class BeatValue
{
	public List<int> note_indexes;

	public BeatValue(List<int> _note_indexes)
    {
        this.note_indexes = _note_indexes;
    }
}
