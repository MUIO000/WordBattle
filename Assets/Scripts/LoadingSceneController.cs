    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;
    using System.Collections;
    using TMPro;
    using System.Data.Common;

    public class LoadingSceneController : MonoBehaviour
    {
        public Slider loadingBar;
        public TextMeshProUGUI loadingText; // 或 TMP_Text

        public string targetSceneName = "MainMenu"; // 要跳转的目标场景名

        void Start()
        {
            StartCoroutine(LoadSceneAsync());
        }

        IEnumerator LoadSceneAsync()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
            asyncLoad.allowSceneActivation = false;
            loadingBar.value = 0;

            // 第一阶段：真实加载
            while (asyncLoad.progress < 0.9f)
            {
                // 真实进度（0~1）
                float realProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

                // 进度条平滑追赶真实进度（0~100）
                float targetValue = realProgress * 100f;
                loadingBar.value = Mathf.Lerp(loadingBar.value, targetValue, Time.unscaledDeltaTime * 5f);
                loadingText.text = "Loading... " + Mathf.RoundToInt(loadingBar.value) + "%";
                yield return null;
            }

            // 第二阶段：动画补满
            float finalProgress = loadingBar.value;
            while (finalProgress < 100f)
            {
                finalProgress = Mathf.MoveTowards(finalProgress, 100f, Time.unscaledDeltaTime * 80f);
                loadingBar.value = finalProgress;
                loadingText.text = "Loading... " + Mathf.RoundToInt(loadingBar.value) + "%";
                yield return null;
            }

            loadingText.text = "Loading Complete!";
            yield return new WaitForSecondsRealtime(0.5f);

            asyncLoad.allowSceneActivation = true;
        }

    }
