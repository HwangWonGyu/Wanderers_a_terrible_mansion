using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : Singleton<GameManager> {

	public event System.Action OnPlayerClear;
	public event System.Action OnGameReset;
	public int lives;
	private int originlife;
	private int jewelNumber;
	private float timer;
	public int JewelNumber
	{
		get; set;
	}

    private int chaseCount;
	public int ChaseCount
    {
        get { return chaseCount; }
    }

	public int Originlife
	{
		get
		{
			return originlife;
		}

		set
		{
			originlife = value;
		}
	}

	public void ChaseCountUp()
    {
        if (ChaseCount == 0)
        {
            chaseCount++;
            SoundManager.GetInstance.PlayChaseBGM();
			// 2018.12.03 원규
			// 시간 아이템 먹은 상태 아니라면 카메라 색조 붉게 변경
			if (GameObject.Find("Player Camera").GetComponent<GrayScaleEffect>().intensity != GlobalData.PLAYER_CAM_TIMESTOP)
				GameObject.Find("Player Camera").GetComponent<GrayScaleEffect>().intensity = GlobalData.PLAYER_CAM_CHASE;
			// 시간 아이템 먹은 상태 아니라면 카메라 색조 붉게 변경
		}
		else
            chaseCount++;
    }
    public void ChaseCountDown()
    {
        chaseCount--;
		
		if(ChaseCount == 0)
		{
			// 2018.12.03 원규
			// 시간 아이템 먹은 상태 아니라면 카메라 색조 원래대로 변경
			if(GameObject.Find("Player Camera").GetComponent<GrayScaleEffect>().intensity != GlobalData.PLAYER_CAM_TIMESTOP)
				GameObject.Find("Player Camera").GetComponent<GrayScaleEffect>().intensity = GlobalData.PLAYER_CAM_DEFAULT;
			// 시간 아이템 먹은 상태 아니라면 카메라 색조 원래대로 변경
		}
	}
	[Header("Watch")]
	public GameObject watchParent;
	public GameObject watchPrefab;
	public int WatchNumber;
	private List<GameObject> rooms;
	private List<GameObject> potions;
	[Header("Potion")]
	public GameObject potionParent;
	public GameObject potionPrefab;
	public int potionNumber=1;

	private void Start()
    {
		JewelNumber = GameObject.FindGameObjectsWithTag("ITEM").Length;
		Originlife = lives;
		rooms = new List<GameObject>();
		potions = new List<GameObject>();
		MakeWatch();
		MakePotion();
		UIManager.GetInstance.DisplayJewel(JewelNumber);
        SoundManager.GetInstance.PlayBGM();
        chaseCount = 0;
		timer = 0;
		

	}
	private void Update()
	{
		timer += Time.deltaTime;
	}
	private void MakeWatch()
	{
		rooms.AddRange(GameObject.FindGameObjectsWithTag("Room"));
		
		
		if (rooms.Count == 0)
			return;

		for (int i = 0; i < WatchNumber; ++i)
		{
			int rand = Random.Range(0, rooms.Count);
			Instantiate(watchPrefab, rooms[rand].transform.Find("WatchSpawnPosition").position, Quaternion.identity,watchParent.transform);
			rooms.RemoveAt(rand);
		}
	}
	private void MakePotion()
	{
		//GameObject[] PotionSpawnPoints = GameObject.FindGameObjectsWithTag("PotionSpawnPoint");
		potions.AddRange(GameObject.FindGameObjectsWithTag("PotionSpawnPoint"));

		if (potions.Count == 0)
			return;
		for (int i = 0; i < potionNumber; ++i)
		{
			int rand = Random.Range(0, potions.Count);
			Instantiate(potionPrefab, potions[rand].transform.position, Quaternion.identity, potionParent.transform);
			potions.RemoveAt(rand);
		}
	}
	public void JewelNumberDown()
    {
		JewelNumber -= 1;
        UIManager.GetInstance.DisplayJewel(JewelNumber);

    }

	private void OnPlayerGameClear()
	{
		if (OnPlayerClear != null)
			OnPlayerClear();
	}
	public void GameReset()
	{
		if (OnGameReset != null)
			OnGameReset();
	}


	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			if (JewelNumber == 0)
			{
				OnPlayerGameClear();
				UIManager.GetInstance.GameClearViewDisplay();

				JewelNumber--;
#if UNITY_ANDROID
				switch (SceneManager.GetActiveScene().name)
				{

					case GlobalData.SCENE_EASY:
						GooglePlayServiceManager.GetInstance.ReportEasyModeScore((long)(timer*1000));
						break;
					case GlobalData.SCENE_NORMAL:
						GooglePlayServiceManager.GetInstance.ReportNormalModeScore((long)(timer * 1000));
						break;
					case GlobalData.SCENE_MEDIUM:
						GooglePlayServiceManager.GetInstance.ReportMediumModeScore((long)(timer * 1000));
						break;
					case GlobalData.SCENE_HARD:
					GooglePlayServiceManager.GetInstance.ReportHardModeScore((long)(timer * 1000));
						if (lives == Originlife)
						{
							if (!SaveLoadDataManager.GetInstance.data.IsExtremeValue)
							{
								SaveLoadDataManager.GetInstance.data.IsExtremeValue = true;
								SaveLoadDataManager.GetInstance.SaveSettingData();
							}
							// print("익스트림 모드 해금"); // 2018.11.27 원규, 이후에 SaveLoadDataManager에 익스트림 모드 해금된 정보 저장해둬야됨
						}
						break;
					case GlobalData.SCENE_EXTREME:
						GooglePlayServiceManager.GetInstance.ReportExtremeModeScore((long)(timer * 1000));
						break;
				}
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
					if (lives == Originlife)
					{
						if (!SaveLoadDataManager.GetInstance.data.IsExtremeValue)
						{
							SaveLoadDataManager.GetInstance.data.IsExtremeValue = true;
							SaveLoadDataManager.GetInstance.SaveSettingData();
						}
					// print("익스트림 모드 해금"); // 2018.11.27 원규, 이후에 SaveLoadDataManager에 익스트림 모드 해금된 정보 저장해둬야됨
					}

			#endif

			}
		}
	}

}
