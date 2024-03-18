using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PicturePuzzleSystem : MonoBehaviour
{
    public static PicturePuzzleSystem Instance;

    public GameObject boxCover;
    public GameObject item;

    public GameObject lightObj;
    public AudioClip successClip;

    private static bool isFirstEncounter = true;

    private void Awake()
    {
        Instance = this;
    }

    public void checkIfAllInserted() //모든 조각이 끼워졌을시 카드키 제공
    {
        bool allInsertedFlag = true;

        int pictureCount = 4;
        for(int i=0;i< pictureCount; i++)
        {
            PicturePuzzle picture = transform.GetChild(i).GetComponent<PicturePuzzle>();
            if (!picture.pictureInserted)
                allInsertedFlag = false;
        }

        if(allInsertedFlag)
        {
            boxCover.transform.DOLocalRotate(new Vector3(-120, 0, 0), 4); //상자 개봉
            item.SetActive(true);

            //그림 비밀번호 파악 완료
            GameManager.Instance.checkedPasswords.Add(GameManager.PasswordType.PICTURE);
            UIManager.Instance.SetConversation(9, 0);

            lightObj.SetActive(true);
            SoundManager.Instance.PlaySFX(successClip);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(isFirstEncounter && GameManager.Instance.HasPlayerReadPuzzleInfo)
            {
                isFirstEncounter = false;
                UIManager.Instance.SetConversation(16, 0);
            }
        }
    }
}
