using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataSO", menuName = "Scriptable Objects/LevelDataSO")]
public class LevelDataSO : ScriptableObject
{
    // List นี้จะเก็บ EXP ที่ต้องใช้เพื่อไปถึงเลเวลถัดไป
    // Index 0 = EXP ที่ต้องใช้เพื่อไป Lv. 2
    // Index 1 = EXP ที่ต้องใช้เพื่อไป Lv. 3
    // ...
    [Tooltip("EXP ที่ต้องใช้เพื่อไปถึงเลเวลถัดไป (Index 0 = EXP to reach Lv. 2)")]
    public List<long> experienceRequirements;

    /// <summary>
    /// ดึงค่า EXP ที่ต้องใช้เพื่อไปถึงเลเวลถัดไป
    /// </summary>
    /// <param name="currentLevel">เลเวลปัจจุบันของตัวละคร (เริ่มที่ 1)</param>
    /// <returns>ค่า EXP ที่ต้องการ หรือ long.MaxValue ถ้าเลเวลเกินตาราง</returns>
    public long GetExperienceForLevel(int currentLevel)
    {
        // เลเวล 1 ต้องการ EXP ที่ index 0 เพื่อไปเลเวล 2
        int index = currentLevel - 1;

        if (index >= 0 && index < experienceRequirements.Count)
        {
            return experienceRequirements[index];
        }

        // ถ้าเลเวลเกินตารางที่กำหนดไว้
        Debug.LogWarning($"Level {currentLevel} is out of range for the Experience Table. Returning max value.");
        
        // ทางเลือก:
        // 1. คืนค่าสุดท้ายในตาราง (ทำให้เลเวลตัน)
        if (experienceRequirements.Count > 0)
        {
            return experienceRequirements[experienceRequirements.Count - 1];
        }
        
        // 2. คืนค่าสูงสุดที่เป็นไปได้ (ทำให้ไม่มีทางเลเวลอัพได้อีก)
        return long.MaxValue;
    }
}
