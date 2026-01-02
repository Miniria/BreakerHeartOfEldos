using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public Image skillImage;
    public int skillIndex; // 0, 1, 2, ...
    public Image frame;
    public TMP_Text orderText;
    public TMP_Text cooldownText;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private Button button;
    private SkillDataSO skill;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Init(PlayerController controller)
    {
        this.playerController = controller;

        // --- SAFETY CHECK ---
        if (playerController.playerUnit.skills != null && skillIndex < playerController.playerUnit.skills.Count)
        {
            this.skill = playerController.playerUnit.skills[skillIndex];
            
            skillImage.sprite = skill.icon;
            playerController.OnQueueUpdated += UpdateButtonUI;
            
            button.interactable = true;
            gameObject.SetActive(true);
        }
        else
        {
            // ถ้าไม่มีสกิลสำหรับปุ่มนี้ ให้ปิดการใช้งานไปเลย
            Debug.LogWarning($"[SkillButton] No skill found at index {skillIndex}. Disabling button.");
            button.interactable = false;
            gameObject.SetActive(false); 
        }
        // --------------------
    }

    private void Update()
    {
        if (playerController != null && skill != null)
        {
            UpdateCooldownUI();
            UpdateQueueUI(playerController.playerUnit.selectedSkill);
        }
    }

    public void OnClick()
    {
        if (skill == null) return;
        
        // ถ้า cooldown > 0 → ไม่สามารถกดได้
        if (playerController.IsSkillOnCooldown(skill))
        {
            return;
        }
        
        playerController.EnqueueSkill(skillIndex);
    }

    private void UpdateButtonUI(List<SkillDataSO> queue)
    {
        if (skill == null) return;

        float cdRemaining = skill.currentCooldown;

        if (cdRemaining > 0f)
        {
            frame.color = Color.gray;
            cooldownText.gameObject.SetActive(true);
            cooldownText.text = Mathf.Ceil(cdRemaining).ToString();
            orderText.text = "";
        }
        else
        {
            cooldownText.gameObject.SetActive(false);
            int order = queue.FindIndex(s => s == skill);
            if (order >= 0)
            {
                frame.color = Color.green;
                orderText.text = (order + 1).ToString();
            }
            else
            {
                frame.color = Color.white;
                orderText.text = "";
            }
        }
    }
    
    private void UpdateQueueUI(List<SkillDataSO> queue)
    {
        if (skill == null) return;

        int order = queue.FindIndex(s => s == skill);
        if (order >= 0)
        {
            frame.color = Color.green;
            orderText.text = (order + 1).ToString();
        }
        else
        {
            frame.color = Color.white;
            orderText.text = "";
        }
    }
    
    private void UpdateCooldownUI()
    {
        if (skill == null) return;

        float cdRemaining = skill.currentCooldown;

        if (cdRemaining > 0f)
        {
            frame.color = Color.gray;
            cooldownText.gameObject.SetActive(true);
            cooldownText.text = Mathf.Ceil(cdRemaining).ToString();
            orderText.text = "";
        }
        else
        {
            cooldownText.gameObject.SetActive(false);
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (playerController != null)
        {
            playerController.OnQueueUpdated -= UpdateButtonUI;
        }
    }
}
