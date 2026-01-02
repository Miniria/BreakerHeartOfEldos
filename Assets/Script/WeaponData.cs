using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Breaker/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public int attack;
    public float baseCooldown;
    public float minCooldown;
}