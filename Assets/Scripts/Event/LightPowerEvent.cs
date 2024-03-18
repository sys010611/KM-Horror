using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LightPowerEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject[] lights;
    [SerializeField]
    private AudioClip[] audioClip;
    [SerializeField]
    private GameObject KnobObj;

    [SerializeField]
    private GameObject[] Monster;

    [SerializeField]
    private bool ishit = false; //상호작용 변수

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        if (GameManager.Instance.islighton)
        {
            KnobCtrl();
            StartCoroutine(PowerLightCoroutine()); //불 켜짐
            GetComponent<BoxCollider>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            #if UNITY_EDITOR
            Debug.Log("ishit");
#endif
            GameManager.Instance.player.interActionQueue.Enqueue(Use);
            ishit = true;
            //ON UI
            UIManager.Instance.SetInteractionText("USE");
            UIManager.Instance.ActivateInteractionUI();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ishit = false;
            //Off UI
            UIManager.Instance.DeactivateInteractionUI();
        }
    }

    public void Use()
    {
        if (!GameManager.Instance.islighton && ishit)
        {
            KnobCtrl();
            StartCoroutine(PowerLightCoroutine()); //불 켜짐
            UIManager.Instance.DeactivateInteractionUI();
            GetComponent<BoxCollider>().enabled = false;
            #if UNITY_EDITOR
            Debug.Log("Light turn on");
#endif
        }
    }

    private void Onsound()
    {
        SoundManager.Instance.PlaySound(transform.position ,audioClip[0]); //발전기 작동 중 소리
        SoundManager.Instance.PlaySound(transform.position, audioClip[1]); //발전기 조작 소리
    }

    private void KnobCtrl()
    {
        KnobObj.transform.DOLocalRotate(new Vector3(-87.3f,0f,0f), 0.8f);
    }

    IEnumerator PowerLightCoroutine()
    {
        int togglecount = 2;
        Onsound();
        while (togglecount != 0) //불 킬때 깜박거리는 연출
        {
            yield return new WaitForSeconds(1.1f);
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].SetActive(true);

            }

            yield return new WaitForSeconds(0.35f);

            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].SetActive(false);

            }

            yield return new WaitForSeconds(0.14f);

            togglecount--; //횟수
        }

        for (int i = 0; i < lights.Length; i++) //불 키기
        {
            lights[i].SetActive(true);
        }
        GameManager.Instance.islighton = true; //켜짐 활성화

        for (int i = 0; i < Monster.Length; i++) 
        {
            Monster[i].SetActive(true); //난이도 증가
        }

        yield return null;

    }
}
