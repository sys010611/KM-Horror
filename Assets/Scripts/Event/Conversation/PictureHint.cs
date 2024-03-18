using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureHint : ConversationEvent
{
    public PicturePuzzle.PICTURE pictureType;

    private void Start()
    {
        id = 1;
    }

    protected override void Event()
    {
        UIManager.Instance.SetConversation(id, 0);
    }
}
