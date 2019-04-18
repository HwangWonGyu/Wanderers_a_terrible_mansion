//using System.Collections;
//using UnityEngine;
//using UnityEngine.Monetization;

//using UnityEngine.UI;

//public class AdsTest : MonoBehaviour {

//	public float my_money = 0;
//	public Text money;
//#if UNITY_IOS
//	private string gameId="2870654";
//#elif UNITY_ANDROID
//	private string gameId = "2870656";
//#endif
//	public string placementId = "video";
//	public string rewardPlacementId = "rewardedVideo";
//	// Use this for initialization
//	void Start () {
//		if(Monetization.isSupported)
//			Monetization.Initialize(gameId, true);
		
//	}
//	private void Update()
//	{
//		money.text = my_money.ToString();
//	}
//	public void ShowNonRewardedAd()
//	{
//		StartCoroutine(ShowAdWhenReady());
//	}
//	public void ShowRewardAd()
//	{
//		ShowAdCallbacks options = new ShowAdCallbacks();
//		options.finishCallback = HandelShowResult;
//		ShowAdPlacementContent ad = Monetization.GetPlacementContent(rewardPlacementId) as ShowAdPlacementContent;
//		ad.Show(options);
//	}
//	void HandelShowResult(ShowResult result)
//	{
//		if (result == ShowResult.Finished)
//		{
//			my_money += 100;
//		}
//		else if (result == ShowResult.Skipped)
//		{
//			Debug.Log("The player skipped the video - DO NOT REWARD!");
//		}
//		else if (result == ShowResult.Failed)
//		{
//			Debug.LogError("Video failed to show");
//		}
//	}
//	private IEnumerator ShowAdWhenReady()
//	{
//		while (!Monetization.IsReady(placementId))
//		{
//			yield return new WaitForSeconds(0.25f);
//		}
//		ShowAdPlacementContent ad = null;
//		ad = Monetization.GetPlacementContent(placementId) as ShowAdPlacementContent;

//		if (ad != null)
//		{
//			ad.Show();
//		}
//	}


//}
