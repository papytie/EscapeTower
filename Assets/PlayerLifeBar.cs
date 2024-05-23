using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLifeBar : MonoBehaviour
{
    [SerializeField] Slider lifeBar;
    [SerializeField] PlayerLifeSystem playerLifeSystem;
    [SerializeField] TextMeshProUGUI textMeshPro;

    private void Awake()
    {
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
    }
    private void Update()
    {
        lifeBar.value = playerLifeSystem.CurrentLifePoints / playerLifeSystem.MaxLifePoints;
        textMeshPro.text = playerLifeSystem.CurrentLifePoints.ToString() + "/" + playerLifeSystem.MaxLifePoints.ToString();
    }
}
