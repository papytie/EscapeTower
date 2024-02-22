using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class NewCustomRuleTile : RuleTile<NewCustomRuleTile.Neighbor>
{
    public List<TileBase> sibings = new List<TileBase>();

    //on ajoute des voisins qui font matcher la tile, un voisin renvoit THIS
    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Sibing = 3;
        public const int troudbal = 4;
    }
    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch(neighbor) {
            case Neighbor.Sibing: return sibings.Contains(tile);
            case Neighbor.troudbal: return sibings.Contains(tile);
        }
        return base.RuleMatch(neighbor, tile);
    }
}