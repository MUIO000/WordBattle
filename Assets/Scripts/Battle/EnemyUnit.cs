using UnityEngine;
using System.Collections;

public class EnemyUnit : Unit
{
    [Header("Boss设置")]
    public float attackInterval = 2f; // 攻击间隔

    [Header("受伤效果")]
    [SerializeField] private Color hurtColor = new Color(1f, 1f, 0.5f, 1f); // 淡黄色
    [SerializeField] private float hurtFlashTime = 0.15f;

    protected override void Start()
    {
        base.Start();
        // 不要在这里重新设置属性值，让 Inspector 中的值生效
    }

    protected override void UpdateBehavior()
    {
        // 查找范围内的士兵
        PlayerUnit target = FindNearestPlayer();
        // Debug.Log($"target: {target}");
        if (target != null)
        {
            currentTarget = target;
            float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
            // Debug.Log($"攻击范围: {attackRange}, 距离: {distanceToTarget}, 攻击间隔: {attackInterval}, 上次攻击: {lastAttackTime}, 当前时间: {Time.time}");
            // 只有在攻击范围内才进行攻击
            if (distanceToTarget <= attackRange)
            {
                // 设置为战斗状态
                if (animator != null)
                {
                    animator.SetBool("IsInCombat", true);
                }

                // 检查是否可以攻击
                if (Time.time - lastAttackTime >= attackInterval)
                {
                    AttackTarget(currentTarget);
                    lastAttackTime = Time.time;
                }
            }
            else
            {
                // 目标在检测范围内但不在攻击范围内，退出战斗状态
                currentTarget = null;
                if (animator != null)
                {
                    animator.SetBool("IsInCombat", false);
                }
            }
        }
        else
        {
            // 没有目标时退出战斗状态
            currentTarget = null;
            if (animator != null)
            {
                animator.SetBool("IsInCombat", false);
            }
        }
    }

    public PlayerUnit FindNearestPlayer()
    {
        return FindNearestPlayer(attackRange);
    }
        
    public PlayerUnit FindNearestPlayer(float range)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, 
            range, 
            LayerMask.GetMask("PlayerUnit")
        );

        PlayerUnit nearest = null;
        float minDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            PlayerUnit player = hit.GetComponent<PlayerUnit>();
            if (player != null && player.isAlive)
            {
                float distance = Vector2.Distance(transform.position, player.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = player;
                }
            }
        }

        return nearest;
    }

    public override void Die()
    {
        base.Die();

        // 设置死亡动画参数
        if (animator != null)
        {
            animator.SetBool("IsDead", true);
            animator.SetBool("IsInCombat", false);
        }
    }

    public override void TakeDamage(int damage, Unit attacker = null)
    {
        base.TakeDamage(damage, attacker);

        // 受伤闪烁淡黄色
        FlashHurt();
    }

    private void FlashHurt()
    {
        if (spriteRenderer != null)
            StartCoroutine(HurtFlashCoroutine());
    }

    private IEnumerator HurtFlashCoroutine()
    {
        spriteRenderer.color = hurtColor;
        yield return new WaitForSeconds(hurtFlashTime);
        spriteRenderer.color = originalColor;
    }
}