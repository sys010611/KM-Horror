using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePrevention : MonoBehaviour
{
    [SerializeField] private int floor;
    [SerializeField] private float teleportPos;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (MapManager.Instance.currentFloor == floor)
            {
                other.GetComponent<CharacterController>().Move(Vector3.zero);
                Vector3 playerPos = other.transform.position;
                other.transform.position = new Vector3(playerPos.x, teleportPos, playerPos.z);
            }
        }
    }
}
