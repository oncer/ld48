using Godot;
using System;

public class SFX : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    Random rng = new Random();

    public void Coin()
    {
        string name = "Coin" + rng.Next(1, 5);
        Play(name);
    }

    public void Dig()
    {
        string name = "Dig" + rng.Next(1, 4);
        Play(name);
    }

    public void Death()
    {
        Play("Death");
    }

    public void GetShovel()
    {
        Play("GetShovel");
    }

    public void EnemyDeath()
    {
        Play("EnemyDeath");
    }
    public void Play(string name)
    {
        var stream = GetNodeOrNull<AudioStreamPlayer>(name);
        if (stream != null) stream.Play();
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
