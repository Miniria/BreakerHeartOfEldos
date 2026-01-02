using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JobDataSO", menuName = "Scriptable Objects/JobDataSO")]
public class JobDataSO : ScriptableObject
{
    [Header("Job Info")]
    public string jobName;
    public Sprite icon;
    public List<JobDataSO> nextClasses;
    
    [Header("Skills")]
    public List<SkillDataSO> jobSkills;
}
