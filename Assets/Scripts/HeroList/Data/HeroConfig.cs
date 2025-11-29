/*
 * 英雄配置数据
 */

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeroConfig
{
    public string heroId; // 英雄ID
    public string name; // 英雄名称
    public string classIcon; // 英雄职业类型图标
    public string className; // 英雄职业类型
    public string description; // 英雄描述
    public string skillIcon; // 技能图标
    public string skillDesc; // 技能描述
    public int unlockCost; // 解锁费用
    public List<HeroStarStats> starStats; // 每一星级对应的属性集合 
    public List<int> starUpgradeCost; // 各星级升级费用
}

[System.Serializable]
public class HeroStarStats
{
    public int star; // 星级
    public int hp; // 生命值
    public int atk; // 攻击力
    public int skill; // 技能值
    public int def; // 防御力
}