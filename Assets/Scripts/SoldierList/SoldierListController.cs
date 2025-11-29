/*
 * 士兵选择界面控制器
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SoldierListController : MonoBehaviour
{
    [Header("UI引用")]
    public TextMeshProUGUI soldierNameText;           // 士兵名称
    public TextMeshProUGUI soldierDescText;           // 士兵描述
    public TextMeshProUGUI currencyText;              // 货币显示
    
    [Header("职业按钮")]
    public Button archerButton;                       // 弓箭手按钮
    public Button warriorButton;                      // 战士按钮  
    public Button mageButton;                         // 法师按钮
    public Button assassinButton;                     // 刺客按钮
    
    [Header("属性显示")]
    public Slider hpSlider;                           // 生命值滑条
    public Slider atkSlider;                          // 攻击力滑条
    public Slider defSlider;                          // 防御力滑条
    public TextMeshProUGUI hpValueText;               // 生命值数值
    public TextMeshProUGUI atkValueText;              // 攻击力数值
    public TextMeshProUGUI defValueText;              // 防御力数值
    
    [Header("等级和升级")]
    public Transform levelContainer;                  // 等级图标容器
    public Image levelIcon;                           // 等级图标
    public TextMeshProUGUI levelText;                 // 等级文字
    public Button upgradeButton;                      // 升级按钮
    public TextMeshProUGUI upgradeCostText;           // 升级费用文本
    
    [Header("切换和选择")]
    public Button prevButton;                         // 上一个士兵按钮
    public Button nextButton;                         // 下一个士兵按钮
    public Button unlockSelectButton;                 // 选择按钮
    public TextMeshProUGUI unlockSelectText;          // 选择按钮文字
    public Button confirmButton;                      // 确认选择按钮
    public Button backButton;                         // 返回按钮
    
    [Header("设置")]
    public float maxStatValue = 50f;                  // 属性最大值（用于滑条显示）
    public int playerCurrency = 50000;                // 玩家货币
    
    [Header("士兵模型")]
    public Transform characterRoot;                   // 士兵模型根节点
    
    [Header("加载界面")]
    public GameObject loadingPanel;                   // 加载面板
    
    [Header("职业按钮父物体")]
    public Transform archerRoleButtonRoot;
    public Transform warriorRoleButtonRoot;
    public Transform mageRoleButtonRoot;
    public Transform assassinRoleButtonRoot;
    
    // 私有变量
    private List<SoldierConfig> allSoldierConfigs;    // 所有士兵配置
    private List<SoldierConfig> currentClassSoldiers; // 当前职业的士兵列表
    private List<SoldierStatus> soldierStatuses;      // 士兵状态列表
    private List<string> selectedSoldierIds;          // 已选择的士兵ID列表
    
    private string currentClass = "archer";           // 当前选择的职业
    private int currentSoldierIndex = 0;              // 当前士兵索引
    private SoldierConfig currentSoldierConfig;       // 当前士兵配置
    private SoldierStatus currentSoldierStatus;       // 当前士兵状态
    
    void Start()
    {
        LoadData();
        InitializeUI();
        SelectClass("archer"); // 默认选择弓箭手
    }
    
    // 加载数据
    void LoadData()
    {
        allSoldierConfigs = SoldierTestData.GetSoldierConfigs();
        soldierStatuses = SoldierTestData.GetSoldierStatuses();
        selectedSoldierIds = new List<string>();
        
        // 确保每个士兵都有状态数据
        foreach (var config in allSoldierConfigs)
        {
            if (!soldierStatuses.Any(s => s.soldierId == config.soldierId))
            {
                soldierStatuses.Add(new SoldierStatus
                {
                    soldierId = config.soldierId,
                    isUnlocked = false,
                    isSelected = false
                });
            }
        }
    }
    
    // 初始化UI
    void InitializeUI()
    {
        // 绑定职业按钮
        if (archerButton != null)
            archerButton.onClick.AddListener(() => SelectClass("archer"));
        if (warriorButton != null)
            warriorButton.onClick.AddListener(() => SelectClass("warrior"));
        if (mageButton != null)
            mageButton.onClick.AddListener(() => SelectClass("mage"));
        if (assassinButton != null)
            assassinButton.onClick.AddListener(() => SelectClass("assassin"));
            
        // 绑定其他按钮
        if (prevButton != null)
            prevButton.onClick.AddListener(PreviousSoldier);
        if (nextButton != null)
            nextButton.onClick.AddListener(NextSoldier);
        if (unlockSelectButton != null)
            unlockSelectButton.onClick.AddListener(UnlockOrSelectSoldier);
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(UpgradeCurrentSoldier);
        if (confirmButton != null)
            confirmButton.onClick.AddListener(ConfirmSelection);
        if (backButton != null)
            backButton.onClick.AddListener(GoBack);
            
        UpdateCurrencyDisplay();
    }
    
    // 选择职业
    void SelectClass(string className)
    {
        currentClass = className;
        currentClassSoldiers = allSoldierConfigs.Where(s => s.soldierClass == className).ToList();
        currentSoldierIndex = 0;
        
        // 查找当前职业中是否有已选择的士兵，如果有则优先显示
        for (int i = 0; i < currentClassSoldiers.Count; i++)
        {
            var status = soldierStatuses.Find(s => s.soldierId == currentClassSoldiers[i].soldierId);
            if (status != null && status.isSelected)
            {
                currentSoldierIndex = i;
                break;
            }
        }
        
        // 更新职业按钮高亮
        UpdateClassButtonHighlight();
        
        // 显示当前职业的士兵
        ShowCurrentSoldier();
    }
    
    // 更新职业按钮高亮
    void UpdateClassButtonHighlight()
    {
        SetRoleButtonHighlight(archerRoleButtonRoot, currentClass == "archer");
        SetRoleButtonHighlight(warriorRoleButtonRoot, currentClass == "warrior");
        SetRoleButtonHighlight(mageRoleButtonRoot, currentClass == "mage");
        SetRoleButtonHighlight(assassinRoleButtonRoot, currentClass == "assassin");
    }
    
    void SetRoleButtonHighlight(Transform roleButtonRoot, bool isFocus)
    {
        if (roleButtonRoot == null) return;
        var normal = roleButtonRoot.Find("RoleButton");
        var focus = roleButtonRoot.Find("RoleButton_Focus");
        if (normal != null) normal.gameObject.SetActive(!isFocus);
        if (focus != null) focus.gameObject.SetActive(isFocus);
    }
    
    // 显示当前士兵
    void ShowCurrentSoldier()
    {
        if (currentClassSoldiers == null || currentClassSoldiers.Count == 0) return;
        
        currentSoldierConfig = currentClassSoldiers[currentSoldierIndex];
        currentSoldierStatus = soldierStatuses.Find(s => s.soldierId == currentSoldierConfig.soldierId);
        
        if (currentSoldierConfig == null || currentSoldierStatus == null) return;
        
        // 显示基本信息
        if (soldierNameText != null)
            soldierNameText.text = currentSoldierConfig.name;
        if (soldierDescText != null)
            soldierDescText.text = currentSoldierConfig.description;
            
        // 显示属性
        ShowSoldierStats();
        
        // 显示等级
        ShowSoldierLevel();
        
        // 更新升级按钮
        UpdateUpgradeButton();
        
        // 更新选择按钮
        UpdateUnlockSelectButton();
        
        // 显示士兵模型
        ShowSoldierPrefab();
    }
    
    // 显示士兵属性
    void ShowSoldierStats()
    {
        var stats = currentSoldierConfig.levelStats.Find(s => s.level == currentSoldierStatus.level);
        if (stats == null) return;
        
        SetStatSlider(hpSlider, hpValueText, stats.hp);
        SetStatSlider(atkSlider, atkValueText, stats.atk);
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
    
    // 显示士兵等级
    void ShowSoldierLevel()
    {
        if (levelContainer == null) return;
        
        // 只有升级后（level > 0）才显示等级
        if (currentSoldierStatus.level > 0)
        {
            levelContainer.gameObject.SetActive(true);
            if (levelText != null)
                levelText.text = currentSoldierStatus.level.ToString();
        }
        else
        {
            levelContainer.gameObject.SetActive(false);
        }
    }
    
    // 更新升级按钮
    void UpdateUpgradeButton()
    {
        if (upgradeButton == null) return;

        // 未解锁
        if (!currentSoldierStatus.isUnlocked)
        {
            if (upgradeCostText != null)
            {
                upgradeCostText.text = $"解锁{currentSoldierConfig.unlockCost}";
            }
            if (playerCurrency >= currentSoldierConfig.unlockCost)
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
        if (currentSoldierStatus.level >= 3)
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
        int cost = currentSoldierConfig.upgradeCost[currentSoldierStatus.level];
        if (upgradeCostText != null)
        {
            upgradeCostText.text = cost.ToString();
        }
        if (playerCurrency < cost)
        {
            upgradeButton.interactable = false;
            upgradeButton.image.color = Color.gray;
        }
        else
        {
            upgradeButton.interactable = true;
            upgradeButton.image.color = new Color(0f, 1f, 1f, 1f); // 青色
        }
    }
    
    // 更新选择按钮
    void UpdateUnlockSelectButton()
    {
        if (unlockSelectButton == null || unlockSelectText == null) return;

        if (!currentSoldierStatus.isUnlocked)
        {
            unlockSelectButton.interactable = false;
            unlockSelectText.text = "未解锁";
            unlockSelectButton.image.color = Color.gray;
        }
        else if (currentSoldierStatus.isSelected)
        {
            unlockSelectButton.interactable = false;
            unlockSelectText.text = "已选择";
            unlockSelectButton.image.color = Color.yellow;
        }
        else
        {
            unlockSelectButton.interactable = true;
            unlockSelectText.text = "选择";
            unlockSelectButton.image.color = Color.white;
        }
    }
    
    // 更新货币显示
    void UpdateCurrencyDisplay()
    {
        if (currencyText != null)
        {
            currencyText.text = playerCurrency.ToString("N0") + " +";
        }
    }
    
    // 上一个士兵
    public void PreviousSoldier()
    {
        if (currentClassSoldiers.Count == 0) return;
        currentSoldierIndex = (currentSoldierIndex - 1 + currentClassSoldiers.Count) % currentClassSoldiers.Count;
        ShowCurrentSoldier();
    }
    
    // 下一个士兵
    public void NextSoldier()
    {
        if (currentClassSoldiers.Count == 0) return;
        currentSoldierIndex = (currentSoldierIndex + 1) % currentClassSoldiers.Count;
        ShowCurrentSoldier();
    }
    
    // 选择士兵
    public void UnlockOrSelectSoldier()
    {
        // 只处理已解锁士兵的选择逻辑
        if (currentSoldierStatus.isUnlocked && !currentSoldierStatus.isSelected)
        {
            // 取消同职业其他士兵的选择状态
            foreach (var config in currentClassSoldiers)
            {
                var status = soldierStatuses.Find(s => s.soldierId == config.soldierId);
                if (status != null && status.isSelected)
                {
                    status.isSelected = false;
                    selectedSoldierIds.Remove(config.soldierId);
                }
            }
            
            // 选择当前士兵
            currentSoldierStatus.isSelected = true;
            selectedSoldierIds.Add(currentSoldierConfig.soldierId);
            UpdateUnlockSelectButton();
            Debug.Log($"选择士兵: {currentSoldierConfig.name}");
        }
        else if (currentSoldierStatus.isUnlocked)
        {
            Debug.Log("士兵已选择");
        }
        else
        {
            Debug.Log("士兵未解锁，无法选择");
        }
    }
    
    // 确认选择
    public void ConfirmSelection()
    {
        if (selectedSoldierIds.Count > 0)
        {
            Debug.Log($"确认选择了 {selectedSoldierIds.Count} 个士兵");
            // TODO: 保存选择的士兵，进入下一个场景
        }
        else
        {
            Debug.Log("请至少选择一个士兵");
        }
    }
    
    // 返回
    public void GoBack()
    {
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
    
    // 显示士兵模型
    void ShowSoldierPrefab()
    {
        if (characterRoot == null || currentSoldierConfig == null) return;
        
        // 隐藏所有士兵模型
        foreach (Transform child in characterRoot)
        {
            child.gameObject.SetActive(false);
            
            // 清除特效
            Transform unlockEffect = child.Find("UnlockEffect");
            Transform lockEffect = child.Find("LockEffect");
            if (unlockEffect != null) unlockEffect.gameObject.SetActive(false);
            if (lockEffect != null) lockEffect.gameObject.SetActive(false);
        }
        
        // 显示当前士兵
        Transform soldierObj = characterRoot.Find(currentSoldierConfig.soldierId);
        if (soldierObj != null)
        {
            soldierObj.gameObject.SetActive(true);
            UpdateSoldierPrefabState(soldierObj);
        }
    }
    
    // 更新士兵模型状态
    void UpdateSoldierPrefabState(Transform soldierObj)
    {
        if (soldierObj == null || currentSoldierStatus == null) return;
        
        Transform unlockEffect = soldierObj.Find("UnlockEffect");
        Transform lockEffect = soldierObj.Find("LockEffect");
        
        if (currentSoldierStatus.isUnlocked)
        {
            // 已解锁
            if (unlockEffect != null) unlockEffect.gameObject.SetActive(true);
            if (lockEffect != null) lockEffect.gameObject.SetActive(false);
        }
        else
        {
            // 未解锁
            if (unlockEffect != null) unlockEffect.gameObject.SetActive(false);
            if (lockEffect != null) lockEffect.gameObject.SetActive(true);
        }
    }
    
    // 播放解锁特效
    IEnumerator PlayUnlockEffect(string soldierId)
    {
        Transform soldierObj = characterRoot.Find(soldierId);
        if (soldierObj == null) yield break;
        
        Transform unlockEffect = soldierObj.Find("UnlockEffect");
        Transform lockEffect = soldierObj.Find("LockEffect");
        
        // 播放解锁特效
        if (unlockEffect != null) unlockEffect.gameObject.SetActive(true);
        if (lockEffect != null) lockEffect.gameObject.SetActive(false);
    }
    
    // 升级当前士兵
    public void UpgradeCurrentSoldier()
    {
        // 如果未解锁，执行解锁逻辑
        if (!currentSoldierStatus.isUnlocked)
        {
            if (playerCurrency >= currentSoldierConfig.unlockCost)
            {
                playerCurrency -= currentSoldierConfig.unlockCost;
                currentSoldierStatus.isUnlocked = true;
                UpdateCurrencyDisplay();
                Debug.Log($"解锁士兵: {currentSoldierConfig.name}");
                
                // 播放解锁特效
                StartCoroutine(PlayUnlockEffect(currentSoldierConfig.soldierId));
                
                // 更新UI状态
                ShowCurrentSoldier();
            }
            else
            {
                Debug.Log("货币不足，无法解锁");
            }
            return;
        }
        
        // 如果已满级，不能升级
        if (currentSoldierStatus.level >= 3) return;
        
        int cost = currentSoldierConfig.upgradeCost[currentSoldierStatus.level];
        if (playerCurrency >= cost)
        {
            playerCurrency -= cost;
            currentSoldierStatus.level++;
            
            UpdateCurrencyDisplay();
            ShowCurrentSoldier();
            
            Debug.Log($"升级士兵: {currentSoldierConfig.name} 到等级 {currentSoldierStatus.level}");
        }
        else
        {
            Debug.Log("货币不足，无法升级");
        }
    }
} 