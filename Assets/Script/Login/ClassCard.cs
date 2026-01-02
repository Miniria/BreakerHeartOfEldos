using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClassCard : MonoBehaviour
{
    public TMP_Text classNameText;
    public Image classIcon;
    public PlayerClass playerClass;

    [Header("Reference to CharacterCreationManager")]
    public CharacterCreationManager manager;

    public void Setup(PlayerClass _class, Sprite icon, CharacterCreationManager _manager)
    {
        playerClass = _class;
        classNameText.text = _class.ToString();
        classIcon.sprite = icon;
        manager = _manager;
    }

    public void OnCardSelected()
    {
        if (manager == null)
        {
            Debug.LogError("ClassCard: manager is not assigned! Did you forget to call Setup() or drag it in Inspector?");
            return;
        }

        manager.OnClassCardClicked(playerClass);
    }
}