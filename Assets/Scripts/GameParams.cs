using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameParams
{
    public static class Animation
    {
        #region PLAYER
        public static string PLAYER_DASH_TRIGGER = "dash";
        public static string PLAYER_TAKEDAMAGE_TRIGGER = "takeDamage";
        public static string PLAYER_DIE_BOOL = "isDead";
        public static string PLAYER_FORWARDAXIS_FLOAT = "forwardAxis";
        public static string PLAYER_RIGHTDAXIS_FLOAT = "rightAxis";
        #endregion

        #region WEAPON
        public static string WEAPON_ATTACK_TRIGGER = "attack";
        public static string WEAPON_ATTACKSPEED_FLOAT = "attackSpeed";
        #endregion

        #region ENEMIES
        public static string ENEMY_TAKEDAMAGE_TRIGGER = "takeDamage";
        public static string ENEMY_ATTACKFX_TRIGGER = "attackFX";
        public static string ENEMY_DIE_BOOL = "isDead";
        public static string ENEMY_ATTACK_TRIGGER = "attack";
        #endregion
    }

    public static class Path {
        public static string GAME_SETTINGS_PATH = "GameSettings";
    }

    public static class SortingLayer {
        public static string DEFAULT = "Default";
        public static string CHARACTER = "Character";
        public static string UI = "UI";
    }
}
