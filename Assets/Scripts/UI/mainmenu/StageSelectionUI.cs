using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class StageSelectionUI : MonoBehaviour
{
    [Header("Chapter Layouts")]
    [Tooltip("ลาก GameObject ที่เป็น Parent ของด่านในแต่ละบทมาใส่ (เรียงตามลำดับบท)")]
    public List<GameObject> chapterLayouts;

    [Header("UI Elements")]
    public Button backButton;
    public StageDetailsUI stageDetailsUI;

    [Header("Dependencies")]
    public BattlePanelController battlePanelController;

    private void Awake()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(BackToChapterSelection);
        }

        // เริ่มต้นโดยการซ่อน Panel รายละเอียดไว้ที่นี่
        if (stageDetailsUI != null)
        {
            stageDetailsUI.gameObject.SetActive(false);
        }
    }

    public IEnumerator ShowStagesForChapter(int chapterIndex)
    {
        // 1. เปิด/ปิด Layout ของบทที่ถูกต้อง
        for (int i = 0; i < chapterLayouts.Count; i++)
        {
            bool isActive = (i == chapterIndex - 1);
            if (chapterLayouts[i] != null)
            {
                chapterLayouts[i].SetActive(isActive);
            }
        }

        // รอ 1 เฟรมเพื่อให้ Layout ที่เพิ่งเปิดพร้อมใช้งาน
        yield return null;

        GameObject currentLayout = chapterLayouts.FirstOrDefault(layout => layout.activeSelf);
        if (currentLayout == null) yield break;

        List<Stage> stagesInChapter = GameDatabase.Instance.allStages
            .Where(s => s.chapterIndex == chapterIndex)
            .ToList();

        StageNodeUI[] stageNodes = currentLayout.GetComponentsInChildren<StageNodeUI>(true);
        foreach (var node in stageNodes)
        {
            Stage matchingStage = stagesInChapter.FirstOrDefault(s => s.stageIndexInChapter == node.stageIndexInChapter);
            if (matchingStage != null)
            {
                node.Show();
                node.Setup(matchingStage, this);
            }
            else
            {
                node.Hide();
            }
        }
    }

    public void OnStageNodeSelected(Stage selectedStage)
    {
        Debug.Log($"Selected Stage: {selectedStage.stageName}");
        if (stageDetailsUI != null)
        {
            // สั่งเปิด Panel ก่อน แล้วค่อยส่งข้อมูล
            stageDetailsUI.gameObject.SetActive(true);
            stageDetailsUI.Show(selectedStage);
        }
    }

    public void BackToChapterSelection()
    {
        if (battlePanelController != null)
        {
            battlePanelController.BackToChapterList();
        }
    }
}
