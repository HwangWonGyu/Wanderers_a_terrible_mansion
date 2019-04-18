using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Advertisements;
//using UnityEngine.Monetization;
using UnityEngine.Analytics;
using UnityEngine.AI;
using UnityEngine.UI;

public class AdsTestWonGyu : Singleton<AdsTestWonGyu>
{
	//public GameObject player;
	public event Action ShowresultFinished;

#if UNITY_IOS
	private string gameId = "2870654";
#elif UNITY_ANDROID
	private string gameId = "2870656";
#endif
	public string placementId = "video";
	public string rewardPlacementId = "rewardedVideo";
	private void Awake()
	{
		if (sInstance == null)
		{
			sInstance = this;
			DontDestroyOnLoad(this);
		}
		else
		{
			Destroy(gameObject);
		}

	}
	void Start()
	{
#if UNITY_ANDROID || UNITY_IOS
        if (/*Monetization*/Advertisement.isSupported)
        {
            //player = GameObject.Find("Player");
			/*Monetization*/Advertisement.Initialize(gameId, false);
			
			
        }
#endif   
	}

	public void ShowNonRewardedAd()
	{
		StartCoroutine(ShowAdWhenReady());
	}
	public bool IsAdsReady()
	{
		return Advertisement.IsReady();
	}
	public void ShowRewardAd()
	{
		
		//ShowAdCallbacks options = new ShowAdCallbacks();
		//options.finishCallback = HandleShowResult;
		//ShowAdPlacementContent ad = Monetization.GetPlacementContent(rewardPlacementId) as ShowAdPlacementContent;
		//if(ad.ready)
		//	ad.Show(options);

		ShowOptions options = new ShowOptions();
		options.resultCallback = HandleShowResult;
		if (Advertisement.IsReady())
		{
			Advertisement.Show(rewardPlacementId, options);
		}
		else
		{
			UIManager.GetInstance.IsBlockingClick = false;
		}
	}

	void HandleShowResult(ShowResult result)
	{
		if (result == ShowResult.Finished)
		{
			if (ShowresultFinished != null)
				ShowresultFinished();
			//player.GetComponent<PlayerController>().PlayerReset();
	        //GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
         //   foreach (GameObject enemy in enemys)
         //   {
         //       enemy.GetComponent<EnemyController>().EnemyStateChange(GlobalData.EnemyState.Reset);
         //   }
            //UIManager.GetInstance.isAlreadyWatchAd = true;
			GameManager.GetInstance.lives -= 1;
			GameManager.GetInstance.GameReset();
            UIManager.GetInstance.GameStartViewDisplay();
            UIManager.GetInstance.IsBlockingClick = false;
		}
		else if (result == ShowResult.Skipped)
		{
			Debug.Log("The player skipped the video - DO NOT REWARD!");
		}
		else if (result == ShowResult.Failed)
		{
			UIManager.GetInstance.IsBlockingClick = false;
			Debug.LogError("Video failed to show");
		}
	}

	private IEnumerator ShowAdWhenReady()
	{
		while (!/*Monetization*/Advertisement.IsReady(placementId))
		{
			yield return new WaitForSeconds(0.25f);
		}
		//ShowAdPlacementContent ad = null;
		//ad = Monetization.GetPlacementContent(placementId) as ShowAdPlacementContent;

		//if (ad != null)
		//{
		//	ad.Show();
		//}
		
		//Advertisement.Show(placementId);
	}
	


}
