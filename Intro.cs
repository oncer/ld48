using Godot;
using System;

public class Intro : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    int animationState = 0;
    AnimationPlayer animation;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animation = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _Input(InputEvent ev)
    {
        if ((ev is InputEventKey && ev.IsPressed())
            || (ev is InputEventMouse && ev.IsPressed())) {
                if (animationState == 1) {
                    animation.Play("FadeOut");
                }
            }
    }

    public void OnAnimationPlayerAnimationFinished(string name)
    {
        GD.Print("finished " + name);
        if (name == "FadeIn") {
            animationState = 1;
        } else if (name == "FadeOut") {
            GetTree().ChangeScene("res://Game.tscn");
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
