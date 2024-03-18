using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : Item
{

    [SerializeField][TextArea(15,20)] string text;
    [SerializeField] GameManager.PasswordType passwordType;
    private Inventory inventory;

    [SerializeField] bool isPuzzleGuide = false;
    [SerializeField] private bool isPasswordHint = false;

    private void Start()
    {
        inventory = GameManager.Instance.player.GetComponent<Inventory>();
    }
    protected override void Get()
    {
        SoundManager.Instance.PlaySFX(equipSound);
        GameManager.Instance.player.CanControl = false; //조작 비활성화
        UIManager.Instance.setLetterContent(text);
        UIManager.Instance.SetPannel(UIManager.UIState.LETTER);

        if(passwordType != GameManager.PasswordType.NONE)
        {
            GameManager.Instance.checkedPasswords.Add(passwordType);
            UIManager.Instance.SetConversation((int)passwordType + 5, 0);
        }

        if (gameObject.layer == LayerMask.NameToLayer("EndPassword1"))
        {
            inventory.endpassword1=1;
        }
        else if(gameObject.layer == LayerMask.NameToLayer("EndPassword2"))
        {
            inventory.endpassword2 = 1;
        }
        else if(gameObject.layer == LayerMask.NameToLayer("EndPassword3"))
        {
            inventory.endpassword3 = 1;
        }
        else if(gameObject.layer == LayerMask.NameToLayer("EndPassword4"))
        {
            inventory.endpassword4 = 1;
        }

        if (isPuzzleGuide)
            GameManager.Instance.HasPlayerReadPuzzleInfo = true;

        if (isPasswordHint)
        {
            UIManager.Instance.hasPasswordHints = true;
            UIManager.Instance.SetPasswordHint();
            UIManager.Instance.informMinimapUpdate += () => UIManager.Instance.SetConversation("Minimap has Updated");
        }
    }
}
