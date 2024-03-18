using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    IEnumerator LoadCoroutine()
    {
        if (SaveManager.Instance.isLoad)
        {
            var temp = SceneManager.LoadSceneAsync(PlayerPrefs.GetString("playerLastScene"), LoadSceneMode.Additive);

            while (!temp.isDone)
            {
                yield return null;
            }

            var sector = FindObjectOfType<Sector>();

            yield return sector.StartCoroutine("ActiveObjectCoroutine");
            sector.EnableEnemies();

            AsyncOperation operation = null;

            for (int i = 0; i < PlayerPrefs.GetInt("neighborScenesLength"); i++)
            {
                operation = SceneManager.LoadSceneAsync(PlayerPrefs.GetString($"neighborScene{i}"), LoadSceneMode.Additive);
            }

            while (!operation.isDone)
            {
                yield return null;
            }

            SaveManager.Instance.Load(GameManager.Instance.inventory, GameManager.Instance.player.weaponManager);

            for (int i = 0; i < MapManager.Instance.donDestroyObj.Count; i++)
            {
                MapManager.Instance.donDestroyObj[i].SetActive(true);
            }

            yield break;
        }


        Application.backgroundLoadingPriority = ThreadPriority.High;

        var op = SceneManager.LoadSceneAsync("PlayScene_1F_WayOut", LoadSceneMode.Additive);

        while (!op.isDone)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("PlayScene_1F_WayOut"));
        yield return FindObjectOfType<Sector>().StartCoroutine("ActiveObjectCoroutine");

        op = SceneManager.LoadSceneAsync("PlayScene_1F_Room V1", LoadSceneMode.Additive);

        while (!op.isDone)
        {
            yield return null;
        }

        op = SceneManager.LoadSceneAsync("PlayScene_1F_Room V1 (2)", LoadSceneMode.Additive);

        while (!op.isDone)
        {
            yield return null;
        }

        for (int i = 0; i < MapManager.Instance.donDestroyObj.Count; i++)
        {
            MapManager.Instance.donDestroyObj[i].SetActive(true);
        }
    }

    private void Start()
    {
        StartCoroutine(LoadCoroutine());
    }
}
