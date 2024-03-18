using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 메인씬에 활용할 라이트 이펙트 스크립트
// 일정 주기마다 라이트가 꺼졌다 켜졌다 반복함.
// 라이트는 실시간으로 설정되어야하고 큐브하나가 할당 되어야함(전등 오브젝트 가리는 용도).
public class LightEffect : MonoBehaviour
{
    [SerializeField]
    private GameObject lightObj;

    void Start()
    {
        StartCoroutine(LightEffectCoroutine());
    }

    IEnumerator LightEffectCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(4f,6f));
            int l = Random.Range(2, 5);
            for (int i = 0; i < l; i++)
            {
                lightObj.SetActive(true);
                yield return new WaitForSeconds(0.05f);
                lightObj.SetActive(false);
                yield return new WaitForSeconds(0.025f);
            }
        }
    }
}
