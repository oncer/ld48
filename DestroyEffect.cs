using Godot;
using System;
using static Globals;

public class DestroyEffect : Node2D
{
    private AnimatedSprite animatedSprite;
    private EffectState state;
    public EffectState State 
    {
        get => state;
        set
        {
            animatedSprite.Play(value.GetString());
            state = value;
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        State = EffectState.Poof;
        Position = new Vector2(8f, 0f);
        animatedSprite.Connect("animation_finished",this, "Delete");
    }

    public void Delete() {
        GetParent().RemoveChild(this);
    }

}
