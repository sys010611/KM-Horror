using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    public Camera cam; // 1인칭 카메라 가져오기 (조준점은 화면의 정중앙으로 함)
    public AudioClip gunSound;
    public AudioClip outOfAmmoSound;
    public AudioClip reloadSound;
    public AudioClip magInSound;
    public AudioClip magOutSound;
    public AudioClip magCockPullSound;

    [SerializeField]
    private int dmg = 50;
    public bool isShot = false;
    private bool isReloading;

    Enemy attackedEnemy; //적의 정보
    Animator anim; //발사 애니메이션
    ParticleSystem gunFlame;
    Inventory inventory;

    RaycastHit hit; // 맞춘 오브젝트의 정보

    private int maxAmmo = 8;
    public int currentAmmo = 8;

    [SerializeField] private AudioClip hitSound;

    void Awake()
    {
        inventory = GameManager.Instance.inventory;
        anim = GetComponent<Animator>();
        gunFlame = transform.GetChild(2).GetComponent<ParticleSystem>();
    }


    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.player.CanControl)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1")) //마우스 왼쪽 버튼
        {
            if(currentAmmo > 0)
            {
                isShot = true;
                this.Invoke(() => isShot = false, 0.3f);
                anim.SetTrigger("Shoot"); //발사 애니메이션 재생
                gunFlame.Play(); //화염효과 재생
                SoundManager.Instance.PlaySFX(gunSound);

                // 판정: 시작점 - 카메라의 정중앙 / 방향 - 카메라로부터 정면 / 사거리 - 무한
                if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
                {
                    if (hit.transform.CompareTag("Enemy") || hit.transform.CompareTag("Mimic")) //맞춘 오브젝트의 태그가 enemy
                    {
                        attackedEnemy = hit.transform.GetComponent<Enemy>(); //맞춘 적의 정보 가져오기
                        attackedEnemy.Hit(dmg, true); // 적 체력 감소
                        SoundManager.Instance.PlaySound(attackedEnemy.transform.position, hitSound);
                    }
                }
                StartCoroutine(CamShake());
                --currentAmmo;
            }
            else
            {
                SoundManager.Instance.PlaySFX(outOfAmmoSound);
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo != maxAmmo && !isReloading && inventory.PistolBullet > 0)
        {
            isReloading = true;
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        SoundManager.Instance.PlaySFX(reloadSound);
        yield return new WaitForSeconds(2f);
        SoundManager.Instance.PlaySFX(magOutSound);
        yield return new WaitForSeconds(2f);
        SoundManager.Instance.PlaySFX(magInSound);
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.PlaySFX(magCockPullSound);

        currentAmmo = Mathf.Min(maxAmmo, inventory.PistolBullet);
        inventory.PistolBullet -= currentAmmo;

        isReloading = false;
    }

    IEnumerator CamShake()
    {
        Vector3 startPos = Camera.main.transform.localPosition;

        float duration = 0.05f;
        float magnitude = 0.05f;
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
