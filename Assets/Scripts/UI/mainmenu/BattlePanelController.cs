using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BattlePanelController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject chapterListPanel; // Panel ที่มี Scroll View ของบทต่างๆ
    public StageSelectionUI stageSelectionUI; // Panel/Controller ของหน้าเลือกด่าน

    [Header("Chapter List Setup")]
    public GameObject chapterButtonPrefab;
    public Transform chapterButtonContainer; // คือ Content ของ Scroll View

    private void Start()
    {
        SetupChapterList();
        // เริ่มต้นโดยการแสดงหน้าเลือกบท และซ่อนหน้าเลือกด่าน
        chapterListPanel.SetActive(true);
        stageSelectionUI.gameObject.SetActive(false);
    }

    void SetupChapterList()
    {
        // ทำความสะอาดของเก่า
        foreach (Transform child in chapterButtonContainer)
        {
            Destroy(child.gameObject);
        }

        int maxChapter = 0;
        if (GameDatabase.Instance != null && GameDatabase.Instance.allStages.Count > 0)
        {
            maxChapter = GameDatabase.Instance.allStages.Max(s => s.chapterIndex);
        }

        for (int i = 1; i <= maxChapter; i++)
        {
            GameObject buttonGO = Instantiate(chapterButtonPrefab, chapterButtonContainer);
            ChapterButton chapterBtn = buttonGO.GetComponent<ChapterButton>();
            if (chapterBtn != null)
            {
                chapterBtn.Setup(i, this);
            }
        }
    }

    /// <summary>
    /// เมธอดนี้จะถูกเรียกโดย ChapterButton เมื่อผู้เล่นเลือกบท
    /// </summary>
    public void OnChapterSelected(int chapterIndex)
    {
        // สลับหน้าจอก่อน
        chapterListPanel.SetActive(false);
        stageSelectionUI.gameObject.SetActive(true);
        
        // เริ่ม Coroutine จากที่นี่ (เพราะ BattlePanelController Active อยู่เสมอ)
        StartCoroutine(stageSelectionUI.ShowStagesForChapter(chapterIndex));
    }

    /// <summary>
    /// เมธอดนี้จะถูกเรียกโดยปุ่ม Back ใน StageSelectionUI
    /// </summary>
    public void BackToChapterList()
    {
        chapterListPanel.SetActive(true);
        stageSelectionUI.gameObject.SetActive(false);
    }
}
