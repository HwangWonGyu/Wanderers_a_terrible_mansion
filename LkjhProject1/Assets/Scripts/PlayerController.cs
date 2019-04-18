using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerController : MonoBehaviour {

	[Header("Character")]
	public float moveSpeed;
	public float walkSpeed;
	public float runSpeed;
	public float sitSpeed;
	public float staminaValue=1f;
	public float staminaRecoveryValue;
	public float staminaConsumptionValue;
	public float staminaItemRecoveryValue;
	public GlobalData.PlayerState playerState;
	public bool soundedState;
	private PlayerJoystick joystick;
	private NavMeshAgent navMeshAgent;
	private Vector3 moveVector;
	private bool soundCoroutineflag;
	private Coroutine soundedCoroutine;
	public bool isDead;
	
    public bool isTrigger;
	public GameObject visibleTarget;

	public GameObject playerCam;
	public Transform runnerGameOverAnimStartPos;
	
    private PlayerAudio playerAudio;
    [SerializeField]
    private float cameraDownDistance;
	[SerializeField]
	private float visibleTargetDownDistance;
	private float reStartAppearDelay = 2.5f;
    private Vector3 visibleTargetInitialpos;
    private Vector3 playerCamInitialpos;
    private Vector3 playerStartPosition;
    
    public event System.Action OnPlayerDead;
	public event System.Action OnPlayerItemEat;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
	public MouseLook mouseLook;
	private bool isCursorOn;
	private Dictionary<KeyCode, System.Action> keyDictionary;
#endif
	/// <summary> 스턴 아이템 소지하고 있는지 아닌지 </summary>
	private bool isStunItemActive;
	/// <summary> 스태미나 회복 아이템 소지하고 있는지 아닌지 </summary>
	private bool isStaminaItemActive;
	private bool isStun;

	void Start()
	{

		isTrigger = true;
		isStun = false;
		isStunItemActive = false;
		isStaminaItemActive = false;
		playerState = GlobalData.PlayerState.Walk;
		navMeshAgent = GetComponent<NavMeshAgent>();
		joystick = GetComponent<PlayerJoystick>();
        playerAudio = GetComponent<PlayerAudio>();
        
        moveSpeed = walkSpeed;
        if(visibleTarget !=null)
            visibleTargetInitialpos = visibleTarget.transform.localPosition;
		if (playerCam != null)
		{
			playerCamInitialpos = playerCam.transform.localPosition;
			playerCam.GetComponent<GrayScaleEffect>().intensity = GlobalData.PLAYER_CAM_DEFAULT;
		}
        playerStartPosition = transform.position;
        runnerGameOverAnimStartPos = GameObject.FindGameObjectWithTag("RunnerGameOver").transform;
		AdsTestWonGyu.GetInstance.ShowresultFinished += PlayerReset;
		
			
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		if (playerCam != null)
			mouseLook.Init(transform, playerCam.transform);
			

		// 2018.11.25 ~ 원규
		// 키 셋팅 제작중
		keyDictionary = new Dictionary<KeyCode, System.Action>
		{
			{ KeyCode.LeftControl, BtnSitCliked	},
			{ KeyCode.LeftShift, BtnRunCliked },
			{ KeyCode.Space, null }
		};
#endif
	}

	private void Update()
	{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			if (isCursorOn == false)
			{
				mouseLook.SetCursorLock(false);
				isCursorOn = true;
			}
			else
			{
				mouseLook.SetCursorLock(true);
				isCursorOn = false;
			}
		}
		
#endif

		if (isDead)
			return;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			BtnRunCliked();
		}
		if (Input.GetKeyDown(KeyCode.LeftControl))
		{
			BtnSitCliked();
		}
		if (Input.GetKeyUp(KeyCode.Space))
		{
			ActiveStunItem();
		}
		if (Input.GetKeyUp(KeyCode.F))
		{
			ActiveStaminaItem();
		}
#endif
		//UpdateplayerState();
		PoolInput();

		#region 게임오버시 카메라워크 테스트
		if (Input.GetKeyDown(KeyCode.P))
		{
			PlayGameOverCamAnimation();
		}
		#endregion
	}
	#region 게임오버시 카메라워크 테스트
	private void PlayGameOverCamAnimation()
	{
        StartCoroutine(RotateCam());
	}

	private IEnumerator RotateCam()
	{
		float curTime = Time.time;
		while(Time.time - curTime < 3.0f)
		{
			playerCam.transform.Rotate(Vector3.forward, 90.0f * Time.deltaTime);
			yield return null;
		}
	}
	#endregion

	private void FixedUpdate()
	{

		if (isDead)
            return;
		
		

		Move();
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		if (playerCam != null)
		{
			mouseLook.LookRotation(transform, playerCam.transform);
		}
#endif

		UIManager.GetInstance.SetStamina(staminaValue);
	}


	void PoolInput()
	{
		if(joystick != null)
			moveVector = joystick.GetMoveVector();
		if (isStun)
			moveVector = Vector3.zero;
	}

	void Move()
	{
		
        if (moveVector.sqrMagnitude > 0.1f)
        {
            playerAudio.SetPlayerFootStepAudiosourceMute(false);
            switch (playerState)
            {
                case GlobalData.PlayerState.Run:
                    soundedState = true;
                    if (soundCoroutineflag)
                    {
                        StopCoroutine(soundedCoroutine);
                        soundCoroutineflag = false;
                    }
                    staminaValue -= staminaConsumptionValue;
                    if (staminaValue <= 0)
                    {
                        staminaValue = 0;   
                        BtnRunCliked();
                    }
                    break;
                case GlobalData.PlayerState.Walk:
                    soundedState = true;
                    if (soundCoroutineflag)
                    {
                        StopCoroutine(soundedCoroutine);
                        soundCoroutineflag = false;
                    }
                    staminaValue += staminaRecoveryValue;
                    if (staminaValue >= 1)
                        staminaValue = 1f;
                    
                    break;
                case GlobalData.PlayerState.Sit:
                    if (soundedState)
                    {
                        if (!soundCoroutineflag)
                        {
                            soundCoroutineflag = true;
                            soundedCoroutine = StartCoroutine("OnSoundedMode");
                        }
                    }
                    staminaValue += staminaRecoveryValue * 3;
                    if (staminaValue >= 1)
                        staminaValue = 1f;
                    
                    break;
            }
            navMeshAgent.Move(moveVector * moveSpeed * Time.deltaTime);
        }		
	    else  // 어떤 상태든 움직이지 않는다면 Stamina회복
		{
			if(playerAudio != null)
				playerAudio.SetPlayerFootStepAudiosourceMute(true);

			switch(playerState)
			{
				case GlobalData.PlayerState.Sit:
					staminaValue += staminaRecoveryValue * 3;
					break;
				default:
					staminaValue += staminaRecoveryValue;
					break;
			}
            if (staminaValue >= 1)
                staminaValue = 1f;

            if (soundedState)
            {
                if (!soundCoroutineflag)
                {
                    soundCoroutineflag = true;
                    soundedCoroutine = StartCoroutine("OnSoundedMode");
                }
            }
        }

    }

    public void BtnRunCliked()
	{
        
		if (playerState != GlobalData.PlayerState.Run)
		{
			if(playerState == GlobalData.PlayerState.Sit)
			{
				visibleTarget.transform.localPosition += Vector3.up * visibleTargetDownDistance; // 배회자가 시야로 감지할 수 있는 타겟을 위로
				playerCam.transform.localPosition += Vector3.up * cameraDownDistance; // 카메라를 위로
			}
			playerState = GlobalData.PlayerState.Run;
            playerAudio.PlayplayerRun();
			UIManager.GetInstance.RunSpriteChange(true);
			UIManager.GetInstance.SitSpriteChange(false);
			moveSpeed = runSpeed;
		}
		else
		{
			playerState = GlobalData.PlayerState.Walk;
            playerAudio.PlayPlayerWalk();
			UIManager.GetInstance.RunSpriteChange(false);
			moveSpeed = walkSpeed;
		}
	}

	public void BtnSitCliked()
	{
		if (playerState != GlobalData.PlayerState.Sit)
		{
			playerState = GlobalData.PlayerState.Sit;
            playerAudio.PlayPlayerSit();
			visibleTarget.transform.localPosition -= Vector3.up * visibleTargetDownDistance; // 배회자가 시야로 감지할 수 있는 타겟을 아래로
			playerCam.transform.localPosition -= Vector3.up * cameraDownDistance;
			UIManager.GetInstance.SitSpriteChange(true);
			UIManager.GetInstance.RunSpriteChange(false);
;			moveSpeed = sitSpeed;
		}
		else
		{
			playerState = GlobalData.PlayerState.Walk;
            playerAudio.PlayPlayerWalk();
			visibleTarget.transform.localPosition += Vector3.up * visibleTargetDownDistance; // 배회자가 시야로 감지할 수 있는 타겟을 위로
			playerCam.transform.localPosition += Vector3.up * cameraDownDistance;
			UIManager.GetInstance.SitSpriteChange(false);
			moveSpeed = walkSpeed;
		}
	}
	public void TakeStun(float stuntime)
	{
		StartCoroutine(PlayerStun(stuntime));
	}
	IEnumerator PlayerStun(float stuntime)
	{
		isStun = true;

		yield return new WaitForSeconds(stuntime);
		isStun = false;

	}
	public void PlayerReset() // 광고본후 Player Reset 메소드
    {
		playerCam.GetComponent<GrayScaleEffect>().intensity = GlobalData.PLAYER_CAM_DEFAULT;
		playerState = GlobalData.PlayerState.Walk;
        
		
		playerCam.transform.localPosition =playerCamInitialpos;
        visibleTarget.transform.localPosition = visibleTargetInitialpos;
        moveSpeed = walkSpeed;
        staminaValue = 1f;
        transform.localRotation = Quaternion.identity;
        navMeshAgent.Warp(playerStartPosition);
        playerAudio.PlayerAudioSetInitialize();
        SoundManager.GetInstance.SetMuteBGM(false);
		isDead = false;
		isStun = false;

	}

	IEnumerator OnSoundedMode()
	{
		yield return new WaitForSeconds(1.0f);
		soundedState = false;
		soundCoroutineflag = false;
		
	}
	public void OnPlayerDeathWithScan()
	{
		OnPlayerdeath();
		// 서쳐 연출보여줄것
		UIManager.GetInstance.gameOverSearcher.GetComponent<Animator>().Play("Run_Ani", -1, 0);

	}
	private void OnTriggerEnter(Collider other)
	{
        if (isDead )
            return;
        if (!isTrigger)
            return;
		if (other.CompareTag("ITEM"))
		{
            SoundManager.GetInstance.PlayOneShotItemEatClip();
            GameManager.GetInstance.JewelNumberDown();
			other.gameObject.SetActive(false);
		}
		if (other.gameObject.CompareTag("Enemy"))
		{
			if (other.GetComponent<EnemyController>().IsEnemyStun())
				return;
            OnPlayerdeath();

			switch (other.gameObject.GetComponent<EnemyController>().enemyType)
			{
				case GlobalData.EnemyType.Worker:
					UIManager.GetInstance.gameOverWalker.GetComponent<Animator>().Play("Walker_Run_GameOver", -1, 0);
					break;
				case GlobalData.EnemyType.Runner:
					//UIManager.GetInstance.gameOverRunner.GetComponent<Animator>().Play("Runner_Run_GameOver", -1, 0);
					StartCoroutine(RunnerGameOverAnimationTest());
					break;
				case GlobalData.EnemyType.Type3:
					UIManager.GetInstance.gameOverType3.GetComponent<Animator>().Play("WaitingPs_Move_GameOver", -1, 0);
					break;
				case GlobalData.EnemyType.GhostCart:
					UIManager.GetInstance.gameOverGhostCart.GetComponent<Animator>().Play("Walker_Run_GameOver", -1, 0);
					break;
				case GlobalData.EnemyType.GhostRunner:
					//UIManager.GetInstance.gameOverGhostRunner.GetComponent<Animator>().Play("Runner_Run_GameOver", -1, 0);
					StartCoroutine(GhostRunnerGameOverAnimationTest());
					break;
			}

        }
		if (other.gameObject.CompareTag("StunItem"))
		{
			if (isStunItemActive) // 스턴아이템 가지고 있으면 return 해서 아무 효과없게
				return;
			isStunItemActive = true;
			UIManager.GetInstance.StunSpriteChange(true);
			SoundManager.GetInstance.PlayOneShotStunItemEatClip();
			other.gameObject.SetActive(false);
			
		}
		if(other.gameObject.CompareTag("Potion"))
		{
			if (isStaminaItemActive)
				return;
			isStaminaItemActive = true;
			UIManager.GetInstance.PotionSpriteChange(true);
			SoundManager.GetInstance.PlayOneShotItemPotionEatClip();
			other.gameObject.SetActive(false);
				
		}
	}
	public void ActiveStaminaItem()
	{
		if (isStaminaItemActive)
		{
			SoundManager.GetInstance.PlayOneShotItemPotionActiveClip();
			staminaValue = 1f;
			if (staminaValue >= 1)
				staminaValue = 1f;
			isStaminaItemActive = false;
			UIManager.GetInstance.PotionSpriteChange(false);
		}
	}
	public void ActiveStunItem()
	{
		if (isStunItemActive)
		{
			SoundManager.GetInstance.PlayItemStunEatClip();
			StartCoroutine(GrayScaleEffect());
			if (OnPlayerItemEat != null)
				OnPlayerItemEat();
			isStunItemActive = false;
			UIManager.GetInstance.StunSpriteChange(false);
		}

	}
	
    private void OnPlayerdeath()
    {
        isDead = true;
        playerAudio.SetPlayerFootStepAudiosourceMute(true);
        SoundManager.GetInstance.SetMuteBGM(true);
        UIManager.GetInstance.GameOverViewDisplay();
        if (OnPlayerDead != null)
            OnPlayerDead();
    }
	IEnumerator GrayScaleEffect()
	{
		playerCam.GetComponent<GrayScaleEffect>().intensity = GlobalData.PLAYER_CAM_TIMESTOP;
		yield return new WaitForSeconds(6f);
		if(GameManager.GetInstance.ChaseCount == 0)
			playerCam.GetComponent<GrayScaleEffect>().intensity = GlobalData.PLAYER_CAM_DEFAULT;
		else
			playerCam.GetComponent<GrayScaleEffect>().intensity = GlobalData.PLAYER_CAM_CHASE;
	}

	IEnumerator RunnerGameOverAnimationTest()
	{
		float curTime = Time.time;
		while(Time.time - curTime < 3.0f)
		{
			UIManager.GetInstance.gameOverRunner.transform.Translate(Vector3.forward * 20.0f *  Time.deltaTime);
			yield return null;
		}
		UIManager.GetInstance.gameOverRunner.transform.localPosition = runnerGameOverAnimStartPos.localPosition;
	} // 러너프리팹은 fbx에서 Optimized Game Objects 체크 ON인 상태로 만들어서 애니메이션 녹화 안됨, 임시로 코드로 해결함
	IEnumerator GhostRunnerGameOverAnimationTest()
	{
		float curTime = Time.time;
		while (Time.time - curTime < 3.0f)
		{
			UIManager.GetInstance.gameOverGhostRunner.transform.Translate(Vector3.forward * 20.0f * Time.deltaTime);
			yield return null;
		}
		UIManager.GetInstance.gameOverGhostRunner.transform.localPosition = runnerGameOverAnimStartPos.localPosition;
	} // 고스트러너프리팹은 fbx에서 Optimized Game Objects 체크 ON인 상태로 만들어서 애니메이션 녹화 안됨, 임시로 코드로 해결함

	private void OnDestroy()
	{
		if(AdsTestWonGyu.GetInstance!=null)
			AdsTestWonGyu.GetInstance.ShowresultFinished -= PlayerReset;
		
	}
}
