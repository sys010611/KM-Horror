using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WeaponManager : MonoBehaviour
{
    public Inventory inventory;
    
    GameObject flashlight;
    GameObject baseballBat;
    GameObject crowbar;
    GameObject axe;
    GameObject pistol;
    GameObject shotgun;
    GameObject flask;
    GameObject HItBlood;
    public GameObject currentWeapon = null;
    public GameObject weaponCam;


    private Enemy attackedEnemy;
    [SerializeField] private int[] dmg = {10, 20, 30}; //야구배트, 쇠지레, 도끼의 데미지
    private int currDmg;

    public bool[] weaponList = new bool[6]; //각 무기의 소지 여부 

    [SerializeField] private AudioClip hitSound;


    private void Awake()
    {
        flashlight = transform.GetChild(0).gameObject;
        baseballBat = transform.GetChild(1).gameObject;
        crowbar = transform.GetChild(2).gameObject;
        axe = transform.GetChild(3).gameObject;
        pistol = transform.GetChild(4).gameObject;
        shotgun = transform.GetChild(5).gameObject;
        flask = transform.GetChild(6).gameObject;
    }

    private void Update()
    {
        if (!GameManager.Instance.player.CanControl)
        {
            return;
        }

        selectWeapon();
        throwFlask();
    }

    void selectWeapon()
    {

        if(Input.GetKeyDown(KeyCode.Alpha1) && weaponList[0]) //손전등 선택
        {
            if(currentWeapon == flashlight) //현재 무기와 같은 무기 선택 -> 맨손으로 전환
            {
                toBareHands();
                return;
            }

            if (currentWeapon != null) //들고있던 다른 무기 집어넣기
                StartCoroutine(release(currentWeapon));

            StartCoroutine(equip(flashlight));
            currentWeapon = flashlight;
            UIManager.Instance.hotBar.SelectItem(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && weaponList[1]) //야구배트 선택
        {
            if (currentWeapon == baseballBat) //현재 무기와 같은 무기 선택 -> 맨손으로 전환
            {
                toBareHands();
                return;
            }

            if (currentWeapon != null) //들고있던 다른 무기 집어넣기
                StartCoroutine(release(currentWeapon));

            StartCoroutine(equip(baseballBat));
            currentWeapon = baseballBat;
            currDmg = dmg[0];
            UIManager.Instance.hotBar.SelectItem(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && weaponList[2]) //크로우바 선택
        {
            if (currentWeapon == crowbar) //현재 무기와 같은 무기 선택 -> 맨손으로 전환
            {
                toBareHands();
                return;
            }

            if (currentWeapon != null) //들고있던 다른 무기 집어넣기
                StartCoroutine(release(currentWeapon));

            StartCoroutine(equip(crowbar));
            currentWeapon = crowbar;
            currDmg = dmg[1];
            UIManager.Instance.hotBar.SelectItem(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && weaponList[3]) //도끼 선택
        {
            if (currentWeapon == axe) //현재 무기와 같은 무기 선택 -> 맨손으로 전환
            {
                toBareHands();
                return;
            }

            if (currentWeapon != null) //들고있던 다른 무기 집어넣기
                StartCoroutine(release(currentWeapon));

            StartCoroutine(equip(axe));
            currentWeapon = axe;
            currDmg = dmg[2];
            UIManager.Instance.hotBar.SelectItem(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) && weaponList[4]) //권총 선택
        {
            if (currentWeapon == pistol) //현재 무기와 같은 무기 선택 -> 맨손으로 전환
            {
                toBareHands();
                return;
            }

            if (currentWeapon != null) //들고있던 다른 무기 집어넣기
                StartCoroutine(release(currentWeapon));

            StartCoroutine(equip(pistol));
            currentWeapon = pistol;
            UIManager.Instance.hotBar.SelectItem(4);
        }
    }

    public void toBareHands()
    {
        if (currentWeapon != null)
        {
            StartCoroutine(release(currentWeapon));
        }   
        currentWeapon = null;
        return;
    }

    void throwFlask()
    {
        if(inventory.Flask != 0 && Input.GetKeyDown(KeyCode.G)) // G를 눌러 플라스크 던지기
        {
            inventory.Flask--;
            flask.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other) //근접무기로 공격했을 시
    {
        if (other.transform.CompareTag("Enemy") || other.transform.CompareTag("Mimic")) // 닿은 오브젝트의 태그가 Enemy일 경우
        {
            attackedEnemy = other.gameObject.GetComponent<Enemy>(); //적의 정보 가져오기
            attackedEnemy.Hit(currDmg);
            SoundManager.Instance.PlaySound(attackedEnemy.transform.position, hitSound);
        }
    }
    //각 무기들의 y좌표(로컬): 착용시 1.35, 미착용시 1
    IEnumerator equip(GameObject weapon)
    {
        if (currentWeapon != null)
            yield return new WaitForSeconds(0.5f);
        weapon.SetActive(true);
        weapon.transform.DOLocalMoveY(1.35f, 0.5f); 
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator release(GameObject weapon)
    {
        weapon.transform.DOLocalMoveY(1f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        weapon.SetActive(false);
    }
}
