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
    private float Speed = 50;

    private AnimatedSprite sprite;

    private float idleWait = 1;

    public ObjectType Type {get; set;}

    void OnOverlapDetectorBodyEntered(Node body)
    {
        Player player = body as Player;
        if (player == null) return;
        player.Kill();
    }

    public override void _PhysicsProcess(float delta)
    {
        bool isOnFloor = IsOnFloor();
        bool isOnCeil = IsOnCeiling();
        bool isOnWall = IsOnWall();

        velocity.y += Gravity * delta;
        if (idleWait > 0) {
            velocity.x = 0;
        } else {
            sprite.FlipH = xDir < 0;
            velocity.x = xDir * Speed;
        }
        velocity = MoveAndSlide(velocity);

        if (isOnWall && velocity.x == 0 && idleWait <= 0) {
            //if (Type == ObjectType.Enemy2) GD.Print("enemy change dir");
            xDir = -xDir;
            idleWait = 1;
        }
        if (isOnFloor) velocity.y = 0;
    }

    public override void _Process(float delta)
    {
        if (idleWait > 0) idleWait -= delta;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        xDir = 1;
        velocity = Vector2.Zero;
        sprite = GetNode<AnimatedSprite>("Sprite");

        switch (Type)
        {
        case ObjectType.Enemy1:
            sprite.Play("enemy1");
            break;
        case ObjectType.Enemy2:
            sprite.Play("enemy2");
            break;
        case ObjectType.Enemy3:
            sprite.Play("enemy3");
            break;
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
