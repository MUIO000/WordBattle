using UnityEngine;
using UnityEngine.Events;

public class TimerController : MonoBehaviour
{
    [Header("计时器设置")]
    public float totalTime = 180f;          // 总时间（秒）
    public float warningTime = 30f;         // 警告时间阈值
    public bool isCountingDown = false;     // 是否正在倒计时
    public bool autoStart = false;          // 是否自动开始
    
    [Header("当前状态")]
    public float currentTime;               // 当前剩余时间
    public float elapsedTime;               // 已过去的时间
    public bool isWarning = false;          // 是否处于警告状态
    public bool isFinished = false;         // 是否已结束
    
    [Header("时间格式化")]
    public bool showHours = false;          // 是否显示小时
    public bool showMilliseconds = false;   // 是否显示毫秒
    public string timeFormat = "mm:ss";     // 时间格式
    
    [Header("事件系统")]
    public UnityEvent OnTimerStart;                    // 计时器开始
    public UnityEvent OnTimerPause;                    // 计时器暂停
    public UnityEvent OnTimerResume;                   // 计时器恢复
    public UnityEvent OnTimerFinished;                 // 计时器结束
    public UnityEvent OnWarningStart;                  // 警告开始
    public UnityEvent OnWarningEnd;                    // 警告结束
    public UnityEvent<float> OnTimeChanged;            // 时间改变
    public UnityEvent<string> OnTimeFormatted;         // 格式化时间更新
    public UnityEvent<float> OnProgressChanged;        // 进度改变（0-1）
    
    private bool wasPaused = false;                    // 暂停状态
    private float pauseStartTime;                      // 暂停开始时间
    
    private void Start()
    {
        InitializeTimer();
        
        if (autoStart)
        {
            StartTimer();
        }
    }
    
    private void Update()
    {
        if (isCountingDown && !wasPaused && !isFinished)
        {
            UpdateTimer();
        }
    }
    
    // 初始化计时器
    public void InitializeTimer()
    {
        currentTime = totalTime;
        elapsedTime = 0f;
        isCountingDown = false;
        isWarning = false;
        isFinished = false;
        wasPaused = false;
        
        // 触发初始事件
        OnTimeChanged?.Invoke(currentTime);
        OnTimeFormatted?.Invoke(FormatTime(currentTime));
        OnProgressChanged?.Invoke(GetProgress());
        
        Debug.Log($"计时器初始化完成 - 总时间: {totalTime}秒");
    }
    
    // 开始计时器
    public void StartTimer()
    {
        if (isFinished)
        {
            Debug.LogWarning("计时器已结束，无法开始！请先重置计时器。");
            return;
        }
        
        isCountingDown = true;
        wasPaused = false;
        
        OnTimerStart?.Invoke();
        Debug.Log("计时器开始");
    }
    
    // 暂停计时器
    public void PauseTimer()
    {
        if (isCountingDown && !wasPaused)
        {
            wasPaused = true;
            pauseStartTime = Time.time;
            
            OnTimerPause?.Invoke();
            Debug.Log("计时器暂停");
        }
    }
    
    // 恢复计时器
    public void ResumeTimer()
    {
        if (isCountingDown && wasPaused)
        {
            wasPaused = false;
            
            OnTimerResume?.Invoke();
            Debug.Log("计时器恢复");
        }
    }
    
    // 停止计时器
    public void StopTimer()
    {
        isCountingDown = false;
        wasPaused = false;
        
        Debug.Log("计时器停止");
    }
    
    // 重置计时器
    public void ResetTimer()
    {
        StopTimer();
        InitializeTimer();
        Debug.Log("计时器重置");
    }
    
    // 设置总时间
    public void SetTotalTime(float newTotalTime)
    {
        totalTime = newTotalTime;
        if (!isCountingDown)
        {
            InitializeTimer();
        }
        Debug.Log($"总时间设置为: {newTotalTime}秒");
    }
    
    // 添加时间
    public void AddTime(float timeToAdd)
    {
        currentTime += timeToAdd;
        currentTime = Mathf.Max(0, currentTime); // 确保不为负数
        
        OnTimeChanged?.Invoke(currentTime);
        OnTimeFormatted?.Invoke(FormatTime(currentTime));
        OnProgressChanged?.Invoke(GetProgress());
        
        Debug.Log($"添加时间: {timeToAdd}秒，当前时间: {currentTime}秒");
    }
    
    // 减少时间
    public void SubtractTime(float timeToSubtract)
    {
        currentTime -= timeToSubtract;
        currentTime = Mathf.Max(0, currentTime); // 确保不为负数
        
        if (currentTime <= 0)
        {
            FinishTimer();
        }
        else
        {
            OnTimeChanged?.Invoke(currentTime);
            OnTimeFormatted?.Invoke(FormatTime(currentTime));
            OnProgressChanged?.Invoke(GetProgress());
        }
        
        Debug.Log($"减少时间: {timeToSubtract}秒，当前时间: {currentTime}秒");
    }
    
    // 更新计时器
    private void UpdateTimer()
    {
        float deltaTime = Time.deltaTime;
        currentTime -= deltaTime;
        elapsedTime += deltaTime;
        
        // 检查是否进入警告时间
        CheckWarningState();
        
        // 检查是否时间到
        if (currentTime <= 0)
        {
            currentTime = 0;
            FinishTimer();
            return;
        }
        
        // 触发事件
        OnTimeChanged?.Invoke(currentTime);
        OnTimeFormatted?.Invoke(FormatTime(currentTime));
        OnProgressChanged?.Invoke(GetProgress());
    }
    
    // 检查警告状态
    private void CheckWarningState()
    {
        bool shouldBeWarning = currentTime <= warningTime && currentTime > 0;
        
        if (shouldBeWarning && !isWarning)
        {
            isWarning = true;
            OnWarningStart?.Invoke();
            Debug.Log("进入警告时间");
        }
        else if (!shouldBeWarning && isWarning)
        {
            isWarning = false;
            OnWarningEnd?.Invoke();
            Debug.Log("退出警告时间");
        }
    }
    
    // 结束计时器
    private void FinishTimer()
    {
        isFinished = true;
        isCountingDown = false;
        currentTime = 0;
        
        if (isWarning)
        {
            isWarning = false;
            OnWarningEnd?.Invoke();
        }
        
        OnTimerFinished?.Invoke();
        OnTimeChanged?.Invoke(currentTime);
        OnTimeFormatted?.Invoke(FormatTime(currentTime));
        OnProgressChanged?.Invoke(GetProgress());
        
        Debug.Log("计时器结束");
    }
    
    // 格式化时间显示
    public string FormatTime(float timeInSeconds)
    {
        int hours = Mathf.FloorToInt(timeInSeconds / 3600);
        int minutes = Mathf.FloorToInt((timeInSeconds % 3600) / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 1000) % 1000);
        
        string formattedTime = "";
        
        if (showHours || hours > 0)
        {
            formattedTime += hours.ToString("00") + ":";
        }
        
        formattedTime += minutes.ToString("00") + ":" + seconds.ToString("00");
        
        if (showMilliseconds)
        {
            formattedTime += "." + milliseconds.ToString("000");
        }
        
        return formattedTime;
    }
    
    // 获取进度（0-1）
    public float GetProgress()
    {
        if (totalTime <= 0) return 0f;
        return Mathf.Clamp01(elapsedTime / totalTime);
    }
    
    // 获取剩余进度（0-1）
    public float GetRemainingProgress()
    {
        if (totalTime <= 0) return 0f;
        return Mathf.Clamp01(currentTime / totalTime);
    }
    
    // 获取时间状态信息
    public TimerStatus GetTimerStatus()
    {
        return new TimerStatus
        {
            isRunning = isCountingDown,
            isPaused = wasPaused,
            isFinished = isFinished,
            isWarning = isWarning,
            currentTime = currentTime,
            elapsedTime = elapsedTime,
            totalTime = totalTime,
            progress = GetProgress(),
            remainingProgress = GetRemainingProgress(),
            formattedTime = FormatTime(currentTime)
        };
    }
    
    // 从GameManager同步时间
    public void SyncWithGameManager()
    {
        if (GameManager.Instance != null)
        {
            currentTime = GameManager.Instance.remainingTime;
            totalTime = GameManager.Instance.currentLevel != null ? 
                       GameManager.Instance.currentLevel.timeLimit : totalTime;
            
            OnTimeChanged?.Invoke(currentTime);
            OnTimeFormatted?.Invoke(FormatTime(currentTime));
            OnProgressChanged?.Invoke(GetProgress());
        }
    }
    
    // 设置警告时间
    public void SetWarningTime(float newWarningTime)
    {
        warningTime = newWarningTime;
        Debug.Log($"警告时间设置为: {warningTime}秒");
    }
}

// 计时器状态信息
[System.Serializable]
public class TimerStatus
{
    public bool isRunning;
    public bool isPaused;
    public bool isFinished;
    public bool isWarning;
    public float currentTime;
    public float elapsedTime;
    public float totalTime;
    public float progress;
    public float remainingProgress;
    public string formattedTime;
}
