using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : MonoBehaviour
{
    [SerializeField]
    private int maxHP;
    [SerializeField]
    private int currentHP;
    [SerializeField]
    private GrayscaleFilter grayscaleFilter;

    public float MaxStamina = 150.0f; // 최대 스테미나
    public float healStamina; // 기본 회복량 제어 변수
    public float CurrentStamina; // 현재 스테미나
    public float StaminaDesreaseRate = 10f; // 감소 스테미나
    public float staminaIncreaseRate; // 증가될 스테미나 수치
    public float IncreaseStamina; // 가변 저장 스테미나
    public bool isStaminaMax; // 최대치 확인

    private PlayerControl playerControl; //히트 ui 출력
    private bool showHealTooltip = true; //회복 툴팁 출력
    
    public AudioClip hitSound;

    private bool isDead;

    private void Start()
    {
        playerControl = GetComponent<PlayerControl>();  
    }
    public void Hit(int dmg)
    {
        if (isDead)
        {
            return;
        }

        currentHP -= dmg;
        playerControl.Hitfeedback();
        SoundManager.Instance.PlaySFX(hitSound);

        if (currentHP <= 0)
        {
            isDead = true;
            EventManager.Instance.DeadEvent(GameManager.Instance.player);
        }

        if(showHealTooltip && (currentHP <= 50 && GameManager.Instance.player.GetComponent<Inventory>().FirstAidKit > 0))
        {
            UIManager.Instance.SetToolTip("Press T to recover HP");
            showHealTooltip = false;
        }

        grayscaleFilter.setFilter(currentHP);
    }

    public void Heal(int amount)
    {
        currentHP = Math.Min(currentHP + amount, maxHP);

        grayscaleFilter.setFilter(currentHP);
    }

    
}
