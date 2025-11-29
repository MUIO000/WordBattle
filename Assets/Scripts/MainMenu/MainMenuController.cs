using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button heroesButton;
    [SerializeField] private Button soldierButton;
    [SerializeField] private Button challengeButton;
    
    [Header("加载界面")]
    [SerializeField] private GameObject loadingPanel;     // 加载面板

    private void Awake()
    {
        // 绑定按钮事件
        heroesButton.onClick.AddListener(OnHeroesClicked);
        soldierButton.onClick.AddListener(OnSoldierClicked);
        challengeButton.onClick.AddListener(OnChallengeClicked);
    }

    private void OnHeroesClicked()
    {
        StartCoroutine(LoadSceneWithLoading("HeroList"));
    }

    private void OnSoldierClicked()
    {
        StartCoroutine(LoadSceneWithLoading("SoldierList"));
    }

    private void OnChallengeClicked()
    {
        StartCoroutine(LoadSceneWithLoading("Battle"));
    }
    
    /// <summary>
    /// 带加载界面的场景切换协程
    /// </summary>
    /// <param name="sceneName">要加载的场景名称</param>
    private IEnumerator LoadSceneWithLoading(string sceneName)
    {
        // 显示加载界面
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }
        
        // 短暂等待让用户看到加载界面
        yield return new WaitForSeconds(0.5f);
        
        // 加载场景
        SceneManager.LoadScene(sceneName);
    }
}
