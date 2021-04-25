using Godot;
using System;
using static Globals;

/*
export var speed = Vector2(150.0, 350.0)
onready var gravity = ProjectSettings.get("physics/2d/default_gravity")

const FLOOR_NORMAL = Vector2.UP

var _velocity = Vector2.ZERO

# _physics_process is called after the inherited _physics_process function.
# This allows the Player and Enemy scenes to be affected by gravity.
func _physics_process(delta):
	_velocity.y += gravity * delta

    var snap_vector = Vector2.ZERO
	if direction.y == 0.0:
		snap_vector = Vector2.DOWN * FLOOR_DETECT_DISTANCE
	var is_on_platform = platform_detector.is_colliding()
	_velocity = move_and_slide_with_snap(
		_velocity, snap_vector, FLOOR_NORMAL, not is_on_platform, 4, 0.9, false
	)
    */
public class Player : KinematicBody2D
{
    private AnimatedSprite animatedSprite;
        
    [Export]
    private float speedX, speedY;
    private float maxSpeedX = 100;
    private float dragX = .1f;
    private float accX = 10f;

    private Camera2D camera;
    //private bool isOnPlatform = true;
    
    public Direction Direction {get; private set; }

    Vector2 velocity = new Vector2(0.0f, 0.0f);
    float Gravity = 800.0f;
    const float FloorDetectDistance = 20.0f;
    RayCast2D platformDetector;

    public override void _PhysicsProcess(float delta)
    {

        Label debug;
        debug = GetNode<Label>("../../DebugText");

        bool isOnPlatform = platformDetector.IsColliding();
        bool isOnFloor = IsOnFloor();

        debug.Text = isOnFloor.ToString();
          // MOVE RIGHT && LEFT
         if (Input.IsActionPressed("ui_left")) {
            if (!isOnFloor){
                speedX = Math.Max(speedX - dragX * accX, -maxSpeedX);
            } else {
                State = PlayerState.Walk;
                speedX = Math.Max(speedX - accX, -maxSpeedX);
            }

            if (speedX <= 0) Direction = Direction.Left;
        }
        else if (Input.IsActionPressed("ui_right")) {
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
            if (isOnFloor){
                velocity.y = -200f;
                State = PlayerState.JumpUp;
            }
        }

        if (State == PlayerState.JumpUp){
            if (speedY >= 0)
                State = PlayerState.JumpDown;
        }

        velocity.y += Gravity * delta;

        Vector2 direction = new Vector2(0.0f, 0.0f); // TODO input goes here
        Vector2 snapVector = Vector2.Zero;
	    if (direction.y == 0.0f) {
		    snapVector = Vector2.Down * FloorDetectDistance;
        }
        velocity.x = speedX;
        velocity = MoveAndSlide(velocity, Vector2.Up, !isOnPlatform, 4, 0.9f, false);
    }    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        platformDetector = GetNode<RayCast2D>("PlatformDetector");
        camera = GetNode<Camera2D>("Camera");
        camera.CustomViewport = GetNode("../..");
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