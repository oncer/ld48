using Godot;
using System;
using static Globals;

public class DestroyEffect : Node2D
{
    private AnimatedSprite animatedSprite;

    private Vector2 effPos;
    private EffectType effType = EffectType.Poof;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        animatedSprite.Play(effType.GetString());
        animatedSprite.Connect("animation_finished", this, "Delete");
    }

    public void StartAt(Vector2 position, EffectType effect)
    {
        effType = effect;
        effPos = position;
    }

    public void Delete() {
        GetParent().RemoveChild(this);
    }

}
