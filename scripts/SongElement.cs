using Godot;
using System;

public partial class SongElement : Control
{
    [Export] TextureRect song_art;
    [Export] RichTextLabel song_title;
    [Export] RichTextLabel song_artist;
    [Export] RichTextLabel song_charter;

    public void Initialize(Image art, string title, string artist, string charter)
    {
        song_art.Texture = ImageTexture.CreateFromImage(art);

        string new_title = title.Replace("<", "[");
        new_title = new_title.Replace(">", "]");
        song_title.Text = new_title;

        string new_artist = artist.Replace("<", "[");
        new_artist = new_artist.Replace(">", "]");
        song_artist.Text = new_artist;

        string new_charter = charter.Replace("<", "[");
        new_charter = new_charter.Replace(">", "]");
        song_charter.Text = new_charter;

        GD.Print("alive");
    }
}
