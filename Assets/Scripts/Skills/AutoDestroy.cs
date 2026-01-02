using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [Tooltip("เวลา (วินาที) ก่อนที่ GameObject นี้จะถูกทำลาย")]
    public float lifetime = 2.0f; // ตั้งค่าเริ่มต้นไว้ที่ 2 วินาที

    void Start()
    {
        // สั่งให้ทำลาย GameObject นี้ (gameObject) หลังจากผ่านไป 'lifetime' วินาที
        Destroy(gameObject, lifetime);
    }
}
