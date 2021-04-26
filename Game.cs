using Godot;
using System;
using static Globals;

public class Game : Node2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	private Player player;

	private Label tutorialText;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tutorialText = GetNode<Label>("CanvasLayer/TutorialText");
		player = GetNode<Player>("Map/Player");
		player.State = PlayerState.Idle;

		ShowTutorialText("Find the shovel!");
	}

	public void ShowTutorialText(string text)
	{
		tutorialText.Text = text;
		tutorialText.Visible = true;
	}

	public void HideTutorialText()
	{
		tutorialText.Visible = false;
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
