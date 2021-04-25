using Godot;
using System;
using static Globals;

public class Player : KinematicBody2D
{
    private AnimatedSprite animatedSprite;

    private Map map;
    
    private float speedX, speedY;
    private float maxSpeedX = 100;
    private float dragX = .7f;
    private const float accX = 20f;

    private Camera2D camera;

    private int shovelPower = 1;
    
    public Direction Direction {get; private set; }

    private Vector2 velocity = new Vector2(0.0f, 0.0f);
    private const float GravityDefault = 700.0f;
    private const float GravityJump = 400.0f;
    private float gravity = GravityDefault;

    private const float FloorDetectDistance = 20.0f;
    private RayCast2D platformDetector;

    private float ghostTime = 1000f;

    private Label debugText;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        platformDetector = GetNode<RayCast2D>("PlatformDetector");
        camera = GetNode<Camera2D>("Camera");
        camera.CustomViewport = GetNode("../..");
        map = GetNode<Map>("..");
        debugText = GetNode<Label>("Z/DebugText");

        animatedSprite.Connect("animation_finished", this, "OnFinished");
    }

    public void death() {
        //GetTree().ReloadCurrentScene();
        State = PlayerState.Die;
        // if (State == PlayerState.Die) {
        //     while (ghostTime > 0) {
        //         State = PlayerState.Die;
        //         ghostTime -= delta;
        //         Vector2 move = new Vector2(0.0f, 20f);
        //         MoveAndSlide(move, Vector2.Up, !isOnPlatform, 4, 0.9f, false);
        //     }
        // }
    }

    public override void _Process(float delta)
    {
        if (State == PlayerState.Die) {
            if(ghostTime > 0) {
                ghostTime -= 0.1f;
            }
            else {
                ghostTime = 1000f;
                State = PlayerState.Idle;
                Position = new Vector2(50f,16f);
            }
            
        }
        
    }
    bool hasJumped = false;
    bool hitHead = false;

    public override void _PhysicsProcess(float delta)
    {
        bool isOnPlatform = platformDetector.IsColliding();
        bool isOnFloor = IsOnFloor();
        bool isOnCeil = IsOnCeiling();
        bool isOnWall = IsOnWall();

        bool onGround = isOnFloor || isOnPlatform;

        if (isOnCeil)
            hitHead = true;

        if (onGround)
            hasJumped = false;

        if (State != PlayerState.DigDown && State != PlayerState.DigSide && State != PlayerState.Die)
        {

            if (!isOnFloor && (State == PlayerState.Walk || State == PlayerState.Idle))
            {
                State = speedY < 0 ? PlayerState.JumpUp : PlayerState.JumpDown;
            }

            // MOVE RIGHT && LEFT
            if (Input.IsActionPressed("ui_left") && !Input.IsActionPressed("ui_right"))
            {
                if (!onGround)
                {
                    speedX = Math.Max(speedX - dragX * accX, -maxSpeedX);
                }
                else
                {
                    State = PlayerState.Walk;
                    speedX = Math.Max(speedX - accX, -maxSpeedX);
                }

                if (speedX <= 0) Direction = Direction.Left;
            }
            else if (Input.IsActionPressed("ui_right") && !Input.IsActionPressed("ui_left"))
            {
                if (!onGround)
                {
                    speedX = Math.Min(speedX + dragX * accX, maxSpeedX);
                }
                else
                {
                    State = PlayerState.Walk;
                    speedX = Math.Min(speedX + accX, maxSpeedX);
                }

                if (speedX >= 0) Direction = Direction.Right;
            }
            else
            {
                speedX *= .7f;
                if (onGround)
                    State = PlayerState.Idle;
                if (Math.Abs(speedX) < .2f)
                {
                    speedX = 0;
                }
            }

            // dig side
            if (onGround && /*Input.IsActionPressed("ui_down") &&*/ (Input.IsActionPressed("ui_right") || Input.IsActionPressed("ui_left")))
            {
                if (Dig(Direction == Direction.Left ? Vector2.Left : Vector2.Right))
                {
                    State = PlayerState.DigSide;
                }
            }

            if (Input.IsActionPressed("ui_up"))
            {
                if (!hasJumped && !hitHead)
                {
                    speedY = -180f;
                    State = PlayerState.JumpUp;
                    hasJumped = true;
                }
                gravity = GravityJump;
            }
            else
            {
                hitHead = false;
                gravity = GravityDefault;
            }

            if (Input.IsActionPressed("ui_down") && isOnFloor && State != PlayerState.DigSide)
            {
                var hasDigged = Dig(Vector2.Down);

                if (hasDigged)
                {
                    hasJumped = false;
                }
                State = PlayerState.DigDown;
            }

            if (Input.IsActionPressed("ui_focus_next"))
            {
                death();
            }


            if(Input.IsActionPressed("ui_select")) {
                var effect = GD.Load<PackedScene>("res://DestroyEffect.tscn");
                var node = effect.Instance<Node>();
                AddChild(node);            
            }

            animatedSprite.FlipH = (Direction == Direction.Left);
            
            if (State == PlayerState.JumpUp)
            {
                if (speedY >= 0)
                    State = PlayerState.JumpDown;
            }
        } else // digging:
        {
            speedX = 0;
            speedY = -gravity * delta;
        }

        animatedSprite.FlipH = (Direction == Direction.Left);

        velocity.x = speedX;
        speedY += gravity * delta;
        velocity.y = speedY;

        Vector2 direction = new Vector2(0.0f, 0.0f); // TODO input goes here
        Vector2 snapVector = Vector2.Zero;
	    if (direction.y == 0.0f) {
		    snapVector = Vector2.Down * FloorDetectDistance;
        }
        
        velocity = MoveAndSlide(velocity, Vector2.Up, !isOnPlatform, 4, 0.9f, false);

        if (isOnWall) speedX = 0;
        if (isOnCeil) speedY = Math.Max(speedY, 0);
        if (isOnFloor && !hasJumped) speedY = 0;

    }

    public void OnFinished()
    {
        if (State == PlayerState.DigDown || State == PlayerState.DigSide)
        {
            State = PlayerState.Idle;            
        }
    }

    bool CheckAndClearAt(Vector2 digPoint)
    {
        var tileType = map.GetEarthTileAt(digPoint);
        if (tileType == EarthTileType.Unknown)
            return false;

        if ((int)tileType <= shovelPower)
        {
            map.ClearEarthTileAt(digPoint);
            return true;
        }

        return false;
    }

    //
    //  dir is a unit vector, either Vector2.Left, Vector2.Right, Vector2.Down
    //
    private bool Dig(Vector2 dir)
    {
        Node2D collision = GetNode<Node2D>("Collision");
        Vector2 digPoint = collision.GlobalPosition;
        
        if (dir == Vector2.Left) {
            return CheckAndClearAt(collision.GlobalPosition + new Vector2(-8, 0));            
        } else if (dir == Vector2.Right) {
            return CheckAndClearAt(collision.GlobalPosition + new Vector2(8, 0));            
        } else if (dir == Vector2.Down) {
            for(int i = -2; i <= 2; i+= 2)
            {
                digPoint = collision.GlobalPosition + new Vector2(i, 10);
                var s = CheckAndClearAt(digPoint);
                if (s == true)
                    return s;
            }            
        }        
        return false;
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

}
