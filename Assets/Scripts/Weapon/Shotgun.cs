using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Shotgun : MonoBehaviour
{
    public AudioClip gunSound;
    public AudioClip outOfAmmoSound;
    public AudioClip reloadSound;
    public AudioClip insertSound;
    public AudioClip pumpSound;

    [SerializeField]
    private int dmg = 70;
    private bool isShot = false; //사격 후 딜레이 (true일 시 사격 불가능)
    private bool isReloading;

    Enemy attackedEnemy; //적의 정보
    Animator anim; //발사 애니메이션
    ParticleSystem gunFlame;
    CapsuleCollider attackRange;
    Inventory inventory;

    private int maxAmmo = 2;
    public int currentAmmo = 0;

    List<GameObject> hitEnemies = new List<GameObject>();

    void Awake()
    {
        inventory = GameManager.Instance.inventory;
        anim = GetComponent<Animator>();
        gunFlame = transform.GetChild(1).GetComponent<ParticleSystem>();
        attackRange = GetComponent<CapsuleCollider>();
    }


    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.player.CanControl)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1") && !isReloading) //마우스 왼쪽 버튼
        {
            if(currentAmmo > 0 && !isShot)
            {
                isShot = true; StartCoroutine("Shoot");
                anim.SetTrigger("Shoot"); //발사 애니메이션 재생
                gunFlame.Play(); //화염효과 재생
                SoundManager.Instance.PlaySFX(gunSound); //격발음 재생
                StartCoroutine(CamShake());
                --currentAmmo;
            }
            else
            {
                if(!(currentAmmo >0) && !isShot)
                {
                    SoundManager.Instance.PlaySFX(outOfAmmoSound); // 잔탄 없음 재생
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo != maxAmmo && !isReloading && inventory.ShotgunBullet > 0)
        {
            isReloading = true;
            StartCoroutine(Reload());
        }
    }

    private void OnTriggerEnter(Collider other) //공격 판정
    {
        if(other.transform.CompareTag("Enemy") || other.transform.CompareTag("Mimic") || other.transform.CompareTag("Creature") && !hitEnemies.Contains(other.gameObject))
        {
            hitEnemies.Add(other.gameObject);
            attackedEnemy = other.transform.GetComponent<Enemy>(); //맞춘 적의 정보 가져오기
            attackedEnemy.Hit(dmg); // 적 체력 감소
        }
    }

    IEnumerator Shoot()
    {
        hitEnemies.Clear();
        attackRange.enabled = true; //콜라이더 활성화
        yield return new WaitForSeconds(0.05f);
        attackRange.enabled = false; //콜라이더 비활성화
        yield return new WaitForSeconds(1f);
        isShot = false;
    }

    IEnumerator Reload()
    {
        SoundManager.Instance.PlaySFX(reloadSound);
        yield return new WaitForSeconds(2f);
        SoundManager.Instance.PlaySFX(insertSound);
        --inventory.ShotgunBullet;
        ++currentAmmo;
        if (inventory.ShotgunBullet > 0 && currentAmmo != maxAmmo)
        {
            yield return new WaitForSeconds(1f);
            SoundManager.Instance.PlaySFX(insertSound);
            --inventory.ShotgunBullet;
            ++currentAmmo;
        }
        yield return new WaitForSeconds(2f);
        SoundManager.Instance.PlaySFX(pumpSound);
        isReloading = false;
    }

    IEnumerator CamShake()
    {
        Vector3 startPos = Camera.main.transform.localPosition;

        float duration = 0.1f;
        float magnitude = 0.3f;
        float timer = 0;
        while (timer <= duration)
        {
            Camera.main.transform.localPosition = (Vector3)Random.insideUnitSphere * magnitude + startPos;

            timer += Time.deltaTime;
            yield return null;
        }
        Camera.main.transform.localPosition = startPos;
    }
    private void OnDisable()
    {
        isReloading = false;
    }
}
