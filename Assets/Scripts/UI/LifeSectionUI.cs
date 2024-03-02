using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeSectionUI : MonoBehaviour {

    [SerializeField] private PlayerLifeSystem playerLife;
    [SerializeField] private LifePointUI lifePointPrefab;

    private int CurrentLife => Mathf.RoundToInt(playerLife.CurrentLifePoints);
    private int MaxLife => Mathf.RoundToInt(playerLife.MaxLifePoints);

    private List<LifePointUI> lifePoints = new();


    private void Start() {
        for(int i = 0; i < MaxLife; i++) {
            LifePointUI lifePoint = Instantiate(lifePointPrefab, transform);
            lifePoint.CreateLifePoint(CurrentLife >= i + 1);
            lifePoints.Add(lifePoint);
        } 
    }


    private void Update() {
        while(lifePoints.Count > MaxLife) {
            lifePoints[^1].DestroyLifePoint();
            lifePoints.RemoveAt(lifePoints.Count - 1);
        }

        for(int i = 0; i < lifePoints.Count; i++) {
            LifePointUI lifePoint = lifePoints[i];
            if(CurrentLife < i + 1 && !lifePoint.IsLost) {
                lifePoint.LoseLifePoint();
            } else if(CurrentLife >= i + 1 && lifePoint.IsLost) {
                lifePoint.RestoreLifePoint();
            }
        }
    }
}
