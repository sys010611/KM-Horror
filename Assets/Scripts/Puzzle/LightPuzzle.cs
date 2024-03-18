using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class LightPuzzle : Puzzle
{
    public GameObject canvas;

    public Material red;
    public Material green;

    public GameObject[] lights = new GameObject[4];
    public GameObject boxCover;

    public GameObject item;

    [SerializeField] float rewardPopUpHeight;

    Renderer rend;

    private static bool isFirstEncounter = true;

    // Start is called before the first frame update
    protected override void EnablePuzzle()
    {
        base.EnablePuzzle();
        UIManager.Instance.DeactivateInteractionUI();
        UIManager.Instance.currLightPuzzle = gameObject.GetComponent<LightPuzzle>();
        UIManager.Instance.SetPannel(UIManager.UIState.PUZZLE_LIGHT); //ui 표시
        GameManager.Instance.player.CanControl = false; //움직임 잠그기
        GameManager.Instance.weaponManager.toBareHands(); //무기 집어넣기

        if(isFirstEncounter && GameManager.Instance.HasPlayerReadPuzzleInfo)
        {
            isFirstEncounter = false;
            UIManager.Instance.SetConversation(15, 0);
        }
    }

    void reverseColor(GameObject light)
    {
        rend = light.GetComponent<Renderer>();
        Material mat = rend.sharedMaterial;

        if (mat == red)
        {
            rend.sharedMaterial = green;
        }
        else if (mat == green)
        {
            rend.sharedMaterial = red;
        }
    }

    public void button1Pressed()
    {
        reverseColor(lights[1]);

        solveCheck();
    }
    public void button2Pressed()
    {
        reverseColor(lights[0]);
        reverseColor(lights[1]);
        reverseColor(lights[2]);

        solveCheck();
    }
    public void button3Pressed()
    {
        reverseColor(lights[1]);
        reverseColor(lights[2]);
        reverseColor(lights[3]);

        solveCheck();
    }
    public void button4Pressed()
    {
        reverseColor(lights[2]);

        solveCheck();
    }

    void solveCheck()
    {
        bool solved = true;
        for(int i=0; i<4; i++)
        {
            if (lights[i].GetComponent<Renderer>().sharedMaterial == red)
                solved = false;
        }

        if (solved)
        {
            SoundManager.Instance.PlaySFX(successSound);
            GameManager.Instance.player.interActionQueue = new Queue<System.Action>(GameManager.Instance.player.interActionQueue.Where(x => x != EnablePuzzle));
            UIManager.Instance.DeactivateInteractionUI();

            GetComponent<BoxCollider>().enabled = false; //터미널 비활성화

            this.isClear = true;

            UIManager.Instance.SetPannel(UIManager.UIState.NONE);

            boxCover.transform.DOLocalRotate(new Vector3(-120, 0, 0), 4); //상자 개봉
            item.SetActive(true);
            item.transform.DOLocalMoveY(rewardPopUpHeight, 2);
        }
    }
}
