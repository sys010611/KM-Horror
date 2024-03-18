using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerMail : Item
{
    [SerializeField] int TotalPage;

    [SerializeField] [TextArea(15,20)] string texts;
    [SerializeField] GameManager.PasswordType passwordType;

    int currentPage = 0;

    [SerializeField] private bool isPasswordHint;

    protected override void Get()
    {
        SoundManager.Instance.PlaySFX(equipSound);
        GameManager.Instance.player.CanControl = false; //조작 비활성화
        UIManager.Instance.setComputerMailContent(texts);
        UIManager.Instance.SetPannel(UIManager.UIState.COMPUTER_MAIL);
        GameManager.Instance.player.interActionQueue.Enqueue(Get);

        if(passwordType != GameManager.PasswordType.NONE)
        {
            GameManager.Instance.checkedPasswords.Add(passwordType);
            UIManager.Instance.SetConversation((int)passwordType + 5, 0);
        }

        if (isPasswordHint)
        {
            UIManager.Instance.hasPasswordHints = true;
            UIManager.Instance.SetPasswordHint();
            UIManager.Instance.informMinimapUpdate += () => UIManager.Instance.SetConversation("Minimap has Updated");
        }
    }
}
