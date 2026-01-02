using UnityEngine;

public enum weaponType
{
    Sword,
    Bow,
    Staff
}

[CreateAssetMenu(fileName = "WeaponDataSO", menuName = "Scriptable Objects/WeaponDataSO")]
public class WeaponDataSO : ScriptableObject
{
    public int weaponID;
    public string weaponName;
    public weaponType type;
    public int power;
    public GameObject weaponPrefab;
    public GameObject HitEffect;
    
    public Sprite icon;
    public string description;
}
