using Godot;
using System;

public class Map : TileMap
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    public Resource tmx;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Label debugText = GetNode<Label>("../DebugText");
        File f = new File();
        if (f.Open("res://map/map.tmx", File.ModeFlags.Read) == Error.Ok)
        {
            string contents = f.GetAsText();
            debugText.Text = contents;
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
