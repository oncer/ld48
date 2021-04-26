using Godot;
using System;

public class Enemy : KinematicBody2D, TMXObject
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private float xDir = 0;
    public Vector2 Velocity;
    private const float Gravity = 400;
    private float Speed = 50;

    private AnimatedSprite sprite;

    private float idleWait = 1;

    public ObjectType Type {get; set;}


    public void Kill()
    {
        Globals.CreateEffect("destroyEnemy", Position);
        QueueFree();
    }

    void OnOverlapDetectorBodyEntered(Node body)
    {
        if (body is Player) {
            Player player = body as Player;        
            if (Type == ObjectType.Spike) {
                if (!player.IsOnFloor() && player.Velocity.y > 0) {
                    player.Kill();
                }
            } else {
                player.Kill();
            }
        } else if (body is Enemy) {
            Enemy enemy = body as Enemy;
            if (Type == ObjectType.Spike) {
                if (!enemy.IsOnFloor() && enemy.Velocity.y > 0) {
                    GD.Print(enemy.Type.ToString() + " killed by " + Type.ToString());
                    enemy.Kill();
                }
            }
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        bool isOnFloor = IsOnFloor();
        bool isOnCeil = IsOnCeiling();
        bool isOnWall = IsOnWall();

        Velocity.y += Gravity * delta;
        if (idleWait > 0 || Type == ObjectType.Spike) {
            Velocity.x = 0;
        } else {
            sprite.FlipH = xDir < 0;
            Velocity.x = xDir * Speed;
        }
        Velocity = MoveAndSlide(Velocity);

        if (isOnWall && Velocity.x == 0 && idleWait <= 0) {
            //if (Type == ObjectType.Enemy2) GD.Print("enemy change dir");
            xDir = -xDir;
            idleWait = 1;
        }
        if (isOnFloor) Velocity.y = 0;
    }

    public override void _Process(float delta)
    {
        if (idleWait > 0) idleWait -= delta;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        xDir = 1;
        Velocity = Vector2.Zero;
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
        case ObjectType.Spike:
            sprite.Play("spike");
            break;
        case ObjectType.Trap:
            sprite.Play("trap");
            break;
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
