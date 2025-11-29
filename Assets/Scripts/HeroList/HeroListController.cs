/*
 * 英雄选择界面控制器
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HeroListController : MonoBehaviour
{
    [Header("UI引用")]
    public TextMeshProUGUI heroNameText;              // 英雄名称
    public TextMeshProUGUI heroClassText;             // 英雄职业
    public TextMeshProUGUI heroDescriptionText;       // 英雄描述
    public Image heroClassIcon;                       // 英雄职业图标
    public Image heroSkillIcon;                       // 技能图标
    public TextMeshProUGUI skillDescText;             // 技能描述
    public TextMeshProUGUI currencyText;              // 货币显示
    
    [Header("属性显示")]
    public Slider hpSlider;                           // 生命值滑条
    public Slider atkSlider;                          // 攻击力滑条
    public Slider skillSlider;                        // 技能强度滑条
    public Slider defSlider;                          // 防御力滑条
    public TextMeshProUGUI hpValueText;               // 生命值数值
    public TextMeshProUGUI atkValueText;              // 攻击力数值
    public TextMeshProUGUI skillValueText;            // 技能强度数值
    public TextMeshProUGUI defValueText;              // 防御力数值
    
    [Header("星级和升级")]
    public Transform starContainer;                   // 星星容器
    public Button upgradeButton;                      // 升级按钮
    public TextMeshProUGUI upgradeCostText;           // 升级费用文本
    
    [Header("切换按钮")]
    public Button prevButton;                         // 上一个英雄按钮
    public Button nextButton;                         // 下一个英雄按钮
    public Button selectButton;                       // 选择按钮
    public Button backButton;                         // 返回按钮
    
    [Header("设置")]
    public float maxStatValue = 500f;                 // 属性最大值（用于滑条显示）
    public int playerCurrency = 50000;               // 玩家货币
    
    [Header("英雄模型")]
    public Transform characterRoot;                   // 英雄模型根节点
    
    [Header("加载界面")]
    public GameObject loadingPanel;                   // 加载面板
    
    // 私有变量
    private List<HeroConfig> heroConfigs;
    private List<HeroStatus> heroStatuses;
    private int currentHeroIndex = 0;
    private HeroConfig currentHeroConfig;
    private HeroStatus currentHeroStatus;
    
    void Start()
    {
        LoadData();
        InitializeUI();
        ShowCurrentHero();
    }
    
    // 加载数据
    void LoadData()
    {
        heroConfigs = TestData.GetHeroConfigs();
        heroStatuses = TestData.GetHeroStatuses();
        
        // 确保每个英雄都有对应的状态数据
        foreach (var config in heroConfigs)
        {
            if (!heroStatuses.Any(s => s.heroId == config.heroId))
            {
                // 如果没有状态数据，创建默认的
                heroStatuses.Add(new HeroStatus
                {
                    heroId = config.heroId,
                    isUnlocked = false,
                    isSelected = false,
                    star = 1,
                });
            }
        }
    }
    
    // 初始化UI
    void InitializeUI()
    {
        // 绑定按钮事件
        if (prevButton != null)
            prevButton.onClick.AddListener(PreviousHero);
        if (nextButton != null)
            nextButton.onClick.AddListener(NextHero);
        if (selectButton != null)
            selectButton.onClick.AddListener(SelectCurrentHero);
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(UpgradeCurrentHero);
        if (backButton != null)
            backButton.onClick.AddListener(GoBack);
            
        // 更新货币显示
        UpdateCurrencyDisplay();
    }
    
    // 显示当前英雄
    void ShowCurrentHero()
    {
        if (heroConfigs == null || heroConfigs.Count == 0) return;
        
        currentHeroConfig = heroConfigs[currentHeroIndex];
        currentHeroStatus = heroStatuses.Find(s => s.heroId == currentHeroConfig.heroId);
        
        if (currentHeroConfig == null || currentHeroStatus == null) return;
        
        // 显示基本信息
        if (heroNameText != null)
            heroNameText.text = currentHeroConfig.name;
        if (heroClassText != null)
            heroClassText.text = currentHeroConfig.className;
        if (heroDescriptionText != null)
            heroDescriptionText.text = currentHeroConfig.description;
        if (skillDescText != null)
            skillDescText.text = currentHeroConfig.skillDesc;
            
        // 加载图标（这里用Resources.Load，你可以根据实际情况调整）
        LoadIcon(currentHeroConfig.classIcon, heroClassIcon);
        LoadIcon(currentHeroConfig.skillIcon, heroSkillIcon);
        
        // 显示属性
        ShowHeroStats();
        
        // 显示星级
        ShowStars();
        
        // 更新升级按钮
        UpdateUpgradeButton();
        
        // 更新选择按钮
        UpdateSelectButton();
        
        // 显示英雄模型
        ShowHeroPrefab();
    }
    
    // 加载图标
    void LoadIcon(string iconPath, Image targetImage)
    {
        if (targetImage == null || string.IsNullOrEmpty(iconPath)) return;
        
        // 尝试从Resources加载
        Sprite icon = Resources.Load<Sprite>(iconPath);
        if (icon != null)
        {
            targetImage.sprite = icon;
        }
        else
        {
            Debug.LogWarning($"无法加载图标: {iconPath}");
        }
    }
    
    // 显示英雄属性
    void ShowHeroStats()
    {
        var stats = currentHeroConfig.starStats.Find(s => s.star == currentHeroStatus.star);
        if (stats == null) return;
        
        // 设置滑条和数值
        SetStatSlider(hpSlider, hpValueText, stats.hp);
        SetStatSlider(atkSlider, atkValueText, stats.atk);
        SetStatSlider(skillSlider, skillValueText, stats.skill);
        SetStatSlider(defSlider, defValueText, stats.def);
    }
    
    // 设置属性滑条
    void SetStatSlider(Slider slider, TextMeshProUGUI valueText, int value)
    {
        if (slider != null)
        {
            slider.maxValue = maxStatValue;
            slider.value = value;
        }
        if (valueText != null)
        {
            valueText.text = value.ToString();
        }
    }
    
    // 显示星级
    void ShowStars()
    {
        if (starContainer == null) return;

        int starCount = 5; // 总星数
        int currentStar = currentHeroStatus.star;
        
        // 先统一关闭所有星星（无论之前是否点亮）
        for (int i = 0; i < starCount; i++)
        {
            Transform star = starContainer.Find($"Star ({i})");
            if (star != null)
                star.gameObject.SetActive(false);
        }

        // StarOff 只在未解锁时显示
        Transform starOff = starContainer.Find("StarOff");
        if (!currentHeroStatus.isUnlocked)
        {
            if (starOff != null)
                starOff.gameObject.SetActive(true);
            return;
        }
        
        // 已解锁时隐藏StarOff，显示对应星级
        if (starOff != null)
            starOff.gameObject.SetActive(false);

        // 显示已点亮的星星
        for (int i = 0; i < starCount; i++)
        {
            Transform star = starContainer.Find($"Star ({i})");
            if (star != null)
                star.gameObject.SetActive(i < currentStar);
        }
    }
    
    // 更新升级按钮
    void UpdateUpgradeButton()
    {
        if (upgradeButton == null) return;

        // 未解锁
        if (!currentHeroStatus.isUnlocked)
        {
            if (upgradeCostText != null)
            {
                upgradeCostText.text = $"解锁{currentHeroConfig.unlockCost}";
            }
            if (playerCurrency >= currentHeroConfig.unlockCost)
            {
                upgradeButton.interactable = true;
                upgradeButton.image.color = Color.white;
            }
            else
            {
                upgradeButton.interactable = false;
                upgradeButton.image.color = Color.gray;
            }
            return;
        }

        // 已满级
        if (currentHeroStatus.star >= 5)
        {
            upgradeButton.interactable = false;
            if (upgradeCostText != null)
            {
                upgradeCostText.text = "已满级";
                upgradeButton.image.color = Color.yellow;
            }
            return;
        }

        // 可升级
        int cost = currentHeroConfig.starUpgradeCost[currentHeroStatus.star];
        if (upgradeCostText != null)
        {
            upgradeCostText.text = cost.ToString();
        }
        if (playerCurrency < cost)
        {
            upgradeButton.interactable = false;
            if (upgradeCostText != null) upgradeButton.image.color = Color.gray;
        }
        else
        {
            upgradeButton.interactable = true;
            if (upgradeCostText != null) upgradeButton.image.color = new Color(0f, 1f, 1f, 1f); // 青色
        }
    }
    
    // 更新选择按钮
    void UpdateSelectButton()
    {
        if (selectButton == null) return;

        TextMeshProUGUI buttonText = selectButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText == null) return;

        if (!currentHeroStatus.isUnlocked)
        {
            selectButton.interactable = false;
            buttonText.text = $"未解锁";
            selectButton.image.color = Color.gray;
        }
        else if (currentHeroStatus.isSelected)
        {
            selectButton.interactable = false;
            buttonText.text = "已选择";
            selectButton.image.color = Color.yellow;
        }
        else
        {
            selectButton.interactable = true;
            buttonText.text = "选择";
            selectButton.image.color = Color.white;
        }
    }
    
    // 更新货币显示
    void UpdateCurrencyDisplay()
    {
        if (currencyText != null)
        {
            currencyText.text = playerCurrency.ToString("N0");
        }
    }
    
    // 按钮事件
    public void PreviousHero()
    {
        currentHeroIndex = (currentHeroIndex - 1 + heroConfigs.Count) % heroConfigs.Count;
        ShowCurrentHero();
    }
    
    public void NextHero()
    {
        currentHeroIndex = (currentHeroIndex + 1) % heroConfigs.Count;
        ShowCurrentHero();
    }
    
    public void SelectCurrentHero()
    {
        // 只处理已解锁英雄的选择逻辑
        if (currentHeroStatus.isUnlocked)
        {
            // 取消其他英雄的选择状态
            foreach (var status in heroStatuses)
            {
                status.isSelected = false;
            }
            
            // 选择当前英雄
            currentHeroStatus.isSelected = true;
            UpdateSelectButton();
            
            Debug.Log($"选择英雄: {currentHeroConfig.name}");
        }
        else
        {
            Debug.Log("英雄未解锁，无法选择");
        }
    }
    
    public void UpgradeCurrentHero()
    {
        // 如果未解锁，执行解锁逻辑
        if (!currentHeroStatus.isUnlocked)
        {
            if (playerCurrency >= currentHeroConfig.unlockCost)
            {
                playerCurrency -= currentHeroConfig.unlockCost;
                currentHeroStatus.isUnlocked = true;
                UpdateCurrencyDisplay();
                Debug.Log($"解锁英雄: {currentHeroConfig.name}");
                
                // 播放解锁特效
                StartCoroutine(PlayUnlockEffect(currentHeroConfig.heroId));
                
                // 更新UI状态
                ShowCurrentHero();
            }
            else
            {
                Debug.Log("货币不足，无法解锁");
            }
            return;
        }
        
        // 如果已满级，不能升级
        if (currentHeroStatus.star >= 5) return;
        
        int cost = currentHeroConfig.starUpgradeCost[currentHeroStatus.star];
        if (playerCurrency >= cost)
        {
            playerCurrency -= cost;
            currentHeroStatus.star++;
            
            UpdateCurrencyDisplay();
            ShowCurrentHero();
            
            Debug.Log($"升级英雄: {currentHeroConfig.name} 到 {currentHeroStatus.star} 星");
        }
        else
        {
            Debug.Log("货币不足，无法升级");
        }
    }
    
    public void GoBack()
    {
        // 返回主菜单或上一个场景
        Debug.Log("返回主界面");
        StartCoroutine(GoBackWithLoading());
    }
    
    // 带加载界面的返回协程
    IEnumerator GoBackWithLoading()
    {
        // 显示加载界面
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }
        
        // 短暂等待让用户看到加载界面
        yield return new WaitForSeconds(0.5f);
        
        // 加载场景
        SceneManager.LoadScene("MainMenu");
    }
    
    // 显示英雄模型
    void ShowHeroPrefab()
    {
        if (characterRoot == null || currentHeroConfig == null) return;

        // 1. 切换前清除所有特效
        foreach (Transform child in characterRoot)
        {
            Transform unlockEffect = child.Find("UnlockEffect");
            Transform lockEffect = child.Find("LockEffect");
            if (unlockEffect != null) unlockEffect.gameObject.SetActive(false);
            if (lockEffect != null) lockEffect.gameObject.SetActive(false);
        }

        // 2. 只激活当前英雄
        foreach (Transform child in characterRoot)
        {
            if (child.name == currentHeroConfig.heroId)
            {
                child.gameObject.SetActive(true);
                UpdateHeroPrefabState(child);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    // 更新英雄Prefab状态，控制特效显示
    void UpdateHeroPrefabState(Transform heroObj)
    {
        if (heroObj == null || currentHeroStatus == null) return;

        Transform unlockEffect = heroObj.Find("UnlockEffect");
        Transform lockEffect = heroObj.Find("LockEffect");

        if (currentHeroStatus.isUnlocked)
        {
            // 已解锁：显示解锁特效
            if (unlockEffect != null) unlockEffect.gameObject.SetActive(true);
            if (lockEffect != null) lockEffect.gameObject.SetActive(false);
        }
        else
        {
            // 未解锁：显示锁定特效
            if (unlockEffect != null) unlockEffect.gameObject.SetActive(false);
            if (lockEffect != null) lockEffect.gameObject.SetActive(true);
        }
    }
    
    // 播放解锁特效
    IEnumerator PlayUnlockEffect(string heroId)
    {
        Transform heroObj = null;
        foreach (Transform child in characterRoot)
        {
            if (child.name == heroId)
            {
                heroObj = child;
                break;
            }
        }
        
        if (heroObj == null) yield break;
        
        Transform unlockEffect = heroObj.Find("UnlockEffect");
        Transform lockEffect = heroObj.Find("LockEffect");
        
        // 播放解锁特效
        if (unlockEffect != null) unlockEffect.gameObject.SetActive(true);
        if (lockEffect != null) lockEffect.gameObject.SetActive(false);
    }
}
