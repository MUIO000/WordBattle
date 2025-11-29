using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public abstract class Unit : MonoBehaviour
{
    [Header("基础属性")]
    [SerializeField] protected string unitName = "单位";
    [SerializeField] protected bool isPlayerUnit = true;
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected int attackDamage = 20;
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float attackRange = 1.5f;

    // 添加属性访问器
    public string UnitName => unitName;
    public bool IsPlayerUnit => isPlayerUnit;
    public int MaxHealth => maxHealth;
    public int AttackDamage => attackDamage;
    public float MoveSpeed => moveSpeed;
    public float AttackRange => attackRange;

    [Header("当前状态")]
    public int currentHealth;
    public bool isAlive = true;
    public bool canAttack = true;
    public bool canMove = true;

    [Header("组件")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("事件")]
    public UnityEvent OnUnitDeath;

    protected Unit currentTarget;
    protected float lastAttackTime;

    // 保存原始颜色和缩放
    public Color originalColor;
    public Vector3 originalScale;
    string previousState;

    public GameObject healthBarPrefab; // 拖入不同的血条Prefab
    [Header("血条调试")]
    public Vector3 healthBarOffset = new Vector3(0, 2.7f, 0); // 可调节血条偏移
    [SerializeField] private float healthBarAppearDelay = 0f; // 血条出现延迟时间
    protected GameObject healthBarInstance;
    protected Image healthBarFillImage; // 用于控制Fill
    private bool isHealthBarVisible = false; // 控制血条显示状态

    protected virtual void Start()
    {
        // 初始化血量
        currentHealth = maxHealth;
        
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 初始化时保存原始颜色和缩放
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
        originalScale = transform.localScale;

        // 实例化血条但初始隐藏
        if (healthBarPrefab != null)
        {   
            // Debug.Log($"实例化血条 - {unitName} 初始血量: {currentHealth}/{maxHealth}");
            healthBarInstance = Instantiate(healthBarPrefab, transform);
            healthBarInstance.transform.localPosition = healthBarOffset;
            healthBarInstance.SetActive(false); // 初始隐藏血条
            
            // 获取Fill对象的Image组件
            var fillTrans = healthBarInstance.transform.Find("FillArea/Fill");
            if (fillTrans != null)
            {
                healthBarFillImage = fillTrans.GetComponent<Image>();
                // 立即更新血条显示，确保初始血量正确显示
                UpdateHealthBar();
                // Debug.Log($"血条初始化完成 - {unitName} 血量比例: {(float)currentHealth / maxHealth}");
            }

            // 启动延迟显示血条的协程
            StartCoroutine(ShowHealthBarAfterDelay());
        }
    }

    private IEnumerator ShowHealthBarAfterDelay()
    {
        yield return new WaitForSeconds(healthBarAppearDelay);
        if (healthBarInstance != null && isAlive)
        {
            healthBarInstance.SetActive(true);
            isHealthBarVisible = true;
            // 再次确保血条显示正确的血量
            UpdateHealthBar();
            // Debug.Log($"血条显示 - {unitName} 当前血量: {currentHealth}/{maxHealth}");
        }
    }

    protected virtual void Update()
    {
        if (!isAlive) return;
        // 只在血条可见时更新位置
        if (healthBarInstance != null && isHealthBarVisible)
            healthBarInstance.transform.localPosition = healthBarOffset;
        UpdateBehavior();
    }

    protected virtual void UpdateBehavior()
    {
        // 子类重写具体行为
    }

    public virtual void TakeDamage(int damage, Unit attacker = null)
    {
        if (!isAlive) return;
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        UpdateHealthBar();

        // Debug.Log($"{unitName} 受到 {damage} 伤害，剩余 {currentHealth} 血量");

        // 当血量为0时立即隐藏血条
        if (currentHealth <= 0 && healthBarInstance != null)
        {
            healthBarInstance.SetActive(false);
            isHealthBarVisible = false;
            Die();
        }

        // 触发受击动画（如果有）
        PlayTakeDamageAnimation();
    }

    public virtual void Die()
    {
        if (!isAlive) return;

        isAlive = false;
        canAttack = false;
        canMove = false;

        if (animator != null)
        {
            animator.SetTrigger("Death");
            animator.SetBool("IsDead", true);
        }

        OnUnitDeath?.Invoke();
        // Debug.Log($"{unitName} 死亡");
    }

    protected bool CanAttack()
    {
        return false;
    }

    public virtual void AttackTarget(Unit target)
    {
        if (target == null || !target.isAlive) return;
        Debug.Log($"目标：{target.UnitName} 攻击：{unitName}");

        // 触发攻击动画
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // 实际伤害逻辑由动画事件触发
        // Debug.Log($"{unitName} 攻击了 {target.UnitName}");
    }

    #region Animation Events
    public virtual void OnAttackHit()
    {
        if (currentTarget != null && currentTarget.isAlive)
        {
            currentTarget.TakeDamage(attackDamage, this);
        }
        // Debug.Log($"{unitName} 攻击 {currentTarget.unitName}");
    }

    public virtual void OnAttackStart()
    {
        canMove = false;
    }

    public virtual void OnAttackEnd()
    {
        canMove = true;
        lastAttackTime = Time.time; // 只在攻击动画完整结束时更新
    }
    #endregion

    protected virtual void PlayTakeDamageAnimation()
    {
        if (animator == null) return;

        // 检查 Animator 是否有名为 "TakeDamage" 的动画状态
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("TakeDamage")) return; // 避免重复切入

        previousState = GetCurrentStateName(); // 保存当前状态名（可选）

        // 立即中断任何动画，平滑过渡到 TakeDamage 动画，0 表示无过渡时间
        animator.CrossFade("TakeDamage", 0f);
    }

    // 获取当前动画状态名
    private string GetCurrentStateName() {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Idle") ? "Idle" :
            stateInfo.IsName("Walk") ? "Walk" :
            stateInfo.IsName("Attack") ? "Attack" :
            stateInfo.IsName("CombatIdle") ? "CombatIdle" :
            "Idle"; // fallback
    }

    public void OnTakeDamageEnd() 
    {
        animator.Play("CombatIdle"); // 或你想回的状态，比如 Idle
    }

    protected void UpdateHealthBar()
    {
        if (healthBarFillImage != null)
        {
            healthBarFillImage.fillAmount = (float)currentHealth / maxHealth;
        }
    }
}
