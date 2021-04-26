using Godot;
using System;

public class ScoreHUD : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    Label text;
    NinePatchRect rect;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        rect = GetNode<NinePatchRect>("ColorRect");
        text = GetNode<Label>("ScoreText");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        rect.RectSize = text.GetFont("font").GetStringSize(text.Text) + new Vector2(8, 2);
    }
}
