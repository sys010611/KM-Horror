using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EndDoorPuzzle : PasswordPuzzle
{
    [SerializeField]
    EndPassword endPassword;

    [SerializeField]
    private string password_str;

    [SerializeField]
    private GameObject door_Left;
    [SerializeField]
    private GameObject door_Right;
    [SerializeField]
    private Transform spawningEnemies;
    [SerializeField]
    private GameObject warningEvent;

    [SerializeField]
    private AudioClip grawlSound;

    private List<int> password_List = new List<int>();

    protected override void EnablePuzzle()
    {
        UIManager.Instance.DeactivateInteractionUI();
        password_str = endPassword.Result;

        int passwordNum;
        if (!Int32.TryParse(password_str, out passwordNum)) // parse 실패 (password에 '?'이 섞여있음)
        {
            UIManager.Instance.SetConversation(6, 0);
            UIManager.Instance.SetPannel(UIManager.UIState.PUZZLE_PASSWORD_DUMMY); // 의미없는 비밀번호 패널 활성화
            return;
        }

        // parse 성공: 비밀번호를 알고 있음
        UIManager.Instance.SetConversation(6, 1);

        if (password_List.Count == 0)
        {
            for (int i = 0; i < password_str.Length; i++) //password 리스트에 숫자 하나씩 추가
            {
                password_List.Add(password_str[i] - '0');
            }
        }

        GameManager.Instance.player.interActionQueue.Enqueue(EnablePuzzle);
        UIManager.Instance.currentPuzzle = this;

        UIManager.Instance.SetPannel(UIManager.UIState.PUZZLE_PASSWORD);
        inputNums.Clear();
    }

    public override void CheckAnswer(int num)
    {
        inputNums.Add(num);
#if UNITY_EDITOR
        Debug.Log("password length: " + password_List.Count);
#endif
        if (inputNums.Count.Equals(password_List.Count))
        {
            if (password_List.SequenceEqual(inputNums))
            {
                // 성공
                GameManager.Instance.player.interActionQueue = new Queue<System.Action>(GameManager.Instance.player.interActionQueue.Where(x => x != EnablePuzzle));
                UIManager.Instance.DeactivateInteractionUI();

                for (int i = 0; i < monitorClosed.Length; i++)
                    monitorClosed[i].SetActive(false);
                for (int i = 0; i < monitorOpen.Length; i++)
                    monitorOpen[i].SetActive(true);
                UIManager.Instance.SetPannel(UIManager.UIState.NONE);
                SoundManager.Instance.PlaySFX(successSound);
                SoundManager.Instance.PlaySound(spawningEnemies.GetChild(spawningEnemies.childCount - 1).position, grawlSound);
                
                GameManager.Instance.playerSoundIncrement = 100000f; //플레이어 소리 증폭
                GetComponent<BoxCollider>().enabled = false;

                UIManager.Instance.SetConversation(13, 0);
                warningEvent.GetComponent<BoxCollider>().enabled = true;
                StartCoroutine(SpawnEnemies());
            }
            else
            {
                // 실패
                inputNums.Clear();
                SoundManager.Instance.PlaySFX(failureSound);
            }
        }
    }

    private IEnumerator SpawnEnemies()
    {
        foreach (Transform enemy in spawningEnemies)
        {
            yield return new WaitForSeconds(5f);

            enemy.gameObject.SetActive(true);

            if (enemy.GetSiblingIndex() == spawningEnemies.childCount-1)
                OpenDoor();
        }
    }

    private void OpenDoor()
    {
        door_Left.transform.DOLocalMoveX(door_Left.transform.localPosition.x - 1.5f, 1.5f).SetEase(Ease.Linear);
        door_Right.transform.DOLocalMoveX(door_Right.transform.localPosition.x + 1.5f, 1.5f).SetEase(Ease.Linear);

        SoundManager.Instance.PlaySound(door_Left.transform.position, doorOpenSound);
        SoundManager.Instance.PlaySound(door_Right.transform.position, doorOpenSound);
    }
}
