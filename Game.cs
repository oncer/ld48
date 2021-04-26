using Godot;
using System;
using static Globals;

public class Game : Node2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	private Player player;
	private Label scoreText;

	public SFX Sfx{get;set;}

	private TutorialText tutorialText;


    public void OnAnimationPlayerAnimationFinished(string name)
	{
		if (name == "FadeIn") {
			var music = GetNode<AudioStreamPlayer>("Music");
			music.Play();
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tutorialText = GetNode<TutorialText>("CanvasLayer/TutorialText");
		player = GetNode<Player>("Map/Player");
		scoreText = GetNode<Label>("CanvasLayer/ScoreHUD/ScoreText");
		player.State = PlayerState.Idle;
		var fadeRect = GetNode<ColorRect>("CanvasLayer/Z/FadeRect");
		fadeRect.Visible = true;
		var fadeAnim = fadeRect.GetNode<AnimationPlayer>("AnimationPlayer");
		fadeAnim.Play("FadeIn");

		Sfx = GetNode<SFX>("SFX");

		ShowTutorialText("Find the shovel!");
	}

    public void UpdateScoreText()
    {
        scoreText.Text = "Score: " + player.Score;
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
