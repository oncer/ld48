using Godot;
using System;
using static Globals;

public class Player : KinematicBody2D
{
    private AnimatedSprite animatedSprite;
        
    [Export]
    private float speedX, speedY;
    private float maxSpeedX = 50;
    private float accX = .5f;
    private bool onGround = true;
    
    public Direction Direction {get; private set; }

    // Called when the node enters the scene tree for the first time.
    
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

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
    }

    public override void _Process(float delta)
    {

        if (Input.IsActionPressed("ui_left"))
        {
            if (!onGround){
                speedX = Math.Max(speedX - .2f * accX, -maxSpeedX);
            } else {
                State = PlayerState.Walk;
                speedX = Math.Max(speedX - accX, -maxSpeedX);
            }

            if (speedX <= 0) Direction = Direction.Left;
        }
        else if (Input.IsActionPressed("ui_right"))
        {
            if (!onGround){
                speedX = Math.Min(speedX + .2f * accX, maxSpeedX);
            } else {
                State = PlayerState.Walk;
                speedX = Math.Min(speedX + accX, maxSpeedX);
            }

            if (speedX >= 0) Direction = Direction.Right;
        } else {
            speedX *= .97f;
                if (onGround)
                    State = PlayerState.Idle;
            if (Math.Abs(speedX) < .2f) {

                speedX = 0;
            }
        }

        animatedSprite.FlipH = (Direction == Direction.Left);

        if (Input.IsActionPressed("ui_up")) {
            if (onGround){
                onGround = false;
                speedY = -40f;
                State = PlayerState.JumpUp;
            }
        }

        if (State == PlayerState.JumpUp){
            if (speedY >= 0)
                State = PlayerState.JumpDown;
        }

        Position += new Vector2(speedX, speedY) * delta;
     
    }

}
