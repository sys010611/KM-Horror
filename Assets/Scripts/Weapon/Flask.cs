using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플라스크가 플레이어 콜라이더와 충돌판정이 생겨 제대로 던져지지 않을 가능성이 있어,
//프로젝트 설정에서 플레이어 레이어와 플라스크 레이어를 구분하여 둘은 충돌판정을 없앴습니다. (오브젝트에 레이어 설정 필요)

public class Flask : MonoBehaviour
{
    public Transform cam;
    public Transform parent;

    Rigidbody rigid;
    [SerializeField] AudioClip crackClip;
    [SerializeField] int throwingForce = 10;
    [SerializeField] float aggroRange = 15;
    [SerializeField] LayerMask enemyLayer;
    bool collisionFlag = false;


    Vector3 initPosition = new Vector3(0,1.5f,0.5f);

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        rigid.AddForce(cam.forward * throwingForce, ForceMode.Impulse);
        transform.SetParent(null);
        collisionFlag = false;
    }

    private void OnCollisionEnter(Collision collision) //지형지물에 부딪혀 깨졌을때
    {
        if (collisionFlag == false)
            Crack();
    }

    private void Crack()
    {
        collisionFlag = true;

        SoundManager.Instance.PlaySound(transform.position, crackClip);

        rigid.velocity = Vector3.zero;

        Collider[] creatureColliders = Physics.OverlapSphere(transform.position, aggroRange, enemyLayer);
        for (int i = 0; i < creatureColliders.Length; i++) 
        {
            if (creatureColliders[i].CompareTag("Creature"))
            {
                creatureColliders[i].GetComponent<Enemy>().SetAggro(transform.position);
            }
        }

        Collider[] zombieColliders = Physics.OverlapSphere(transform.position, aggroRange / 2, enemyLayer);
        for (int i = 0; i < zombieColliders.Length; i++)
        {
            if (zombieColliders[i].CompareTag("Mimic"))
            {
                zombieColliders[i].GetComponent<Enemy>().SetAggro(transform.position);
            }
        }

        transform.SetParent(parent); //초기화 작업
        transform.localPosition = initPosition;
        gameObject.SetActive(false);
    }
}
