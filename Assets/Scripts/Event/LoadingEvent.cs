using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadingEvent : MonoBehaviour
{
    public enum TriggerType
    {
        FirstFloor,
        SecondFloor,
        Interaction
    }

    [SerializeField] private string[] toLoadScene;
    [SerializeField] private string[] toUnloadScene;
    [SerializeField] private TriggerType triggerType;
    [SerializeField] private int toFloor;
    [SerializeField] private Transform teleportTransform;
    [SerializeField] private string destination;
    private BoxCollider trigger;

    private void Awake()
    {
        trigger = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (triggerType)
            {
                case TriggerType.FirstFloor:
                    if (MapManager.Instance.currentFloor == 2)
                    {
                        UIManager.Instance.SetPannel(UIManager.UIState.LOADING);
                        MapManager.Instance.currentFloor = 1;
                        for (int i = GameManager.Instance.activatedCreatureList.Count - 1; i >= 0; i--)
                            GameManager.Instance.activatedCreatureList[i].gameObject.SetActive(false);
                        MapManager.Instance.StartCoroutine(MapManager.Instance.LoadSceneCoroutine(toLoadScene, toUnloadScene, Vector3.zero));
                    }
                    break;
                case TriggerType.SecondFloor:
                    if (MapManager.Instance.currentFloor == 1)
                    {
                        UIManager.Instance.SetPannel(UIManager.UIState.LOADING);
                        MapManager.Instance.currentFloor = 2;
                        for (int i = GameManager.Instance.activatedCreatureList.Count - 1; i >= 0; i--)
                            GameManager.Instance.activatedCreatureList[i].gameObject.SetActive(false);
                        MapManager.Instance.StartCoroutine(MapManager.Instance.LoadSceneCoroutine(toLoadScene, toUnloadScene, Vector3.zero));
                    }
                    break;
                case TriggerType.Interaction:
                    MapManager.Instance.currentFloor = toFloor;
                    string destinationStr = "\n\nDestination : " + destination;
                    UIManager.Instance.SetToolTip("You can immediately move to another sector." + destinationStr);
                    GameManager.Instance.player.interActionQueue.Enqueue(Teleport);
                    UIManager.Instance.SetInteractionText("Enter");
                    UIManager.Instance.ActivateInteractionUI();
                    break;
                default:
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.player.interActionQueue = new Queue<System.Action>(GameManager.Instance.player.interActionQueue.Where(x => x != Teleport));
            UIManager.Instance.DeactivateInteractionUI();
        }
    }

    private void Teleport()
    {
        trigger.enabled = false; //툴팁 두번 뜨는 버그 수정
        this.Invoke(() => trigger.enabled = true, 10f);

        SoundManager.Instance.StopMainBgm();
        SoundManager.Instance.StopSubBgm();
        UIManager.Instance.DeactivateInteractionUI();
        UIManager.Instance.SetPannel(UIManager.UIState.LOADING);
        for (int i = GameManager.Instance.activatedCreatureList.Count - 1; i >= 0 ; i--)
            GameManager.Instance.activatedCreatureList[i].gameObject.SetActive(false);
        MapManager.Instance.StartCoroutine(MapManager.Instance.LoadSceneCoroutine(toLoadScene, toUnloadScene, teleportTransform.position));
    }
}
