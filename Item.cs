using Godot;
using System;

public class Item : Area2D, TMXObject
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    public ObjectType Type {get; set; }= 0;

    private AnimatedSprite sprite;
    private float time;

    private CollisionShape2D collision;

    private Game game;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite>("AnimatedSprite");
        game = GetNode<Game>("/root/Game");
        collision = GetNode<CollisionShape2D>("Collision"); 

        switch (Type)
        {
            case ObjectType.Player:
                break;
            case ObjectType.Coin1:
                sprite.Play("coin1");
                (collision.Shape as RectangleShape2D).Extents = new Vector2(4, 4);
                break;
            case ObjectType.Coin2:
                sprite.Play("coin2");
                (collision.Shape as RectangleShape2D).Extents = new Vector2(5, 5);
                break;
            case ObjectType.Coin3:
                sprite.Play("coin3");
                break;
            case ObjectType.Coin4:
                sprite.Play("coin4");
                break;
            case ObjectType.Coin5:
                sprite.Play("coin5");
                break;
            case ObjectType.Coin6:
                sprite.Play("coin6");
                break;
            case ObjectType.Shovel1:
                sprite.Play("shovel1");
                break;
            case ObjectType.Shovel2:
                sprite.Play("shovel2");
                break;
            case ObjectType.Shovel3:
                sprite.Play("shovel3");
                break;
            default:
                break;
        }
    }

    public override void _Process(float delta)
    {
        time += delta;
        if (time > Mathf.Pi * 2) time -= Mathf.Pi * 2;
        sprite.Position = new Vector2(0f, Mathf.Sin(time * 3));
    }

    public void OnItemBodyEntered(PhysicsBody2D body2D)
    {
        Player player = body2D as Player;
        if (player == null) return;
        switch (Type) {
        case ObjectType.Coin1:
            player.AcquireCoin(1);
            break;
        case ObjectType.Coin2:
            player.AcquireCoin(5);
            break;
        case ObjectType.Coin3:
            player.AcquireCoin(10);
            break;
        case ObjectType.Coin4:
            player.AcquireCoin(100);
            break;
        case ObjectType.Coin5:
            player.AcquireCoin(200);
            break;
        case ObjectType.Coin6:
            player.AcquireCoin(1000);
            break;
        case ObjectType.Shovel1:
            player.AcquireShovel(1);
            game.ShowTutorialText("Awesome! Now start digging!");
            GD.Print("Acquired shovel 1!");
            break;
        case ObjectType.Shovel2:
            player.AcquireShovel(2);
            GD.Print("Acquired shovel 2!");
            break;
        case ObjectType.Shovel3:
            player.AcquireShovel(3);
            GD.Print("Acquired shovel 3!");
            break;
        }

        Globals.CreateEffect("takeItem", Position);
        QueueFree();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
