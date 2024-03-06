using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LifePointUI : MonoBehaviour {

    public bool IsLost { get; private set; }

    [SerializeField] private Image lifeImage;


    public void CreateLifePoint(bool isLost) {
        IsLost = isLost;
        lifeImage.color = IsLost ? Color.black : Color.white;
    }

    public void DestroyLifePoint() {
        IsLost = true;
        Destroy(gameObject);
    }

    public void LoseLifePoint() {
        IsLost = true;
        lifeImage.color = Color.black;
    }

    public void RestoreLifePoint() {
        IsLost = false;
        lifeImage.color = Color.white;
    }
}
