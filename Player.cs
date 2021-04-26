using Godot;
using System;
using static Globals;

public class Player : KinematicBody2D
{
    public int ShovelPower { get; set; } = 0;
    public Direction Direction { get; private set; }
    public Vector2 SpawnPosition { get; set; }

    private AnimatedSprite animatedSprite;
    private Map map;    
    private float speedX, speedY;
    private float maxSpeedX = 100;
    private float maxSpeedY = 250;
    private float dragX = .7f;
    private const float accX = 20f;
    private Camera2D camera;
    public Vector2 Velocity = Vector2.Zero;
    private const float GravityDefault = 700.0f;
    private const float GravityJump = 400.0f;
    private float gravity = GravityDefault;
    private const float FloorDetectDistance = 20.0f;
    private RayCast2D platformDetector;
    private const float maxGhostTime = 3f; // seconds
    private float ghostTime = maxGhostTime;
    private double ghostSin = .5f * Math.PI;
    private Label debugText;
    
    private bool hasJumped = false;
    private bool hitHead = false;

    private bool firstDig = true;

    public int Score = 0;

    private float initialDigTime = .2f;
    private float digTimer = 0;

    private float dieTutorialTimer = 0;
    private bool suicideHappened = false;

    private Game game;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        game = GetNode<Game>("/root/Game");
        Globals.Scene = GetNode<Node2D>("..");
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        platformDetector = GetNode<RayCast2D>("PlatformDetector");
        
        camera = GetNode<Camera2D>("Camera");
        camera.CustomViewport = GetNode("../..");
        map = GetNode<Map>("..");
        debugText = GetNode<Label>("Z/DebugText");

        animatedSprite.Connect("animation_finished", this, "OnFinished");
        SpawnPosition = Position;
    }

    public void AcquireCoin(int value)
    {
        GD.Print("Acquired coin with value " + value);
        Score += value;
        game.UpdateScoreText();
    }

    public void Kill()
    {
        speedX = 0;
        speedY = 0;
        State = PlayerState.Die;
    }

    public override void _Process(float delta)
    {
        if (dieTutorialTimer > 0 && !suicideHappened) {
            dieTutorialTimer -= delta;
            if (dieTutorialTimer <= 0) {
                game.ShowTutorialText("When you are stuck: press TAB!");
                game.HideTutorialText(3.5f);
                dieTutorialTimer = 34;
            }
        }
    }

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

        int moveInput = 0;
        if (Input.IsActionPressed("ui_left") && !Input.IsActionPressed("ui_right")) {
            moveInput = -1;
        } else if (Input.IsActionPressed("ui_right") && !Input.IsActionPressed("ui_left")) {
            moveInput = 1;
        }

        if (State != PlayerState.Die) {
            if (State != PlayerState.DigDown && State != PlayerState.DigSide)
            {

                if (!isOnFloor && (State == PlayerState.Walk || State == PlayerState.Idle))
                {
                    State = speedY < 0 ? PlayerState.JumpUp : PlayerState.JumpDown;
                }

                // MOVE RIGHT && LEFT
                if (moveInput < 0)
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
                else if (moveInput > 0)
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
                if (ShovelPower >= 1 && /*onGround &&*/ /*Input.IsActionPressed("ui_down") &&*/ moveInput != 0)
                {
                    if (CanDigDirection(Direction == Direction.Left ? Vector2.Left : Vector2.Right))
                    {
                        State = PlayerState.DigSide;
                        digTimer = initialDigTime;
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

                if (ShovelPower >= 1 && Input.IsActionPressed("ui_down") && isOnFloor && State != PlayerState.DigSide)
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
                    Kill();
                    suicideHappened = true;
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

                if (State == PlayerState.DigSide && digTimer > 0) {
                    if (moveInput != (int)Direction) {
                        State = moveInput == 0 ? PlayerState.Idle : PlayerState.Walk;
                    }
                    else 
                    {
                        digTimer -= delta;
                        if (digTimer <= 0) {
                            if (!Dig(Direction == Direction.Left ? Vector2.Left : Vector2.Right)) {
                                State = moveInput == 0 ? PlayerState.Idle : PlayerState.Walk;
                            }
                        }
                    }
                }
            }
        } else // dead:
        {
            gravity = 0f;
            ghostSin = (ghostSin + .08f) % (2 * Math.PI);
            speedX = (float)Math.Sin(ghostSin) * 40;
            speedY = Math.Max(speedY - 2f, -100f);
            
            ghostTime = Math.Max(ghostTime - delta, 0);
            if (ghostTime == 0 || Position.y <= -64)
            {
                ghostSin = .5f * Math.PI;
                ghostTime = maxGhostTime;
                State = PlayerState.Idle;
                gravity = GravityDefault;
                speedY = 0;
                speedX = 0;
                hasJumped = false;
                hitHead = false;

                Position = SpawnPosition;
            }
        }

        animatedSprite.FlipH = (Direction == Direction.Left);

        Velocity.x = speedX;
        speedY += gravity * delta;
        Velocity.y = speedY;

        Vector2 direction = new Vector2(0.0f, 0.0f); // TODO input goes here
        Vector2 snapVector = Vector2.Zero;
	    if (direction.y == 0.0f) {
		    snapVector = Vector2.Down * FloorDetectDistance;
        }

        Position = new Vector2(Mathf.Clamp(Position.x, 8, 640 - 8), Position.y);

        // disable collision if dead
        var collision = GetNode<CollisionShape2D>("Collision");
        collision.Disabled = State == PlayerState.Die;

        Velocity.y = Mathf.Clamp(Velocity.y, -maxSpeedY, maxSpeedY);
        Velocity = MoveAndSlide(Velocity, Vector2.Up, !isOnPlatform, 4, 0.9f, false);

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

    bool ClearTileAt(Vector2 digPoint)
    {
        if (CanClearTileAt(digPoint)) {
            map.ClearEarthTileAt(digPoint);
            Globals.CreateEffect("destroyBlock", digPoint);
            return true;
        }
        return false;
    }


    private bool CanClearTileAt(Vector2 digPoint)
    {
        var tileType = map.GetEarthTileAt(digPoint);
        if (tileType == EarthTileType.Unknown || tileType == EarthTileType.Empty)
            return false;

        return ((int)tileType <= ShovelPower);
    }

    private Vector2 GetDigPoint(Vector2 dir)
    {
        Node2D collision = GetNode<Node2D>("Collision");
        Vector2 digPoint = collision.GlobalPosition;

        if (dir == Vector2.Left) {
            return collision.GlobalPosition + new Vector2(-8, 0);            
        } else if (dir == Vector2.Right) {
            return collision.GlobalPosition + new Vector2(8, 0);            
        } else if (dir == Vector2.Down) {
            for(int i = -2; i <= 2; i+= 2)
            {
                digPoint = collision.GlobalPosition + new Vector2(i, 10);
                if (CanClearTileAt(digPoint)) {
                    return digPoint;
                }
            }
            return collision.GlobalPosition + new Vector2(0, 10); // fallback
        }
        GD.Print($"GetDigPoint: invalid direction ({dir.x},{dir.y})!");
        return Vector2.Zero;
    }

    private bool CanDigDirection(Vector2 dir)
    {
        return CanClearTileAt(GetDigPoint(dir));
    }
    //
    //  dir is a unit vector, either Vector2.Left, Vector2.Right, Vector2.Down
    //
    private bool Dig(Vector2 dir)
    {
        Vector2 digPoint = GetDigPoint(dir);
        if (CanClearTileAt(digPoint)) {
            ClearTileAt(digPoint);
            if (dir == Vector2.Down && firstDig)
            {
                firstDig = false;
                game.HideTutorialText();
                dieTutorialTimer = 20;
            }
            return true;
        }
        return false;
    }

    private PlayerState state;
    public PlayerState State 
    {
        get => state;
        set
        {
            var str = value.GetString();
            if (value == PlayerState.DigDown || value == PlayerState.DigSide)
            {
                if (ShovelPower > 0)
                    str += ShovelPower;
            }
            animatedSprite.Play(str);
            state = value;
        }
    }

}
