using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

public class EndingEvent : MonoBehaviour
{
    [SerializeField] private AudioClip endingSound;
    [SerializeField] private TextMeshProUGUI endingText;
    private string endtext; 
    public string RealEndText;
    public string BadEndText;

    private Inventory Inventory;

    private void Start()
    {
        Inventory = GameManager.Instance.player.GetComponent<Inventory>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var enemies = FindObjectsOfType<Enemy>(); // 적들 비활성화
        foreach (Enemy enemy in enemies)
            enemy.gameObject.SetActive(false);

        var volume = Array.Find(FindObjectsOfType<Volume>(), x => x.priority == 10);      
        volume.profile.TryGet<ColorAdjustments>(out var colorAdjustment);
        DOTween.To(() => colorAdjustment.postExposure.value, x => colorAdjustment.postExposure.value = x, 20, 0.5f)
            .OnComplete(() => UIManager.Instance.FadeImage());

        endingText.gameObject.SetActive(true);
        if (Inventory.syringe == 1)
        {
            endtext = RealEndText;
        }
        else if (Inventory.syringe == 0)
        {
            endtext = BadEndText;
        }

        this.Invoke(() => endingText.DOText(endtext, 0.5f), 5);

        SoundManager.Instance.StopSubBgm();
        SoundManager.Instance.PlayMainBGM(endingSound, true);


        this.Invoke(() => endingText.DOFade(0, 3), 12);
        this.Invoke(() => SceneManager.LoadScene("MainScene"), 16f);
    }
}
