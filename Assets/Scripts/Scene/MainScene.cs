using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class MainScene : MonoBehaviour
{
    [SerializeField]
    private Image fadeImage;

    [SerializeField]
    private AudioClip startSound;
    [SerializeField]
    private AudioClip optionSound;
    [SerializeField]
    private AudioClip quitSound;
    [SerializeField]
    private AudioClip mainBGM;
    [SerializeField]
    private Button continueButton;

    public void Start()
    {
        if (!PlayerPrefs.HasKey("PosX"))
            continueButton.interactable = false;
        else
            continueButton.interactable = true;

        this.Invoke(() =>
        {
            UIManager.Instance.FadeImage(0);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SoundManager.Instance.PlayMainBGM(mainBGM, true);
            UIManager.Instance.hotBar.Clear();
        }, 1f);
    }

    public void StartGame()
    {
        //다른 버튼들 비활성화
        var buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
            button.enabled = false;

        UIManager.Instance.ClearPicutrMaskIcon();
        SoundManager.Instance.PlaySFX(startSound);
        SoundManager.Instance.StopMainBgm();
        SaveManager.Instance.isLoad = false;
        fadeImage.DOFade(1, 3f).OnComplete(() => 
        {
            SoundManager.Instance.StopMainBgm();
            SceneManager.LoadScene("LoadingScene");
            UIManager.Instance.hasPasswordHints = false;
            GameManager.Instance.islighton = false;
            GameManager.Instance.HasPlayerReadPuzzleInfo = false;
            GameManager.Instance.checkedPasswords.Clear();
            UIManager.Instance.SetPasswordHint();
        });
    }

    public void ContinueGame()
    {
        if (PlayerPrefs.HasKey("PosX"))
        {
            SaveManager.Instance.isLoad = true;
            SoundManager.Instance.PlaySFX(startSound);
            SoundManager.Instance.StopMainBgm();
            fadeImage.DOFade(1, 3f).OnComplete(() =>
            {
                SoundManager.Instance.StopMainBgm();
                SceneManager.LoadScene("LoadingScene");
            });
        }
    }

    public void ShowOption()
    {
        SoundManager.Instance.PlaySFX(optionSound);
        UIManager.Instance.SetPannel(UIManager.UIState.SETTING);
    }

    public void QuitGame()
    {
        SoundManager.Instance.PlaySFX(quitSound);

#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
