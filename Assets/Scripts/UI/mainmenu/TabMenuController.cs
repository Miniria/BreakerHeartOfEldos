using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class Tab
{
    [Tooltip("ปุ่มสำหรับกดเพื่อเปิด Tab นี้")]
    public Button tabButton;
    [Tooltip("Panel หรือเนื้อหาที่จะแสดงผลเมื่อ Tab นี้ถูกเลือก")]
    public GameObject tabPanel;
    [Tooltip("GameObject ที่เป็นกรอบ/ไฮไลท์ของปุ่มนี้ (จะถูกเปิดเมื่อเลือก)")]
    public GameObject selectionFrame;
}

public class TabMenuController : MonoBehaviour
{
    [Header("Tabs")]
    [Tooltip("ลาก-วาง ปุ่ม, Panel, และกรอบที่คู่กันมาใส่ที่นี่")]
    public List<Tab> tabs;

    [Header("Tab Colors")]
    [Tooltip("สีของ Graphic (เช่น Image) ของปุ่มเมื่อถูกเลือก")]
    public Color selectedColor = Color.white;
    [Tooltip("สีของ Graphic ของปุ่มเมื่อไม่ได้ถูกเลือก")]
    public Color deselectedColor = new Color(0.8f, 0.8f, 0.8f, 0.5f); // สีเทาจางๆ

    [Header("Initial State")]
    [Tooltip("Tab ที่จะถูกเปิดไว้เป็นอันแรก (เริ่มนับที่ 0)")]
    [SerializeField] private int startingTabIndex = 0;

    private void Start()
    {
        // เพิ่ม Listener ให้กับปุ่มแต่ละอัน
        for (int i = 0; i < tabs.Count; i++)
        {
            int index = i; // สำคัญมาก: ต้องสร้างตัวแปร local copy สำหรับ Listener
            tabs[i].tabButton.onClick.AddListener(() => OnTabSelected(index));
        }
    }

    private void OnEnable()
    {
        // ใช้ OnEnable เพื่อให้แน่ใจว่า Tab เริ่มต้นจะถูกตั้งค่าทุกครั้งที่เปิด GameObject นี้
        InitializeTabs();
    }

    private void InitializeTabs()
    {
        if (tabs.Count > startingTabIndex)
        {
            OnTabSelected(startingTabIndex);
        }
        else if (tabs.Count > 0)
        {
            OnTabSelected(0); // ถ้าตั้งค่าผิด ให้เปิดอันแรกแทน
        }
    }

    public void OnTabSelected(int tabIndex)
    {
        if (tabIndex < 0 || tabIndex >= tabs.Count) return;

        for (int i = 0; i < tabs.Count; i++)
        {
            bool isSelected = (i == tabIndex);

            // 1. เปิด/ปิด Panel เนื้อหา
            if (tabs[i].tabPanel != null)
                tabs[i].tabPanel.SetActive(isSelected);

            // 2. เปิด/ปิด กรอบที่คลุมปุ่ม
            if (tabs[i].selectionFrame != null)
                tabs[i].selectionFrame.SetActive(isSelected);

            // 3. เปลี่ยนสีปุ่ม (ทำให้ปุ่มที่ไม่ได้เลือกมืดลง)
            var buttonGraphic = tabs[i].tabButton.GetComponent<Graphic>();
            if (buttonGraphic != null)
            {
                buttonGraphic.color = isSelected ? selectedColor : deselectedColor;
            }
        }
        
        // คุณสามารถเพิ่ม Logic อื่นๆ ตรงนี้ได้ เช่น เล่นเสียง
        // FindObjectOfType<AudioManager>().Play("TabClick");
    }
}
