using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [Tooltip("ความเร็วในการเข้าล็อค")]
    public float snapSpeed = 10f;

    [Tooltip("Event ที่จะถูกเรียกเมื่อมีการเปลี่ยนหน้า (ส่งค่า index ของหน้าปัจจุบันไป)")]
    public UnityEvent<int> OnPageChanged;

    private ScrollRect scrollRect;
    private RectTransform contentPanel;
    private RectTransform[] pageRects;

    private int currentPageIndex = 0;
    private bool isSnapping = false;
    private Vector2 targetPosition;

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        contentPanel = scrollRect.content;
        
        // รอเฟรมนึงเพื่อให้ Layout Group จัดเรียงเสร็จ
        Invoke(nameof(SetupPages), 0.1f);
    }

    void SetupPages()
    {
        int pageCount = contentPanel.childCount;
        if (pageCount == 0) return;

        pageRects = new RectTransform[pageCount];
        for (int i = 0; i < pageCount; i++)
        {
            pageRects[i] = contentPanel.GetChild(i) as RectTransform;
        }
    }

    private void Update()
    {
        if (isSnapping)
        {
            contentPanel.anchoredPosition = Vector2.Lerp(contentPanel.anchoredPosition, targetPosition, snapSpeed * Time.deltaTime);

            if (Vector2.Distance(contentPanel.anchoredPosition, targetPosition) < 1f)
            {
                contentPanel.anchoredPosition = targetPosition;
                isSnapping = false;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isSnapping = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (pageRects == null || pageRects.Length == 0) return;

        float minDistance = float.MaxValue;
        int closestPageIndex = currentPageIndex;

        for (int i = 0; i < pageRects.Length; i++)
        {
            float distance = Vector2.Distance(contentPanel.anchoredPosition, -pageRects[i].anchoredPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPageIndex = i;
            }
        }

        if (currentPageIndex != closestPageIndex)
        {
            currentPageIndex = closestPageIndex;
            OnPageChanged?.Invoke(currentPageIndex);
        }

        targetPosition = -pageRects[currentPageIndex].anchoredPosition;
        isSnapping = true;
    }
}
