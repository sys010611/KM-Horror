using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class settingUI : MonoBehaviour
{
    [SerializeField] private GameObject setting;
    [SerializeField] private GameObject keySetting;
    [SerializeField] private Transform masterVolume;
    [SerializeField] private Transform bgmVolume;
    [SerializeField] private Transform effectVolume;
    [SerializeField] private Transform mouseSensitivity;

    [SerializeField] private GameObject QuitObj;

    [SerializeField] private AudioClip clickSound;

    float percentValue;
    TextMeshProUGUI percentText;

    public float tempMouseSensitivity;

    private void Awake()
    {
        masterVolume.GetChild(0).GetComponent<Scrollbar>().value = PlayerPrefs.GetFloat("MasterVolume", 1);
        setMasterVolume();

        bgmVolume.GetChild(0).GetComponent<Scrollbar>().value = PlayerPrefs.GetFloat("BgmVolume", 1);
        setBGMVolume();

        effectVolume.GetChild(0).GetComponent<Scrollbar>().value = PlayerPrefs.GetFloat("EffectVolume", 1);
        setEffectVolume();

        mouseSensitivity.GetChild(0).GetComponent<Scrollbar>().value = PlayerPrefs.GetFloat("MouseSensitivity", 1);
        setMouseSensitivity();
    }

    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "MainScene")
            QuitObj.SetActive(false);
        else
            QuitObj.SetActive(true);
    }

    public void quit()
    {
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        SaveSetting();
        SoundManager.Instance.PlaySFX(clickSound);
        UIManager.Instance.SetPannel(UIManager.UIState.NONE);
    }

    public void allOut()
    {
        SoundManager.Instance.PlaySFX(clickSound);
        if (SceneManager.GetActiveScene().name != "MainScene")
        {
            SoundManager.Instance.StopMainBgm();
            SoundManager.Instance.StopSubBgm();
            SoundManager.Instance.StopSubSubBgm();
            SceneManager.LoadScene("MainScene");
        }

        UIManager.Instance.SetPannel(UIManager.UIState.NONE);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SaveSetting();
    }

    public void showSetting()
    {
        SoundManager.Instance.PlaySFX(clickSound);
        keySetting.SetActive(false);
        setting.SetActive(true);
    }

    public void showKeySetting()
    {
        SoundManager.Instance.PlaySFX(clickSound);
        setting.SetActive(false);
        keySetting.SetActive(true);
    }

    public void setMasterVolume()
    {
        percentValue = masterVolume.GetChild(0).GetComponent<Scrollbar>().value; // 0 ~ 1 float
        percentText = masterVolume.GetChild(1).GetComponent<TextMeshProUGUI>();
        percentText.text = $"{percentValue*100}%";

        AudioListener.volume = percentValue;
    }

    public void setBGMVolume()
    {
        percentValue = bgmVolume.GetChild(0).GetComponent<Scrollbar>().value; // 0 ~ 1 float
        percentText = bgmVolume.GetChild(1).GetComponent<TextMeshProUGUI>();
        percentText.text = $"{percentValue * 100}%";

        SoundManager.Instance.SetBgmVolume(percentValue);
    }

    public void setEffectVolume()
    {
        percentValue = effectVolume.GetChild(0).GetComponent<Scrollbar>().value; // 0 ~ 1 float
        percentText = effectVolume.GetChild(1).GetComponent<TextMeshProUGUI>();
        percentText.text = $"{percentValue * 100}%";

        SoundManager.Instance.SetEffectVolume(percentValue);
    }

    public void setMouseSensitivity()
    {
        percentValue = mouseSensitivity.GetChild(0).GetComponent<Scrollbar>().value; // 0 ~ 1 float
        percentText = mouseSensitivity.GetChild(1).GetComponent<TextMeshProUGUI>();
        percentText.text = $"{percentValue * 100}%";

        if (SceneManager.GetActiveScene().name == "MainScene")
            UIManager.Instance.setTempMouseSensitivity(percentValue + 0.5f);
        else
            GameManager.Instance.player.SettingMouse = percentValue + 0.5f;
    }

    private void SaveSetting()
    {
        PlayerPrefs.SetFloat("MasterVolume", AudioListener.volume);
        PlayerPrefs.SetFloat("BgmVolume", bgmVolume.GetChild(0).GetComponent<Scrollbar>().value);
        PlayerPrefs.SetFloat("EffectVolume", effectVolume.GetChild(0).GetComponent<Scrollbar>().value);
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity.GetChild(0).GetComponent<Scrollbar>().value);
    }
}
