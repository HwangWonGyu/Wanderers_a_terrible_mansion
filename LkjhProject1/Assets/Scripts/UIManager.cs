using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager> {

	[Header("Canvas")]
	public GameObject startCanvas;
	public GameObject gameCanvas;
	public GameObject optionCanvas;
   
	[Header("Options")]
	private static int UPDATE_SELECTED_FREQUENCY = 3;
    private int updateSelectedFrequencyCounter = UPDATE_SELECTED_FREQUENCY;
	public Text sensitivityValueText;
	public Text thresholdValueText;
    public Text BgmValueText;
    public Text EffectValueText;
	private float timer = 0;
	public float Counter = 0.1f;
    public GameObject gameMenu;
    public Image fadePanel;
    public float fadeSpeed = 0.5f;

	private int sensitivity;
	public int Sensitivity
	{
		get	{ return sensitivity; }
		set
		{
			if (value > 15)
				sensitivity = 15;
			else if (value < 1)
				sensitivity = 1;
			else
				sensitivity = value;
		}
	}

	private float threshold;
	public float Threshold
	{
		get	{ return threshold;	}
		set
		{
			if (value < 0.01f)
				threshold = 0.01f;
			else if (value > 0.9f)
				threshold = 0.9f;
			else
				threshold = value;
		}
	}

	public enum GameOverSceneType
	{
		NONE,
		IMAGE,
		ANIMATION,
		CAMERAWORK
	};

	[Header("Leaderboard")]
	public GameObject timeAttackRankingPanel;

	[Header("Game Start")]
	public GameObject extremeLock;
	public GameObject extremeUnlock;
	public Image spr_Extreme;
    public Text text_Extreme;
    public GameObject gameStartPanel;
	public GameObject gameStartPanel2; // 2018.12.06(목) 원규, 소용돌이의 저택 테마
	public GameObject gameStartPanel3; // 2018.12.06(목) 원규, 한적한 곳 테마
	public GameObject gamestartPanel4; // 2018.12.17(월) 상균, 돌연변이 저택 테마

    [Header("Game Over")]
	public GameOverSceneType gameOverSceneType;
	public Text gameOverText;
	public Image gameOverImage;
    public GameObject gameOverCamera;
	public GameObject gameOverWalker;
	public GameObject gameOverRunner;
	public GameObject gameOverType3;
	public GameObject gameOverGhostCart;
	public GameObject gameOverGhostRunner;
	public GameObject gameOverSearcher;

	[Header("Game Clear")]
	public Text gameClearText;
    public Image gameClearImage;
    public GameObject gameClearButton;

	[Header("Game Scene Button")]
	public Image runButton;
	public Image sitButton;
    public GameObject gameOverbtnGrid;
	public GameObject watchAdButton;
	public GameObject pauseButton;
	public Image stunButton;
	public Image StaminaButton;
	//[HideInInspector]
	//public bool isAlreadyWatchAd;

	[Header("Shop")]
	public GameObject shopPanel;

    [Header("Sprites")]
    public Sprite spr_SitOn;
    public Sprite spr_SitOff;
    public Sprite spr_RunOn;
    public Sprite spr_RunOff;
	public Sprite spr_StunOn;
	public Sprite spr_StunOff;
	public Sprite spr_PotionOn;
	public Sprite spr_PotionOff;
	[Header("JoyStick")]
	public GameObject fixedJoystick;
	public GameObject fixedJoystickHandle;
	public GameObject floatingJoystick;

	[Header("Mini Map")]
	public GameObject minimap;
	public MiniMapController mmc;
	[Header("Stamina")]
	public GameObject StaminaSlider;
	public Slider stamina;

	[Header("Jewel")]
	public Image jewelImage;
	public Text jewelText;
	private int jewel;

	[Header("Player")]
	public GameObject player;
    public bool IsBlockingClick;

	[Header("Error Panel")]
	public GameObject errorPanel;

	[Header("Tile Wall Top Material & Texture")]
	public Material tileWallTop;
	public Texture[] tileWallTextures;

	public int UpdateSelectedFrequencyCounter
	{
		get	{ return updateSelectedFrequencyCounter; }
		set
		{
			if (updateSelectedFrequencyCounter < 0)
				updateSelectedFrequencyCounter = UPDATE_SELECTED_FREQUENCY;
			else
				updateSelectedFrequencyCounter = value;
		}
	}
	public bool IsUpdateSelectedFirst {	get; set; }
	public bool IsPressing { get; set; }

	private void Awake()
    {
        if (sInstance == null)
        {
            sInstance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
	}
    private void Start()
    {
		// 2018.11.29 원규
		// PC버전, 모바일버전 빌드 바꿀때마다 자꾸 Hierarchy창에서 SitButton이 Floating Joystick위로 올라가서
		// SitButton이 먼저 렌더링되니까 터치가 안됨
		// 그래서 Hierarchy창에서 GameSceneCanvas 자식들 정렬 순서를 6번째(인덱스 5)로 강제시킴
		//mmc = GetComponentInChildren<MiniMapController>();
		
		sitButton.rectTransform.SetSiblingIndex(5);

		StartCoroutine(InitializeSetting());
    }
//	private void Update()
//	{
//#if UNITY_STANDALONE_WIN || UNITY_EDITOR

//		if (Input.GetKeyDown(KeyCode.Escape))
//		{
//			if (SceneManager.GetActiveScene().name != "TitleScene")
//			{
//				PauseButtonClick();
//				Cursor.visible = gameMenu.activeSelf;
//				print(Cursor.visible);
//				print(gameMenu.activeSelf);
				
//			}
//		}
		
//#endif
//	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		optionCanvas.SetActive(false);
        gameStartPanel.SetActive(false);
		gameStartPanel2.SetActive(false);
		gameStartPanel3.SetActive(false);
		gamestartPanel4.SetActive(false);
		timeAttackRankingPanel.SetActive(false);
		minimap.SetActive(false);
		StartCoroutine(FadeIn());
        if (scene.name == GlobalData.SCENE_TITLE)
		{

#if UNITY_STANDALONE || UNITY_EDITOR
			Color currentColor = fixedJoystick.GetComponent<Image>().color;
			currentColor.a = 0;
			fixedJoystick.GetComponent<Image>().color = currentColor;
			fixedJoystickHandle.GetComponent<Image>().color = currentColor;

			fixedJoystick.GetComponent<Image>().raycastTarget = false;
			fixedJoystickHandle.GetComponent<Image>().raycastTarget = false;
			floatingJoystick.GetComponent<Image>().raycastTarget = false;
#elif UNITY_ANDROID || UNITY_IOS
			fixedJoystick.GetComponent<Image>().raycastTarget = true;
			fixedJoystickHandle.GetComponent<Image>().raycastTarget = true;
			floatingJoystick.GetComponent<Image>().raycastTarget = true;
#endif
			errorPanel.SetActive(false);
			startCanvas.SetActive(true);
			gameCanvas.SetActive(false);
			UIManager.GetInstance.ShopPanelOff();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
#endif
		}
		else
		{
            
            gameCanvas.SetActive(true);
			startCanvas.SetActive(false);
            
            player = GameObject.Find("Player");
			mmc.target = player.transform;
			pauseButton.SetActive(true);
			minimap.SetActive(true);
			gameOverCamera.SetActive(false);
			gameOverText.enabled = false;
			SitSpriteChange(false);
            RunSpriteChange(false);
			StunSpriteChange(false);
			PotionSpriteChange(false);
			runButton.gameObject.SetActive(true);
			sitButton.gameObject.SetActive(true);
			stunButton.gameObject.SetActive(true);
			StaminaButton.gameObject.SetActive(true);
			StaminaSlider.SetActive(true);
            gameOverbtnGrid.SetActive(false);
            gameMenu.SetActive(false);
            gameOverImage.enabled = false;
            gameClearImage.enabled = false;
			gameClearText.enabled = false;
			gameClearButton.SetActive(false);
            fixedJoystick.GetComponent<Image>().enabled = true;
			fixedJoystickHandle.GetComponent<Image>().enabled = true;
			jewelImage.enabled = true;
			jewelText.enabled = true;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

			player.GetComponent<PlayerController>().mouseLook.XSensitivity = Sensitivity;
			player.GetComponent<PlayerController>().mouseLook.YSensitivity = Sensitivity;
			player.GetComponent<PlayerController>().mouseLook.MaximumX = (90 - Threshold * 100);
			player.GetComponent<PlayerController>().mouseLook.MinimumX = - (90 - Threshold * 100);
#endif
		}

		//2018.12.07 원규
		//벽지 텍스처 교체
		switch (scene.name)
		{
			case GlobalData.SCENE_EASY:
			case GlobalData.SCENE_MEAL:
			case GlobalData.SCENE_MUTATION_EASY:
				tileWallTop.SetTexture("_MainTex", tileWallTextures[0]);
				break;
			case GlobalData.SCENE_NORMAL:
			case GlobalData.SCENE_REST:
			case GlobalData.SCENE_LAST_SUPPER:
			case GlobalData.SCENE_MUTATION_NORMAL:
				tileWallTop.SetTexture("_MainTex", tileWallTextures[1]);
				break;
			case GlobalData.SCENE_MEDIUM:
			case GlobalData.SCENE_WALK:
			case GlobalData.SCENE_DEER_HUNTER:
			case GlobalData.SCENE_MUTATION_MEDIUM:
				tileWallTop.SetTexture("_MainTex", tileWallTextures[2]);
				break;
			case GlobalData.SCENE_HARD:
			case GlobalData.SCENE_EXHIBITION_A:
			case GlobalData.SCENE_MUTATION_HARD:
				tileWallTop.SetTexture("_MainTex", tileWallTextures[3]);
				break;
			case GlobalData.SCENE_EXTREME:
			case GlobalData.SCENE_EXHIBITION_B:
			case GlobalData.SCENE_MUTATION_EXTREME:
				tileWallTop.SetTexture("_MainTex", tileWallTextures[4]);
				break;
			case GlobalData.SCENE_TITLE:
			case GlobalData.SCENE_WATCHING:
				tileWallTop.SetTexture("_MainTex", tileWallTextures[5]);
				break;
		}
		//벽지 텍스처 교체

	}



	#region [Button Click Method]

	#region [Start Scene]
	public void GameStartBtnClick()
    {
        gameStartPanel.SetActive(true);
        if (SaveLoadDataManager.GetInstance.data.IsExtremeValue)
        {
            Color color = new Color(1, 1, 1, 1);
            spr_Extreme.color = color;
            text_Extreme.color = color;
			extremeUnlock.SetActive(true);
			extremeLock.SetActive(false);
        }
        else
        {
            Color color = new Color(1, 1, 1, 0.5f);
            spr_Extreme.color = color;
            text_Extreme.color = color;
			extremeLock.SetActive(true);
			extremeUnlock.SetActive(false);
		}
    }

	public void StartGameEasy()
	{
        SoundManager.GetInstance.PlayOneShotUIBtnClick();
        if (IsBlockingClick)
            return;
        IsBlockingClick = true;
        StartCoroutine(LoadSceneWithFadeOut(GlobalData.SCENE_EASY));
	}
	public void StartKSKTestScene()
	{
		StartCoroutine(LoadSceneWithFadeOut("Map1_JunWook_MutationNormal01"));
	}
    IEnumerator LoadSceneWithFadeOut(string SceneName)
    {
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(SceneName);
        IsBlockingClick = false;

    }
	public void StartGameNormal()
	{
        SoundManager.GetInstance.PlayOneShotUIBtnClick();
        if (IsBlockingClick)
            return;
        IsBlockingClick = true;
        StartCoroutine(LoadSceneWithFadeOut(GlobalData.SCENE_NORMAL));
        
    }
	public void StartGameHard()
	{
        SoundManager.GetInstance.PlayOneShotUIBtnClick();
        if (IsBlockingClick)
            return;
        IsBlockingClick = true;
        StartCoroutine(LoadSceneWithFadeOut(GlobalData.SCENE_HARD));
    }
	public void StartGameMedium()
	{
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		if (IsBlockingClick)
			return;
		IsBlockingClick = true;
		StartCoroutine(LoadSceneWithFadeOut(GlobalData.SCENE_MEDIUM));

	}
	public void StartGameExtreme()
    {
        SoundManager.GetInstance.PlayOneShotUIBtnClick();
        if (!SaveLoadDataManager.GetInstance.data.IsExtremeValue)
            return;
        if (IsBlockingClick)
            return;
        IsBlockingClick = true;
        StartCoroutine(LoadSceneWithFadeOut(GlobalData.SCENE_EXTREME));
        
    }
    public void DevelopmentExtreme()
    {
        SoundManager.GetInstance.PlayOneShotUIBtnClick();
        if (IsBlockingClick)
            return;
        IsBlockingClick = true;
        StartCoroutine(LoadSceneWithFadeOut(GlobalData.SCENE_EXTREME));
    }
	public void StartGameLastSupper()
	{
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		if (IsBlockingClick)
			return;
		IsBlockingClick = true;
		StartCoroutine(LoadSceneWithFadeOut(GlobalData.SCENE_LAST_SUPPER));
	}
	public void StartGameExhibition()
	{
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		if (IsBlockingClick)
			return;
		IsBlockingClick = true;
		StartCoroutine(LoadSceneWithFadeOut(GlobalData.SCENE_EXHIBITION_A));
	}
	public void StartGameJunWookExhibition()
	{
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		if (IsBlockingClick)
			return;
		IsBlockingClick = true;
		StartCoroutine(LoadSceneWithFadeOut(GlobalData.SCENE_EXHIBITION_B));
	}
	public void StartGameJunWookDeerHunter()
	{
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		if (IsBlockingClick)
			return;
		IsBlockingClick = true;
		StartCoroutine(LoadSceneWithFadeOut(GlobalData.SCENE_JUNWOOK_DEER_HUNTER));
	}
	public void StartGameDeerHunter()
	{
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		if (IsBlockingClick)
			return;
		IsBlockingClick = true;
		StartCoroutine(LoadSceneWithFadeOut(GlobalData.SCENE_DEER_HUNTER));
	}
	public void StartGameMeal()
	{
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		if (IsBlockingClick)
			return;
		IsBlockingClick = true;
		StartCoroutine(LoadSceneWithFadeOut(GlobalData.SCENE_MEAL));
	}
	public void StartGameWalk()
	{
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		if (IsBlockingClick)
			return;
		IsBlockingClick = true;
		StartCoroutine(LoadSceneWithFadeOut(GlobalData.SCENE_WALK));
	}
	public void StartGameWatching()
	{
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		if (IsBlockingClick)
			return;
		IsBlockingClick = true;
		StartCoroutine(LoadSceneWithFadeOut(GlobalData.SCENE_WATCHING));
	}
	public void StartGameRest()
	{
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		if (IsBlockingClick)
			return;
		IsBlockingClick = true;
		StartCoroutine(LoadSceneWithFadeOut(GlobalData.SCENE_REST));
	}
	public void StartGameMutationEasy()
	{
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		if (IsBlockingClick)
			return;
		IsBlockingClick = true;
		StartCoroutine(LoadSceneWithFadeOut(GlobalData.SCENE_MUTATION_EASY));
	}
	public void StartGameMutationNormal()
	{
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		if (IsBlockingClick)
			return;
		IsBlockingClick = true;
		StartCoroutine(LoadSceneWithFadeOut(GlobalData.SCENE_MUTATION_NORMAL));
	}
	public void StartGameMutationMedium()
	{
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		if (IsBlockingClick)
			return;
		IsBlockingClick = true;
		StartCoroutine(LoadSceneWithFadeOut(GlobalData.SCENE_MUTATION_MEDIUM));
	}
	public void StartGameMutationHard()
	{
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		if (IsBlockingClick)
			return;
		IsBlockingClick = true;
		StartCoroutine(LoadSceneWithFadeOut(GlobalData.SCENE_MUTATION_HARD));
	}
	public void StartGameMutationExtreme()
	{
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		if (IsBlockingClick)
			return;
		IsBlockingClick = true;
		StartCoroutine(LoadSceneWithFadeOut(GlobalData.SCENE_MUTATION_EXTREME));
	}

	public void ApplicationQuit()
    {
        SoundManager.GetInstance.PlayOneShotUIBtnClick();
        if (IsBlockingClick)
            return;
        IsBlockingClick = true;
        Application.Quit();
        IsBlockingClick = false;
    }

#endregion

#region [GameScene]

    public void SitSpriteChange(bool value)
    {
        if (value)
            sitButton.sprite = spr_SitOn;
        else
            sitButton.sprite = spr_SitOff;
    }
    public void RunSpriteChange(bool value)
    {
        if (value)
            runButton.sprite = spr_RunOn;
        else
            runButton.sprite = spr_RunOff;
    }
	public void StunSpriteChange(bool value)
	{
		if (value)
			stunButton.sprite = spr_StunOn;
		else
			stunButton.sprite = spr_StunOff;
	}
	public void PotionSpriteChange(bool value)
	{
		if (value)
			StaminaButton.sprite = spr_PotionOn;
		else
			StaminaButton.sprite = spr_PotionOff;
	}
    public void Btn_Run()
    {
        player.GetComponent<PlayerController>().BtnRunCliked();
    }
    public void Btn_Sit()
    {
        player.GetComponent<PlayerController>().BtnSitCliked();
    }
	public void Btn_Stun()
	{
		player.GetComponent<PlayerController>().ActiveStunItem();
	}
	public void Btn_Stamina()
	{
		player.GetComponent<PlayerController>().ActiveStaminaItem();
	}
    public void GoToMainScene()
    {
        if (IsBlockingClick)
            return;
        IsBlockingClick = true;
        player.GetComponent<PlayerController>().isTrigger = false;
        SoundManager.GetInstance.PlayOneShotUIBtnClick();
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemys)
        {
            enemy.GetComponent<EnemyController>().EnemyStateChange(GlobalData.EnemyState.Wait);
        }
        //isAlreadyWatchAd = false;
        StartCoroutine(LoadSceneWithFadeOut(GlobalData.SCENE_TITLE));
        
    }
	public void GoToCurrentScene()
	{
		if (IsBlockingClick)
			return;
		IsBlockingClick = true;
		player.GetComponent<PlayerController>().isTrigger = false;
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject enemy in enemys)
		{
			enemy.GetComponent<EnemyController>().EnemyStateChange(GlobalData.EnemyState.Wait);
		}
		//isAlreadyWatchAd = false;
		StartCoroutine(LoadSceneWithFadeOut(SceneManager.GetActiveScene().name));
	}
	public void PauseButtonClick()
    {
        gameMenu.SetActive(!gameMenu.activeSelf);
        SoundManager.GetInstance.PlayOneShotUIBtnClick();
    }
    public void ShowRewardAds()
    {
        if (IsBlockingClick)
            return;
        IsBlockingClick = true;
        SoundManager.GetInstance.PlayOneShotUIBtnClick();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		StartCoroutine(ContinueGamePCVersion());
#else
		if (InAppPurchaser.GetInstance.HasAdsSkip())
			StartCoroutine(ContineuGameWithAdsSkip());
		else
			AdsTestWonGyu.GetInstance.ShowRewardAd();
#endif
	}
	IEnumerator ContineuGameWithAdsSkip()
	{
		yield return StartCoroutine(FadeOut());
		player.GetComponent<PlayerController>().PlayerReset();
		GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject enemy in enemys)
		{
			
			enemy.GetComponent<EnemyController>().EnemyStateChange(GlobalData.EnemyState.Reset);
		}
		//GameObject[] searchers = GameObject.FindGameObjectsWithTag("Searcher");
		//foreach (GameObject searcher in searchers)
		//{
		//	searcher.GetComponent<SearcherControll>().ResetEnemy();
		//}
		//isAlreadyWatchAd = true;
		GameManager.GetInstance.lives -= 1;
		GameStartViewDisplay();
		IsBlockingClick = false;
		GameManager.GetInstance.GameReset();
		yield return null;
		yield return StartCoroutine(FadeIn());
	}

	IEnumerator ContinueGamePCVersion()
	{
		yield return StartCoroutine(FadeOut());

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
#endif

		player.GetComponent<PlayerController>().PlayerReset();
		GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject enemy in enemys)
		{
			enemy.GetComponent<EnemyController>().EnemyStateChange(GlobalData.EnemyState.Reset);
		}
		//GameObject[] searchers = GameObject.FindGameObjectsWithTag("Searcher");
		//foreach (GameObject searcher in searchers)
		//{
		//	searcher.GetComponent<SearcherControll>().ResetEnemy();
		//}
		//isAlreadyWatchAd = true;0
		GameManager.GetInstance.lives -= 1;
		GameManager.GetInstance.GameReset();
		GameStartViewDisplay();
		IsBlockingClick = false;

		yield return null;
		yield return StartCoroutine(FadeIn());
	}

    public void Btn_Restart()
    {
        if (IsBlockingClick)
            return;
        IsBlockingClick = true;
        SoundManager.GetInstance.PlayOneShotUIBtnClick();
        //isAlreadyWatchAd = false;
        StartCoroutine(LoadSceneWithFadeOut(SceneManager.GetActiveScene().name));
    }
#endregion

#region[Option Canvas]


	public void UpSensitivityUpdate()
	{
		if (IsPressing == false)
			return;
		timer += Time.deltaTime;
		if (timer >= Counter)
		{
			Sensitivity++;
			sensitivityValueText.text = Sensitivity.ToString();
			timer = 0;
		}
	}
	public void UpSensitivityPointerDown()
	{
		IsPressing = true;
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		Sensitivity++;
		sensitivityValueText.text = Sensitivity.ToString();
	}

	public void DownSensitivityUpdate()
	{
		if (IsPressing == false)
			return;
		timer += Time.deltaTime;
		if (timer >= Counter)
		{
			Sensitivity--;
			sensitivityValueText.text = Sensitivity.ToString();
			timer = 0;
		}
	}
	public void DownSensitivityPointerDown()
	{
		IsPressing = true;
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		Sensitivity--;
		sensitivityValueText.text = Sensitivity.ToString();
	}
	public void UpThresholdUpdate()
	{
		if (IsPressing == false)
			return;
		timer += Time.deltaTime;
		if (timer >= Counter)
		{
			Threshold = Threshold - 0.01f;
			thresholdValueText.text = (90 - Threshold * 100).ToString("0");
			timer = 0;
		}
	}
	public void UpThresholdPointerDown()
	{
		IsPressing = true;
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		Threshold = Threshold - 0.01f;
		thresholdValueText.text = (90 - Threshold * 100).ToString("0");
	}
	public void DownThresholdUpdate()
	{
		if (IsPressing == false)
			return;
		timer += Time.deltaTime;
		if (timer >= Counter)
		{
			Threshold = Threshold + 0.01f;
			thresholdValueText.text = (90 - Threshold * 100).ToString("0");
			timer = 0;
		}
	}
	public void DownThresholdPointerDown()
	{
		IsPressing = true;
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		Threshold = Threshold + 0.01f;
		thresholdValueText.text = (90 - Threshold * 100).ToString("0");
	}
#region [Sound]
	public void DownBgmVolumeUpdate()
	{
		if (IsPressing == false)
		{
			return;
		}
		timer += Time.deltaTime;
		if (timer >= Counter)
		{
			SaveLoadDataManager.GetInstance.data.BgmVolume = SaveLoadDataManager.GetInstance.data.BgmVolume - 0.1f;
			BgmValueText.text = (SaveLoadDataManager.GetInstance.data.BgmVolume * 100).ToString("0");
			SoundManager.GetInstance.SetVolume();
			timer = 0;
		}

	}
	public void DownBgmVolumePointerDown()
	{
		IsPressing = true;
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		SaveLoadDataManager.GetInstance.data.BgmVolume = SaveLoadDataManager.GetInstance.data.BgmVolume - 0.1f;
		BgmValueText.text = (SaveLoadDataManager.GetInstance.data.BgmVolume * 100).ToString("0");
		SoundManager.GetInstance.SetVolume();
	}
	public void UpBgmVolumePointerDown()
	{
		IsPressing = true;
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		SaveLoadDataManager.GetInstance.data.BgmVolume = SaveLoadDataManager.GetInstance.data.BgmVolume + 0.1f;
		BgmValueText.text = (SaveLoadDataManager.GetInstance.data.BgmVolume * 100).ToString("0");
		SoundManager.GetInstance.SetVolume();
	}
	public void UpBgmVolumeUpdate()
	{
		if (IsPressing == false)
		{
			return;
		}
		timer += Time.deltaTime;
		if (timer >= Counter)
		{
			SaveLoadDataManager.GetInstance.data.BgmVolume = SaveLoadDataManager.GetInstance.data.BgmVolume + 0.1f;
			BgmValueText.text = (SaveLoadDataManager.GetInstance.data.BgmVolume * 100).ToString("0");
			SoundManager.GetInstance.SetVolume();
			timer = 0;
		}

	}
	public void UpEffectVolumeUpdate()
	{
		if (IsPressing == false)
		{
			return;
		}
		timer += Time.deltaTime;
		if (timer >= Counter)
		{
			SaveLoadDataManager.GetInstance.data.EffectVolume = SaveLoadDataManager.GetInstance.data.EffectVolume + 0.1f;
			EffectValueText.text = (SaveLoadDataManager.GetInstance.data.EffectVolume * 100).ToString("0");
			SoundManager.GetInstance.SetVolume();
			timer = 0;
		}

	}
	public void UpEffectVolumePointerDown()
	{
		IsPressing = true;
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		SaveLoadDataManager.GetInstance.data.EffectVolume = SaveLoadDataManager.GetInstance.data.EffectVolume + 0.1f;
		EffectValueText.text = (SaveLoadDataManager.GetInstance.data.EffectVolume * 100).ToString("0");
		SoundManager.GetInstance.SetVolume();
	}
	public void DownEffectVolumeUpdate()
	{
		if (IsPressing == false)
		{
			return;
		}
		timer += Time.deltaTime;
		if (timer >= Counter)
		{
			SaveLoadDataManager.GetInstance.data.EffectVolume = SaveLoadDataManager.GetInstance.data.EffectVolume - 0.1f;
			EffectValueText.text = (SaveLoadDataManager.GetInstance.data.EffectVolume * 100).ToString("0");
			SoundManager.GetInstance.SetVolume();
			timer = 0;
		}

	}
	public void DownEffectVolumePointerDown()
	{
		IsPressing = true;
		SoundManager.GetInstance.PlayOneShotUIBtnClick();
		SaveLoadDataManager.GetInstance.data.EffectVolume = SaveLoadDataManager.GetInstance.data.EffectVolume - 0.1f;
		EffectValueText.text = (SaveLoadDataManager.GetInstance.data.EffectVolume * 100).ToString("0");
		SoundManager.GetInstance.SetVolume();
	}
#endregion
	public void OptionPointerUp()
	{
		IsPressing = false;
		timer = 0;
	}



	public void SaveOptionValue()
    {
        if (SceneManager.GetActiveScene().name != GlobalData.SCENE_TITLE)
        {
#if UNITY_ANDROID || UNITY_IOS //|| UNITY_EDITOR
			player.GetComponent<PlayerJoystick>().horizontalRotateSensitive = SaveLoadDataManager.GetInstance.data.HorizontalRotateSensitive = Sensitivity;
            player.GetComponent<PlayerJoystick>().verticalRotateThreshold = SaveLoadDataManager.GetInstance.data.VerticalRotateThreshold = Threshold;
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
			player.GetComponent<PlayerController>().mouseLook.XSensitivity = Sensitivity;
			player.GetComponent<PlayerController>().mouseLook.YSensitivity = Sensitivity;
			SaveLoadDataManager.GetInstance.data.HorizontalRotateSensitive = Sensitivity;

			// 최대 카메라 상하회전 가능 각도를 위로 90도 아래로 90도에서 Threshold * 100 만큼(예 : 0.15 * 100 = 15도) 위 아래 각도를 줄임
			player.GetComponent<PlayerController>().mouseLook.MaximumX = (90 - Threshold * 100);
			player.GetComponent<PlayerController>().mouseLook.MinimumX = - (90 - Threshold * 100);
			SaveLoadDataManager.GetInstance.data.VerticalRotateThreshold = Threshold;

			player.GetComponent<PlayerController>().playerCam.transform.localRotation = Quaternion.identity;

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
#endif
		}
		else
        {
            SaveLoadDataManager.GetInstance.data.HorizontalRotateSensitive = Sensitivity;
            SaveLoadDataManager.GetInstance.data.VerticalRotateThreshold = Threshold;
		}
		SoundManager.GetInstance.SetVolume();//Soundmanager에있는 BGM 과 효과음 셋
        SaveLoadDataManager.GetInstance.SaveSettingData();
        SoundManager.GetInstance.PlayOneShotUIBtnClick();
    }

	/// <summary> 설정 버튼 누를때 실행되는 메소드 </summary>
    public void SyncOptionValue()
    {
        if (IsBlockingClick)
            return;
        optionCanvas.SetActive(true);
        Sensitivity = SaveLoadDataManager.GetInstance.data.HorizontalRotateSensitive;
        Threshold = SaveLoadDataManager.GetInstance.data.VerticalRotateThreshold;
        sensitivityValueText.text = Sensitivity.ToString();
        thresholdValueText.text = (90 - Threshold * 100).ToString("0");
        BgmValueText.text = (SaveLoadDataManager.GetInstance.data.BgmVolume * 100).ToString("0");
        EffectValueText.text = (SaveLoadDataManager.GetInstance.data.EffectVolume * 100).ToString("0");
        SoundManager.GetInstance.PlayOneShotUIBtnClick();
    }
#endregion

#endregion


#region [Display UI]
    public void DisplayJewel(int jewel)
    {
		if (jewel == 0)
		{
			jewelImage.enabled = false;
			jewelText.text = "The door has been unlocked";
			SoundManager.GetInstance.PlayOneShotAllItemEatClip();
		}
		else
			jewelText.text = "x" + jewel;
    }
    public void SetStamina(float value)
    {
        stamina.value = value;
    }
    public void GameStartViewDisplay() // 이어하기 경우 뷰
    {
		//ON
		minimap.SetActive(true);
        fixedJoystick.GetComponent<Image>().enabled = true;
        fixedJoystickHandle.GetComponent<Image>().enabled = true;
		if(GameManager.GetInstance.JewelNumber != 0)
			jewelImage.enabled = true;
        jewelText.enabled = true;
        pauseButton.SetActive(true);
        SitSpriteChange(false);
        RunSpriteChange(false);
        runButton.gameObject.SetActive(true);
        sitButton.gameObject.SetActive(true);
		stunButton.gameObject.SetActive(true);
		StaminaButton.gameObject.SetActive(true);
        StaminaSlider.SetActive(true);

        //OFF
        gameOverCamera.SetActive(false);
        gameOverText.enabled = false;
        gameOverImage.enabled = false;
        gameClearImage.enabled = false;
        gameOverbtnGrid.SetActive(false);
		gameClearText.enabled = false;
		gameClearButton.SetActive(false);
	}

    public void GameOverViewDisplay()
    {
        // OFF
        minimap.SetActive(false);
        fixedJoystick.GetComponent<Image>().enabled = false;
        fixedJoystickHandle.GetComponent<Image>().enabled = false;
        jewelImage.enabled = false;
        jewelText.enabled = false;
        pauseButton.SetActive(false);
        gameMenu.SetActive(false);
        runButton.gameObject.SetActive(false);
        sitButton.gameObject.SetActive(false);
		stunButton.gameObject.SetActive(false);
		StaminaButton.gameObject.SetActive(false);
		StaminaSlider.SetActive(false);
        optionCanvas.SetActive(false);

        //ON
        gameOverCamera.SetActive(true);
        StartCoroutine(FadeInGameoverImage(2.5f));
    }

	public void GameClearViewDisplay()
	{
		player.GetComponent<PlayerController>().isDead = true;
		player.GetComponent<PlayerAudio>().SetPlayerFootStepAudiosourceMute(true);
		SoundManager.GetInstance.SetMuteBGM(true);

		// OFF
		minimap.SetActive(false);
		fixedJoystick.GetComponent<Image>().enabled = false;
		fixedJoystickHandle.GetComponent<Image>().enabled = false;
		jewelImage.enabled = false;
		jewelText.enabled = false;
		pauseButton.SetActive(false);
		gameMenu.SetActive(false);
		runButton.gameObject.SetActive(false);
		sitButton.gameObject.SetActive(false);
		stunButton.gameObject.SetActive(false);
		StaminaButton.gameObject.SetActive(false);
		StaminaSlider.SetActive(false);
		optionCanvas.SetActive(false);

		//ON
		gameOverCamera.SetActive(true);
		StartCoroutine(FadeInGameclearImage(1.0f));
	}
	public void LogInErrorPanelOn()
	{
		errorPanel.SetActive(true);
	}
	public void ShopPanelOn()
	{
		shopPanel.SetActive(true);
	}
	public void ShopPanelOff()
	{
		shopPanel.SetActive(false);
	}
	
#endregion

#region [ETC]
	IEnumerator FadeInGameoverImage(float delay)
    {
        yield return new WaitForSeconds(delay);
        Color fadeColor = new Color(1, 1, 1, 0);
        gameOverImage.color = fadeColor;
        gameOverText.color = fadeColor;
        gameOverImage.enabled = true;
        gameOverText.enabled = true;
        while (fadeColor.a < 1)
        {
            fadeColor.a += Time.deltaTime * 0.3f;
            gameOverText.color = fadeColor;
            gameOverImage.color = fadeColor;
            yield return null;
        }
        gameOverbtnGrid.SetActive(true);

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
#endif

		
		
		if(GameManager.GetInstance.lives>0)
        {
#if UNITY_ANDROID || UNITY_IOS
			if(AdsTestWonGyu.GetInstance.IsAdsReady()) //인터넷 연결되었으면
				watchAdButton.SetActive(true);
			else if(InAppPurchaser.GetInstance.HasAdsSkip()) // 인터넷은 안연결되어있지만 Skip 버튼을 구매했으면 그경우도 이어하기 보임
				watchAdButton.SetActive(true);
			else //인터넷 x 구매 x
				watchAdButton.SetActive(false);
#else
			watchAdButton.SetActive(true);
#endif

		}
		else
            watchAdButton.SetActive(false);


		}

		IEnumerator FadeInGameclearImage(float delay)
	{
		yield return new WaitForSeconds(delay);
		Color fadeColor = new Color(1, 1, 1, 0);
		gameClearImage.color = fadeColor;
		gameClearText.color = fadeColor;
		gameClearImage.enabled = true;
		gameClearText.enabled = true;
		while (fadeColor.a < 1)
		{
			fadeColor.a += Time.deltaTime * 0.3f;
			gameClearText.color = fadeColor;
			gameClearImage.color = fadeColor;
			yield return null;
		}
		gameClearButton.SetActive(true);

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
#endif
	}

	IEnumerator InitializeSetting()
    {
        while (!SaveLoadDataManager.GetInstance.isLoadingComplete)
            yield return null;
        Sensitivity = SaveLoadDataManager.GetInstance.data.HorizontalRotateSensitive;
        Threshold = SaveLoadDataManager.GetInstance.data.VerticalRotateThreshold;
    }

    IEnumerator FadeOut()// 어두워지는거
    {
        fadePanel.gameObject.SetActive(true);
        Color fadeColor = new Color(0, 0, 0, 0);
        fadePanel.color = fadeColor;
        while (fadeColor.a < 1)
        {
            fadeColor.a += Time.deltaTime * fadeSpeed;
            fadePanel.color = fadeColor;
            yield return null;
        }
        
    }
    IEnumerator FadeIn()
    {
        fadePanel.gameObject.SetActive(true);
        Color fadeColor = new Color(0, 0, 0, 1);
        fadePanel.color = fadeColor;
        while (fadeColor.a >0)
        {
            fadeColor.a -= Time.deltaTime * fadeSpeed;
            fadePanel.color = fadeColor;
            yield return null;
        }
        fadePanel.gameObject.SetActive(false);

    }
#endregion



}
