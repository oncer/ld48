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

    private Label debugText;

    public override void _PhysicsProcess(float delta)
    {
        bool ididajump = false;

        Dig(Vector2.Down);

        bool isOnPlatform = platformDetector.IsColliding();
        bool isOnFloor = IsOnFloor();
        bool isOnCeil = IsOnCeiling();
        bool isOnWall = IsOnWall();

          // MOVE RIGHT && LEFT
         if (Input.IsActionPressed("ui_left") && !Input.IsActionPressed("ui_right")) {
            if (!isOnFloor){
                speedX = Math.Max(speedX - dragX * accX, -maxSpeedX);
            } else {
                State = PlayerState.Walk;
                speedX = Math.Max(speedX - accX, -maxSpeedX);
            }

            if (speedX <= 0) Direction = Direction.Left;
        }
        else if (Input.IsActionPressed("ui_right") && !Input.IsActionPressed("ui_left")) {
            if (!isOnFloor){
                speedX = Math.Min(speedX + dragX * accX, maxSpeedX);
            } else {
                State = PlayerState.Walk;
                speedX = Math.Min(speedX + accX, maxSpeedX);
            }

            if (speedX >= 0) Direction = Direction.Right;
        } else {
            speedX *= .7f;
            if (isOnFloor)
                State = PlayerState.Idle;
            if (Math.Abs(speedX) < .2f) {
                speedX = 0;
            }
        }

        animatedSprite.FlipH = (Direction == Direction.Left);

        if (Input.IsActionPressed("ui_up")) {
            if (isOnFloor) {
                speedY = -180f;
                State = PlayerState.JumpUp;
                ididajump = true;
            }
            gravity = GravityJump;
        } else {
            gravity = GravityDefault;
        }

        if (Input.IsActionPressed("ui_down"))
        {
            Dig(Vector2.Down);            
        }

        if (State == PlayerState.JumpUp){
            if (speedY >= 0)
                State = PlayerState.JumpDown;
        }

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
        if (isOnFloor && !ididajump) speedY = 0;

    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        platformDetector = GetNode<RayCast2D>("PlatformDetector");
        camera = GetNode<Camera2D>("Camera");
        camera.CustomViewport = GetNode("../..");
        map = GetNode<Map>("..");
        debugText = GetNode<Label>("Z/DebugText");
    }

    //
    //  dir is a unit vector, either Vector2.Left, Vector2.Right, Vector2.Down
    //
    private void Dig(Vector2 dir)
    {
        Node2D collision = GetNode<Node2D>("Collision");
        Vector2 digPoint = collision.GlobalPosition;
        if (dir == Vector2.Left) {
            digPoint += new Vector2(-8, 0);
        } else if (dir == Vector2.Right) {
            digPoint += new Vector2(8, 0);
        } else if (dir == Vector2.Down) {
            digPoint += new Vector2(0, 10);
        }
        EarthTileType et = map.GetEarthTileAt(digPoint);
        debugText.Text = $"{et.ToString()} {(int)digPoint.x},{(int)digPoint.y}";

        if ()
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
