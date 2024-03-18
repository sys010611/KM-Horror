using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HotBarUI : MonoBehaviour
{
    [SerializeField]
    private GameObject[] items;

    [SerializeField]
    private Image[] keycards;

    [SerializeField]
    private Image[] images;

    [SerializeField]
    private TMP_Text[] texts;

    [SerializeField]
    private TMP_Text[] pictures;

    private int currentIdx = -1;

    private void Awake()
    {
        images = GetComponentsInChildren<Image>();
    }

    public void SetPistolAmmoText(int count)
    {
        texts[0].text = "Pistol Ammo : " + count;
    }

    public void SetShotgunAmmoText(int count)
    {
        texts[1].text = "Shotgun Ammo : " + count;
    }

    public void SetFirstAidKitText(int count)
    {
        texts[2].text = "FirstAidKit : " + count;
    }

    public void SetFlaskText(int count)
    {
        texts[3].text = "Flask : " + count;
    }

    public void SetoutpasswordText(string currentpassword)
    {
        texts[4].text = "Number : " + currentpassword;
    }

    public void SetPictureText(int PictureNum)
    {
        pictures[PictureNum - 1].text = $"Picture {PictureNum} : O";
    }

    /*
 * 
 * *********************** 추가한 코드 *****************************
 * */
    public void ResetPicturePossession()
    {
        for(int i=0;i <4;i++)
            pictures[i].text = $"Picture {i+1} : X";
    }
//여기까지

    public void SelectItem(int index)
    {
        if (index > 0)
            items[index].transform.GetChild(0).gameObject.SetActive(true);

        if (currentIdx > 0)
            items[currentIdx].transform.GetChild(0).gameObject.SetActive(false);

       currentIdx = index;
    }

    public void GetItme(int index)
    {
        items[index].transform.GetChild(1).gameObject.SetActive(true);
    }

    public void GetKeycard(int idx, bool remove = false)
    {
        keycards[idx].gameObject.SetActive(!remove);
    }

    public void FadeUI()
    {
        for (int i = 0; i < images.Length; i++)
            images[i].DOFade(0, 2);
        for (int i = 0; i < texts.Length; i++)
            texts[i].DOFade(0, 2);
    }

    public void ActiveUI()
    { 
        for (int i = 0; i < images.Length; i++)
        {
            images[i].DOKill();
            images[i].color = new Color(images[i].color.r, images[i].color.g, images[i].color.b, 1);
        }

        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].DOKill();
            texts[i].color = new Color(texts[i].color.r, texts[i].color.g, texts[i].color.b, 1);
        }
    }

    public void Clear()
    {
        SetFlaskText(0);
        SetPistolAmmoText(0);
        SetShotgunAmmoText(0);
        SetFirstAidKitText(0);
        SelectItem(0);
        for (int i = 0; i < items.Length; i++)
        {
            items[i].transform.GetChild(1).gameObject.SetActive(false);
        }
    }
}
