using Godot;
using System;
using static Globals;

public class Player : KinematicBody2D
{
    private AnimatedSprite animatedSprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
    }

    private PlayerState state;
    public PlayerState State 
    {
        get => state;
        set
        {
            animatedSprite.Play(value.GetString());
            state = value;
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
