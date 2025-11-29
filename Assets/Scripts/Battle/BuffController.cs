using UnityEngine;
using System.Collections;

public class BuffAuraController : MonoBehaviour
{
    public GameObject buffAuraPrefab;         // 光圈预制体
    private GameObject currentAuraInstance;

    public float buffDuration = 5f;           // Buff 持续时间（秒）

    public void ApplyBuff()
    {
        // 已存在 Buff 不重复添加
        if (currentAuraInstance != null) return;

        // 实例化并挂在脚下
        currentAuraInstance = Instantiate(buffAuraPrefab, transform);
        currentAuraInstance.transform.localPosition = Vector3.zero;

        // 启动计时器
        StartCoroutine(RemoveBuffAfterDelay(buffDuration));
    }

    private IEnumerator RemoveBuffAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);

        // 销毁光圈
        if (currentAuraInstance != null)
        {
            Destroy(currentAuraInstance);
            currentAuraInstance = null;
        }

        // TODO: 这里可以取消Buff属性效果，例如恢复速度、攻击力等
        Debug.Log("Buff ended.");
    }
}
