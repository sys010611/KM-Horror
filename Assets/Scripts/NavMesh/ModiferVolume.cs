using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ModiferVolume : MonoBehaviour
{
    [SerializeField]
    private NavMeshSurface navMeshSurface;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            transform.position = new Vector3(0, 50, 0);
            navMeshSurface.BuildNavMesh();
        }
    }
}
