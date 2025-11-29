using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class QuestionPanel : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public Button[] optionButtons;
    public TextMeshProUGUI[] optionTexts;
    public Sprite correctSprite;
    public Sprite wrongSprite;
    public TextMeshProUGUI coinText;
    public Button nextButton;
    public GameObject correctBannerPrefab; // 拖预制体
    public GameObject coinPrefab; // 拖入金币预制体
    public Transform coinTarget; // 拖入金币计数器的Transform

    [Header("问题奖励金币数量")]
    public int questionCoinCount = 10;
    private const int COIN_COUNT = 3; // 每次生成的金币数量
    private const float COIN_SPAWN_INTERVAL = 0.1f; // 金币生成间隔
    private const float COIN_FLY_DURATION = 0.5f; // 金币飞行时间

    // 连对横幅位置
    public Transform correctBannerPosition;
    private int coins = 0;
    private VocabularyManager vocabManager;
    private bool answered = false;
    private int lastSelected = -1;
    private GameObject correctBannerInstance;
    private int comboCount = 0;
    private Coroutine bannerCoroutine;

    private Vector3 originalScale;
    public float scaleFactor = 0.9f;
    
    // 答题统计
    [Header("答题统计")]
    public int totalAnswered = 0;
    public int correctAnswered = 0;

    private void Start()
    {
        originalScale = optionButtons[0].transform.localScale;
        vocabManager = FindObjectOfType<VocabularyManager>();
        if (vocabManager == null)
        {
            Debug.LogError("未找到 VocabularyManager!");
            return;
        }
        
        // 重置答题统计
        ResetStatistics();
        
        // 延迟一帧调用ShowQuestion，确保VocabularyManager初始化完成
        StartCoroutine(DelayedShowQuestion());
        UpdateCoinUI();
        nextButton.onClick.AddListener(OnNextQuestion);
        nextButton.gameObject.SetActive(false);
    }

    private System.Collections.IEnumerator DelayedShowQuestion()
    {
        yield return null; // 等待一帧
        ShowQuestion();
    }

    void ShowQuestion()
    {
        if (vocabManager == null)
    {
            Debug.LogError("VocabularyManager not found!");
            return;
        }

        answered = false;
        lastSelected = -1;
        nextButton.gameObject.SetActive(false);

        QuestionData currentQ = vocabManager.CurrentQuestion;
        string[] options = vocabManager.CurrentOptions;
        
        Debug.Log($"ShowQuestion: currentQ={currentQ?.question}, options长度={options?.Length}");

        if (currentQ == null || options == null || options.Length == 0)
        {
            Debug.LogWarning("No question available.");
            questionText.text = "题库已完成!";
            foreach (var btn in optionButtons) btn.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
            return;
        }

        // 显示题目
        questionText.text = currentQ.question;

        // 显示选项
        for (int i = 0; i < optionButtons.Length; i++)
        {
            bool active = i < options.Length;
            optionButtons[i].gameObject.SetActive(active);
            if (active)
            {
            optionButtons[i].interactable = true;
            optionTexts[i].color = Color.white;
                optionTexts[i].text = options[i];

                // 重置按钮颜色为默认状态
            ColorBlock cb = optionButtons[i].colors;
            cb.normalColor = Color.white;
            cb.selectedColor = Color.white;
            cb.highlightedColor = Color.white;
                cb.disabledColor = Color.white; // 也重置禁用状态的颜色
            optionButtons[i].colors = cb;

                int idx = i;
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => OnOptionSelected(idx));
            }
        }
    }

    void OnOptionSelected(int selectedIndex)
    {
        optionButtons[selectedIndex].transform.localScale = originalScale * scaleFactor;

        if (answered) return;
        answered = true;
        lastSelected = selectedIndex;

        bool isCorrect = vocabManager.CheckAnswer(selectedIndex);

        // 更新按钮颜色
        for (int i = 0; i < optionButtons.Length; i++)
        {
            optionButtons[i].interactable = false;
            ColorBlock cb = optionButtons[i].colors;

            if (i == vocabManager.CorrectAnswerIndex)
            {
                cb.normalColor = Color.green;
                cb.selectedColor = Color.green;
                cb.highlightedColor = Color.green;
                cb.disabledColor = Color.green;
            }
            else if (i == selectedIndex && !isCorrect)
            {
                cb.normalColor = Color.red;
                cb.selectedColor = Color.red;
                cb.highlightedColor = Color.red;
                cb.disabledColor = Color.red;
            }
            optionButtons[i].colors = cb;
        }

        if (isCorrect)
        {
            comboCount++;
            ShowCorrectBanner(comboCount);
            StartCoroutine(SpawnCoinsAndAddCoins(optionButtons[selectedIndex].transform.position));
            FindObjectOfType<HeroSkillProgressBar>().AddProgress();
            correctAnswered++;
        }
        else
        {
            FindObjectOfType<HeroSkillProgressBar>().OnWrongAnswer();
            comboCount = 0;
            HideCorrectBanner();
        }

        nextButton.gameObject.SetActive(true);

        // 重置按钮大小
        StartCoroutine(ResetButtonScale(selectedIndex));
        totalAnswered++;
    }

    IEnumerator ResetButtonScale(int selectedIndex)
    {
        yield return new WaitForSeconds(0.1f);
        optionButtons[selectedIndex].transform.localScale = originalScale;
    }

    void OnNextQuestion()
    {
        vocabManager.NextQuestion();
        ShowQuestion();
    }

    void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = coins.ToString();
    }

    void ShowCorrectBanner(int combo)
    {
        if (correctBannerInstance == null)
            correctBannerInstance = Instantiate(correctBannerPrefab, correctBannerPosition);

        correctBannerInstance.SetActive(true);

        // 设置文本
        var text = correctBannerInstance.transform.Find("Banner/Text").GetComponent<TMPro.TextMeshProUGUI>();
        var num = correctBannerInstance.transform.Find("TotalNum").GetComponent<TMPro.TextMeshProUGUI>();
        text.text = "Correct!";
        num.text = "x " + combo;

        // 控制缩放动画
        float targetScale = 1f;
        if (combo == 1) targetScale = 1.0f;
        else if (combo == 2) targetScale = 1.2f;
        else targetScale = 1.4f; // 3及以上

        // 控制火焰特效
        var burn = correctBannerInstance.transform.Find("burnEffect");
        if (burn != null) burn.gameObject.SetActive(combo >= 3);

        if (bannerCoroutine != null) StopCoroutine(bannerCoroutine);
        bannerCoroutine = StartCoroutine(BannerScaleRoutine(targetScale, combo));
    }

    IEnumerator BannerScaleRoutine(float targetScale, int combo)
    {
        var rect = correctBannerInstance.transform as RectTransform;
        float t = 0f;
        float duration = 0.25f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one * targetScale;
        rect.localScale = startScale;

        // 膨胀动画
        while (t < duration)
        {
            t += Time.deltaTime;
            rect.localScale = Vector3.Lerp(startScale, endScale, t / duration);
            yield return null;
        }
        rect.localScale = endScale;

        if (combo < 3)
        {
            yield return new WaitForSeconds(1f);
            HideCorrectBanner();
        }
        // 3连对及以上，保持显示，不自动消失
    }

    void HideCorrectBanner()
    {
        if (correctBannerInstance == null) return;
        var burn = correctBannerInstance.transform.Find("burnEffect");
        if (burn != null) burn.gameObject.SetActive(false);
        StartCoroutine(BannerHideRoutine());
    }

    IEnumerator BannerHideRoutine()
    {
        var rect = correctBannerInstance.transform as RectTransform;
        float t = 0f;
        float duration = 0.2f;
        Vector3 startScale = rect.localScale;
        Vector3 endScale = Vector3.zero;

        // 缩小动画
        while (t < duration)
        {
            t += Time.deltaTime;
            rect.localScale = Vector3.Lerp(startScale, endScale, t / duration);
            yield return null;
        }
        rect.localScale = endScale;
        correctBannerInstance.SetActive(false);
    }

    // 添加生成金币的协程
    IEnumerator SpawnCoins(Vector3 startPos)
    {
        for (int i = 0; i < COIN_COUNT; i++)
        {
            // 生成金币
            GameObject coin = Instantiate(coinPrefab, startPos, Quaternion.identity, transform);
            
            // 随机偏移，使金币不会完全重叠
            Vector3 randomOffset = new Vector3(
                Random.Range(-20f, 20f),
                Random.Range(-20f, 20f),
                0
            );
            coin.transform.position += randomOffset;

            // 启动金币飞行动画
            StartCoroutine(FlyCoin(coin));

            yield return new WaitForSeconds(COIN_SPAWN_INTERVAL);
        }
    }

    IEnumerator FlyCoin(GameObject coin)
    {
        Vector3 startPos = coin.transform.position;
        Vector3 endPos = coinTarget.position;
        float t = 0f;

        // 添加一个向上的弧度
        Vector3 controlPoint = startPos + (endPos - startPos) * 0.5f + Vector3.up * 100f;

        while (t < COIN_FLY_DURATION)
        {
            t += Time.deltaTime;
            float progress = t / COIN_FLY_DURATION;

            // 使用二次贝塞尔曲线计算位置
            Vector3 pos = Vector3.Lerp(
                Vector3.Lerp(startPos, controlPoint, progress),
                Vector3.Lerp(controlPoint, endPos, progress),
                progress
            );

            coin.transform.position = pos;


            yield return null;
        }

        // 到达目标后销毁金币
        Destroy(coin);
    }

    // 添加新的协程方法
    IEnumerator SpawnCoinsAndAddCoins(Vector3 startPos)
    {
        // 先播放金币动画
        yield return StartCoroutine(SpawnCoins(startPos));
        
        // 等待0.5秒
        yield return new WaitForSeconds(0.5f);
        
        // 最后增加金币
        FindObjectOfType<SoldierShopPanel>().AddCoins(questionCoinCount);
    }

    // 重置答题统计
    public void ResetStatistics()
    {
        totalAnswered = 0;
        correctAnswered = 0;
        Debug.Log("答题统计已重置");
    }
    
    // 获取答题准确率
    public float GetAccuracy()
    {
        return totalAnswered > 0 ? (float)correctAnswered / totalAnswered : 0f;
    }
}
