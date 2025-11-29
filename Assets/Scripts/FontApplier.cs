using TMPro;
using UnityEngine;

public class FontApplier : MonoBehaviour
{
    public TMP_FontAsset targetFont; // 拖入你想统一使用的字体

    void Start()
    {
        ApplyFontToAll();
    }

    public void ApplyFontToAll()
    {
        TextMeshProUGUI[] allTexts = Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
        foreach (var tmp in allTexts)
        {
            tmp.font = targetFont;
        }
    }
}
