using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPassword : MonoBehaviour
{
    private Inventory inventory;
    [SerializeField] private string[] password;
    [SerializeField] private string result = "????";

    public string Result { get { return result; } }

    private void Awake()    
    {
        inventory = GameManager.Instance.inventory;
        //CurrentPassword();
    }

    //private void Update()
    //{
    //    CurrentPassword();
    //}

    public void CurrentPassword()
    {
        if (inventory == null)
            inventory = GameManager.Instance.inventory;

        result = "";
        if (inventory.endpassword1 == 1)
            result += password[0];
        else
            result += "?";

        if (inventory.endpassword2 == 1)
            result += password[1];
        else
            result += "?";

        if (inventory.endpassword3 == 1)
            result += password[2];
        else
            result += "?";

        if(inventory.endpassword4 == 1)
            result += password[3];
        else
            result += "?";
        UIManager.Instance.hotBar.SetoutpasswordText(result);  //결과값 리턴
    }
}
