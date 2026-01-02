using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ทำการ Login เป็น Guest (ถ้ายังไม่ได้ Login) แล้วเปลี่ยน Scene
/// </summary>
public class ChangeScene : MonoBehaviour
{
    [Tooltip("ชื่อของ Scene ที่ต้องการจะโหลด (ต้องตรงกับชื่อไฟล์ Scene)")]
    public string sceneNameToLoad;

    [Tooltip("ติ๊กถ้าต้องการให้ทำการ Login เป็น Guest ก่อนเปลี่ยน Scene")]
    public bool loginAsGuest = true;

    public void LoadTargetScene()
    {
        if (string.IsNullOrEmpty(sceneNameToLoad))
        {
            Debug.LogError("Scene Name to Load is not set in the Inspector!");
            return;
        }

        // ตรวจสอบว่าต้องการให้ Login หรือไม่
        if (loginAsGuest)
        {
            // ตรวจสอบว่ายังไม่ได้ Login อยู่ใช่ไหม
            if (PlayerDataManager.Instance != null && !PlayerDataManager.Instance.IsLoggedIn())
            {
                Debug.Log("[ChangeScene] Player is not logged in. Logging in as Guest...");
                PlayerDataManager.Instance.LoginAsGuest();
            }
        }

        // เปลี่ยน Scene
        Debug.Log($"[ChangeScene] Loading scene: {sceneNameToLoad}");
        SceneManager.LoadScene(sceneNameToLoad);
    }
}