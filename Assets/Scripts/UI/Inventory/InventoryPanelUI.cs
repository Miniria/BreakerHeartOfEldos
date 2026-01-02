using UnityEngine;
using System.Collections.Generic;

public class InventoryPanelUI : MonoBehaviour
{
    [Header("Inventory Display")]
    [Tooltip("ลาก Prefab ของช่องไอเทม (ที่มีสคริปต์ InventorySlotUI) มาใส่")]
    public GameObject slotPrefab;
    [Tooltip("Container (ที่มี Layout Group) ที่จะให้สร้าง Slot เข้าไปข้างใน")]
    public Transform slotContainer;

    [Header("Dependencies")]
    [Tooltip("ลาก Panel ที่ใช้แสดงรายละเอียดไอเทมที่เลือกมาใส่")]
    public SelectedItemUI selectedItemPanel; 

    private void OnEnable()
    {
        // อัปเดต UI ทุกครั้งที่เปิดหน้าต่างนี้ขึ้นมา
        RefreshInventory();

        // ซ่อนหน้าต่างรายละเอียดไว้ก่อน
        if (selectedItemPanel != null)
        {
            selectedItemPanel.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// อ่านข้อมูล Inventory ล่าสุดแล้วสร้าง Slot UI ขึ้นมาใหม่ทั้งหมด
    /// </summary>
    public void RefreshInventory()
    {
        if (slotContainer == null || slotPrefab == null)
        {
            Debug.LogError("InventoryPanelUI is not setup correctly in the Inspector!");
            return;
        }

        // 1. ล้าง Slot เก่าทั้งหมด
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. อ่านข้อมูล Inventory ล่าสุด
        if (PlayerDataManager.Instance == null || PlayerDataManager.Instance.gameData == null) return;
        List<InventoryItemData> inventory = PlayerDataManager.Instance.gameData.playerData.inventory;

        // 3. สร้าง Slot ใหม่สำหรับไอเทมทุกชิ้น
        foreach (InventoryItemData item in inventory)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotContainer);
            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();

            if (slotUI != null)
            {
                // ส่งข้อมูลไอเทม และ Action ที่จะให้ทำเมื่อถูกกด (ในที่นี้คือเปิดหน้าต่างรายละเอียด)
                slotUI.Setup(item, OnInventoryItemClicked);
            }
        }
    }

    /// <summary>
    /// จะถูกเรียกเมื่อ Slot ไอเทมใน Inventory ถูกกด
    /// </summary>
    private void OnInventoryItemClicked(InventoryItemData clickedItem)
    {
        Debug.Log($"Clicked on item: {clickedItem.itemID}");
        
        // เปิดหน้าต่างแสดงรายละเอียดไอเทมที่เลือก
        if (selectedItemPanel != null)
        {
            selectedItemPanel.Show(clickedItem);
        }
    }
}
