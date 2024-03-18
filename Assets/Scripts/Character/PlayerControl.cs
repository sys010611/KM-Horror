using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Schema;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 5f; //걷기 속도
    [SerializeField] private float runSpeed = 8f; //달리기 속도
    [SerializeField] private float crouchSpeed = 2.5f; //앉았을 때 속도 
    [SerializeField] private float gravity = -20f; //중력
    [SerializeField] private Transform groundCheck; //ray 시작점  
    [SerializeField] private float groundDistance = 0.5f; //ray 길이
    [SerializeField] private LayerMask groundMask; //땅인지 체크
    [SerializeField] private float mouseSensitivity = 180f; //마우스 감도
    [SerializeField] private Transform playerCamera; //카메라 오브젝트
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject weaponCam;
    [SerializeField] private float maxCameraAngle = 70f; //카메라 앵글 제한

    [SerializeField] private float MaxRunRot = 1.3f; //달릴 때 최대 회전 각도 조절
    [SerializeField] private float runRotSpeed = 7f; //달릴 때 회전 스피드 조절
    [SerializeField] private float MaxWalkRot = 0.4f; //걸을 때 회전 스피드 조절
    [SerializeField] private float walkRotSpeed = 2f; //걸을 때 회전 스피드 조절
    public float SettingMouse = 1f; // 마우스 속도 조절
    public CharacterController controller; //캐릭터 콜라이더 
    private Vector3 velocity; 
    private bool isGrounded; //땅인지 체크
    private bool isCrouching; //앉아있는 중인지 체크
    private bool isRunning; //달리는 중인지 체크
    private bool isJumping; //점프하는 중인지 체크
    private bool ismove = false; //움직이는 중인지 체크
    private float cameraAngle = 0f; //카메라 엥글 담는 변수
    private float CamRotZ = 0f;
    private float cameraRotation = 0f; // 카메라의 y축 회전값 초기값
    private float rotationDirection = 1f; // 회전 방향 (1: 왼쪽으로, -1: 오른쪽으로)
    private float rotationSpeed = 2f; //회전 속도

    private bool ishealingstamina = false; //달리기중 걷는 상황 제어 변수
    public bool shakeRotate = false;

    private Vector3 originalPos;
    private Quaternion originalRot;

    //public Image bloodimage;

    private PlayerStat playerStat;

    [SerializeField]
    protected LayerMask excludeLayer;
    protected int excludePlayerLayer;
    public WeaponManager weaponManager;
    public MeleeWeapon weapon1;
    public MeleeWeapon weapon2;
    public MeleeWeapon weapon3;
    public Pistol pistol;

    private bool _canControl = true;
    public bool CanControl { get => _canControl; 
        set
        {
            if (!value)
            {
                StopWalkSound();
                StopRunSound();
            }
            _canControl = value; 
        }
    }
    public bool isHide = false;
    public Vector3 CenterPos() { return controller.bounds.center; }
     
    public Queue<Action> interActionQueue = new Queue<Action>();

    [SerializeField]
    private AudioClip footstepClip;
    [SerializeField]
    private AudioSource walkAudioSource;
    [SerializeField]
    private AudioSource breathAudioSource;

    [SerializeField]
    private AudioClip breathEndSound;
    [SerializeField]
    private AudioClip breathSound;

    private float originSizeY;

    public enum Playerstates 
    {
        None,

        IDLE, //평소
        Walk, //걷기
        Run, //달리기
        Crouch, //앉기
        Jump,
        InterAction,
        Attack,
        Shot,
        END
    }

    [SerializeField] private Playerstates _pState;
    bool isInteraction;
    public Playerstates PState { get => _pState; 
        set 
        {
            if (isInteraction)
            {
                return;
            }


            if (!_pState.Equals(value))
            {
                switch (_pState)
                {
                    case Playerstates.Walk:
                        StopCoroutine(walkSoundCoroutine);
                        StopWalkSound();
                        break;
                    case Playerstates.Run:
                        StopCoroutine(runSoundCoroutine);
                        StopWalkSound();
                        break;
                    case Playerstates.Crouch:
                        StopCoroutine(croughSoundCoroutine);
                        StopWalkSound();
                        break;
                    case Playerstates.Attack:
                        //
                        //
                        break;
                    default:
                        break;

                }

                switch (value)
                {
                    case Playerstates.Walk:
                        walkSoundCoroutine = StartCoroutine(PlayWalkSound());
                        break;
                    case Playerstates.Run:
                        runSoundCoroutine = StartCoroutine(PlayRunSound());
                        break;
                    case Playerstates.InterAction:
                        isInteraction = true;
                        this.Invoke(() => isInteraction = false, 0.1f);
                        break;
                    case Playerstates.Attack:
                        break;

                    case Playerstates.Crouch:
                        croughSoundCoroutine = StartCoroutine(PlayCrouchSound());
                        break;
                    default:
                        break;
                }
            }

            _pState = value; 
        } 
    }

    private Coroutine walkSoundCoroutine;
    public IEnumerator PlayWalkSound()
    {
        walkAudioSource.clip = footstepClip;

        while (true)
        {
            walkAudioSource.volume = UnityEngine.Random.Range(0.3f, 0.45f); //걷는 발자국 소리
            walkAudioSource.pitch = UnityEngine.Random.Range(0.6f, 0.7f);

            if (!walkAudioSource.isPlaying) {
                //
                //
                //("WalkSound");
                walkAudioSource.Play();
            }
            
            yield return new WaitForSeconds(0.65f);
        }
    }

    private Coroutine runSoundCoroutine;
    public IEnumerator PlayRunSound()
    {
        walkAudioSource.clip = footstepClip;

        while (true)
        {
            walkAudioSource.volume = UnityEngine.Random.Range(0.5f, 0.6f); //달리기 발자국 소리
            walkAudioSource.pitch = UnityEngine.Random.Range(0.7f, 0.8f);

            if(!walkAudioSource.isPlaying)
            {
                //Debug.Log("Runsound");
                walkAudioSource.Play();
            }
            
            yield return new WaitForSeconds(0.3f);
        }
    }

    private Coroutine croughSoundCoroutine;
    public IEnumerator PlayCrouchSound()
    {
        walkAudioSource.clip = footstepClip;

        while (true)
        {
            walkAudioSource.volume = UnityEngine.Random.Range(0.25f, 0.35f); //앉기 발자국 소리
            walkAudioSource.pitch = UnityEngine.Random.Range(0.35f, 0.45f); //피치 늘리기

            if (!walkAudioSource.isPlaying)
            {
                walkAudioSource.Play();
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    public void StopWalkSound()
    {
        if (walkSoundCoroutine != null)
        {
            StopCoroutine(walkSoundCoroutine);
        }
    }

    public void StopRunSound()
    {
        if (runSoundCoroutine != null)
        {
            StopCoroutine(runSoundCoroutine);
        }
    }

    public void StopCroughSound()
    {
        if (croughSoundCoroutine != null)
        {
            StopCoroutine(croughSoundCoroutine);
        }
    }

    void Start()
    {
        originalPos = playerCamera.localPosition;
        originalRot = playerCamera.localRotation;
        
        SettingMouse = UIManager.Instance.tempMouseSensitivity; // Ui 설정에 있는 mouse감도 값 가져와 초기값 설정
        controller = GetComponent<CharacterController>();
        playerStat = GetComponent<PlayerStat>();
        Cursor.lockState = CursorLockMode.Locked; //마우스 포인트 제거 
        Cursor.visible = false;

        playerStat.CurrentStamina = playerStat.MaxStamina;  //초기 스테미나 설정 
        playerStat.staminaIncreaseRate = playerStat.healStamina; //초기 스테미나 회복속도 수치 설정 
        playerStat.IncreaseStamina = playerStat.staminaIncreaseRate; //감소 스테미나 수치 설정

        PState = Playerstates.IDLE; //현재 플레이어 상태 
        GameManager.Instance.player = this;
        EnemyState.player = this.transform;

        EventManager.Instance.deadAction += () => { breathAudioSource.Stop(); };

        excludePlayerLayer = ~excludeLayer;
        weaponCam.SetActive(false);
        this.Invoke(() => weaponCam.SetActive(true), 3f);
        originSizeY = transform.localScale.y;
    }

    private void Update()
    {
        //Debug.Log(originalPos);
        InterAction();
        /*
        if (Input.GetButtonDown("Fire2"))
        {
            StartCoroutine(ShakeCamera());
        }
        */
        if (!CanControl)
        {
            return;
        }

        // Move player
        Move();

        // Handle crouch
        HandleCrouch();

        // Handle run
        HandleRun();

        // Handle camera rotation
        HandleRotation();

        //Stamina
        StaminaState();
    }

    private void FixedUpdate()
    {
        // Apply gravity
        ApplyGravity();
        CheckSeeingCreature();
    }

    private void Move() 
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = (transform.forward * v) + (transform.right * h);

        float speed = walkSpeed;
        if (isCrouching)
        {
            speed = crouchSpeed;
        }
        else if (isRunning && playerStat.CurrentStamina > 0f)
        {
            speed = runSpeed;
            playerStat.CurrentStamina -= playerStat.StaminaDesreaseRate * Time.deltaTime;   
        }
        else if(playerStat.CurrentStamina <= 0.5f)
        {
            speed = walkSpeed;
        }

        controller.Move(move * speed * Time.deltaTime);

        if (v >= 0.1f || v <= -0.1f || h >= 0.1f || h <= -0.1f)
        {
            ismove = true;
        }
        else
        {
            ismove = false;
        }


        //플레이어상황 변환
        if (!isCrouching && !isRunning && !isJumping && ismove) //걷기
        {
            PState = Playerstates.Walk;
            rotationSpeed = walkRotSpeed;
            CamRotZ = WarkingCamMovement(MaxWalkRot);

        }
        else if (isRunning && !isCrouching && !isJumping && ismove) //달리기
        {
            PState = Playerstates.Run;
            rotationSpeed = runRotSpeed;
            CamRotZ = WarkingCamMovement(MaxRunRot);
        }
        else if (isCrouching && !isRunning && !isJumping) //앉은 => 조용히 걷기
        {
            if (ismove)
            {
                PState = Playerstates.Crouch;
            }
            else if (!ismove)
            {
                PState = Playerstates.IDLE;
            }
        }
        else if (!ismove && !isCrouching && !isJumping && !isRunning) //기본상태
        {
            PState = Playerstates.IDLE;
            rotationSpeed = 0f;
            CamRotZ = 0f;
        }
        if (weapon1.attacking || weapon2.attacking || weapon3.attacking)
        {
            //Debug.Log("Attack");
            PState = Playerstates.Attack;
        }
        if (pistol.isShot)
        {
            PState = Playerstates.Shot;
        }

    }

    private void HandleCrouch() 
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = true;
            transform.DOScaleY(0.5f, 0.7f);
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouching = false;
            transform.DOScaleY(originSizeY, 0.7f);
        }
    }

    private void HandleRun()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && playerStat.isStaminaMax)
        {
            isRunning = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
        }

    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime * SettingMouse;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime * SettingMouse;

        cameraAngle -= mouseY;
        cameraAngle = Mathf.Clamp(cameraAngle, -maxCameraAngle, maxCameraAngle); //카메라 각도 조절
        playerCamera.localRotation = Quaternion.Euler(cameraAngle, 0f, CamRotZ);

        transform.Rotate(Vector3.up * mouseX);
    }

    private void ApplyGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); //땅인지 체크

        if (isGrounded && velocity.y < 0) //중력 적용 
        {
            velocity.y = -2f; 
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    
    private void StaminaState() //스테미나
    {
        if (isRunning && playerStat.CurrentStamina > 0f)
        {
            playerStat.CurrentStamina -= playerStat.StaminaDesreaseRate * Time.deltaTime; //행동감소
        }
        else
        {
            playerStat.CurrentStamina += playerStat.staminaIncreaseRate * Time.deltaTime; //회복
        }

        if (isRunning)
        {
            playerStat.staminaIncreaseRate = 0f; //달리는 동안 스테니너 회복 불가
        }
        else if (!isRunning)
        {
            playerStat.staminaIncreaseRate = playerStat.IncreaseStamina; //아닐때 회복
        }
        
        if (playerStat.CurrentStamina == 0)
        {
            playerStat.staminaIncreaseRate = playerStat.IncreaseStamina / 1.35f; //완전 소모시 차는 속도 느리게
            playerStat.isStaminaMax = false; //최대수치까지 스테미나 비활성화
            isRunning = false; //현재 진행중인 행동 해제
            isJumping = false; //현재 진행중인 행동 해제

        }
        else if (playerStat.CurrentStamina >= playerStat.MaxStamina / 2) //최대의 절반 수치 시 다시 달리기
        {
            playerStat.IncreaseStamina = playerStat.healStamina; //회복식 초기화
            playerStat.staminaIncreaseRate = playerStat.IncreaseStamina; //증가
            playerStat.isStaminaMax = true;
            ishealingstamina = false;

        }

        playerStat.CurrentStamina = Mathf.Clamp(playerStat.CurrentStamina, 0, playerStat.MaxStamina); //스테미나 계산 


        if (playerStat.isStaminaMax == false) //완전히 회복될때 까지 지친 소리 출력
        {
            //지친 소리
            if (!breathAudioSource.isPlaying)
            {
                breathAudioSource.clip = breathSound;
                breathAudioSource.Play();
            }
        }
        else if (playerStat.CurrentStamina >= 149) //스테미나 수치에 따른 소리 조절
        {
            //최대치
            if (breathAudioSource.clip == breathSound)
            {
                breathAudioSource.DOFade(0, 0.5f)
                    .OnComplete(() => 
                    {
                        breathAudioSource.volume = 0.35f;
                        breathAudioSource.clip = breathEndSound;
                        breathAudioSource.Play();
                    });

            }
        } 
        else if (playerStat.CurrentStamina >= 100 && playerStat.CurrentStamina < 150)
        {
            //조금 힘듬
        }
        else if (playerStat.CurrentStamina >= 50 && playerStat.CurrentStamina < 100)
        {
            // 힘듬
        }
        else if(playerStat.CurrentStamina > 0 && playerStat.CurrentStamina < 50)
        {
            //매우 지침
            if (!breathAudioSource.isPlaying)
            {
                breathAudioSource.clip = breathSound;
                breathAudioSource.Play();
            }
        }
    }

    public void InterAction()
    {
        if (Input.GetKeyDown(KeyCode.F))
            if (interActionQueue.Count != 0)
            {
                interActionQueue.Dequeue().Invoke();
            }
    }

    public float WarkingCamMovement(float max)
    {
        // 회전 방향에 따라 카메라 회전값을 증가 또는 감소시킴
        cameraRotation += rotationDirection * Time.deltaTime * rotationSpeed;

        if (cameraRotation > max)
        {
            cameraRotation = max; // max를 넘어가면 값을 max로 고정
            rotationDirection = -1f; // 회전 방향을 반대로 설정
        }
        else if (cameraRotation < -max)
        {
            cameraRotation = -max; // -max를 넘어가면 값을 -max로 고정
            rotationDirection = 1f; // 회전 방향을 반대로 설정
        }

        return cameraRotation;
    }

    public void Hitfeedback() 
    {
        StartCoroutine(HitBlood(UIManager.Instance.Bloodimage, 0.9f));
        StartCoroutine(ShakeCamera());


        if (UIManager.Instance.Bloodimage.color.a != 0f) {
            Color color = UIManager.Instance.Bloodimage.color;
            color.a = 0;

            UIManager.Instance.Bloodimage.color = color;
        }
    }

    public IEnumerator ShakeCamera(float duration = 0.3f, float magnitudePos = 0.3f, float magnitudeRot = 0.3f) //히트 시 카메라 흔들림
    {
        float passTime = 0.0f;
        Quaternion originalRot = playerCamera.localRotation;
        Vector3 originalEulerAngles = originalRot.eulerAngles;

        while (passTime < duration)
        {
            Vector3 shakePos = UnityEngine.Random.insideUnitSphere * magnitudePos;
            playerCamera.localPosition = originalPos + shakePos;

            if (shakeRotate)
            {
                Vector3 shakeRot = new Vector3(originalEulerAngles.x, originalEulerAngles.y, UnityEngine.Random.Range(-magnitudeRot, magnitudeRot));
                playerCamera.localRotation = Quaternion.Euler(shakeRot);
            }

            passTime += Time.deltaTime;
            yield return null;
        }

        playerCamera.localPosition = originalPos;
        playerCamera.localRotation = originalRot;
    }


    public IEnumerator HitBlood(Image image, float duration) //히트시 피Ui
    {
        image.gameObject.SetActive(true);
        float elapsedTime = 0f;
        Color originalColor = image.color;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(0.85f, 0f, elapsedTime / duration);
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }

    public void playerout()
    {
        controller.Move(MapManager.Instance.playerCurrentRoom.transform.position);
    } 

    public void CheckSeeingCreature()
    {
        for (int i = 0; i < GameManager.Instance.activatedCreatureList.Count; i++)
        {
            Vector3 viewPos = cam.WorldToViewportPoint(GameManager.Instance.activatedCreatureList[i].transform.position);
            if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 && viewPos.z > 0)
            {
                RaycastHit hit;
                Vector3 targetDir = (GameManager.Instance.activatedCreatureList[i].CenterPos - cam.transform.position).normalized;

                if (Physics.Raycast(cam.transform.position, targetDir, out hit, 200, excludePlayerLayer))
                {
                    if (hit.transform.CompareTag("Creature"))
                    {
                        EventManager.Instance.PlayerSeeCreatureEvent();
                        return;
                    }
                }
            }
        }
    }

}