/*
 * 英雄状态数据
 */
    
[System.Serializable]
public class HeroStatus
{
    public string heroId; // 英雄ID
    public bool isUnlocked; // 是否解锁
    public bool isSelected; // 是否被选中
    public int star; // 当前星级（即等级）
}