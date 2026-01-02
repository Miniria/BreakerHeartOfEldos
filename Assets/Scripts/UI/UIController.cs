using System;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    [Header("Pause Setup")]
    public GameObject pauseMenuUI;
    private bool isPaused = false;
    
    [Header("Game Speed setup")]
    public TMP_Text buttonText;
    public Button speedButton; 
    private bool isDoubleSpeed = false;


    private void Awake()
    {
        pauseMenuUI.SetActive(false);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {   
            Time.timeScale = 0f;
            pauseMenuUI.SetActive(true);
        }
        else
        {
            if (isDoubleSpeed)
            {
                Time.timeScale = 2f;
            }
            else
            {
                Time.timeScale = 1f;
            }
            pauseMenuUI.SetActive(false);
        }
    }
    
    public void ToggleSpeed()
    {
        isDoubleSpeed = !isDoubleSpeed;

        // ปรับเวลาเกม
        Time.timeScale = isDoubleSpeed ? 2f : 1f;

        // อัปเดต UI
        UpdateButtonVisual();
    }
    
    private void UpdateButtonVisual()
    {
        if (isDoubleSpeed)
        {
            // ตัวหนา สีเหลือง
            buttonText.fontStyle = FontStyles.Bold;
            buttonText.color = Color.yellow;
        }
        else
        {
            // ตัวปกติ สีขาว
            buttonText.fontStyle = FontStyles.Normal;
            buttonText.color = Color.white;
        }
    }
}
