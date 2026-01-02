using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public SkillUIButton[] skillButtons; // Drag your 3 skill buttons here in inspector

    void Awake() => Instance = this;

    public void AssignSkillButtons(SkillController skillCtrl)
    {
        for (int i = 0; i < skillButtons.Length; i++)
        {
            skillButtons[i].skillController = skillCtrl;
            skillButtons[i].skillSlotIndex = i;
        }
    }
}
