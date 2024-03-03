public class GameParams
{
    public class Animation
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
}
