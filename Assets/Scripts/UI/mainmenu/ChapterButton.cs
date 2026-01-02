using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChapterButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI chapterNameText;

    private int chapterIndex;
    private BattlePanelController ownerController;

    public void Setup(int index, BattlePanelController owner)
    {
        this.chapterIndex = index;
        this.ownerController = owner;
        chapterNameText.text = $"Chapter {index}";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        ownerController.OnChapterSelected(chapterIndex);
    }
}
