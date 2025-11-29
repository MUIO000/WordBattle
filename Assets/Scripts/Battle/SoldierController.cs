using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoldierController : PlayerUnit
{
    [Header("士兵AI")]
    public float attackInterval = 1f;

    [Header("远程攻击设置")]
    public GameObject arrowPrefab;        // 箭矢预制体
    public Transform arrowSpawnPoint;     // 箭矢生成点
    public float arrowSpeed = 10f;        // 箭矢飞行速度
    public bool isRangedUnit = false;     // 是否为远程单位

    [Header("受伤效果")]
    [SerializeField] private Color hurtColor = Color.red;
    [SerializeField] private float hurtFlashTime = 0.15f;

    public Transform bossTarget;
    private Vector3 attackPosition; // 终点线位置
    private bool hasReachedAttackPosition = false;
    private float spawnDelay = 1f; // 生成后延迟移动的时间
    private float spawnTime; // 生成时间点

    [SerializeField] private float attackPositionOffset = 0.5f; // 终点线偏移量
    
    public Vector3 arrowSpawnOffset = Vector3.zero; // 箭矢发射点偏移
    public Vector3 arrowTargetOffset = Vector3.zero; // 箭矢目标点偏移

    public static List<SoldierController> allSoldiers = new List<SoldierController>();

    private bool isDead = false;

    protected override void Start()
    {
        base.Start();
        isPlayerUnit = true;
        FindBoss();
        allSoldiers.Add(this);
        spawnTime = Time.time; // 记录生成时间
        lastAttackTime = Time.time;
        // 设置终点线为Boss前方一定距离
        if (bossTarget != null)
        {
            Vector3 directionToBoss = (bossTarget.position - transform.position).normalized;
            attackPosition = bossTarget.position - directionToBoss * (attackRange + attackPositionOffset); // 终点线在攻击范围外
        }
    }

    protected void OnDestroy()
    {
        allSoldiers.Remove(this);
    }

    protected override void UpdateBehavior()
    {
        // 如果还在生成延迟时间内，不执行移动逻辑
        if (Time.time - spawnTime < spawnDelay)
        {
            return;
        }
        // 检查boss是否死亡
        if (bossTarget == null || !bossTarget.gameObject.activeInHierarchy)
        {
            if (animator != null)
            {
                animator.SetBool("IsInCombat", false);
                animator.SetBool("IsMoving", false);
            }
            return;
        }

        Unit bossUnit = bossTarget.GetComponent<Unit>();
        if (bossUnit != null && !bossUnit.isAlive)
        {
            if (animator != null)
            {
                animator.SetBool("IsInCombat", false);
                animator.SetBool("IsMoving", false);
            }
            return;
        }

        // 只靠碰撞体移动到boss前方一定距离
        float stopDistance = attackRange + attackPositionOffset;
        float distanceToBoss = Vector2.Distance(transform.position, bossTarget.position);
        if (distanceToBoss > stopDistance)
        {
            // 向boss移动
            Vector2 direction = (bossTarget.position - transform.position).normalized;
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

            if (animator != null)
            {
                animator.SetBool("IsMoving", true);
                animator.SetBool("IsInCombat", false);
            }

            // 设置朝向
            if (direction.x > 0)
                transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            else
                transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else
        {
            // 到达攻击距离，停下并攻击
            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
                animator.SetBool("IsInCombat", true);
            }

            if (Time.time - lastAttackTime >= attackInterval)
            {
                AttackTarget(currentTarget);
                lastAttackTime = Time.time;
            }
        }
    }

    private void FindBoss()
    {
        EnemyUnit boss = FindAnyObjectByType<EnemyUnit>();
        if (boss != null)
        {
            bossTarget = boss.transform;
            currentTarget = boss;
        }
    }

    public override void AttackTarget(Unit target)
    {
        base.AttackTarget(target);
    }

    // 由动画事件调用
    public void FireArrow()
    {
        if (isRangedUnit && arrowPrefab != null && arrowSpawnPoint != null && currentTarget != null && currentTarget.isAlive)
        {
            StartCoroutine(SpawnArrow(currentTarget));
        }
    }
    
    private IEnumerator SpawnArrow(Unit target)
    {
        // 等待动画播放到发射箭矢的时机
        yield return new WaitForSeconds(0.2f); // 根据实际动画调整时间

        // 生成箭矢
        Vector3 spawnPos = arrowSpawnPoint.position + arrowSpawnOffset;
        GameObject arrow = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
        Vector3 targetPosition = target.transform.position + arrowTargetOffset;
        Vector3 direction = (targetPosition - spawnPos).normalized;
        
        // 设置箭矢朝向
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle);

        // 移动箭矢直到击中目标
        while (arrow != null && Vector3.Distance(arrow.transform.position, targetPosition) > 0.1f)
        {
            arrow.transform.position += direction * arrowSpeed * Time.deltaTime;
            yield return null;
        }

        // 箭矢击中目标
        if (arrow != null)
        {
            OnAttackHit(); // 触发伤害
            Destroy(arrow);
        }
    }

    public override void Die()
    {
        if (isDead) return; // 防止重复死亡
        isDead = true;

        base.Die();
        
        // 设置死亡动画参数
        if (animator != null)
        {
            animator.SetBool("IsDead", true);
            animator.SetBool("IsMoving", false);
            animator.SetBool("IsInCombat", false);
            animator.SetTrigger("Death");
            Debug.Log("死亡动画");
        }
        
        // 通知BattleManager
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnPlayerUnitDeath(this);
        }

        // 5秒后销毁尸体
        StartCoroutine(DestroyAfterDelay(5f));
    }
    
    
    
    // 受伤闪红效果（从SoldierAnimationController合并过来）
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

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }


}