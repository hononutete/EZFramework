using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZFramework.Game.AI
{
    public class GameEntityAICommand
    {
        public GameEntityAICommandType aICommandType;
    }

    public enum GameEntityAICommandType
    {
        MOVE,
        MOVE_GRID,
        MOVE_DIRECTION,
        ATTACK,
        PLAY_CARD,

        EVADE,
        HUNT
    }
}