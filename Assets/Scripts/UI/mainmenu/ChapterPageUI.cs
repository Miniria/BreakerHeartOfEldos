using UnityEngine;
using TMPro;

public class ChapterPageUI : MonoBehaviour
{
    // อาจจะมี UI Element อื่นๆ ในอนาคต เช่น รูปภาพประกอบบท
    // public Image chapterImage;
    public TextMeshProUGUI chapterNumberText;

    public int ChapterIndex { get; private set; }

    public void Setup(int chapterIndex)
    {
        this.ChapterIndex = chapterIndex;
        if (chapterNumberText != null)
        {
            chapterNumberText.text = $"Chapter {chapterIndex}";
        }
    }
}
