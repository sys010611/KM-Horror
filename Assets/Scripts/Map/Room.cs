using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Room : MonoBehaviour
{
    public Sector sector;

    [SerializeField]
    private Room[] nearestRooms;

    public int floor;
    // 씬 오브젝트 활성화를 위한 변수
    public bool isActiveEvent;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.CompareTag("Player"))
        {
            if (!isActiveEvent)
            {
                MapManager.Instance.playerCurrentRoom = this;
                MapManager.Instance.PlayerCurrentSector = sector;
                MapManager.Instance.CheckPlayerCreatureInSameRoom();
            }
            if (!sector.isLoaded)
                sector.StartCoroutine("ActiveObjectCoroutine");
        }
        else if (collider.transform.CompareTag("Creature"))
        {
            var c = collider.GetComponent<Creature>();
                c.currentRoom = this;
            MapManager.Instance.CheckPlayerCreatureInSameRoom(c);
        }
        else if (collider.transform.CompareTag("Mimic"))
        {
            var c = collider.GetComponent<Enemy>();
            c.currentRoom = this;
        }
    }

    public Room[] GetNearestRooms()
    {
        return nearestRooms;
    }
}
