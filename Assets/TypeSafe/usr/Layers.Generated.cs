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



public sealed class SRLayers {
    
    private SRLayers() {
    }
    
    private const string _tsInternal = "1.5.0";
    
    public static global::TypeSafe.Layer Default {
        get {
            return @__all[0];
        }
    }
    
    public static global::TypeSafe.Layer TransparentFX {
        get {
            return @__all[1];
        }
    }
    
    public static global::TypeSafe.Layer Ignore_Raycast {
        get {
            return @__all[2];
        }
    }
    
    public static global::TypeSafe.Layer Water {
        get {
            return @__all[3];
        }
    }
    
    public static global::TypeSafe.Layer UI {
        get {
            return @__all[4];
        }
    }
    
    public static global::TypeSafe.Layer Player {
        get {
            return @__all[5];
        }
    }
    
    public static global::TypeSafe.Layer Enemy {
        get {
            return @__all[6];
        }
    }
    
    public static global::TypeSafe.Layer Wall {
        get {
            return @__all[7];
        }
    }
    
    public static global::TypeSafe.Layer Pickup {
        get {
            return @__all[8];
        }
    }
    
    public static global::TypeSafe.Layer Projectiles {
        get {
            return @__all[9];
        }
    }
    
    public static global::TypeSafe.Layer Traps {
        get {
            return @__all[10];
        }
    }
    
    public static global::TypeSafe.Layer Effects {
        get {
            return @__all[11];
        }
    }
    
    private static global::System.Collections.Generic.IList<global::TypeSafe.Layer> @__all = new global::System.Collections.ObjectModel.ReadOnlyCollection<global::TypeSafe.Layer>(new global::TypeSafe.Layer[] {
                new global::TypeSafe.Layer("Default", 0),
                new global::TypeSafe.Layer("TransparentFX", 1),
                new global::TypeSafe.Layer("Ignore Raycast", 2),
                new global::TypeSafe.Layer("Water", 4),
                new global::TypeSafe.Layer("UI", 5),
                new global::TypeSafe.Layer("Player", 6),
                new global::TypeSafe.Layer("Enemy", 7),
                new global::TypeSafe.Layer("Wall", 8),
                new global::TypeSafe.Layer("Pickup", 9),
                new global::TypeSafe.Layer("Projectiles", 10),
                new global::TypeSafe.Layer("Traps", 11),
                new global::TypeSafe.Layer("Effects", 12)});
    
    public static global::System.Collections.Generic.IList<global::TypeSafe.Layer> All {
        get {
            return @__all;
        }
    }
}
