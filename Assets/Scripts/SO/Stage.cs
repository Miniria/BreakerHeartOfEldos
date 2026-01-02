using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewStage", menuName = "GameData/Stage")]
public class Stage : ScriptableObject
{
    [Header("Stage Identity")]
    [Tooltip("ใช้ ID ที่ไม่ซ้ำกัน เช่น stage_1_1, stage_1_2")]
    public string stageID; 
    public int chapterIndex;
    public int stageIndexInChapter;

    [Header("Stage Information")]
    public string stageName;
    [TextArea] public string stageDescription;
    public Sprite stageIcon;

    [Header("Combat Setup")]
    public string combatSceneName = "CombatScene";
    public List<UnitsDataSO> enemiesInThisStage;

    [Header("Rewards")]
    [Tooltip("รางวัลที่จะได้รับเมื่อผ่านด่านนี้เป็นครั้งแรกเท่านั้น")]
    public List<RewardItem> firstTimeRewards;

    [Tooltip("รางวัลที่จะได้รับทุกครั้งที่ผ่านด่าน")]
    public List<RewardItem> guaranteedRewards;

    [Tooltip("ไอเทมที่มีโอกาสดรอปเมื่อผ่านด่าน")]
    public List<RandomLootItem> randomDrops;

    [Header("Item Drop Settings")]
    [Tooltip("Quality Level ต่ำสุดของไอเทมที่ดรอปได้ในด่านนี้")]
    [Range(1, 100)] public int minItemQuality = 1;
    [Tooltip("Quality Level สูงสุดของไอเทมที่ดรอปได้ในด่านนี้")]
    [Range(1, 100)] public int maxItemQuality = 1;

    // ทำให้เราสามารถใช้ .name เป็น ID ได้โดยอัตโนมัติ ถ้าไม่ได้ตั้งค่า stageID เอง
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(stageID))
        {
            stageID = this.name;
        }
        // ตรวจสอบให้แน่ใจว่า minItemQuality ไม่เกิน maxItemQuality
        if (minItemQuality > maxItemQuality)
        {
            maxItemQuality = minItemQuality;
        }
    }
}
