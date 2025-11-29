using UnityEngine;

[CreateAssetMenu(fileName = "NewUnit", menuName = "WordBattle/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("单位基本信息")]
    public string unitName;         // 单位名称
    public UnitType unitType;       // 单位类型
    public string description;      // 单位描述
    public Color unitColor = Color.white; // 单位颜色
    public bool isUnlocked = true;  // 是否已解锁
    
    [Header("单位预制体")]
    public GameObject unitPrefab;   // 单位预制体
    
    [Header("基础属性")]
    public int maxHealth = 100;     // 最大生命值
    public int attackDamage = 10;   // 攻击力
    public float attackSpeed = 1f;  // 攻击速度
    public float moveSpeed = 2f;    // 移动速度
    
    [Header("战斗属性")]
    public float attackRange = 1f;  // 攻击范围
    public float detectionRange = 3f; // 检测范围
    public bool canAttack = true;   // 是否可以攻击
    public bool canMove = true;     // 是否可以移动
    
    [Header("友军特殊属性")]
    public int summonCost = 20;     // 召唤消耗
    public float specialAbilityCooldown = 5f; // 特殊技能冷却
    public string abilityDescription; // 技能描述
    
    [Header("特效设置")]
    public GameObject attackEffectPrefab; // 攻击特效
    public GameObject deathEffectPrefab;  // 死亡特效
    public GameObject spawnEffectPrefab;  // 生成特效
}

public enum UnitType
{
    Warrior,        // 战士 - 高血量，近战
    Archer,         // 弓箭手 - 远程攻击
    Mage,           // 法师 - 魔法攻击，群体伤害
    Boss        // Boss
}
