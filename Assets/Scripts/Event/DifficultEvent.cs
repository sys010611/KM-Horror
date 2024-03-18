using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultEvent : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.difficult = true;
        }
    }
}
