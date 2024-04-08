// ------------------------------------------------------------------------------
//  _______   _____ ___ ___   _   ___ ___ 
// |_   _\ \ / / _ \ __/ __| /_\ | __| __|
//   | |  \ V /|  _/ _|\__ \/ _ \| _|| _| 
//   |_|   |_| |_| |___|___/_/ \_\_| |___|
// 
// This file has been generated automatically by TypeSafe.
// Any changes to this file may be lost when it is regenerated.
// https://www.stompyrobot.uk/tools/typesafe
// 
// TypeSafe Version: 1.5.0
// 
// ------------------------------------------------------------------------------



public sealed class SRAnimators {
    
    private SRAnimators() {
    }
    
    private const string _tsInternal = "1.5.0";
    
    public sealed class PlayerAnimator {
        
        private PlayerAnimator() {
        }
        
        public sealed class Parameters {
            
            private Parameters() {
            }
            
            public const int isDashing = 1756643129;
            
            public const int takeDamage = -1533413595;
            
            public const int rightAxis = 1432685542;
            
            public const int upAxis = 394668998;
            
            public const int die = 964231127;
        }
        
        public sealed class Layers {
            
            private Layers() {
            }
            
            public const int Base_Layer = 0;
            
            public const int Effect_Layer = 1;
        }
    }
    
    public sealed class EnemyBaseAnimator {
        
        private EnemyBaseAnimator() {
        }
        
        public sealed class Parameters {
            
            private Parameters() {
            }
            
            public const int isDead = 1276664872;
            
            public const int takeDamage = -1533413595;
            
            public const int attack = 1203776827;
            
            public const int up = 1133833840;
            
            public const int right = -1261800172;
            
            public const int die = 964231127;
        }
        
        public sealed class Layers {
            
            private Layers() {
            }
            
            public const int Base_Layer = 0;
            
            public const int Effects_Layer = 1;
        }
    }
    
    public sealed class WeaponBaseAnim {
        
        private WeaponBaseAnim() {
        }
        
        public sealed class Parameters {
            
            private Parameters() {
            }
            
            public const int attack = 1203776827;
            
            public const int attackSpeed = 1691967492;
        }
        
        public sealed class Layers {
            
            private Layers() {
            }
            
            public const int Base_Layer = 0;
        }
    }
    
    public sealed class ProjectileAnimBase {
        
        private ProjectileAnimBase() {
        }
        
        public sealed class Parameters {
            
            private Parameters() {
            }
            
            public const int hit = 1523721793;
            
            public const int falloff = 1332555000;
            
            public const int obstructed = -722625195;
        }
        
        public sealed class Layers {
            
            private Layers() {
            }
            
            public const int Base_Layer = 0;
        }
    }
}
