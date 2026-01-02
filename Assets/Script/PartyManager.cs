using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public Transform[] playerPositions;
    public List<PlayerUnitData> partyMembers = new List<PlayerUnitData>();

    void Start()
    {
        SpawnParty();
    }

    public void SpawnParty()
    {
        for (int i = 0; i < partyMembers.Count && i < playerPositions.Length; i++)
        {
            PlayerUnitData unitData = partyMembers[i];
            unitData.InitializeStats();

            GameObject unitPrefab = unitData.classData.classPrefab;
            if (unitPrefab == null)
            {
                Debug.LogError($"Missing prefab for class: {unitData.classData.className}");
                continue;
            }

            GameObject unitGO = Instantiate(unitPrefab, playerPositions[i].position, Quaternion.identity);
            unitGO.transform.SetParent(playerPositions[i], worldPositionStays: true);

            PlayerBehavior behavior = unitGO.GetComponent<PlayerBehavior>();
            behavior.Initialize(unitData);

            SkillController skillCtrl = unitGO.GetComponent<SkillController>();

            if (skillCtrl != null)
            {
                // Assign class-based skills from PlayerClassData
                SkillData[] defaultSkills = unitData.classData.defaultSkills;
                for (int s = 0; s < defaultSkills.Length && s < skillCtrl.equippedSkills.Length; s++)
                {
                    skillCtrl.equippedSkills[s] = defaultSkills[s];
                }

                // AI characters auto-use their skills
                skillCtrl.autoUseSkills = (i != 0);
            }

            if (i == 0)
            {
                UIManager.Instance.AssignSkillButtons(unitGO.GetComponent<SkillController>());
            }
        }
    }
}
