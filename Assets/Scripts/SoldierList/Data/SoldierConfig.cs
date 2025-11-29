/*
 * 士兵配置数据
 */

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoldierConfig
{
    public string soldierId;       // 士兵ID
    public string name;            // 士兵名称
    public string soldierClass;    // 士兵职业（archer/warrior/mage/assassin）
    public string description;     // 士兵描述
    public int hp;                 // 生命值
    public int atk;                // 攻击力
    public int def;                // 防御力
    public int unlockCost;         // 解锁费用
    public List<SoldierLevelStats> levelStats; // 各等级属性（0级是基础属性）
    public List<int> upgradeCost;  // 升级费用列表（升到1级、2级、3级的费用）
}

[System.Serializable]
public class SoldierLevelStats
{
    public int level;              // 等级（0-3）
    public int hp;                 // 生命值
    public int atk;                // 攻击力
    public int def;                // 防御力
}

[System.Serializable]
public class SoldierStatus
{
    public string soldierId;       // 士兵ID
    public bool isUnlocked;        // 是否解锁
    public bool isSelected;        // 是否被选中
    public int level;              // 当前等级（0-3）
} 