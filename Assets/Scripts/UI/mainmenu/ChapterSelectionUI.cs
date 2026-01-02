using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class ChapterSelectionUI : MonoBehaviour
{
    [Header("Scroll View Components")]
    public ScrollRectSnap scrollRectSnap; // ลาก Scroll View ที่มีสคริปต์ Snap มาใส่
    public Transform chapterContainer;      // คือ Content ของ Scroll View

    [Header("UI Elements")]
    public TextMeshProUGUI chapterNameText;
    public Button enterChapterButton;

    [Header("Prefabs & Dependencies")]
    public GameObject chapterPagePrefab; // Prefab ของ Panel ที่จะแสดงในแต่ละหน้าของ Scroll View
    public StageSelectionUI stageSelectionUI;

    private List<ChapterPageUI> chapterPages = new List<ChapterPageUI>();
    private int currentChapterIndex = 1;

    private void Start()
    {
        // 1. สร้างหน้าสำหรับแต่ละบท
        SetupChapterPages();

        // 2. ตั้งค่า Listener ให้กับปุ่ม
        enterChapterButton.onClick.AddListener(OnEnterChapterClicked);

        // 3. เพิ่ม Listener เพื่อตรวจจับการเปลี่ยนแปลงหน้า
        if (scrollRectSnap != null)
        {
            scrollRectSnap.OnPageChanged.AddListener(OnPageChanged);
        }

        // 4. อัปเดตข้อมูลสำหรับหน้าแรก
        OnPageChanged(0); 
    }

    void SetupChapterPages()
    {
        // ทำความสะอาดของเก่า
        foreach (Transform child in chapterContainer)
        {
            Destroy(child.gameObject);
        }
        chapterPages.Clear();

        int maxChapter = 0;
        if (GameDatabase.Instance != null && GameDatabase.Instance.allStages.Count > 0)
        {
            maxChapter = GameDatabase.Instance.allStages.Max(s => s.chapterIndex);
        }

        for (int i = 1; i <= maxChapter; i++)
        {
            GameObject pageGO = Instantiate(chapterPagePrefab, chapterContainer);
            ChapterPageUI pageUI = pageGO.GetComponent<ChapterPageUI>();
            if (pageUI != null)
            {
                pageUI.Setup(i); // ส่งเลขบทเข้าไปให้หน้านั้นๆ
                chapterPages.Add(pageUI);
            }
        }
    }

    // เมธอดนี้จะถูกเรียกโดย ScrollRectSnap เมื่อมีการเปลี่ยนหน้า
    void OnPageChanged(int pageIndex)
    {
        currentChapterIndex = pageIndex + 1;
        chapterNameText.text = $"Chapter {currentChapterIndex}";
    }

    void OnEnterChapterClicked()
    {
        // บอกให้หน้าเลือกด่านแสดงผลด่านของบทปัจจุบัน
        stageSelectionUI.ShowStagesForChapter(currentChapterIndex);
        
        // ซ่อนตัวเอง และแสดงหน้าเลือกด่าน
        gameObject.SetActive(false);
        stageSelectionUI.gameObject.SetActive(true);
    }
}
