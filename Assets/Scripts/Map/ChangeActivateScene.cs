using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeActivateScene : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var scene = SceneManager.GetSceneByName("PlayScene_1F_Stair");
            for (int i = 0; i < MapManager.Instance.donDestroyObj.Count; i++)
                if (MapManager.Instance.donDestroyObj[i].scene != scene && MapManager.Instance.donDestroyObj[i].transform.parent == null)
                    SceneManager.MoveGameObjectToScene(MapManager.Instance.donDestroyObj[i], scene);
            SceneManager.SetActiveScene(scene);
        }
    }
}
