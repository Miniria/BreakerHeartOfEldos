using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Breaker/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Basic Info")]
    public string enemyName;
    public GameObject enemyPrefab;

    [Header("Stats")]
    public int maxHP;
    // public int mana;
    public int shield;

    public float attackSpeed;
    public int attack;
    public int defend;
}
