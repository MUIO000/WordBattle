/*
 * 测试数据
 */

using System.Collections.Generic;
using UnityEngine;

public static class TestData
{
    // 英雄配置测试数据
    public static List<HeroConfig> GetHeroConfigs()
    {
        return new List<HeroConfig>
        {
            new HeroConfig
            {
                heroId = "hero001",
                name = "铁甲战士",
                classIcon = "HeroIcons/classIcon/warrior",
                className = "战士",
                description = "强大的近战战士，拥有高生命值和防御力",
                skillIcon = "HeroIcons/skillIcon/Skill001",
                skillDesc = "盾击：对敌人造成伤害并击退",
                unlockCost = 100,
                starStats = new List<HeroStarStats>
                {
                    new HeroStarStats { star = 1, hp = 30, atk = 15, skill = 10, def = 20 },
                    new HeroStarStats { star = 2, hp = 40, atk = 22, skill = 15, def = 30 },
                    new HeroStarStats { star = 3, hp = 50, atk = 32, skill = 22, def = 45 },
                    new HeroStarStats { star = 4, hp = 70, atk = 45, skill = 32, def = 65 },
                    new HeroStarStats { star = 5, hp = 90, atk = 62, skill = 45, def = 90 }
                },
                starUpgradeCost = new List<int> { 0, 200, 500, 1000, 2000 }
            },
            
            new HeroConfig
            {
                heroId = "hero002",
                name = "神射手",
                classIcon = "HeroIcons/classIcon/archer",
                className = "射手",
                description = "远程攻击专家，攻击力高但生命值较低",
                skillIcon = "HeroIcons/skillIcon/Skill002",
                skillDesc = "精准射击：对敌人造成暴击伤害",
                unlockCost = 150,
                starStats = new List<HeroStarStats>
                {
                    new HeroStarStats { star = 1, hp = 30, atk = 25, skill = 15, def = 8 },
                    new HeroStarStats { star = 2, hp = 40, atk = 35, skill = 22, def = 12 },
                    new HeroStarStats { star = 3, hp = 50, atk = 50, skill = 32, def = 18 },
                    new HeroStarStats { star = 4, hp = 70, atk = 70, skill = 45, def = 26 },
                    new HeroStarStats { star = 5, hp = 90, atk = 95, skill = 62, def = 36 }
                },
                starUpgradeCost = new List<int> { 0, 300, 750, 1500, 3000 }
            },
            
            new HeroConfig
            {
                heroId = "hero003",
                name = "元素法师",  
                classIcon = "HeroIcons/classIcon/mage",
                className = "法师",
                description = "掌握强大魔法的法师，技能伤害极高",
                skillIcon = "HeroIcons/skillIcon/Skill003",
                skillDesc = "火球术：对敌人造成大量魔法伤害",
                unlockCost = 200,
                starStats = new List<HeroStarStats>
                {
                    new HeroStarStats { star = 1, hp = 30, atk = 12, skill = 35, def = 5 },
                    new HeroStarStats { star = 2, hp = 40, atk = 18, skill = 50, def = 8 },
                    new HeroStarStats { star = 3, hp = 50, atk = 27, skill = 70, def = 12 },
                    new HeroStarStats { star = 4, hp = 70, atk = 40, skill = 95, def = 18 },
                    new HeroStarStats { star = 5, hp = 90, atk = 57, skill = 125, def = 25 }
                },
                starUpgradeCost = new List<int> { 0, 400, 1000, 2000, 4000 }
            },
            
            new HeroConfig
            {
                heroId = "hero004",
                name = "守护者",
                classIcon = "HeroIcons/classIcon/tank",
                className = "坦克",
                description = "坚不可摧的防御专家，拥有最高的生命值和防御力",
                skillIcon = "HeroIcons/skillIcon/Skill004",
                skillDesc = "嘲讽：吸引敌人攻击自己",
                unlockCost = 120,
                starStats = new List<HeroStarStats>
                {
                    new HeroStarStats { star = 1, hp = 150, atk = 10, skill = 8, def = 35 },
                    new HeroStarStats { star = 2, hp = 225, atk = 15, skill = 12, def = 50 },
                    new HeroStarStats { star = 3, hp = 330, atk = 22, skill = 18, def = 70 },
                    new HeroStarStats { star = 4, hp = 480, atk = 32, skill = 26, def = 95 },
                    new HeroStarStats { star = 5, hp = 675, atk = 45, skill = 36, def = 125 }
                },
                starUpgradeCost = new List<int> { 0, 250, 600, 1200, 2400 }
            },
            
            new HeroConfig
            {
                heroId = "hero005",
                name = "暗影刺客",
                classIcon = "HeroIcons/classIcon/assassin",
                className = "刺客",
                description = "敏捷的刺客，拥有极高的攻击速度和暴击率",
                skillIcon = "HeroIcons/skillIcon/Skill005",
                skillDesc = "背刺：从背后攻击造成致命伤害",
                unlockCost = 180,
                starStats = new List<HeroStarStats>
                {
                    new HeroStarStats { star = 1, hp = 60, atk = 30, skill = 20, def = 6 },
                    new HeroStarStats { star = 2, hp = 90, atk = 42, skill = 28, def = 9 },
                    new HeroStarStats { star = 3, hp = 135, atk = 58, skill = 39, def = 13 },
                    new HeroStarStats { star = 4, hp = 195, atk = 80, skill = 54, def = 19 },
                    new HeroStarStats { star = 5, hp = 270, atk = 110, skill = 74, def = 26 }
                },
                starUpgradeCost = new List<int> { 0, 350, 850, 1700, 3400 }
            }
        };
    }
    
    // 英雄状态测试数据
    public static List<HeroStatus> GetHeroStatuses()
    {
        return new List<HeroStatus>
        {
            new HeroStatus
            {
                heroId = "hero001",
                isUnlocked = true,
                isSelected = true,
                star = 3,
            },
            
            new HeroStatus
            {
                heroId = "hero002",
                isUnlocked = true,
                isSelected = false,
                star = 2,

            },
            
            new HeroStatus
            {
                heroId = "hero003",
                isUnlocked = false,
                isSelected = false,
                star = 1,

            },
            
            new HeroStatus
            {
                heroId = "hero004",
                isUnlocked = true,
                isSelected = false,
                star = 4,

            },
            
            new HeroStatus
            {
                heroId = "hero005",
                isUnlocked = false,
                isSelected = false,
                star = 1,

            }
        };
    }
    
    // 获取指定英雄的配置
    public static HeroConfig GetHeroConfig(string heroId)
    {
        var configs = GetHeroConfigs();
        return configs.Find(c => c.heroId == heroId);
    }
    
    // 获取指定英雄的状态
    public static HeroStatus GetHeroStatus(string heroId)
    {
        var statuses = GetHeroStatuses();
        return statuses.Find(s => s.heroId == heroId);
    }
    
    // 获取指定英雄指定星级的属性
    public static HeroStarStats GetHeroStats(string heroId, int star)
    {
        var config = GetHeroConfig(heroId);
        if (config != null)
        {
            return config.starStats.Find(s => s.star == star);
        }
        return null;
    }
}
