using Godot;
using System;
using static Globals;

public class Game : Node2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	private Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<Player>("Map/Player");
		player.State = PlayerState.DigSide;
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
