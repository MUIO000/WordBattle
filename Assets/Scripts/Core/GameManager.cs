using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class GameManager : MonoBehaviour
{
    [Header("游戏管理器 - 单例")]
    public static GameManager Instance;
    
    [Header("当前关卡设置")]
    public LevelData currentLevel;
    
    [Header("游戏状态")]
    public GameState currentGameState;
    public bool isGameActive = false;
    public bool isPaused = false;
    
    [Header("玩家状态")]
    public int currentPlayerHealth;
    public int playerScore = 0;
    public int correctAnswers = 0;
    public int totalQuestions = 0;
    
    [Header("Boss状态")]
    public int currentBossHealth;
    
    [Header("时间管理")]
    public float remainingTime;
    public float gameStartTime;
    
    [Header("事件系统")]
    public UnityEvent OnGameStart;
    public UnityEvent OnGamePause;
    public UnityEvent OnGameResume;
    public UnityEvent<bool> OnGameEnd; // true = victory, false = defeat
    public UnityEvent<int> OnPlayerHealthChanged;
    public UnityEvent<int> OnBossHealthChanged;
    public UnityEvent<int> OnScoreChanged;
    
    [Header("场景与UI")]
    public GameObject loadingPanel;
    public Button returnButton;
    public Button restartButton;

    private void Awake()
    {
        // 单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        InitializeGame();
        BindManualButtons();
        FindAndBindButtons();
    }
    
    private void Update()
    {
        if (isGameActive && !isPaused)
        {
            UpdateGameTimer();
            CheckGameEndConditions();
        }
    }
    
    // 初始化游戏
    public void InitializeGame()
    {
        if (currentLevel == null)
        {
            Debug.LogWarning("未设置当前关卡数据！");
            return;
        }
        
        // 初始化游戏状态
        currentGameState = GameState.Preparing;
        isGameActive = false;
        isPaused = false;
        
        // 初始化玩家状态
        currentPlayerHealth = currentLevel.playerMaxHealth;
        playerScore = 0;
        correctAnswers = 0;
        totalQuestions = 0;
        
        // 初始化Boss状态
        currentBossHealth = currentLevel.bossMaxHealth;
        
        // 初始化时间
        remainingTime = currentLevel.timeLimit;
        gameStartTime = Time.time;
        
        // 触发事件
        OnPlayerHealthChanged?.Invoke(currentPlayerHealth);
        OnBossHealthChanged?.Invoke(currentBossHealth);
        OnScoreChanged?.Invoke(playerScore);
        
        Debug.Log($"游戏初始化完成 - 关卡: {currentLevel.levelName}");
    }
    
    // 开始游戏
    public void StartGame()
    {
        if (currentLevel == null)
        {
            Debug.LogError("无法开始游戏：未设置关卡数据！");
            return;
        }
        
        currentGameState = GameState.Playing;
        isGameActive = true;
        isPaused = false;
        gameStartTime = Time.time;
        
        OnGameStart?.Invoke();
        Debug.Log("游戏开始！");
    }
    
    // 暂停游戏
    public void PauseGame()
    {
        if (isGameActive)
        {
            isPaused = true;
            Time.timeScale = 0f;
            OnGamePause?.Invoke();
            Debug.Log("游戏已暂停");
        }
    }
    
    // 恢复游戏
    public void ResumeGame()
    {
        if (isGameActive && isPaused)
        {
            isPaused = false;
            Time.timeScale = 1f;
            OnGameResume?.Invoke();
            Debug.Log("游戏已恢复");
        }
    }
    
    // 结束游戏
    public void EndGame(bool isVictory)
    {
        isGameActive = false;
        Time.timeScale = 1f;
        
        currentGameState = isVictory ? GameState.Victory : GameState.Defeat;
        
        // 计算最终分数
        if (isVictory)
        {
            int timeBonus = Mathf.RoundToInt(remainingTime * 10);
            int accuracyBonus = totalQuestions > 0 ? 
                Mathf.RoundToInt((float)correctAnswers / totalQuestions * 100) : 0;
            
            playerScore += timeBonus + accuracyBonus;
            Debug.Log($"胜利！最终分数: {playerScore} (时间奖励: {timeBonus}, 准确率奖励: {accuracyBonus})");
        }
        
        OnGameEnd?.Invoke(isVictory);
        Debug.Log($"游戏结束 - {(isVictory ? "胜利" : "失败")}");
    }
    
    // 更新游戏计时器
    private void UpdateGameTimer()
    {
        remainingTime -= Time.deltaTime;
        
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            EndGame(false); // 时间到 = 失败
        }
    }
    
    // 检查游戏结束条件
    private void CheckGameEndConditions()
    {
        // 玩家血量为0 = 失败
        if (currentPlayerHealth <= 0)
        {
            EndGame(false);
        }
        // Boss血量为0 = 胜利
        else if (currentBossHealth <= 0)
        {
            EndGame(true);
        }
    }
    
    // 玩家受伤
    public void DamagePlayer(int damage)
    {
        currentPlayerHealth = Mathf.Max(0, currentPlayerHealth - damage);
        OnPlayerHealthChanged?.Invoke(currentPlayerHealth);
        
        Debug.Log($"玩家受到 {damage} 点伤害，剩余血量: {currentPlayerHealth}");
    }
    
    // Boss受伤
    public void DamageBoss(int damage)
    {
        currentBossHealth = Mathf.Max(0, currentBossHealth - damage);
        OnBossHealthChanged?.Invoke(currentBossHealth);
        
        Debug.Log($"Boss受到 {damage} 点伤害，剩余血量: {currentBossHealth}");
    }
    
    // 增加分数
    public void AddScore(int points)
    {
        playerScore += points;
        OnScoreChanged?.Invoke(playerScore);
        
        Debug.Log($"获得 {points} 分，总分: {playerScore}");
    }
    
    // 答题正确
    public void OnCorrectAnswer(int points)
    {
        correctAnswers++;
        totalQuestions++;
        AddScore(points);
        
        Debug.Log($"答题正确！正确率: {correctAnswers}/{totalQuestions}");
    }
    
    // 答题错误
    public void OnWrongAnswer()
    {
        totalQuestions++;
        Debug.Log($"答题错误！正确率: {correctAnswers}/{totalQuestions}");
    }
    
    // 重新开始游戏（带加载面板）
    public void RestartGame()
    {
        Debug.Log("重新开始游戏...");
        Time.timeScale = 1f;
        StopAllCoroutines();
        StartCoroutine(LoadSceneWithLoading(SceneManager.GetActiveScene().name));
    }
    
    // 返回主菜单（带加载面板）
    public void ReturnToMainMenu()
    {
        Debug.Log("返回主菜单...");
        Time.timeScale = 1f;
        StopAllCoroutines();
        StartCoroutine(LoadSceneWithLoading("MainMenu"));
    }
    
    // 加载下一关
    public void LoadNextLevel()
    {
        // 这里后续可以实现关卡切换逻辑
        Debug.Log("加载下一关（待实现）");
    }

    #region 场景与按钮绑定

    private void BindManualButtons()
    {
        if (returnButton != null)
        {
            returnButton.onClick.RemoveListener(ReturnToMainMenu);
            returnButton.onClick.AddListener(ReturnToMainMenu);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartGame);
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    /// <summary>
    /// 自动查找并绑定所有 Exit / Restart 按钮
    /// </summary>
    private void FindAndBindButtons()
    {
        Button[] allButtons = FindObjectsOfType<Button>(true);
        int exitButtonsFound = 0;
        int restartButtonsFound = 0;

        foreach (Button button in allButtons)
        {
            string buttonName = button.gameObject.name;

            if (buttonName.Contains("Button_Exit") || buttonName.Contains("Exit") ||
                buttonName.Contains("Return") || buttonName.Contains("Back") ||
                buttonName.Contains("退出") || buttonName.Contains("返回") ||
                buttonName.Contains("主菜单") || buttonName.Contains("主界面"))
            {
                button.onClick.RemoveListener(ReturnToMainMenu);
                button.onClick.AddListener(ReturnToMainMenu);
                exitButtonsFound++;
                Debug.Log($"已绑定退出按钮: {buttonName}");
            }

            if (buttonName.Contains("Button_Restart") || buttonName.Contains("Restart") ||
                buttonName.Contains("Retry") || buttonName.Contains("Again") ||
                buttonName.Contains("重启") || buttonName.Contains("重新开始") ||
                buttonName.Contains("再来一次") || buttonName.Contains("重试"))
            {
                button.onClick.RemoveListener(RestartGame);
                button.onClick.AddListener(RestartGame);
                restartButtonsFound++;
                Debug.Log($"已绑定重启按钮: {buttonName}");
            }
        }

        if (exitButtonsFound > 0 || restartButtonsFound > 0)
        {
            Debug.Log($"总共绑定 {exitButtonsFound} 个退出按钮，{restartButtonsFound} 个重启按钮");
        }
    }

    public void RebindAllButtons()
    {
        FindAndBindButtons();
    }

    public void BindExitButton(Button button)
    {
        if (button == null) return;
        button.onClick.RemoveListener(ReturnToMainMenu);
        button.onClick.AddListener(ReturnToMainMenu);
        Debug.Log($"手动绑定退出按钮: {button.name}");
    }

    public void BindRestartButton(Button button)
    {
        if (button == null) return;
        button.onClick.RemoveListener(RestartGame);
        button.onClick.AddListener(RestartGame);
        Debug.Log($"手动绑定重启按钮: {button.name}");
    }

    private IEnumerator LoadSceneWithLoading(string sceneName)
    {
        Time.timeScale = 1f;

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
            Debug.Log("显示加载面板");
        }

        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.LoadScene(sceneName);
    }

    public void ReturnToMainMenuDirect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartGameDirect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion
}

// 游戏状态枚举
public enum GameState
{
    Preparing,      // 准备中
    Playing,        // 游戏中
    Paused,         // 暂停
    Victory,        // 胜利
    Defeat          // 失败
}
