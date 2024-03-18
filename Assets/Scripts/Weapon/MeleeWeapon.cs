using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*근접공격 판정을 담당하는 트리거 콜라이더, onTriggerEnter 함수는
각각 weaponManager 스크립트와 weapon오브젝트에 있습니다.
*/

public class MeleeWeapon : MonoBehaviour
{
    BoxCollider attackArea; //공격 판정 범위
    Animator anim;

    public bool attacking = false; //현재 공격중인지

    public AudioClip attackSound;

    // Start is called before the first frame update
    void Awake()
    {
        attackArea = GameManager.Instance.weaponManager.GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.player.CanControl)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1") && !attacking) //마우스 왼쪽 버튼 감지 && 현재 공격중이지 않을 시
        {
            anim.SetTrigger("Attack"); //휘두르는 애니메이션 재생
            attacking = true;
            //StartCoroutine("attack"); //attack 코루틴 실행
        }
    }
    /*IEnumerator attack() //공격 모션, 공격 판정
    {
        yield return new WaitForSeconds(0.3f);
        attackArea.enabled = true; // 공격 판정 활성화
        SoundManager.Instance.PlaySFX(attackSound);
        yield return new WaitForSeconds(0.1f);
        attackArea.enabled = false; //공격 판정 비활성화
        yield return new WaitForSeconds(0.1f);
        attacking = false;
    }*/

    public void Attack()
    {
        SoundManager.Instance.PlaySFX(attackSound);
        attackArea.enabled = true;
    }

    public void DisableAttackArea()
    {
        attackArea.enabled = false;
    }

    public void AttackEnd()
    {
        attacking = false;
    }

    private void OnDisable()
    {
        attacking = false;
    }
}
