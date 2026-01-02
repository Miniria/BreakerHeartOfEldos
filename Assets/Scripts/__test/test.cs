using UnityEngine;

public class test : MonoBehaviour
{

    public void OnClickThis()
    {
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.AddExperience(1000);
        }
        else
        {
            Debug.LogError("PlayerDataManager.Instance is not available!");
        }
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
}
