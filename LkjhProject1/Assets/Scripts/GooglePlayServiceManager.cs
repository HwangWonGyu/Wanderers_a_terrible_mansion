using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames.BasicApi;
using GooglePlayGames;
public class GooglePlayServiceManager : Singleton<GooglePlayServiceManager> {
#if UNITY_ANDROID
	private void Awake()
	{
		if (sInstance == null)
		{
			sInstance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
	// Use this for initialization
	void Start ()
	{

		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();

		PlayGamesPlatform.InitializeInstance(config);

		PlayGamesPlatform.DebugLogEnabled = false;

		PlayGamesPlatform.Activate();

		LogIn();


	}
#region[LogIn / LogOut]
	public void LogIn()
	{

		Social.localUser.Authenticate((bool success) =>
		{
			if (success)
			{

			}
			else
			{

			}
		}
		);

	}
	public void SignOut()
	{
		PlayGamesPlatform.Instance.SignOut();
	}
#endregion
	public void ReportEasyModeScore(long score)
	{

		if (!Social.localUser.authenticated)
			return;
		PlayGamesPlatform.Instance.ReportScore(score, GPGSIds.leaderboard_easymode, (bool success) =>
		{
			if (success)
			{
				// Report 성공
				// 그에 따른 처리
			}
			else
			{
				// Report 실패
				// 그에 따른 처리
			}
		});


	}
	public void ReportNormalModeScore(long score)
	{

		if (!Social.localUser.authenticated)
			return; 
		PlayGamesPlatform.Instance.ReportScore(score, GPGSIds.leaderboard_normalmode, null);

	}
	public void ReportHardModeScore(long score)
	{

		if (!Social.localUser.authenticated)
			return;
		PlayGamesPlatform.Instance.ReportScore(score, GPGSIds.leaderboard_hardmode, null);

	}

	public void ReportMediumModeScore(long score)
	{

		if (!Social.localUser.authenticated)
			return;
		PlayGamesPlatform.Instance.ReportScore(score, GPGSIds.leaderboard_mediummode, null);

	}

	public void ReportExtremeModeScore(long score)
	{

		if (!Social.localUser.authenticated)
			return;
		PlayGamesPlatform.Instance.ReportScore(score, GPGSIds.leaderboard_extrememode, null);

	}
	public void ShowLeaderboardUI()
	{

		// Sign In 이 되어있지 않은 상태라면
		// Sign In 후 리더보드 UI 표시 요청할 것
		if (Social.localUser.authenticated == false)
		{
			Social.localUser.Authenticate((bool success) =>
			{
				if (success)
				{
					// Sign In 성공
					// 바로 리더보드 UI 표시 요청
					Social.ShowLeaderboardUI();
					return;
				}
				else
				{
					UIManager.GetInstance.LogInErrorPanelOn();
					// Sign In 실패 
					// 그에 따른 처리
					return;
				}
			});
		}
		else
		{

			PlayGamesPlatform.Instance.ShowLeaderboardUI();

		}




	}
	public void ShowLeaderboardWithId(string lederboardId)
	{
		if (Social.localUser.authenticated == false)
		{
			Social.localUser.Authenticate((bool success) =>
			{
				if (success)
				{
					// Sign In 성공
					// 바로 리더보드 UI 표시 요청
					switch (lederboardId)
					{
						case "Easy":
							PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_easymode);
							break;
						case "Normal":
							PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_normalmode);
							break;
						case "Medium":
							PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_mediummode);
							break;
						case "Hard":
							PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_hardmode);
							break;
						case "Extreme":
							PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_extrememode);
							break;
					}
					return;
				}
				else
				{
					UIManager.GetInstance.LogInErrorPanelOn();
					// Sign In 실패 
					// 그에 따른 처리
					return;
				}
			});
		}
		else
		{
			switch (lederboardId)
			{
				case "Easy":
					PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_easymode);
					break;
				case "Normal":
					PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_normalmode);
					break;
				case "Medium":
					PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_mediummode);
					break;
				case "Hard":
					PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_hardmode);
					break;
				case "Extreme":
					PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_extrememode);
					break;
			}
		}
	}

#endif
	}
