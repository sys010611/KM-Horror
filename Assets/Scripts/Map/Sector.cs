using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Sector : MonoBehaviour
{
    public string sceneName;
    public int sectorIndex;
    public Room[] rooms;
    public NavMeshSurface navMeshSurface;
    public GameObject modiferVolume;
    public AudioClip sectorBgm;

    public string[] neighborSectorSceneName; // 인접한 섹터들의 씬 이름 정보 저장. 비동기 씬 로딩 때 사용.
    [SerializeField]
    private GameObject[] objects;
    [SerializeField]
    private Enemy[] enemies;


    public bool isLoaded;
    public bool isLoading;

    public UnityEvent onNextSectorLoaded = new UnityEvent();

    private void Start()
    {
        MapManager.Instance.sectors.Add(this);
        onNextSectorLoaded.AddListener(GameManager.Instance.OpenDoorAction);
    }

    private void OnDestroy()
    {
        if (MapManager.Instance != null)
        {
            MapManager.Instance.sectors.Remove(this);
        }

    }

    public IEnumerator ActiveObjectCoroutine()
    {
        if (isLoading)
        {
            yield break;
        }

        isLoading = true;

        for (int i = 0; i < objects.Length; i++)
        {
            if ((i % 10).Equals(0))
            {
                yield return null;
            }
            objects[i].SetActive(true); 
        }

        if (enemies != null)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].enabled = false;
            }
        }


        isLoaded = true;
        isLoading = false;
        onNextSectorLoaded.Invoke();
    }

    public void EnableEnemies()
    {
        if (enemies != null)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].enabled = true;
            }
        }
    }
}
