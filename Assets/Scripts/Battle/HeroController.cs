using UnityEngine;

public class HeroController : SoldierController
{
    [Header("英雄技能设置")]
    public float skillCooldown = 5f;
    private float lastSkillTime = -999f;
    public int skillDamage = 100; // 每段伤害
    public int skillHitCount = 3; // 总段数
    private bool isSuperArmor = false;
    // private float spawnDelay = 1f; // 生成后延迟移动的时间
    // private float spawnTime; // 生成时间点

    public bool isHeroAlive = true;

    protected override void Start()
    {
        base.Start();
        if (animator == null) animator = GetComponent<Animator>();
        // spawnTime = Time.time; // 记录生成时间
    }


    // 出生动画
    public void PlayIdleAnimation()
    {
        animator.Play("Idle");
    }


    // 技能释放
    public void ReleaseSkill()
    {
        if (!isHeroAlive) return; // 死亡不能放技能

        if (Time.time - lastSkillTime < skillCooldown) return; // 冷却中
        lastSkillTime = Time.time;
        isSuperArmor = true; // 进入霸体
        Debug.Log("animator:" + animator);
        if (animator != null)
        {
            // 立即打断当前动画，切到Skill
            animator.ResetTrigger("Attack"); // 可选，防止残留
            animator.ResetTrigger("SkillTrigger");
            animator.CrossFade("Skill", 0f); // "Skill"为动画状态名
            animator.SetTrigger("SkillTrigger");
            Debug.Log("技能释放");
        }
        
    }

    // 动画事件：每段伤害时调用
    // 可以在动画事件里传入第几段（如OnSkillHit(1), OnSkillHit(2)...）
    public void OnSkillHit(int hitIndex)
    {
        // 这里可以根据hitIndex做不同的效果
        Debug.Log($"技能第{hitIndex}段伤害触发");

        // 例如对当前目标造成伤害
        if (currentTarget != null && currentTarget.isAlive)
        {
            currentTarget.TakeDamage(skillDamage, this);
        }

        // 也可以做范围伤害、特效等
    }

    public void OnSkillEnd()
    {
        isSuperArmor = false; // 退出霸体
        if (animator != null)
            animator.Play("CombatIdle");
    }

    protected override void PlayTakeDamageAnimation()
    {
        if (isSuperArmor) return; // 霸体时不播放受击动画
        base.PlayTakeDamageAnimation();
    }

    public override void Die(){
        base.Die();
        isHeroAlive = false;
    }
}
