using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;


public class CharacterCreationManager : MonoBehaviour
{
    public TMP_InputField nameInput;
    public GameObject namePanel;
    public GameObject classPanel;
    public GameObject confirmPopup;
    public TMP_Text confirmText;
    //public ClassCard classCardPrefab;
    //public Transform classCardContainer;
    //public Sprite[] classIcons;

    
    private string playerName;
    private PlayerClass selectedClass;

    private void Start()
    {
        namePanel.SetActive(true);
        classPanel.SetActive(false);
        confirmPopup.SetActive(false);
        //GenerateClassCards();
    }

    public void OnNameNext()
    {
        playerName = nameInput.text;
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("กรุณาใส่ชื่อก่อน");
            return;
        }

        namePanel.SetActive(false);
        classPanel.SetActive(true);
    }
    
    public void OnConfirmClassChoice()
    {
        // สร้าง playerData และเซฟ
        PlayeraData data = new PlayeraData
        {
            playerName = playerName,
            playerClass = selectedClass,
            level = 1,
            coin = 0
        };

        //SaveSystem.SavePlayerData(data);
        SceneManager.LoadScene("MainGame");
    }

    public void OnCancelClassChoice()
    {
        confirmPopup.SetActive(false);
    }
    
    public void OnClassCardClicked(PlayerClass selected)
    {
        selectedClass = selected;
        confirmText.text = $"Are you sure {selectedClass}?";
        confirmPopup.SetActive(true);
    }
    
    /*
    private void GenerateClassCards()
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(PlayerClass)).Length; i++)
        {
            PlayerClass playerClass = (PlayerClass)i;

            ClassCard newCard = Instantiate(classCardPrefab, classCardContainer);
            newCard.Setup(playerClass, classIcons[i], this);
        }
    }
    */

}