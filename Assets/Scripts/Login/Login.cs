using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using System.Collections;

public class Login : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private TextMeshProUGUI errorMessageText;
    [SerializeField] private GameObject loadingPanel;

    // 目标场景
    public string targetSceneName = "Loading";

    [Header("配置")]
    private string apiBaseUrl = "http://127.0.0.1:3000/api";
    [SerializeField] private string loginEndpoint = "/auth/login";

    private Coroutine errorCoroutine;

    private void Start()
    {
        // 初始化UI事件监听
        loginButton.onClick.AddListener(OnLoginClick);
        errorPanel.SetActive(false);
        loadingPanel.SetActive(false);
    }

    private async void OnLoginClick()
    {
        // 获取输入
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        // 输入验证
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowError("用户名和密码不能为空");
            return;
        }

        // 显示加载
        SetLoading(true);

        try
        {
            // 构建登录请求数据
            var loginData = new LoginRequest
            {
                username = username,
                password = password
            };

            // 发送登录请求
            var response = await SendLoginRequest(loginData);

            // 处理响应
            if (response != null)
            {
                Debug.Log("登录成功");
                Debug.Log("token: " + response.token);
                Debug.Log("user: " + response.user.username);
                // 保存token
                PlayerPrefs.SetString("AuthToken", response.token);
                PlayerPrefs.Save();

                // 保存用户数据
                SaveUserData(response.user);

                // 登录成功，跳转到主菜单
                StartCoroutine(LoadMainMenuWithLoading());
            }
        }
        catch (Exception ex)
        {
            ShowError($"登录失败: {ex.Message}");
        }
        finally
        {
            SetLoading(false);
        }
    }

    private async Task<LoginResponse> SendLoginRequest(LoginRequest loginData)
    {
        string url = $"{apiBaseUrl}{loginEndpoint}";
        string jsonData = JsonConvert.SerializeObject(loginData);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            Debug.Log(url);
            Debug.Log(jsonData);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 发送请求
            var operation = request.SendWebRequest();

            // 等待请求完成
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            // 处理响应
            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                return JsonConvert.DeserializeObject<LoginResponse>(jsonResponse);
            }
            else
            {
                // 处理错误响应
                if (request.responseCode == 401)
                {
                    throw new Exception("用户名或密码错误");
                }
                else
                {
                    throw new Exception($"服务器错误: {request.error}");
                }
            }
        }
    }

    private void SaveUserData(UserData user)
    {
        // 保存用户数据到PlayerPrefs
        PlayerPrefs.SetString("UserId", user.id.ToString());
        PlayerPrefs.SetString("Username", user.username);
        PlayerPrefs.SetString("UserLevel", user.level.ToString());
        PlayerPrefs.SetString("UserExp", user.exp.ToString());
        PlayerPrefs.Save();
    }

    private void OnRegisterClick()
    {
        // 跳转到注册界面
        SceneManager.LoadScene("Register");
    }

    private void ShowError(string message)
    {
        errorMessageText.text = message;
        errorPanel.SetActive(true);
        
        // 如果已经有正在运行的协程，先停止它
        if (errorCoroutine != null)
        {
            StopCoroutine(errorCoroutine);
        }
        
        // 启动新的协程
        errorCoroutine = StartCoroutine(AutoHideError());
    }

    private IEnumerator AutoHideError()
    {
        yield return new WaitForSeconds(3f);
        errorPanel.SetActive(false);
    }

    private void SetLoading(bool isLoading)
    {
        loadingPanel.SetActive(isLoading);
        loginButton.interactable = !isLoading;
    }

    private void OnDestroy()
    {
        // 清理事件监听
        loginButton.onClick.RemoveListener(OnLoginClick);
        // 确保协程被清理
        if (errorCoroutine != null)
        {
            StopCoroutine(errorCoroutine);
        }
    }

    private IEnumerator LoadMainMenuWithLoading()
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        // 可选：等待0.5秒让用户看到加载界面
        yield return new WaitForSecondsRealtime(0.5f);

        SceneManager.LoadScene(targetSceneName);
    }
}

// 请求和响应数据模型
[Serializable]
public class LoginRequest
{
    public string username;
    public string password;
}

[Serializable]
public class LoginResponse
{
    public string token;
    public UserData user;
}

[Serializable]
public class UserData
{
    public int id;
    public string username;
    public int level;
    public int exp;
    // 其他用户数据字段...
}