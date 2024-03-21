using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[CreateAssetMenu]
public class CustomRuleAny : RuleTile<CustomRuleAny.Neighbor> {
    public bool alwaysConnect;
    public TileBase[] tilesToConnect;
    public bool checkSelf;


    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Any = 3;
        public const int Nothing = 4;

    }
    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch(neighbor) {
            case Neighbor.This: return Check_This(tile);
            case Neighbor.NotThis: return Check_NotThis(tile);
            case Neighbor.Any: return Check_Any(tile);
            case Neighbor.Nothing: return Check_Nothing(tile);

        }
        return base.RuleMatch(neighbor, tile);
    }

    bool Check_This(TileBase tile) {

        if(!alwaysConnect) return tile == this;
        else return tilesToConnect.Contains(tile) || tile == this;
    }

    bool Check_NotThis(TileBase tile) {

        return tile != this;
    }
    bool Check_Any(TileBase tile) {

        if(checkSelf) return tile != null;
        else return tile != null && tile != this;
    }

    bool Check_Nothing(TileBase tile) {

        return tile == null;
    }
}