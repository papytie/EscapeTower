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
        public static string ENEMY_DIE_BOOL = "isDead";
        #endregion
    }

}