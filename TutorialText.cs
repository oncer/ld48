using Godot;
using System;

public class TutorialText : Label
{

    AnimationPlayer animation;
	private float timeout = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animation = GetNode<AnimationPlayer>("Animation");
        Visible = false;
    }

    public void Show(string text)
    {
        if (!Visible) {
            animation.Play("show");
        }
        Text = text;
    }

    public void Disappear(float waitTime)
    {
        if (waitTime > 0) {
            timeout = waitTime;
        } else {
            animation.Play("disappear");
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (timeout > 0) {
            timeout -= delta;
            if (timeout <= 0) {
                animation.Play("disappear");
            }
        }
    }
}
