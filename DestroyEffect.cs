using Godot;
using System;
using static Globals;

public class DestroyEffect : Node2D
{
    private AnimatedSprite animatedSprite;

    public string EffectName { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        animatedSprite.Play(EffectName);
        animatedSprite.Connect("animation_finished", this, "Delete");
    }

    public void Delete() {
        GetParent().RemoveChild(this);
        QueueFree();
    }

}
