using Godot;
using System;
using static Globals;

public class Game : Node2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	private Player player;

	private TutorialText tutorialText;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tutorialText = GetNode<TutorialText>("CanvasLayer/TutorialText");
		player = GetNode<Player>("Map/Player");
		player.State = PlayerState.Idle;

		ShowTutorialText("Find the shovel!");
	}

	public void ShowTutorialText(string text)
	{
		tutorialText.Show(text);
	}

	public void HideTutorialText(float waitTime=0)
	{
		tutorialText.Disappear(waitTime);
	}
}
