using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    [HideInInspector] public bool isLoad;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void Save(Inventory inventory, WeaponManager weaponManager, string sceneName, string[] neighborScenes)
    {
        PlayerPrefs.SetFloat("PosX", inventory.transform.position.x);
        PlayerPrefs.SetFloat("PosY", inventory.transform.position.y);
        PlayerPrefs.SetFloat("PosZ", inventory.transform.position.z);
        PlayerPrefs.SetInt("firstAidKit", inventory.FirstAidKit);
        PlayerPrefs.SetInt("keycard_Red", inventory.keycard_Red);
        PlayerPrefs.SetInt("keycard_Blue", inventory.keycard_Blue);
        PlayerPrefs.SetInt("keycard_Green", inventory.keycard_Green);
        PlayerPrefs.SetInt("keycard_Yellow", inventory.keycard_Yellow);
        PlayerPrefs.SetInt("picture1", inventory.picture1);
        PlayerPrefs.SetInt("picture2", inventory.picture2);
        PlayerPrefs.SetInt("picture3", inventory.picture3);
        PlayerPrefs.SetInt("picture4", inventory.picture4);
        PlayerPrefs.SetInt("_shotgunBullet", inventory.ShotgunBullet);
        PlayerPrefs.SetInt("_pistolBullet", inventory.PistolBullet);
        PlayerPrefs.SetInt("flask", inventory.Flask);
        for (int i = 0; i < weaponManager.weaponList.Length; i++)
        {
            if (weaponManager.weaponList[i])
                PlayerPrefs.SetInt($"weapon{i}", 1);
            else
                PlayerPrefs.SetInt($"weapon{i}", 0);
        }
        PlayerPrefs.SetString("playerLastScene", sceneName);
        PlayerPrefs.SetInt("neighborScenesLength", neighborScenes.Length);
        for (int i = 0; i < neighborScenes.Length; i++)
            PlayerPrefs.SetString($"neighborScene{i}", neighborScenes[i]);
        PlayerPrefs.SetInt("currentPistolAmmo", weaponManager.GetComponentInChildren<Pistol>(true).currentAmmo);
        PlayerPrefs.SetInt("currentShotgunAmmo", weaponManager.GetComponentInChildren<Shotgun>(true).currentAmmo);
        if (GameManager.Instance.difficult)
            PlayerPrefs.SetInt("difficult", 1);
        else
            PlayerPrefs.SetInt("difficult", 0);

        PlayerPrefs.SetInt("endpassword1", inventory.endpassword1);
        PlayerPrefs.SetInt("endpassword2", inventory.endpassword2);
        PlayerPrefs.SetInt("endpassword3", inventory.endpassword3);
        PlayerPrefs.SetInt("endpassword4", inventory.endpassword4);

        if (GameManager.Instance.islighton)
            PlayerPrefs.SetInt("islighton", 1);
        else
            PlayerPrefs.SetInt("islighton", 0);

        PlayerPrefs.SetInt("syringe", inventory.syringe);

        PlayerPrefs.SetInt("hasHint", UIManager.Instance.hasPasswordHints ? 1 : 0);

        PlayerPrefs.SetInt("ENDING_ITEM", GameManager.Instance.checkedPasswords.Contains(GameManager.PasswordType.ENDING_ITEM) ? 1 : 0);
        PlayerPrefs.SetInt("PICTURE", GameManager.Instance.checkedPasswords.Contains(GameManager.PasswordType.PICTURE) ? 1 : 0);
        PlayerPrefs.SetInt("floor", MapManager.Instance.currentFloor);

        PlayerPrefs.SetInt("hasPlayerReadPuzzleInfo", GameManager.Instance.HasPlayerReadPuzzleInfo ? 1 : 0);

        PlayerPrefs.SetInt("islighton", GameManager.Instance.islighton ? 1 : 0);
    }

    public void Load(Inventory inventory, WeaponManager weaponManager)
    {
        Vector3 pos = new Vector3();
        pos.x = PlayerPrefs.GetFloat("PosX");
        pos.y = PlayerPrefs.GetFloat("PosY");
        pos.z = PlayerPrefs.GetFloat("PosZ");
        inventory.transform.position = pos;
        inventory.FirstAidKit = PlayerPrefs.GetInt("firstAidKit");
        inventory.keycard_Red = PlayerPrefs.GetInt("keycard_Red");
        UIManager.Instance.hotBar.GetKeycard(0, inventory.keycard_Red == 0);
        inventory.keycard_Blue = PlayerPrefs.GetInt("keycard_Blue");
        UIManager.Instance.hotBar.GetKeycard(2, inventory.keycard_Blue == 0);
        inventory.keycard_Green = PlayerPrefs.GetInt("keycard_Green");
        inventory.keycard_Yellow = PlayerPrefs.GetInt("keycard_Yellow");
        UIManager.Instance.hotBar.GetKeycard(1, inventory.keycard_Yellow == 0);
        inventory.picture1 = PlayerPrefs.GetInt("picture1");
        inventory.picture2 = PlayerPrefs.GetInt("picture2");
        inventory.picture3 = PlayerPrefs.GetInt("picture3");
        inventory.picture4 = PlayerPrefs.GetInt("picture4");

        if (inventory.picture1 == 1)
            UIManager.Instance.DisablePictureIconOnMap(1);
        if (inventory.picture2 == 1)
            UIManager.Instance.DisablePictureIconOnMap(2);
        if (inventory.picture3 == 1)
            UIManager.Instance.DisablePictureIconOnMap(3);
        if (inventory.picture4 == 1)
            UIManager.Instance.DisablePictureIconOnMap(4);

        inventory.ShotgunBullet = PlayerPrefs.GetInt("_shotgunBullet");
        inventory.PistolBullet = PlayerPrefs.GetInt("_pistolBullet");
        inventory.Flask = PlayerPrefs.GetInt("flask");

        for (int i = 0; i < weaponManager.weaponList.Length; i++)
        {
            if (PlayerPrefs.GetInt($"weapon{i}") == 1)
            {
                weaponManager.weaponList[i] = true;
                UIManager.Instance.hotBar.GetItme(i);
            }
            else
                weaponManager.weaponList[i] = false;
        }

        if (inventory.keycard_Red == 1)
            UIManager.Instance.hotBar.GetKeycard(0);
        if (inventory.keycard_Yellow == 1)
            UIManager.Instance.hotBar.GetKeycard(1);
        if (inventory.keycard_Blue == 1)
            UIManager.Instance.hotBar.GetKeycard(2);

        weaponManager.GetComponentInChildren<Pistol>(true).currentAmmo = PlayerPrefs.GetInt("currentPistolAmmo");
        weaponManager.GetComponentInChildren<Shotgun>(true).currentAmmo = PlayerPrefs.GetInt("currentShotgunAmmo");

        inventory.endpassword1 = PlayerPrefs.GetInt("endpassword1");
        inventory.endpassword2 = PlayerPrefs.GetInt("endpassword2");
        inventory.endpassword3 = PlayerPrefs.GetInt("endpassword3");
        inventory.endpassword4 = PlayerPrefs.GetInt("endpassword4");

        GameManager.Instance.islighton = PlayerPrefs.GetInt("islighton", 0) == 1;

        inventory.syringe = PlayerPrefs.GetInt("syringe", 0);

        UIManager.Instance.hasPasswordHints = PlayerPrefs.GetInt("hasHint", 0) == 1 ? true : false;
        UIManager.Instance.SetPasswordHint();

        /*
         * 
         * *********************** 추가한 코드 *****************************
         * */
        UIManager.Instance.ResetPicturePossessionUI();
        //여기까지

        if (inventory.picture1 == 1) 
            UIManager.Instance.OnGetPicture.Invoke(1);
        if (inventory.picture2 == 1)
            UIManager.Instance.OnGetPicture.Invoke(2);
        if (inventory.picture3 == 1)
            UIManager.Instance.OnGetPicture.Invoke(3);
        if (inventory.picture4 == 1)
            UIManager.Instance.OnGetPicture.Invoke(4);

        if (PlayerPrefs.GetInt("ENDING_ITEM", 0) == 1)
            GameManager.Instance.checkedPasswords.Add(GameManager.PasswordType.ENDING_ITEM);

        if (PlayerPrefs.GetInt("PICTURE", 0) == 1)
            GameManager.Instance.checkedPasswords.Add(GameManager.PasswordType.PICTURE);

        MapManager.Instance.currentFloor = PlayerPrefs.GetInt("floor", 1);

        GameManager.Instance.HasPlayerReadPuzzleInfo = PlayerPrefs.GetInt("hasPlayerReadPuzzleInfo", 0) == 1 ? true : false;

        GameManager.Instance.islighton = PlayerPrefs.GetInt("islighton", 0) == 1 ? true : false;
        if (GameManager.Instance.islighton)
        {
            FindObjectOfType<LightPowerEvent>()?.Init();
        }
    }
}
