/*
 * 士兵测试数据
 */

using System.Collections.Generic;
using UnityEngine;

public static class SoldierTestData
{
    // 获取所有士兵配置
    public static List<SoldierConfig> GetSoldierConfigs()
    {
        return new List<SoldierConfig>
        {
            // 弓箭手
            new SoldierConfig
            {
                soldierId = "archer001",
                name = "见习弓箭手",
                soldierClass = "archer",
                description = "基础的远程攻击单位，攻击力中等",
                unlockCost = 100,
                levelStats = new List<SoldierLevelStats>
                {
                    new SoldierLevelStats { level = 0, hp = 20, atk = 8, def = 2 },
                    new SoldierLevelStats { level = 1, hp = 25, atk = 11, def = 3 },
                    new SoldierLevelStats { level = 2, hp = 30, atk = 14, def = 4 },
                    new SoldierLevelStats { level = 3, hp = 35, atk = 18, def = 5 }
                },
                upgradeCost = new List<int> { 150, 250, 400 }
            },
            new SoldierConfig
            {
                soldierId = "archer002",
                name = "精英弓箭手",
                soldierClass = "archer",
                description = "训练有素的弓箭手，攻击力更高",
                unlockCost = 300,
                levelStats = new List<SoldierLevelStats>
                {
                    new SoldierLevelStats { level = 0, hp = 25, atk = 12, def = 3 },
                    new SoldierLevelStats { level = 1, hp = 30, atk = 16, def = 4 },
                    new SoldierLevelStats { level = 2, hp = 35, atk = 20, def = 5 },
                    new SoldierLevelStats { level = 3, hp = 40, atk = 25, def = 6 }
                },
                upgradeCost = new List<int> { 300, 500, 800 }
            },
            new SoldierConfig
            {
                soldierId = "archer003",
                name = "神射手",
                soldierClass = "archer",
                description = "顶级弓箭手，拥有极高的攻击力",
                unlockCost = 500,
                levelStats = new List<SoldierLevelStats>
                {
                    new SoldierLevelStats { level = 0, hp = 30, atk = 18, def = 4 },
                    new SoldierLevelStats { level = 1, hp = 35, atk = 24, def = 5 },
                    new SoldierLevelStats { level = 2, hp = 40, atk = 32, def = 6 },
                    new SoldierLevelStats { level = 3, hp = 45, atk = 40, def = 8 }
                },
                upgradeCost = new List<int> { 500, 800, 1200 }
            },
            
            // 战士
            new SoldierConfig
            {
                soldierId = "warrior001",
                name = "见习战士",
                soldierClass = "warrior",
                description = "基础的近战单位，生命值较高",
                unlockCost = 100,
                levelStats = new List<SoldierLevelStats>
                {
                    new SoldierLevelStats { level = 0, hp = 30, atk = 6, def = 5 },
                    new SoldierLevelStats { level = 1, hp = 35, atk = 8, def = 7 },
                    new SoldierLevelStats { level = 2, hp = 40, atk = 10, def = 9 },
                    new SoldierLevelStats { level = 3, hp = 45, atk = 12, def = 12 }
                },
                upgradeCost = new List<int> { 150, 250, 400 }
            },
            new SoldierConfig
            {
                soldierId = "warrior002",
                name = "精英战士",
                soldierClass = "warrior",
                description = "经验丰富的战士，攻防平衡",
                unlockCost = 300,
                levelStats = new List<SoldierLevelStats>
                {
                    new SoldierLevelStats { level = 0, hp = 35, atk = 10, def = 8 },
                    new SoldierLevelStats { level = 1, hp = 40, atk = 13, def = 11 },
                    new SoldierLevelStats { level = 2, hp = 45, atk = 16, def = 14 },
                    new SoldierLevelStats { level = 3, hp = 50, atk = 20, def = 18 }
                },
                upgradeCost = new List<int> { 300, 500, 800 }
            },
            new SoldierConfig
            {
                soldierId = "warrior003",
                name = "铁甲勇士",
                soldierClass = "warrior",
                description = "顶级战士，拥有极高的生命值和防御",
                unlockCost = 500,
                levelStats = new List<SoldierLevelStats>
                {
                    new SoldierLevelStats { level = 0, hp = 40, atk = 15, def = 12 },
                    new SoldierLevelStats { level = 1, hp = 45, atk = 18, def = 16 },
                    new SoldierLevelStats { level = 2, hp = 50, atk = 22, def = 20 },
                    new SoldierLevelStats { level = 3, hp = 50, atk = 26, def = 25 }
                },
                upgradeCost = new List<int> { 500, 800, 1200 }
            },
            
            // 法师
            new SoldierConfig
            {
                soldierId = "mage001",
                name = "见习法师",
                soldierClass = "mage",
                description = "基础的魔法单位，攻击力较高但脆弱",
                unlockCost = 150,
                levelStats = new List<SoldierLevelStats>
                {
                    new SoldierLevelStats { level = 0, hp = 15, atk = 10, def = 1 },
                    new SoldierLevelStats { level = 1, hp = 20, atk = 14, def = 2 },
                    new SoldierLevelStats { level = 2, hp = 25, atk = 18, def = 3 },
                    new SoldierLevelStats { level = 3, hp = 30, atk = 23, def = 4 }
                },
                upgradeCost = new List<int> { 200, 350, 550 }
            },
            new SoldierConfig
            {
                soldierId = "mage002",
                name = "元素法师",
                soldierClass = "mage",
                description = "掌握元素魔法的法师，攻击力强大",
                unlockCost = 400,
                levelStats = new List<SoldierLevelStats>
                {
                    new SoldierLevelStats { level = 0, hp = 20, atk = 15, def = 2 },
                    new SoldierLevelStats { level = 1, hp = 25, atk = 20, def = 3 },
                    new SoldierLevelStats { level = 2, hp = 30, atk = 26, def = 4 },
                    new SoldierLevelStats { level = 3, hp = 35, atk = 33, def = 5 }
                },
                upgradeCost = new List<int> { 400, 650, 1000 }
            },
            new SoldierConfig
            {
                soldierId = "mage003",
                name = "大法师",
                soldierClass = "mage",
                description = "顶级法师，拥有毁灭性的魔法攻击",
                unlockCost = 600,
                levelStats = new List<SoldierLevelStats>
                {
                    new SoldierLevelStats { level = 0, hp = 25, atk = 22, def = 3 },
                    new SoldierLevelStats { level = 1, hp = 30, atk = 28, def = 4 },
                    new SoldierLevelStats { level = 2, hp = 35, atk = 36, def = 5 },
                    new SoldierLevelStats { level = 3, hp = 40, atk = 45, def = 6 }
                },
                upgradeCost = new List<int> { 600, 900, 1400 }
            },
            
            // 刺客
            new SoldierConfig
            {
                soldierId = "assassin001",
                name = "见习刺客",
                soldierClass = "assassin",
                description = "基础的刺客单位，攻击速度快",
                unlockCost = 150,
                levelStats = new List<SoldierLevelStats>
                {
                    new SoldierLevelStats { level = 0, hp = 22, atk = 9, def = 3 },
                    new SoldierLevelStats { level = 1, hp = 27, atk = 12, def = 4 },
                    new SoldierLevelStats { level = 2, hp = 32, atk = 16, def = 5 },
                    new SoldierLevelStats { level = 3, hp = 37, atk = 20, def = 6 }
                },
                upgradeCost = new List<int> { 200, 350, 550 }
            },
            new SoldierConfig
            {
                soldierId = "assassin002",
                name = "暗影刺客",
                soldierClass = "assassin",
                description = "潜行在暗影中的刺客，暴击率高",
                unlockCost = 400,
                levelStats = new List<SoldierLevelStats>
                {
                    new SoldierLevelStats { level = 0, hp = 27, atk = 14, def = 4 },
                    new SoldierLevelStats { level = 1, hp = 32, atk = 18, def = 5 },
                    new SoldierLevelStats { level = 2, hp = 37, atk = 23, def = 6 },
                    new SoldierLevelStats { level = 3, hp = 42, atk = 30, def = 8 }
                },
                upgradeCost = new List<int> { 400, 650, 1000 }
            },
            new SoldierConfig
            {
                soldierId = "assassin003",
                name = "幽灵刺客",
                soldierClass = "assassin",
                description = "顶级刺客，攻击力极高且难以捉摸",
                unlockCost = 600,
                levelStats = new List<SoldierLevelStats>
                {
                    new SoldierLevelStats { level = 0, hp = 32, atk = 20, def = 5 },
                    new SoldierLevelStats { level = 1, hp = 37, atk = 26, def = 6 },
                    new SoldierLevelStats { level = 2, hp = 42, atk = 33, def = 8 },
                    new SoldierLevelStats { level = 3, hp = 47, atk = 42, def = 10 }
                },
                upgradeCost = new List<int> { 600, 900, 1400 }
            }
        };
    }
    
    // 获取士兵状态测试数据
    public static List<SoldierStatus> GetSoldierStatuses()
    {
        return new List<SoldierStatus>
        {
            // 默认解锁第一个弓箭手和战士
            new SoldierStatus
            {
                soldierId = "archer001",
                isUnlocked = true,
                isSelected = true,
                level = 0
            },
            new SoldierStatus
            {
                soldierId = "archer002",
                isUnlocked = false,
                isSelected = false,
                level = 0
            },
            new SoldierStatus
            {
                soldierId = "warrior001",
                isUnlocked = true,
                isSelected = true,
                level = 0
            },
            new SoldierStatus
            {
                soldierId = "warrior002",
                isUnlocked = false,
                isSelected = false,
                level = 0
            },
            new SoldierStatus
            {
                soldierId = "mage001",
                isUnlocked = true,
                isSelected = true,
                level = 3
            },
            new SoldierStatus
            {
                soldierId = "mage002",
                isUnlocked = false,
                isSelected = false,
                level = 0
            },
            new SoldierStatus
            {
                soldierId = "assassin001",
                isUnlocked = true,
                isSelected = true,
                level = 0
            },
            new SoldierStatus
            {
                soldierId = "assassin002",
                isUnlocked = false,
                isSelected = false,
                level = 0
            },
        };
    }
} 