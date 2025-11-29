using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SummonerController : MonoBehaviour
{
    public Animator animator;
    public GameObject skillVFX;           // 召唤师技能特效
    public Transform skillVFXSpawnPoint;  // 特效生成位置
    public Vector3 skillVFXRotation;      // 特效旋转角度
    public GameObject heroPrefab;         // 英雄预制体
    public Transform heroSpawnPoint;      // 英雄生成位置
    
    public GameObject SkillBrustVFX;      // 召唤师技能释放光效
    public Transform SkillBrustSpawnPoint; // 光效生成位置

    [Header("Buff光圈设置")]
    public GameObject soldierBuffPrefab; // 士兵buff光圈
    public GameObject heroBuffPrefab;    // 英雄buff光圈
    public float buffDuration = 5f;      // buff持续时间
    public Vector3 soldierBuffOffset = Vector3.zero; // 士兵buff偏移
    public Vector3 heroBuffOffset = Vector3.zero;    // 英雄buff偏移

    private float waitTime = 0.8f;    // 等待召唤师施法时间
    private bool isFirstSummon = true;    // 是否是第一次召唤
    private HeroController heroInstance;  // 当前召唤的英雄实例
    private static readonly int SkillTrigger = Animator.StringToHash("att");
    private bool isPerformingAction = false; // 是否正在执行动作
    private Queue<IEnumerator> actionQueue = new Queue<IEnumerator>();
    private Coroutine actionRunner;

    public void OnProgressBarFull()
    {
        if (isFirstSummon)
            actionQueue.Enqueue(FirstSummonSequence());
        else
            actionQueue.Enqueue(TriggerHeroSkillSequence());
        TryRunActionQueue();
    }

    // 新增：召唤士兵的方法
    public void SummonSoldier(GameObject soldierPrefab, Transform spawnPoint)
    {
        actionQueue.Enqueue(SummonSoldierSequence(soldierPrefab, spawnPoint));
        TryRunActionQueue();
    }

    private void TryRunActionQueue()
    {
        if (actionRunner == null && actionQueue.Count > 0)
        {
            actionRunner = StartCoroutine(RunActionQueue());
        }
    }

    private IEnumerator RunActionQueue()
    {
        while (actionQueue.Count > 0)
        {
            yield return StartCoroutine(actionQueue.Dequeue());
        }
        actionRunner = null;
    }

    private IEnumerator SummonSoldierSequence(GameObject soldierPrefab, Transform spawnPoint)
    {
        animator.SetTrigger(SkillTrigger);
        yield return new WaitForSeconds(waitTime);

        if (soldierPrefab != null && spawnPoint != null)
        {
            Vector3 worldPos = spawnPoint.position;
            GameObject soldier = Instantiate(soldierPrefab, worldPos, Quaternion.identity);

            if (soldierBuffPrefab != null)
            {
                GameObject buff = Instantiate(soldierBuffPrefab, soldier.transform);
                buff.transform.localPosition = soldierBuffOffset;
                Destroy(buff, buffDuration);
            }
        }
    }

    private IEnumerator FirstSummonSequence()
    {
        // 1. 播放技能特效
        TriggerSkillVFX();
        
        // 2. 播放技能释放光效
        TriggerSkillBrustVFX(); 
        
        // 3. 等待1秒
        yield return new WaitForSeconds(waitTime);
        
        // 4. 播放攻击动画
        animator.SetTrigger(SkillTrigger);
        
        // 5. 等待动画播放完成
        yield return new WaitForSeconds(waitTime);
        
        // 6. 召唤英雄
        SummonHero();
        
        // 7. 更新状态
        isFirstSummon = false;

        // 8. 动作完成，处理下一个动作
        isPerformingAction = false;
    }

    private IEnumerator TriggerHeroSkillSequence()
    {
        // 1. 播放技能特效
        TriggerSkillVFX();
        
        // 2. 播放技能释放光效
        TriggerSkillBrustVFX(); 
        
        // 3. 等待1秒
        yield return new WaitForSeconds(waitTime);
        
        // 4. 播放攻击动画
        animator.SetTrigger(SkillTrigger);
        
        // 5. 等待动画播放完成
        yield return new WaitForSeconds(waitTime);
        
        // 6. 触发英雄技能
        if (heroInstance != null)
        {
            heroInstance.ReleaseSkill();
        }

        // 7. 动作完成，处理下一个动作
        isPerformingAction = false;
    }

    private void TriggerSkillBrustVFX()
    {
        if (SkillBrustVFX != null && SkillBrustSpawnPoint != null)
        {   
            GameObject vfx = Instantiate(SkillBrustVFX, SkillBrustSpawnPoint.position, Quaternion.identity);
            ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(vfx, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(vfx, 0.7f);
            }
        }
    }

    private void TriggerSkillVFX()
    {
        if (skillVFX != null && skillVFXSpawnPoint != null)
        {
            GameObject vfx = Instantiate(skillVFX, skillVFXSpawnPoint.position, Quaternion.Euler(skillVFXRotation));
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

    private void SummonHero()
    {
        if (heroPrefab != null && heroSpawnPoint != null)
        {
            GameObject hero = Instantiate(heroPrefab, heroSpawnPoint.position, Quaternion.identity, heroSpawnPoint);
            heroInstance = hero.GetComponent<HeroController>();

            // 生成英雄buff光圈
            if (heroBuffPrefab != null)
            {
                GameObject buff = Instantiate(heroBuffPrefab, hero.transform);
                buff.transform.localPosition = heroBuffOffset;
                Destroy(buff, buffDuration);
            }
        }
        isPerformingAction = false;
    }
} 