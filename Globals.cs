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
    public enum EffectType
    {
        None,
        Poof
    }
}
