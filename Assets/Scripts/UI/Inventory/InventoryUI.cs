using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class InventoryUI : MonoBehaviour
{
    [Header("UI Setup")]
    public GameObject itemSlotPrefab;
    public RectTransform gridContainer;
    public ScrollRect scrollRect;

    [Header("Layout Settings")]
    public GridLayoutGroup gridLayout;

    [Header("Dependencies")]
    public SelectedItemUI selectedItemPanel; 

    private List<ItemSlotUI> itemSlotPool = new List<ItemSlotUI>();
    private RectTransform viewPortRect;

    private void Awake()
    {
        viewPortRect = scrollRect.viewport;
        if (viewPortRect == null)
        {
            viewPortRect = scrollRect.GetComponent<RectTransform>();
        }

        // --- เพิ่มบรรทัดนี้ ---
        EquipmentManager.OnEquipmentChanged += RefreshInventory;
    }

    private void OnEnable()
    {
        RefreshInventory();
        if (selectedItemPanel != null)
        {
            selectedItemPanel.gameObject.SetActive(false);
        }
    }

    public void RefreshInventory()
    {
        if (PlayerDataManager.Instance == null || PlayerDataManager.Instance.gameData == null)
        {
            Debug.LogError("InventoryUI: PlayerData is not ready!");
            return;
        }
        List<InventoryItemData> playerInventory = PlayerDataManager.Instance.gameData.playerData.inventory;

        while (itemSlotPool.Count < playerInventory.Count)
        {
            GameObject slotGO = Instantiate(itemSlotPrefab, gridContainer);
            itemSlotPool.Add(slotGO.GetComponent<ItemSlotUI>());
        }

        for (int i = 0; i < itemSlotPool.Count; i++)
        {
            if (i < playerInventory.Count)
            {
                // --- แก้ไขการเรียกใช้ Setup ---
                itemSlotPool[i].Setup(playerInventory[i], OnItemSlotClicked);
                itemSlotPool[i].gameObject.SetActive(true);
            }
            else
            {
                itemSlotPool[i].gameObject.SetActive(false);
            }
        }
        
        CalculateContentHeight(playerInventory.Count);
    }

    private void CalculateContentHeight(int itemCount)
    {
        if (itemCount <= 0 || gridLayout == null) 
        {
            gridContainer.sizeDelta = new Vector2(gridContainer.sizeDelta.x, viewPortRect.rect.height);
            return;
        }

        int columnCount = gridLayout.constraintCount;
        if (columnCount <= 0) columnCount = 1;

        int rowCount = Mathf.CeilToInt((float)itemCount / columnCount);

        float totalItemHeight = rowCount * gridLayout.cellSize.y;
        float totalSpacingHeight = Mathf.Max(0, rowCount - 1) * gridLayout.spacing.y;
        float contentHeight = gridLayout.padding.top + totalItemHeight + totalSpacingHeight + gridLayout.padding.bottom;

        float viewportHeight = viewPortRect.rect.height;

        if (contentHeight < viewportHeight)
        {
            gridContainer.sizeDelta = new Vector2(gridContainer.sizeDelta.x, viewportHeight);
        }
        else
        {
            gridContainer.sizeDelta = new Vector2(gridContainer.sizeDelta.x, contentHeight);
        }
    }

    public void OnItemSlotClicked(InventoryItemData selectedItem)
    {
        if (selectedItemPanel != null)
        {
            selectedItemPanel.Show(selectedItem);
        }
    }
    
    private void OnDestroy()
    {
        // --- เพิ่มบรรทัดนี้ ---
        EquipmentManager.OnEquipmentChanged -= RefreshInventory;
    }
}
