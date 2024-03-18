using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private int healAmount;

    public int _FirstAidKit = 0; //소지중인 구급상자의 개수

    public int keycard_Red = 0; //카드키 소지 현황
    public int keycard_Blue = 0;
    public int keycard_Green = 0;
    public int keycard_Yellow = 0;

    [SerializeField]
    private int _flask = 0; //소지중인 플라스크의 개수

    public int picture1 = 0;
    public int picture2 = 0;
    public int picture3 = 0;
    public int picture4 = 0;

    [SerializeField] private int _endpassword1 = 0;
    [SerializeField] private int _endpassword2 = 0;
    [SerializeField] private int _endpassword3 = 0;
    [SerializeField] private int _endpassword4 = 0;

    public int syringe = 0;

    private PlayerStat playerStat;
    [SerializeField] private EndPassword endPassword;

    [SerializeField]
    private int _shotgunBullet = 0;
    [SerializeField]
    private int _pistolBullet = 0;

    public int FirstAidKit { get => _FirstAidKit; set { _FirstAidKit = value; UIManager.Instance.hotBar.SetFirstAidKitText(value); } }
    public int Flask { get => _flask; set { _flask = value; UIManager.Instance.hotBar.SetFlaskText(value); } }
    public int ShotgunBullet { get => _shotgunBullet; set { _shotgunBullet = value; UIManager.Instance.hotBar.SetShotgunAmmoText(value); } }
    public int PistolBullet { get => _pistolBullet; set { _pistolBullet = value; UIManager.Instance.hotBar.SetPistolAmmoText(value); } }
    public int endpassword1 { get => _endpassword1; set { _endpassword1 = value; if (endPassword != null) endPassword.CurrentPassword(); } }
    public int endpassword2 { get => _endpassword2; set { _endpassword2 = value; endPassword.CurrentPassword();  } }
    public int endpassword3 { get => _endpassword3; set { _endpassword3 = value; endPassword.CurrentPassword(); } } 
    public int endpassword4 { get => _endpassword4; set { _endpassword4 = value; endPassword.CurrentPassword(); } }

    [SerializeField] private AudioClip healSound;

    private void Start()
    {
        playerStat = GetComponent<PlayerStat>();
    }

    private void Update()
    {
        heal();
    }


    private void heal()
    {
        if (Input.GetKeyDown(KeyCode.T)) // T 키로 체력 회복
        {
            if (FirstAidKit > 0)
            {
                playerStat.Heal(healAmount);

                UIManager.Instance.SetConversation("HP recovered");
                SoundManager.Instance.PlaySFX(healSound);

                Color color = UIManager.Instance.Bloodimage.color;
                color.a = 0;

                UIManager.Instance.Bloodimage.color = color;

                FirstAidKit--;
            }
        }
    }
}
