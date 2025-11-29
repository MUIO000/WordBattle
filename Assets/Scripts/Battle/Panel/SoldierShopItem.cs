using UnityEngine;
using UnityEngine.UI;

public class SoldierShopItem : MonoBehaviour
{
    public Button button;
    public Image iconImage;
    public GameObject soldierPrefab;
    public int cost;

    private Color normalColor = Color.white;
    private Color disabledColor = Color.gray;

    public void SetInteractable(bool interactable)
    {
        // Debug.Log($"{gameObject.name} SetInteractable: {interactable}");
        button.interactable = interactable;
        iconImage.color = interactable ? normalColor : disabledColor;
    }
}
