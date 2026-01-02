using UnityEngine;
using System.Collections.Generic;

public class EncounterManager : MonoBehaviour
{
    [Header("Stage Configuration")]
    public List<Stagea> stages = new List<Stagea>();
    private int currentStageIndex = 0;

    [Header("Battlefield")]
    public Transform[] battlePositions = new Transform[4];

    private List<GameObject> currentEnemies = new List<GameObject>();

    void Start()
    {
        StartEncounter();
    }

    void StartEncounter()
    {
        if (currentStageIndex >= stages.Count)
        {
            Debug.Log("All stages complete!");
            return;
        }

        ClearBattlefield();

        Stagea currentStage = stages[currentStageIndex];
        Debug.Log($"Starting Stage {currentStage.stageNumber}");

        for (int i = 0; i < currentStage.enemies.Count && i < battlePositions.Length; i++)
        {
            EnemyData data = currentStage.enemies[i];
            GameObject enemyGO = Instantiate(data.enemyPrefab, battlePositions[i].position, Quaternion.identity);
            EnemyBehavior enemy = enemyGO.GetComponent<EnemyBehavior>();
            enemyGO.transform.SetParent(battlePositions[i], worldPositionStays: true);

            if (enemy != null)
            {
                enemy.Initialize(data, this);

                //Auto-select the first enemy as target
                if (EnemyBehavior.GetCurrentTarget() == null)
                {
                    EnemyBehavior.SetCurrentTarget(enemy);
                }
            }

            currentEnemies.Add(enemyGO);
        }
    }

    public void OnEnemyDefeated(GameObject enemy)
    {
        currentEnemies.Remove(enemy);
        Destroy(enemy);

        // Auto-retarget if the dead enemy was selected
        if (enemy.TryGetComponent<EnemyBehavior>(out var defeatedEnemy))
        {
            if (EnemyBehavior.GetCurrentTarget() == defeatedEnemy)
            {
                EnemyBehavior.ClearCurrentTarget();

                foreach (var obj in currentEnemies)
                {
                    if (obj == null) continue;
                    var candidate = obj.GetComponent<EnemyBehavior>();
                    if (candidate != null)
                    {
                        EnemyBehavior.SetCurrentTarget(candidate);
                        break;
                    }
                }
            }
        }


        if (currentEnemies.Count == 0)
        {
            Debug.Log($"Stage {currentStageIndex + 1} cleared!");
            currentStageIndex++;
            StartEncounter();
        }
    }

    void ClearBattlefield()
    {
        foreach (var enemy in currentEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        currentEnemies.Clear();
    }
}
