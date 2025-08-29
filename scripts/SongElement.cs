using Godot;
using System;

public partial class SongElement : Node
{
    [Export] TextureRect song_art;
    [Export] RichTextLabel song_title;
    [Export] RichTextLabel song_artist;
    [Export] RichTextLabel song_charter;

    public void Initialize(Image art, string title, string artist, string charter)
    {
        song_art.Texture = ImageTexture.CreateFromImage(art);
        song_title.Text = title;
        song_artist.Text = artist;
        song_charter.Text = charter;
    }
}
