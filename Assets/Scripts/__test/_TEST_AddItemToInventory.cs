using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class _TEST_AddItemToInventory : MonoBehaviour
{
    [Header("Item Settings")]
    public string itemID_ToAdd = "item_1";
    public ItemRarity rarity_ToAdd = ItemRarity.Common;
    public int quality_ToAdd = 1;
    public int amount_ToAdd = 1;

    public void AddItem()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Please enter Play Mode to add items.");
            return;
        }

        if (EquipmentManager.Instance == null)
        {
            Debug.LogError("[TEST] FATAL: EquipmentManager.Instance is NULL. Check Script Execution Order.");
            return;
        }

        Debug.Log($"--- [TEST] Attempting to add item: {itemID_ToAdd} ---");

        for (int i = 0; i < amount_ToAdd; i++)
        {
            InventoryItemData newItemData = ItemFactory.CreateItem(itemID_ToAdd, rarity_ToAdd, quality_ToAdd);
            if (newItemData == null)
            {
                Debug.LogError($"[TEST] FATAL: ItemFactory.CreateItem returned NULL for ID: {itemID_ToAdd}.");
                continue; // ข้ามไปเผื่อแอดหลายชิ้น
            }
            EquipmentManager.Instance.AddItem(newItemData);
        }

        Debug.Log($"[TEST] Successfully added {amount_ToAdd} instance(s) of {itemID_ToAdd} to data.");

        // --- ปรับปรุง Logic การ Refresh UI ---
        // เราจะหา InventoryUI ทั้งหมดในโปรเจกต์ (เผื่อมีหลายหน้า)
        // FindObjectsOfType(true) จะหาเจอแม้ว่า GameObject จะปิดอยู่
        InventoryUI[] allInventoryUIs = FindObjectsOfType<InventoryUI>(true); 
        
        if (allInventoryUIs.Length > 0)
        {
            foreach (var inventoryUI in allInventoryUIs)
            {
                // ถ้าหน้าต่างนั้นเปิดอยู่, ให้สั่ง Refresh ทันที
                if (inventoryUI.gameObject.activeInHierarchy)
                {
                    inventoryUI.RefreshInventory();
                    Debug.Log($"[TEST] Refreshed active Inventory UI: {inventoryUI.name}");
                }
            }
        }
        else
        {
            Debug.LogWarning("[TEST] No InventoryUI found in the scene to refresh.");
        }
        // ------------------------------------
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(_TEST_AddItemToInventory))]
public class _TEST_AddItemToInventory_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        _TEST_AddItemToInventory script = (_TEST_AddItemToInventory)target;
        if (GUILayout.Button("Add Item to Inventory"))
        {
            script.AddItem();
        }
    }
}
#endif
