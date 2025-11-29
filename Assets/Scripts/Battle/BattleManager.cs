using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [Header("生成点")]
    public Transform bossSpawnPoint;

    [Header("召唤师生成")]
    public GameObject summonerPrefab;      
    public Transform summonerSpawnPoint;   
    private SummonerController summonerInstance; // 改为SummonerController类型


    [Header("预制体")]
    public GameObject bossPrefab;


    [Header("计时器设置")]
    public float battleTime = 60f; // 总时长
    public TextMeshProUGUI timerText;         // 拖入UI文本
    public float warningTime = 10f;           // 开始警告的时间
    public float pulseSpeed = 2f;             // 脉冲动画速度
    public float minScale = 1f;               // 最小缩放
    public float maxScale = 1.5f;             // 最大缩放
    public Color normalColor = Color.white;    // 正常颜色
    public Color warningColor = Color.red;     // 警告颜色

    [Header("弹窗")]
    public GameObject winPanel;    // 拖入胜利弹窗
    public GameObject losePanel;   // 拖入失败弹窗
    
    [Header("星星评级显示")]
    public GameObject[] stars = new GameObject[3];  // 拖入三颗星星的GameObject
    public TextMeshProUGUI scoreText;               // 显示分数的文本

    public List<SoldierController> activeSoldiers = new List<SoldierController>();
    public EnemyUnit currentBoss;
    private float timer;
    private bool isBattleActive = false;
    private bool isGameOver = false;
    private Vector3 originalScale;            // 原始缩放值

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (timerText != null)
        {
            // 确保在游戏开始时保存正确的原始缩放值
            originalScale = timerText.transform.localScale;
            // 立即设置一次初始状态
            timerText.transform.localScale = originalScale;
            timerText.color = normalColor;
        }
        SpawnSummoner();
        StartBattle();
    }

    private void Update()
    {
        if (isGameOver) return;

        // 计时器逻辑
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            UpdateTimerUI();

            if (timer <= 0f)
            {
                timer = 0f;
                StartCoroutine(GameOver(false));
                return; // 防止后续Boss判定
            }
        }

        // Boss死亡检测
        if (isBattleActive && currentBoss != null && !currentBoss.isAlive)
        {
            isBattleActive = false;
            StartCoroutine(ShowWinPanelWithDelay(2f));
        }
    }

    public void StartBattle()
    {
        isBattleActive = true;
        isGameOver = false;
        timer = battleTime;
        UpdateTimerUI();
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        
        // 重置QuestionPanel的答题统计
        QuestionPanel questionPanel = FindObjectOfType<QuestionPanel>();
        if (questionPanel != null)
        {
            questionPanel.ResetStatistics();
        }
        

        SpawnBoss();
    }

    private void SpawnSummoner()
    {
        if (summonerPrefab != null && summonerSpawnPoint != null)
        {
            if (summonerInstance != null)
            {
                Destroy(summonerInstance.gameObject);
            }
            GameObject summoner = Instantiate(summonerPrefab, summonerSpawnPoint.position, summonerSpawnPoint.rotation);
            summonerInstance = summoner.GetComponent<SummonerController>();
            // 确保激活
            summoner.SetActive(true);
        }
    }

    private void SpawnBoss()
    {
        if (bossPrefab != null && bossSpawnPoint != null)
        {
            GameObject boss = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
            currentBoss = boss.GetComponent<EnemyUnit>();
        }
    }


    public void OnPlayerUnitDeath(SoldierController soldier)
    {
        if (activeSoldiers.Contains(soldier))
        {
            activeSoldiers.Remove(soldier);
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = Mathf.CeilToInt(timer).ToString();
            
            // 当时间小于警告时间时，添加动画效果
            if (timer <= warningTime)
            {
                // 计算脉冲动画
                float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
                float scale = Mathf.Lerp(minScale, maxScale, pulse);
                timerText.transform.localScale = originalScale * scale;
                
                // 颜色渐变
                timerText.color = warningColor;
            }
            else
            {
                // 恢复正常状态
                timerText.transform.localScale = originalScale;
                timerText.color = normalColor;
            }
        }
    }

    private IEnumerator GameOver(bool isWin)
    {
        isGameOver = true;
        isBattleActive = false;

        // 重置计时器文本状态
        if (timerText != null)
        {
            timerText.transform.localScale = originalScale;
            timerText.color = normalColor;
        }

        if (isWin)
        {
            Debug.Log("显示胜利面板...");

            yield return StartCoroutine(ShowWinPanelWithStars());

            winPanel.SetActive(true);
        }
        else
        {
            losePanel.SetActive(true);
        }

        // 等 UI 面板激活完再暂停游戏
        StartCoroutine(PauseAfterDelay(0.1f));
    }

    private IEnumerator PauseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Time.timeScale = 0f;
    }

    private IEnumerator ShowWinPanelWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(GameOver(true));
    }

    private IEnumerator ShowWinPanelWithStars()
    {
        // 等待一小段时间让胜利面板显示
        yield return new WaitForSeconds(1f);
        
        Debug.Log("开始显示星星评级...");
        
        // 从QuestionPanel获取真实的答题数据
        QuestionPanel questionPanel = FindObjectOfType<QuestionPanel>();
        int correctAnswers = 0;
        int totalAnswers = 0;
        
        if (questionPanel != null)
        {
            correctAnswers = questionPanel.correctAnswered;
            totalAnswers = questionPanel.totalAnswered;
            Debug.Log($"从QuestionPanel获取数据：{correctAnswers}/{totalAnswers}");
        }
        else
        {
            // 备用数据
            Debug.LogWarning("未找到QuestionPanel，使用备用数据");
            correctAnswers = 8;
            totalAnswers = 10;
        }
        
        Debug.Log($"准备计算评级，正确答案：{correctAnswers}，总答案：{totalAnswers}");
        
        // 显示表现评级
        CalculateAndShowPerformance(correctAnswers, totalAnswers);
    }

    
    // 显示星星评级
    public void ShowStarRating(int starCount, float score)
    {
        Debug.Log($"ShowStarRating被调用，星级：{starCount}，分数：{score}");
        StartCoroutine(AnimateStars(starCount, score));
    }
    
    // 星星动画协程
    private IEnumerator AnimateStars(int starCount, float score)
    {
        Debug.Log($"AnimateStars开始执行，星级：{starCount}，分数：{score}");
        
        // 确保星星数量在有效范围内
        starCount = Mathf.Clamp(starCount, 1, 3);
        
        // 显示分数
        if (scoreText != null)
        {
            scoreText.text = $"{score:F1}";
            Debug.Log($"分数文本设置为：{scoreText.text}");
        }
        else
        {
            Debug.LogError("scoreText为null，无法显示分数！");
        }
        
        // 检查stars数组
        Debug.Log($"stars数组长度：{stars.Length}");
        for (int i = 0; i < stars.Length; i++)
        {
            Debug.Log($"stars[{i}] = {(stars[i] != null ? stars[i].name : "null")}");
        }
        
        // 根据评级显示星星
        for (int i = 0; i < stars.Length; i++)
        {
            if (i < starCount)
            {
                stars[i].SetActive(true);
                Debug.Log($"星星{i}显示");
            }
        }

        yield return null;
    }

    
    // 计算并显示表现评级（简化版本，不依赖PerformanceEvaluator）
    public void CalculateAndShowPerformance(int correctAnswers, int totalAnswers)
    {
        Debug.Log($"CalculateAndShowPerformance被调用，参数：{correctAnswers}/{totalAnswers}");
        
        // 简单的评分算法
        float accuracy = totalAnswers > 0 ? (float)correctAnswers / totalAnswers : 0f;
        float score = accuracy * 100f;
        
        // 根据准确率确定星级
        int stars = 1; // 胜利默认1星
        if (accuracy >= 0.9f) stars = 3;        // 90%以上 3星
        else if (accuracy >= 0.7f) stars = 2;   // 70%以上 2星
        
        Debug.Log($"计算结果：准确率={accuracy:F2}, 分数={score:F1}, 星级={stars}");
        
        ShowStarRating(stars, score);
    }

    // 提供获取召唤师的方法
    public SummonerController GetSummoner()
    {
        return summonerInstance;
    }
}