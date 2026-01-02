using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class MenuPage
{
    [Tooltip("ชื่อสำหรับอ้างอิงจากโค้ด")]
    public string pageName;
    [Tooltip("ปุ่มสำหรับกดเพื่อเปิด Page นี้")]
    public Button menuButton;
    [Tooltip("Panel หรือเนื้อหาที่จะแสดงผลเมื่อ Page นี้ถูกเลือก")]
    public GameObject menuPanel;
}

public class MainMenuController : MonoBehaviour
{
    [Header("Pages")]
    public List<MenuPage> menuPages;

    [Header("Initial State")]
    [Tooltip("ชื่อของ Page ที่จะถูกเปิดไว้เป็นอันแรก")]
    public string startingPageName = "Home";

    private void Start()
    {
        // เพิ่ม Listener ให้กับปุ่มแต่ละอัน
        foreach (var page in menuPages)
        {
            if (page.menuButton != null)
            {
                page.menuButton.onClick.AddListener(() => OpenPage(page));
            }
        }

        // เปิดหน้าเริ่มต้น
        var startingPage = menuPages.FirstOrDefault(p => p.pageName == startingPageName);
        if (startingPage != null)
        {
            OpenPage(startingPage);
        }
        else if (menuPages.Count > 0)
        {
            OpenPage(menuPages[0]); // ถ้าหาไม่เจอ ให้เปิดอันแรกแทน
        }
    }

    /// <summary>
    /// เปิด Page ที่ระบุ และปิด Page อื่นๆ ทั้งหมด
    /// </summary>
    public void OpenPage(MenuPage targetPage)
    {
        if (targetPage == null) return;

        foreach (var page in menuPages)
        {
            bool isSelected = (page == targetPage);
            
            if (page.menuPanel != null)
            {
                page.menuPanel.SetActive(isSelected);
            }

            // Optional: เพิ่ม Logic การเปลี่ยนสี/ไฮไลท์ปุ่มที่นี่
            // if (page.menuButton != null)
            // {
            //     var buttonGraphic = page.menuButton.GetComponent<Graphic>();
            //     if (buttonGraphic != null)
            //     {
            //         buttonGraphic.color = isSelected ? Color.white : Color.gray;
            //     }
            // }
        }

        Debug.Log($"Opened Page: {targetPage.pageName}");
    }

    /// <summary>
    /// เปิด Page โดยใช้ชื่อ (สำหรับเรียกจากสคริปต์อื่น)
    /// </summary>
    public void OpenPageByName(string pageName)
    {
        var targetPage = menuPages.FirstOrDefault(p => p.pageName == pageName);
        if (targetPage != null)
        {
            OpenPage(targetPage);
        }
        else
        {
            Debug.LogWarning($"Page with name '{pageName}' not found!");
        }
    }
}
