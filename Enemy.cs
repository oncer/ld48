using Godot;
using System;

public class Enemy : KinematicBody2D, TMXObject
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private float xDir = 0;
    private Vector2 velocity;
    private const float Gravity = 400;
    [Export]
    private float Speed = 200;

    public ObjectType Type {get; set;}

    public override void _PhysicsProcess(float delta)
    {
        bool isOnFloor = IsOnFloor();
        bool isOnCeil = IsOnCeiling();
        bool isOnWall = IsOnWall();

        if (isOnWall) {
            xDir = -xDir;
        } else {
            velocity.y += Gravity * delta;
            velocity.x = xDir * Speed;
            velocity = MoveAndSlide(velocity);
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        xDir = 1;
        velocity = Vector2.Zero;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
