using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Globals
{
    public enum Direction
    {
        Left = -1, Right = 1
    }
    public enum PlayerState
    {
        Idle, Walk, JumpUp, JumpDown, DigSide, DigDown, Die
    }
    
    public static Node2D Scene { get; set; }

    internal static void CreateEffect(string name, Vector2 position)
    {
        var destroyEffect = GD.Load<PackedScene>("res://Effect.tscn");
        var effect = destroyEffect.Instance<Effect>();
        effect.EffectName = name;

        var x = ((int)position.x / 16) * 16 + 8;
        var y = ((int)position.y / 16) * 16 + 8;

        effect.Position = new Vector2(x, y);
        Scene.AddChild(effect);
    }
}
