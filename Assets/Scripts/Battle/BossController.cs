using UnityEngine;

public class BossController : EnemyUnit
{
    [Header("Boss技能设置")]
    public float skillCooldown = 8f;
    private float lastSkillTime = -1f;
    public int skillDamage = 200;
    public int skillHitCount = 3; // 技能多段

    // 技能范围
    public float skillRange = 5f;

    public GameObject skillVFXPrefab;      // 拖入你的技能特效Prefab
    public Vector3 skillVFXSpawnOffset = Vector3.zero;   // 特效生成点偏移量

    [Header("Boss技能2设置")]
    public float skill2Cooldown = 12f;
    private float lastSkill2Time = -1f;
    public float skill2Range = 8f;
    public int skill2Damage = 150;
    public GameObject skill2VFXPrefab;
    public Vector3 skill2VFXSpawnOffset = Vector3.zero;
    public float skill2CooldownShortFactor = 0.25f; // 攻击范围外但技能2范围内有目标时的cd缩放，由用户填写
    private float nextSkill2Time = 0f; // 下次技能2可用时间

    private bool hasReleasedSkill2_50 = false;
    private bool hasReleasedSkill2_20 = false;

    protected override void UpdateBehavior()
    {
        // 先检测血量触发
        float hpPercent = (float)currentHealth / maxHealth;

        if (!hasReleasedSkill2_50 && hpPercent <= 0.5f)
        {
            hasReleasedSkill2_50 = true;
            ReleaseSkill2();
        }
        if (!hasReleasedSkill2_20 && hpPercent <= 0.2f)
        {
            hasReleasedSkill2_20 = true;
            ReleaseSkill2();
        }

        // Boss不移动，只检测攻击和技能
        PlayerUnit target = FindNearestPlayer(skill2Range);
        if (target != null)
        {
            currentTarget = target;
            float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
            if (distanceToTarget <= attackRange)
            {
                if (animator != null)
                    animator.SetBool("IsInCombat", true);

                // 技能优先（可选）
                if (CanReleaseSkill())
                {
                    ReleaseSkill();
                }
                // 普通攻击
                else if (Time.time - lastAttackTime >= attackInterval)
                {
                    AttackTarget(currentTarget);
                    lastAttackTime = Time.time;
                }
            }
            else if (CanReleaseSkill2())
            {
                // 攻击范围外，检测技能2范围
                if (IsPlayerInSkill2Range())
                {
                    Debug.Log("攻击范围外但技能2范围内有目标，缩短cd");
                    ReleaseSkill2();
                }
            else
            {
                currentTarget = null;
                if (animator != null)
                    animator.SetBool("IsInCombat", false);
                }
            }
        }
        else
        {
            currentTarget = null;
            if (animator != null)
                animator.SetBool("IsInCombat", false);
        }
    }

    // Boss不需要受击僵直，重写PlayTakeDamageAnimation为空
    protected override void PlayTakeDamageAnimation() { }

    // 技能释放逻辑
    public void ReleaseSkill()
    {
        lastSkillTime = Time.time;
        if (animator != null)
        {
            animator.SetTrigger("SkillTrigger");
        }
        // 技能效果由动画事件触发
    }

    private bool CanReleaseSkill()
    {
        return Time.time - lastSkillTime >= skillCooldown;
    }

    // 技能动画事件调用，支持多段
    public void OnSkillHit(int hitIndex)
    {
        Debug.Log($"Boss技能第{hitIndex}段伤害触发");
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, skillRange, LayerMask.GetMask("PlayerUnit"));
        foreach (var hit in hits)
        {
            PlayerUnit player = hit.GetComponent<PlayerUnit>();
            if (player != null && player.isAlive)
            {
                player.TakeDamage(skillDamage, this);
            }
        }
        // 可根据hitIndex做不同效果
    }

    public void OnBronEnd()
    {
        animator.Play("Idle");
    }

    public void OnSkillEnd()
    {
        if (animator != null)
            animator.Play("Idle"); // 或"Idle"，根据你的状态机命名
    }

    // 这个方法会被动画事件调用
    public void PlaySkillVFX()
    {
        if (skillVFXPrefab != null)
        {
            Vector3 spawnPos = transform.position + skillVFXSpawnOffset;
            GameObject vfx = Instantiate(skillVFXPrefab, spawnPos, Quaternion.identity);
            // 自动销毁特效
            ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(vfx, ps.main.duration + ps.main.startLifetime.constantMax);
            else
                Destroy(vfx, 0.8f);
        }
    }

    private bool IsPlayerInSkill2Range()
    {
        Debug.Log($"[Boss] 技能2范围检测: {skill2Range}");
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, skill2Range, LayerMask.GetMask("PlayerUnit"));
        foreach (var hit in hits)
        {
            PlayerUnit player = hit.GetComponent<PlayerUnit>();
            Debug.Log($"[Boss] 技能2范围内检测到单位: {player.name}");
            if (player != null && player.isAlive)
                return true;
        }
        return false;
    }

    private bool CanReleaseSkill2()
    {
        return Time.time >= nextSkill2Time;
    }

    public void ReleaseSkill2()
    {
        lastSkill2Time = Time.time;
        if (animator != null)
        {
            animator.SetTrigger("Skill2Trigger");
        }
        
        // 判断下次CD：如果玩家在攻击范围外但在技能2范围内，使用短CD
        float cd = skill2Cooldown;
        if (currentTarget is PlayerUnit player && !IsInAttackRange(player))
        {
            cd *= skill2CooldownShortFactor;
            Debug.Log($"技能2使用短CD: {cd}秒");
        }
        else
        {
            Debug.Log($"技能2使用正常CD: {cd}秒");
        }
        
        nextSkill2Time = Time.time + cd;
    }

    // 技能2动画事件方法
    public void PlaySkill2VFX()
    {
        if (skill2VFXPrefab != null)
        {
            Vector3 spawnPos = transform.position + skill2VFXSpawnOffset;
            GameObject vfx = Instantiate(skill2VFXPrefab, spawnPos, Quaternion.identity);
            ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(vfx, ps.main.duration + ps.main.startLifetime.constantMax);
            else
                Destroy(vfx, 0.6f);
        }
    }

    // 动画事件：造成技能2伤害
    public void OnSkill2Hit()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, skill2Range, LayerMask.GetMask("PlayerUnit"));
        foreach (var hit in hits)
        {
            PlayerUnit player = hit.GetComponent<PlayerUnit>();
            if (player != null && player.isAlive)
            {
                player.TakeDamage(skill2Damage, this);
            }
        }
    }

    private bool IsInAttackRange(PlayerUnit target)
    {
        return Vector2.Distance(transform.position, target.transform.position) <= attackRange;
    }
} 