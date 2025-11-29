using System;
using UnityEngine;

[System.Serializable]
public class WordData
{
    [Header("单词信息")]
    public string englishWord;          // 英文单词
    public string chineseTranslation;   // 中文翻译
    
    [Header("错误选项")]
    public string[] wrongOptions;       // 错误的翻译选项（3个）
    
    [Header("游戏设置")]
    [Range(1, 3)]
    public int difficulty;              // 难度等级 (1-简单, 2-中等, 3-困难)
    public int rewardPoints;            // 答对获得的分数
    
    [Header("单词属性")]
    public WordCategory category;       // 单词分类
    public bool isUnlocked;            // 是否已解锁
}

public enum WordCategory
{
    Verb,           // 动词
    Noun,           // 名词
    Adjective,      // 形容词
    Common          // 常用词
}
