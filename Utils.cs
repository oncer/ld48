﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Globals;

public static class Utils
{
    public static string GetString(this PlayerState a)
    {
        switch (a)
        {
            case PlayerState.Idle:
                return "idle";                    
            case PlayerState.Walk:
                return "walk";
            case PlayerState.JumpUp:
                return "jumpUp";
            case PlayerState.JumpDown:
                return "jumpDown";
            case PlayerState.DigSide:
                return "digSide";                    
            case PlayerState.DigDown:
                return "digDown";                    
            case PlayerState.Die:
                return "die";
            default:
                throw new NotImplementedException();
        }
    }
}