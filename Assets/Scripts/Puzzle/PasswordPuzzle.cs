using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PasswordPuzzle : Puzzle
{
    [SerializeField]
    private int[] password;
    protected List<int> inputNums = new List<int>();
    [SerializeField]
    private GameObject door;
    [SerializeField]
    protected GameObject[] monitorClosed;
    [SerializeField]
    protected GameObject[] monitorOpen;

    [SerializeField]
    private Light pointLight;

    [SerializeField]
    protected AudioClip doorOpenSound;

    [SerializeField]
    private GameManager.PasswordType passwordType;

    protected override void EnablePuzzle()
    {
        base.EnablePuzzle();
        UIManager.Instance.DeactivateInteractionUI();
        UIManager.Instance.SetPannel(UIManager.UIState.PUZZLE_PASSWORD);
        inputNums.Clear();

        if(passwordType != GameManager.PasswordType.NONE)
        {
            // index: 플레이어가 비밀번호를 확인했는지 여부
            int index = 0; //기본값: 0
            if (GameManager.Instance.checkedPasswords.Contains(passwordType))
                index = 1;
            UIManager.Instance.SetConversation((int)passwordType, index);
        }
    }

    public virtual void CheckAnswer(int num)
    {
        inputNums.Add(num);
        if (inputNums.Count.Equals(password.Length))
        {
            if (password.SequenceEqual(inputNums))
            {
                // 성공
                GameManager.Instance.player.interActionQueue = new Queue<System.Action>(GameManager.Instance.player.interActionQueue.Where(x => x != EnablePuzzle));
                UIManager.Instance.DeactivateInteractionUI();

                door.transform.DOLocalMoveY(door.transform.position.y + 2.5f, 1.5f).SetEase(Ease.Linear);
                for (int i = 0; i < monitorClosed.Length; i++)
                    monitorClosed[i].SetActive(false);
                for (int i = 0; i < monitorOpen.Length; i++)
                    monitorOpen[i].SetActive(true);
                pointLight.color = Color.green;
                UIManager.Instance.SetPannel(UIManager.UIState.NONE);
                SoundManager.Instance.PlaySFX(successSound);
                SoundManager.Instance.PlaySound(door.transform.position, doorOpenSound);
            }
            else
            {
                // 실패
                inputNums.Clear();
                SoundManager.Instance.PlaySFX(failureSound);
            }
        }
    }
}
