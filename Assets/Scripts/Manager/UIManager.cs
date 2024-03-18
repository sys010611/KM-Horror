using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    public enum UIState // 현재 열려있는 UI창 상태 필요에 따라 추가
    {
        NONE = 0,
        PUZZLE_MEMMORY,
        PUZZLE_COLOR,
        PUZZLE_LIGHT,
        PUZZLE_PASSWORD,
        PUZZLE_PASSWORD_DUMMY,
        LETTER,
        COMPUTER_MAIL,
        Boold,
        SETTING,
        HOTBAR,
        LOADING,
        MINIMAP
    }

    private static UIManager _instance;
    private UIState currentUIState = UIState.NONE;



    // UI패널창 오브젝트들. 특정 UI를 실행시키고 싶으면 해당 패널을 활성화 하면 됨.
    [SerializeField]
    private GameObject memoryPuzzlePanel;
    [SerializeField]
    private GameObject colorPuzzlePanel;
    [SerializeField]
    private GameObject lightPuzzlePanel;
    [SerializeField]
    private GameObject passwordPuzzlePanel;
    [SerializeField]
    private GameObject passwordPuzzlePanel_Dummy;
    [SerializeField]
    private GameObject letterPanel;
    [SerializeField]
    private GameObject computerMailPanel;
    [SerializeField]
    private GameObject settingPanel;
    [SerializeField]
    private GameObject BloodEffect;
    [SerializeField]
    private GameObject InteractionUIPanel;
    [SerializeField]
    private GameObject messagePanel;
    [SerializeField]
    private GameObject mainSceneUIPanel;
    [SerializeField]
    private GameObject hotBarPanel;
    public HotBarUI hotBar;
    private Coroutine hotBarDisableCoroutine;
    [SerializeField]
    private GameObject tooltipPanel;
    [SerializeField]
    private GameObject loadingPanel;
    [SerializeField]
    private GameObject minimapPanel;
    [SerializeField]
    private GameObject[] minimaps;
    [SerializeField]
    private GameObject[] minimapPictureIconMask;
    [SerializeField]
    private GameObject[] passwordHints;
    [SerializeField]
    private Image player;
    [SerializeField]
    private TextMeshProUGUI conversationText;

    // 현재 열려있는 패널창. UIState변경시마다 패널도 할당.
    private GameObject activatedPanel;

    public Puzzle currentPuzzle;

    [SerializeField]
    private Image[] itemImages;

    public Slider staminaSlider;
    public Slider memoryPuzzleSlider;

    [HideInInspector] public TextMeshProUGUI InteractionText;
    [HideInInspector] public TextMeshProUGUI messageText;

    [SerializeField] private Image fadeImage;
    public Image Bloodimage;

    public UnityEvent<int> OnGetPicture;


    /*
 * 
 * *********************** 추가한 코드 *****************************
 * */
    public void ResetPicturePossessionUI() { hotBar.ResetPicturePossession(); }
//여기까지

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<UIManager>();

            return _instance;
        }
    }

    public void ClearPicutrMaskIcon()
    {
        for (int i = 0; i < minimapPictureIconMask.Length; i++)
        {
            minimapPictureIconMask[i].SetActive(false);
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        _instance = this;


        InteractionText = InteractionUIPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        messageText = messagePanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        SceneManager.activeSceneChanged += DisableTooltipOnTitle;
        OnGetPicture.AddListener(DisablePictureIconOnMap);
        OnGetPicture.AddListener(hotBar.SetPictureText);

        // 대화문 초기화
        var csvFile = Resources.Load<TextAsset>("Conversation");
        string csvText = csvFile.text.Substring(0, csvFile.text.Length - 1);
        string[] rows = csvText.Split(new char[] { '\n' });

        for (int i = 1; i < rows.Length; i++)
        {
            // A, B, C열을 쪼개서 배열에 담음 (CSV파일은 ,로 데이터를 구분하기 때문에 ,를 기준으로 짜름)
            string[] rowValues = rows[i].Split(new char[] { ',' });


            // 유효한 이벤트 이름이 나올때까지 반복
            if (rowValues[0].Trim() == "" || rowValues[0].Trim() == "end")
                continue;

            int id = int.Parse(rowValues[0]);

            List<string> contextList = new List<string>();

            while (rowValues[0].Trim() != "end")
            {
                contextList.Clear();

                do
                {
                    contextList.Add(rowValues[2].ToString());
                    if (++i < rows.Length) rowValues = rows[i].Split(new char[] { ',' });
                    else break;
                } while (rowValues[1] == "" && rowValues[0] != "end");
            } // while문 끝나는 중괄호

            converstationDic.Add(id, contextList.ToArray());
        }
    }

    private void Update()
    {
        KeyEvent();
        UpdatePlayeerMark();
        
    }

    private void KeyEvent()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentUIState.Equals(UIState.NONE))
                SetPannel(UIState.SETTING);
            else if (currentUIState.Equals(UIState.HOTBAR))
            {
                TurnOffPanel();
                SetPannel(UIState.SETTING);
            }
            else
                TurnOffPanel();
        }
        if (SceneManager.GetActiveScene().name != "MainScene")
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (currentUIState.Equals(UIState.NONE))
                    SetPannel(UIState.HOTBAR);
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                SetPannel(UIState.MINIMAP);
            }
        }
    }

    private void UpdatePlayeerMark()
    {
        if (!currentUIState.Equals(UIState.MINIMAP))
        {
            return;
        }

        // 위치 업데이트
        // pivot 설정
        Vector3 pivot = Vector3.zero;
        Vector3 pivotWorld = Vector3.zero;

        switch (MapManager.Instance.PlayerCurrentSector.sectorIndex)
        {
            case 1:
                pivot = new Vector3(-365, 315, 0);
                pivotWorld = new Vector3(-74.5f, 0, 72);
                break;
            case 2:
                pivot = new Vector3(-25, 315, 0);
                pivotWorld = new Vector3(-74.5f, 0, 72);
                break;
            case 3:
                pivot = new Vector3(315, 315, 0);
                pivotWorld = new Vector3(-74.5f, 0, 72);
                break;
            case 4:
                pivot = new Vector3(-365, -25, 0);
                pivotWorld = new Vector3(-74.5f, 0, 72);
                break;
            case 5:
                pivot = new Vector3(-25, -25, 0);
                pivotWorld = new Vector3(-74.5f, 0, 72);
                break;
            case 6:
                if (MapManager.Instance.currentFloor == 1)
                {
                    pivot = new Vector3(315, -25, 0);
                    pivotWorld = new Vector3(-74.5f, 0, 72);
                }
                else
                {
                    pivot = new Vector3(175, -175, 0);
                    pivotWorld = new Vector3(-74.5f, 0, 72);
                }
                break;
            case 7:
                pivot = new Vector3(-365, -365, 0);
                pivotWorld = new Vector3(-74.5f, 0, 72);
                break;
            case 8:
                pivot = new Vector3(-25, -365, 0);
                pivotWorld = new Vector3(-74.5f, 0, 72);
                break;
            case 9:
                pivot = new Vector3(315, -365, 0);
                pivotWorld = new Vector3(-74.5f, 0, 72);
                break;
            case 10:
                pivot = new Vector3(-175, 175, 0);
                pivotWorld = new Vector3(-74.5f, 0, 72);
                break;
            case 11:
                pivot = new Vector3(175, 175, 0);
                pivotWorld = new Vector3(-74.5f, 0, 72);
                break;
            case 12:
                pivot = new Vector3(-175, -175, 0);
                pivotWorld = new Vector3(-74.5f, 0, 72);
                break;
            default:
                break;
        }

        Vector3 playerPos = GameManager.Instance.player.transform.position;
        player.rectTransform.anchoredPosition = pivot;

        // 회전 업데이트
        player.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, -GameManager.Instance.player.transform.rotation.eulerAngles.y));
    }

    public event Action informMinimapUpdate;
    public bool hasPasswordHints = false;
    public void SetPasswordHint()
    {
        Array.ForEach(passwordHints, x => x.SetActive(hasPasswordHints));
    }

    private void TurnOffPanel()
    {
        activatedPanel?.SetActive(false);
        if (GameManager.Instance != null) 
            if (!GameManager.Instance.player.isHide)
                GameManager.Instance.player.CanControl = true;
        currentUIState = UIState.NONE;
        Cursor.lockState = CursorLockMode.Locked; //마우스 포인트 제거 
        Cursor.visible = false;

        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        informMinimapUpdate?.Invoke();
        informMinimapUpdate = null;
    }

    public void SetPannel(UIState state)
    {
        if (!state.Equals(UIState.NONE) && currentUIState.Equals(UIState.HOTBAR))
        {
            TurnOffPanel();
            return;
        }
        else if (state.Equals(currentUIState))
        {
            TurnOffPanel();
            return;
        }
        else if (!state.Equals(UIState.NONE) && !currentUIState.Equals(UIState.NONE))
        {
            return;
        }
        else if (SceneManager.GetActiveScene().name == "MainScene" && state.Equals(UIState.HOTBAR))
        {
            return;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (GameManager.Instance != null) GameManager.Instance.player.CanControl = false;

        switch (state)
        {
            case UIState.NONE:
                TurnOffPanel();
                break;
            case UIState.PUZZLE_MEMMORY:
                memoryPuzzlePanel.SetActive(true);
                activatedPanel = memoryPuzzlePanel;
                memoryPuzzleSlider.value = 0;
                currentUIState = UIState.PUZZLE_MEMMORY;
                break;
            case UIState.PUZZLE_COLOR:
                colorPuzzlePanel.SetActive(true);
                activatedPanel = colorPuzzlePanel;
                currentUIState = UIState.PUZZLE_COLOR;
                break;
            case UIState.PUZZLE_LIGHT:
                lightPuzzlePanel.SetActive(true);
                activatedPanel = lightPuzzlePanel;
                currentUIState = UIState.PUZZLE_LIGHT;
                break;
            case UIState.PUZZLE_PASSWORD:
                passwordPuzzlePanel.SetActive(true);
                activatedPanel = passwordPuzzlePanel;
                currentUIState = UIState.PUZZLE_PASSWORD;
                break;
            case UIState.PUZZLE_PASSWORD_DUMMY:
                passwordPuzzlePanel_Dummy.SetActive(true);
                activatedPanel = passwordPuzzlePanel_Dummy;
                currentUIState = UIState.PUZZLE_PASSWORD_DUMMY;
                break;
            case UIState.LETTER:
                letterPanel.SetActive(true);
                activatedPanel = letterPanel;
                currentUIState = UIState.LETTER;
                break;
            case UIState.COMPUTER_MAIL:
                computerMailPanel.SetActive(true);
                activatedPanel = computerMailPanel;
                currentUIState = UIState.COMPUTER_MAIL;
                break;
            case UIState.SETTING:
                settingPanel.SetActive(true);
                activatedPanel = settingPanel;
                currentUIState = UIState.SETTING;
                break;
            case UIState.HOTBAR:
                hotBarPanel.SetActive(true);
                hotBar.ActiveUI();
                if (hotBarDisableCoroutine != null) StopCoroutine(hotBarDisableCoroutine);
                hotBarDisableCoroutine = StartCoroutine(HotBarDisableCoroutine());
                activatedPanel = hotBarPanel;
                currentUIState = UIState.HOTBAR;
                GameManager.Instance.player.CanControl = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            case UIState.LOADING:
                loadingPanel.SetActive(true);
                activatedPanel = loadingPanel;
                currentUIState = UIState.LOADING;
                break;
            case UIState.MINIMAP:
                Cursor.lockState = CursorLockMode.Locked; //마우스 포인트 제거 
                Cursor.visible = false;
                GameManager.Instance.player.CanControl = true;
                minimapPanel.SetActive(true);
                activatedPanel = minimapPanel;
                minimaps[0].SetActive(MapManager.Instance.currentFloor == 1);
                minimaps[1].SetActive(MapManager.Instance.currentFloor == 2);
                currentUIState = UIState.MINIMAP;
                break;
            default:
                break;
        }
    }

    #region Setting
    [HideInInspector] public float tempMouseSensitivity = 0.5f;

    public void DeactivateSettingUI()
    {
        if(currentUIState.Equals(UIState.SETTING))
        {
            SetPannel(UIState.NONE);
        }
    }

    public void setTempMouseSensitivity(float value)
    {
        tempMouseSensitivity = value;
    }
    #endregion

    #region Interaction
    public void ActivateInteractionUI()
    {
        ++interActionCount;
        InteractionUIPanel.SetActive(true);
    }

    public int interActionCount;
    public void DeactivateInteractionUI()
    {
        interActionCount = Mathf.Max(0, interActionCount - 1);
        if (interActionCount == 0) InteractionUIPanel.SetActive(false);
    }

    public void SetInteractionText(string str)
    {
        InteractionText.text = str;
    }
    #endregion

    #region MemoryPuzzle
    public void PressMemmoryPuzzleButton(int num)
    {
        MemoryPuzzle memoryPuzzle = (MemoryPuzzle)currentPuzzle;
        memoryPuzzleSlider.value = memoryPuzzle.CheckAnswer(num);
    }
    #endregion

    #region PasswordPuzzle
    public void PressPasswordPuzzleButton(int num)
    {
        PasswordPuzzle puzzle = (PasswordPuzzle)currentPuzzle;
        puzzle.CheckAnswer(num);
    }
    #endregion

    #region LightPuzzle

    [HideInInspector] public LightPuzzle currLightPuzzle;
    public void button1()
    {
        currLightPuzzle.button1Pressed();
    }
    public void button2()
    {
        currLightPuzzle.button2Pressed();
    }
    public void button3()
    {
        currLightPuzzle.button3Pressed();
    }
    public void button4()
    {
        currLightPuzzle.button4Pressed();
    }

    #endregion

    #region ComputerMail

    public void setComputerMailContent(string content)
    {
        var text = computerMailPanel.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "";
        text.DOText(content, 2);
    }

    #endregion

    #region Letter
    public void setLetterContent(string text)
    {
        letterPanel.transform.GetChild(0).GetComponent<Text>().text = text;
    }
    #endregion

    #region ToolTip
    Sequence tooltipSequence;
    public void SetToolTip(string content)
    {
        if (tooltipSequence != null)
        {
            tooltipSequence.Kill();
        }

        var text = tooltipPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        tooltipPanel.SetActive(true);
        text.text = "";
        tooltipPanel.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.784f);
        text.color = Color.white;


        tooltipSequence = DOTween.Sequence()
                .Append(text.DOText(content, 1f))
                .AppendInterval(5)
                .Append(text.DOFade(0, 2))
                .Join(tooltipPanel.GetComponent<Image>().DOFade(0, 2))
                .OnComplete(() => tooltipPanel.SetActive(false));
    }

    private void DisableTooltipOnTitle(Scene scene1, Scene scene2)
    {
        if (scene2.name == "MainScene")
        {
            try
            {
                tooltipPanel?.SetActive(false);
            }
            catch(MissingReferenceException e)
            {
                return;
            }
        }
    }
    #endregion

    #region Conversation
    Sequence conversationSequence;
    private Dictionary<int, string[]> converstationDic = new Dictionary<int, string[]>();

    public void SetConversation(int id, int idx)
    {
        if (conversationSequence != null)
        {
            conversationSequence.Kill();
        }

        conversationText.text = "";
        conversationText.color = Color.white;

        conversationSequence = DOTween.Sequence()
                .Append(conversationText.DOText(converstationDic[id][idx], 0.75f))
                .AppendInterval(5)
                .Append(conversationText.DOFade(0, 2));
    }

    public void SetConversation(string text)
    {
        if (conversationSequence != null)
        {
            conversationSequence.Kill();
        }

        conversationText.text = "";
        conversationText.color = Color.white;

        conversationSequence = DOTween.Sequence()
                .Append(conversationText.DOText(text, 0.3f))
                .AppendInterval(5)
                .Append(conversationText.DOFade(0, 2));
    }

    #endregion

    #region Map

    public void DisablePictureIconOnMap(int pictureNum)
    {
        minimapPictureIconMask[pictureNum-1].SetActive(true);
    }

    #endregion

    public void FadeImage(float amount = 1)
    {   
        fadeImage.DOFade(amount, 2f);
    }

    IEnumerator HotBarDisableCoroutine()
    {
        yield return new WaitForSeconds(3);
        hotBar.FadeUI();
        yield return new WaitForSeconds(1.5f);
        TurnOffPanel();
    }
}
