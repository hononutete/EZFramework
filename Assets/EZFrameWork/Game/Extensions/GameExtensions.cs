using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EZFramework.Game
{
    public static class GamePlayerExtension
    {

        public static T Cast<T>(this GameEntity player) where T : GameEntity
        {
            return player as T;
        }
    }

    public static class GamePlayerControllerExtension
    {

        public static T Cast<T>(this GameEntityController player) where T : GameEntityController
        {
            return player as T;
        }
    }

    public static class GameStateExtension
    {
        public static T Cast<T>(this GameState state) where T : GameState
        {
            return state as T;
        }
    }
}
