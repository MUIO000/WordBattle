using UnityEngine;

[CreateAssetMenu(fileName = "TestData", menuName = "Game/Test Data")]
public class TestDataInitializer : ScriptableObject
{
    [Header("测试用单位数据")]
    public UnitData[] testUnits;
    
    // 初始化测试数据
    public void InitializeTestUnits()
    {
        testUnits = new UnitData[3];
        
        // 战士单位
        testUnits[0] = new UnitData
        {
            unitName = "剑士",
            maxHealth = 800,
            attackDamage = 150,
            attackSpeed = 1.5f,
            moveSpeed = 3f,
            summonCost = 20,
            unitType = UnitType.Warrior,
            isUnlocked = true,
            unitColor = Color.red,
            specialAbilityCooldown = 5f,
            abilityDescription = "冲锋攻击，造成额外伤害"
        };
        
        // 弓箭手单位
        testUnits[1] = new UnitData
        {
            unitName = "弓箭手",
            maxHealth = 500,
            attackDamage = 120,
            attackSpeed = 1f,
            moveSpeed = 4f,
            summonCost = 25,
            unitType = UnitType.Archer,
            isUnlocked = true,
            unitColor = Color.green,
            specialAbilityCooldown = 6f,
            abilityDescription = "穿透射击，可攻击多个目标"
        };
        
        // 法师单位
        testUnits[2] = new UnitData
        {
            unitName = "法师",
            maxHealth = 400,
            attackDamage = 200,
            attackSpeed = 2f,
            moveSpeed = 2.5f,
            summonCost = 35,
            unitType = UnitType.Mage,
            isUnlocked = false, // 需要解锁
            unitColor = Color.blue,
            specialAbilityCooldown = 8f,
            abilityDescription = "火球术，范围攻击"
        };
    }
}
