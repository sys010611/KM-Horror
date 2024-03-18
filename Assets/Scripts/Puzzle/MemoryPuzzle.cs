using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MemoryPuzzle : Puzzle
{
    [SerializeField]
    private List<int> passwordAnswer = new List<int>(); // 숫자들이 랜덤으로 섞여있음.
    [SerializeField]
    private GameObject boxCover;
    [SerializeField]
    private Light pointLight;

    [SerializeField]
    private GameObject reward;
    
    private int currentIdx; // 현재 정답까지 도달한 숫자 인덱스를 알려줌

    [SerializeField]
    private AudioClip chestOpenSound;

    private static bool isFirstEncounter = true;

    private void Start()
    {
        List<int> temp = new List<int>();
        for (int i = 0; i < 9; i++)
            temp.Add(i + 1);

        while (temp.Count > 0)
        {
            int i = Random.Range(0, temp.Count);
            passwordAnswer.Add(temp[i]);
            temp.RemoveAt(i);
        }
    }
    
    protected override void EnablePuzzle()
    {
        base.EnablePuzzle();
        UIManager.Instance.SetPannel(UIManager.UIState.PUZZLE_MEMMORY);
        currentIdx = 0;

        if(isFirstEncounter && GameManager.Instance.HasPlayerReadPuzzleInfo)
        {
            isFirstEncounter = false;
            UIManager.Instance.SetConversation(17, 0);
        }
    }

    public int CheckAnswer(int num)
    {
        if (passwordAnswer[currentIdx].Equals(num))
        {
            // 정답
            currentIdx++;
            if (currentIdx.Equals(passwordAnswer.Count))
            {
                // 퍼즐 클리어
                GameManager.Instance.player.interActionQueue = new Queue<System.Action>(GameManager.Instance.player.interActionQueue.Where(x => x != EnablePuzzle));
                UIManager.Instance.DeactivateInteractionUI();
                isClear = true;
                boxCover.transform.DOLocalRotate(new Vector3(-120f, 0, 0), 1f);
                pointLight.color = Color.green;
                UIManager.Instance.SetPannel(UIManager.UIState.NONE);
                SoundManager.Instance.PlaySFX(successSound);
                SoundManager.Instance.PlaySound(reward.transform.position, chestOpenSound);
                reward.SetActive(true);
            }
        }
        else
        {
            // 오답
            SoundManager.Instance.PlaySFX(failureSound);
            currentIdx = 0;
        }

        return currentIdx;
    }
}