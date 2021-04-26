using Godot;
using System;

public class Item : Area2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    public ObjectType Type {get; set; }= 0;

    private AnimatedSprite sprite;
    private float time;

    private Game game;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite>("AnimatedSprite");
        game = GetNode<Game>("/root/Game");

        switch (Type)
        {
            case ObjectType.Player:
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
        case ObjectType.Shovel1:
            player.ShovelPower = 1;
            game.ShowTutorialText("Dig down!");
            Globals.CreateEffect("takeItem", Position);
            GD.Print("Acquired shovel 1!");
            QueueFree();
            break;
        case ObjectType.Shovel2:
            player.ShovelPower = 2;
            Globals.CreateEffect("takeItem", Position);
            GD.Print("Acquired shovel 2!");
            QueueFree();
            break;
        case ObjectType.Shovel3:
            player.ShovelPower = 3;
            Globals.CreateEffect("takeItem", Position);
            GD.Print("Acquired shovel 3!");
            QueueFree();
            break;
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
