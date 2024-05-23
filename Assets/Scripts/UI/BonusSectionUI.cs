using System.Collections.Generic;
using UnityEngine;

public class BonusSectionUI : MonoBehaviour {

    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private BonusUI bonusPrefab;

    private Dictionary<StatModifierTemplate, BonusUI> bonuses = new();
    private List<StatModifierTemplate> toDelete = new();

    private void Update() {
        foreach(var item in playerStats.CurrentBonuses) {
            // Create Bonus
            if(!bonuses.ContainsKey(item.Key)) {
                bonuses.Add(item.Key, Instantiate(bonusPrefab, transform));
                bonuses[item.Key].CreateBonus(item.Key.Sprite, item.Value);
            // Update Bonus
            } else if(bonuses[item.Key].Stack != item.Value) {
                bonuses[item.Key].UpdateStack(item.Value);
            }
        }

        // Delete bonus
        foreach(var item in bonuses) {
            if(!playerStats.CurrentBonuses.ContainsKey(item.Key)) {
                item.Value.DestroyBonus();
                toDelete.Add(item.Key);
            }
        }
        toDelete.ForEach(item => bonuses.Remove(item));
        toDelete.Clear();
    }
}
