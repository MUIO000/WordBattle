using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "WordBattle/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("关卡基本信息")]
    public string levelName;        // 关卡名称
    public int levelNumber;         // 关卡编号
    public string description;      // 关卡描述
    
    [Header("玩家设置")]
    public int playerMaxHealth = 1500; // 玩家最大生命值
    public float timeLimit = 180f;     // 时间限制（秒）
    public int targetScore = 1000;     // 目标分数
    
    [Header("Boss设置")]
    public GameObject bossPrefab;   // Boss预制体
    public UnitData bossData;       // Boss单位数据
    public int bossMaxHealth = 4500;   // Boss最大生命值
    
    [Header("词汇设置")]
    public QuestionDatabase questionDatabase; // 词汇数据库
    public int minWordDifficulty = 1; // 最小词汇难度
    public int maxWordDifficulty = 5; // 最大词汇难度
    
    [Header("奖励设置")]
    public int correctAnswerPoints = 10; // 答对得分
    public int comboBonus = 5;          // 连击加分
    public float timeBonusMultiplier = 1.5f; // 时间奖励倍数
}