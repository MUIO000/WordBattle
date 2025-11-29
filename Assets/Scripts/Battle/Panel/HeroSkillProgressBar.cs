using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeroSkillProgressBar : MonoBehaviour
{
    public Slider progressBar;
    public GameObject DarkPowerIcon;
    public GameObject LightPowerIcon;
    public RectTransform fillArea;
    public GameObject fullProgressVFX;    // 特效预制体
    public Transform spawnPos;             // 特效生成位置
    public Vector3 vfxRotationEuler;       // 特效生成旋转角度
    private SummonerController summoner;    // 召唤师控制器引用

    [Header("进度条设置")]
    public int maxProgressStep = 3;
    public int wrongAnswerPenalty = 1;     // 答错惩罚步数，可在Unity中设置

    private int progressStep = 0;
    private bool isFullProgress = false;
    private float fullProgressKeepTime = 1f;
    private float fullProgressTimer = 0f;
    private bool isAnimating = false;
    private float animationTriggerDelay = 0.5f;
    private float animationTriggerTimer = 0f;
    private float targetFill = 0f;
    private float smoothSpeed = 5f;

    void Start()
    {
        ResetBar();
        StartCoroutine(FindSummonerNextFrame());
    }

    private IEnumerator FindSummonerNextFrame()
    {
        yield return null; // 等一帧
        if (summoner == null)
        {
            summoner = FindObjectOfType<SummonerController>();
            if (summoner == null)
            {
                Debug.LogWarning("延迟一帧后依然未找到召唤师！");
            }
        }
    }

    void Update()
    {
        // 平滑更新进度条
        float currentFill = progressBar.value;
        if (Mathf.Abs(currentFill - targetFill) > 0.001f)
        {
            progressBar.value = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * smoothSpeed);
        }

        // 处理满进度动画触发
        if (progressStep == maxProgressStep && !isAnimating)
        {
            animationTriggerTimer += Time.deltaTime;
            if (animationTriggerTimer >= animationTriggerDelay)
            {
                isAnimating = true;
                animationTriggerTimer = 0f;
                TriggerFullProgressAnimation();
                if (summoner != null)
                {
                    summoner.OnProgressBarFull();
                }
            }
        }
        else if (progressStep < maxProgressStep)
        {
            isAnimating = false;
        }

        // 处理满进度保持时间
        if (isFullProgress)
        {
            fullProgressTimer += Time.deltaTime;
            if (fullProgressTimer >= fullProgressKeepTime)
            {
                isFullProgress = false;
                fullProgressTimer = 0f;
                progressStep = 0;
                UpdateBar();
            }
        }
        else if (progressStep == maxProgressStep)
        {
            isFullProgress = true;
            fullProgressTimer = 0f; 
        }
    }

    public void AddProgress()
    {
        if (progressStep < maxProgressStep)
        {
            progressStep++;
            UpdateBar();
        }
    }

    public void OnWrongAnswer()
    {
        // 确保进度不会小于0
        progressStep = Mathf.Max(0, progressStep - wrongAnswerPenalty);
        UpdateBar();
    }

    void UpdateBar()
    {
        targetFill = (float)progressStep / maxProgressStep;
        MovePowerIcon(targetFill);

        if (targetFill < 1f)
        {
            DarkPowerIcon.SetActive(true);
            LightPowerIcon.SetActive(false);
        }
        else
        {
            DarkPowerIcon.SetActive(false);
            LightPowerIcon.SetActive(true);
        }
    }

    void MovePowerIcon(float fill)
    {
        if (fillArea == null) return;
        var rect = fillArea.rect;
        float x = rect.xMin + rect.width * fill;
    }

    private void TriggerFullProgressAnimation()
    {
        if (fullProgressVFX != null && spawnPos != null)
        {
            GameObject vfx = Instantiate(fullProgressVFX, spawnPos.position, Quaternion.Euler(vfxRotationEuler));
            ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(vfx, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(vfx, 2f);
            }
        }
    }

    void ResetBar()
    {
        progressStep = 0;
        targetFill = 0f;
        progressBar.value = 0f;
        isFullProgress = false;
        fullProgressTimer = 0f;
        isAnimating = false;
        animationTriggerTimer = 0f;
        UpdateBar();
    }
}