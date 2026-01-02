using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class EquippedItemsPanelUI : MonoBehaviour
{
    [Header("Slot Containers")]
    public Transform weaponSlotContainer;
    public Transform armorSlotContainer;
    public Transform bootsSlotContainer;
    public Transform accessorySlotContainer;

    [Header("UI Prefab")]
    public GameObject equippedSlotPrefab;

    [Header("Dependencies")]
    public SelectedEquippedItemUI selectedEquippedItemPanel;

    private void Awake()
    {
        EquipmentManager.OnEquipmentChanged += RefreshEquippedItems;
    }

    private void OnDestroy()
    {
        EquipmentManager.OnEquipmentChanged -= RefreshEquippedItems;
    }

    private void OnEnable()
    {
        RefreshEquippedItems();
        if (selectedEquippedItemPanel != null)
        {
            selectedEquippedItemPanel.gameObject.SetActive(false);
        }
    }

    public void RefreshEquippedItems()
    {
        UpdateSlot(weaponSlotContainer, EquipmentManager.Instance.GetEquippedItem(EquipmentSlot.Weapon));
        UpdateSlot(armorSlotContainer, EquipmentManager.Instance.GetEquippedItem(EquipmentSlot.Armor));
        UpdateSlot(bootsSlotContainer, EquipmentManager.Instance.GetEquippedItem(EquipmentSlot.Boots));
        UpdateSlot(accessorySlotContainer, EquipmentManager.Instance.GetEquippedItem(EquipmentSlot.Accessory));
    }

    private void UpdateSlot(Transform container, InventoryItemData equippedItem)
    {
        if (container == null) return;

        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        if (equippedItem == null) return;

        if (equippedSlotPrefab != null)
        {
            GameObject slotGO = Instantiate(equippedSlotPrefab, container);
            EquippedSlotUI slotUI = slotGO.GetComponent<EquippedSlotUI>(); 
            if (slotUI != null)
            {
                slotUI.Setup(equippedItem, this); 
            }
            else
            {
                Debug.LogError($"Prefab '{equippedSlotPrefab.name}' is missing EquippedSlotUI.cs script!");
            }
        }
    }

    public void OnEquippedItemClicked(InventoryItemData clickedItem)
    {
        if (selectedEquippedItemPanel != null)
        {
            selectedEquippedItemPanel.Show(clickedItem);
        }
    }
}
