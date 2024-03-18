using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;

    public List<GameObject> donDestroyObj; // 씬 이동을할 때 같이 옮겨줄 오브젝트들
    public int currentFloor = 1;

    [SerializeField]
    public List<Sector> sectors = new List<Sector>();

    public Room playerCurrentRoom;
    [SerializeField] private Sector _playerCurrentSector;
    public Sector PlayerCurrentSector { get => _playerCurrentSector;
        set 
        {
            if (_playerCurrentSector == null)
            {
                // 처음 씬 진입이므로 초기 작업 진행
                Scene scene;

                if (!SaveManager.Instance.isLoad)
                {
                    scene = SceneManager.GetSceneByName("PlayScene_1F_WayOut");
                }
                else
                {
                    scene = SceneManager.GetSceneByName(PlayerPrefs.GetString("playerLastScene"));
                }

                for (int i = 0; i < donDestroyObj.Count; i++)
                {
                    SceneManager.MoveGameObjectToScene(donDestroyObj[i], scene);
                }

                StartCoroutine(LoadCoroutine(null, new string[] { "LoadingScene" }));
                Application.backgroundLoadingPriority = ThreadPriority.Low;
                SoundManager.Instance.PlaySubBGM(value.sectorBgm, true);
            }
            else if (_playerCurrentSector != value)
            {
                // 적들 인공지능 활성화
                value.EnableEnemies();

                // 섹터가 바뀐 것이므로 맵 로드
                List<string> toActiveScene = new List<string>(value.neighborSectorSceneName.ToList());
                toActiveScene.Add(value.sceneName);
                // BGM 설정
                if (value.sectorBgm != null)
                    if (value.sectorBgm != _playerCurrentSector.sectorBgm)
                        SoundManager.Instance.PlaySubBGM(value.sectorBgm, true);
                CheckPlayerCreatureInSameSector(value);

                // 크리처 파괴할지 안할지 결정
                for (int i = 0; i < GameManager.Instance.activatedCreatureList.Count; i++)
                {
                    if (toActiveScene.Contains(GameManager.Instance.activatedCreatureList[i].currentRoom.sector.sceneName))
                    {
                        if (!donDestroyObj.Contains(GameManager.Instance.activatedCreatureList[i].gameObject))
                        {
                            donDestroyObj.Add(GameManager.Instance.activatedCreatureList[i].gameObject);
                        }
                    }
                    else
                    {
                        if (donDestroyObj.Contains(GameManager.Instance.activatedCreatureList[i].gameObject))
                        {
                            donDestroyObj.Remove(GameManager.Instance.activatedCreatureList[i].gameObject);
                        }
                    }
                }

                GameManager.Instance.activatedZombieList.RemoveAll(x => x.currentRoom.sector != value);

                var unloadScenes = _playerCurrentSector.neighborSectorSceneName.Except(toActiveScene).ToArray();

                List <string> toDeActiveScene = new List<string>(_playerCurrentSector.neighborSectorSceneName.ToList());
                toDeActiveScene.Add(_playerCurrentSector.sceneName);

                var loadScenes = value.neighborSectorSceneName.Except(toDeActiveScene).ToArray();

                StartCoroutine(LoadCoroutine(loadScenes, unloadScenes, value.sceneName));
            }
            _playerCurrentSector = value;
        }
    }

    public static MapManager Instance 
    { 
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<MapManager>();

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    public void BuildNavPath(Sector sector, Vector3 pos)
    {
        sector.modiferVolume.transform.position = pos;
        sector.navMeshSurface.BuildNavMesh();
    }

    IEnumerator LoadCoroutine(string[] loadSceneNames, string[] unloadSceneNames, string sceneName = null)
    {
        if (sceneName != null)
        {
            var nextScene = SceneManager.GetSceneByName(sceneName);
            for (int i = 0; i < donDestroyObj.Count; i++)
                if (donDestroyObj[i].scene != nextScene && donDestroyObj[i].transform.parent == null)
                    SceneManager.MoveGameObjectToScene(donDestroyObj[i], nextScene);
            SceneManager.SetActiveScene(nextScene);
        }

        // UnLoad
        if (unloadSceneNames != null)
        {
            for (int i = 0; i < unloadSceneNames.Length; i++)
            {
                var scene = SceneManager.GetSceneByName(unloadSceneNames[i]);
                if (scene.isLoaded)
                {
                    var op = SceneManager.UnloadSceneAsync(unloadSceneNames[i], UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

                    while (!op.isDone)
                    {
                        yield return new WaitForFixedUpdate();
                    }
                }
            }
        }

        // Load
        if (loadSceneNames != null)
        {
            for (int i = 0; i < loadSceneNames.Length; i++)
            {
                var scene = SceneManager.GetSceneByName(loadSceneNames[i]);
                if (!scene.isLoaded)
                {
                    var op = SceneManager.LoadSceneAsync(loadSceneNames[i], LoadSceneMode.Additive);
                    while (!op.isDone)
                    {
                        yield return new WaitForFixedUpdate();
                    }
                }
            }
        }
    }

    public IEnumerator LoadSceneCoroutine(string[] loadSceneNames, string[] unloadSceneNames, Vector3 teleportPos)
    {
        Application.backgroundLoadingPriority = ThreadPriority.High;
        GameManager.Instance.player.CanControl = false;

        SceneManager.LoadScene(loadSceneNames[0], LoadSceneMode.Additive);
        var scene = SceneManager.GetSceneByName(loadSceneNames[0]);

        while (!scene.isLoaded)
        {
            yield return new WaitForFixedUpdate();
        }

        Sector currentSector = null;
        if (teleportPos != Vector3.zero)
        {
            var nextScene = SceneManager.GetSceneByName(loadSceneNames[0]);
            for (int i = 0; i < donDestroyObj.Count; i++)
                if (donDestroyObj[i].scene != nextScene && donDestroyObj[i].transform.parent == null)
                    SceneManager.MoveGameObjectToScene(donDestroyObj[i], nextScene);
            SceneManager.SetActiveScene(nextScene);

            var sectors = FindObjectsOfType<Sector>();
            for (int i = 0; i < sectors.Length; i++)
            {
                if (sectors[i].sceneName == loadSceneNames[0])
                {
                    currentSector = sectors[i];
                    _playerCurrentSector = currentSector;
                    yield return currentSector.StartCoroutine("ActiveObjectCoroutine");
                    break;
                }
            }
        }

        for (int i = 0; i < unloadSceneNames.Length; i++)
        {
            if (SceneManager.GetSceneByName(unloadSceneNames[i]).isLoaded)
                SceneManager.UnloadSceneAsync(unloadSceneNames[i], UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        }

        for (int i = 1; i < loadSceneNames.Length; i++)
        {
            if (!SceneManager.GetSceneByName(loadSceneNames[i]).isLoaded)
                SceneManager.LoadScene(loadSceneNames[i], LoadSceneMode.Additive);
        }

        if (teleportPos != Vector3.zero)
        {
            while (!currentSector.isLoaded)
            {
                yield return new WaitForFixedUpdate();
            }

            yield return new WaitForSeconds(1);

            GameManager.Instance.player.controller.Move(Vector3.zero);
            GameManager.Instance.player.transform.position = teleportPos;
        }

        GameManager.Instance.player.CanControl = true;
        Application.backgroundLoadingPriority = ThreadPriority.Low;

        UIManager.Instance.SetPannel(UIManager.UIState.NONE);
    }

    public void CheckPlayerCreatureInSameSector(Sector sector)
    {
        if (!GameManager.Instance.player.isHide && GameManager.Instance.activatedCreatureList.Find(x => x.currentRoom.sector == sector))
        {
            EventManager.Instance.PlayerCreatureInSameSectorEvent();
        }
        else
        {
            SoundManager.Instance.StopMainBgm();
        }
    }

    public void CheckPlayerCreatureInSameRoom(Creature creature = null)
    {
        if (creature != null)
        {
            if (creature.currentRoom == playerCurrentRoom && !GameManager.Instance.player.isHide)
            {
                EventManager.Instance.PlayerCreatureInSameRoomEvent();
            }
        }
        else
        {
            if (!GameManager.Instance.player.isHide && GameManager.Instance.activatedCreatureList.Find(x => x.currentRoom == playerCurrentRoom))
            {
                EventManager.Instance.PlayerCreatureInSameRoomEvent();
            }
        }
    }
}
