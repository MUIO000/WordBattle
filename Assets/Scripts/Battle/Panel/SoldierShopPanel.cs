using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class SoldierShopPanel : MonoBehaviour
{
    public SoldierShopItem[] shopItems;
    public TextMeshProUGUI coinText;
    public Transform gridParent; // 棋盘格父物体
    public List<Transform> gridCells; // 棋盘格所有可用格子
    private SummonerController summoner; // 添加召唤师引用

    private int coins = 0;

    private void Start()
    {
        StartCoroutine(FindSummonerNextFrame());
        UpdateCoinUI();
        foreach (var item in shopItems)
        {
            item.button.onClick.AddListener(() => OnShopItemClicked(item));
        }
        UpdateShopInteractable();
    }

    private IEnumerator FindSummonerNextFrame()
    {
        yield return null; // 等一帧
        summoner = FindObjectOfType<SummonerController>();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateCoinUI();
        UpdateShopInteractable();
    }

    void OnShopItemClicked(SoldierShopItem item)
    {
        if (coins < item.cost || summoner == null) return;

        // 获取所有空格子
        List<Transform> emptyCells = new List<Transform>();
        foreach (var cell in gridCells)
        {
            if (cell.childCount == 0)
                emptyCells.Add(cell);
        }
        if (emptyCells.Count == 0) return;

        // 随机选择一个空格子
        int randomIndex = Random.Range(0, emptyCells.Count);
        Transform selectedCell = emptyCells[randomIndex];
        
        // 使用召唤师在选中的位置召唤士兵
        summoner.SummonSoldier(item.soldierPrefab, selectedCell);

        // 扣除金币并更新UI
        coins -= item.cost;
        UpdateCoinUI();
        UpdateShopInteractable();
    }

    void UpdateCoinUI()
    {
        coinText.text = coins.ToString();
    }

    void UpdateShopInteractable()
    {
        foreach (var item in shopItems)
        {
            // Debug.Log($"当前金币：{coins}，当前商品价格：{item.cost}");
            item.SetInteractable(coins >= item.cost);
        }
    }
}
