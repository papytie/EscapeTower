using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusUI : MonoBehaviour {

    public int Stack { get; private set; }

    [SerializeField] private Image bonusImage;
    [SerializeField] private TextMeshProUGUI stackText;

    public void CreateBonus(Sprite bonusSprite, int numStack) {
        bonusImage.sprite = bonusSprite;
        //bonusImage.color = bonusColor;
        UpdateStack(numStack);
    }

    public void DestroyBonus() {
        Destroy(gameObject);
    }

    public void UpdateStack(int numStack) {
        Stack = numStack;
        stackText.text = Stack.ToString();
    }
}
