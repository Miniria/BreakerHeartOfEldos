using UnityEngine;

public enum SkillsType { Damage, Heal, Buff }

[CreateAssetMenu(menuName = "Breaker/Skill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public Sprite icon;
    public SkillsType type;
    public float cooldown;
    public float powerMultiplier = 1f;
}
