using System.Collections;
using System.Collections.Generic;


public class GlobalData  {

	#region
	public enum Language { Korean,English}
	public const string LanguageKey = "Language";
	public enum LocalizationTag
	{
		gameStart, optionButton, tutorialButton, exitButton, RankingButton, ShopButton,
		horizontalSensitivity, verticalThreshold, bgm, effect, skipButton, reStart,
		GoToMain, continues, removeAds, removeAdsDescription, logInError, yes, timeAttack, SwirlMansion,
		thelastsupper, deerHunter, exhibitionA, exhibitionB, silence, silenceDescription, timeAttackDescription, swirlMansionDescription,
		meal, stroll, watch, postdinner, mutationDescription, mutation, extremeLock, exitDescription, no
	}
	#endregion
	#region [Enemy]
	public enum EnemyType {
		Worker,
		Runner,
		Type3,
		GhostCart,
		GhostRunner,
		Searcher,
		Catcher
	}
	public enum EnemyState { Patrol,Chase,ChaseWithHear,Reset,Idle,Wait}
	#endregion
	#region [Player]
	public enum PlayerState { Walk,Run,Sit}
	#endregion
	#region[Scene]
	public const string SCENE_TITLE = "TitleScene";
	public const string SCENE_EASY = "Map1_JunWookEasy01";
	public const string SCENE_NORMAL = "Map1_JunWookNormal01";
	public const string SCENE_MEDIUM = "Map1_JunWookMedium01";
	public const string SCENE_HARD = "Map1_JunWookHard01";
	public const string SCENE_EXTREME = "Map1_WonGyuExtreme";
	public const string SCENE_LAST_SUPPER = "Map1_WonGyuLastSupper";
	public const string SCENE_DEER_HUNTER = "Map1_WonGyuDeerHunter";
	public const string SCENE_JUNWOOK_DEER_HUNTER = "Map1_JunWook_DeerHunter";
	public const string SCENE_EXHIBITION_A = "Map1_WonGyuExhibition";
	public const string SCENE_EXHIBITION_B = "Map1_JunWook_Exhibition";
	public const string SCENE_MEAL = "Map1_WonGyuMeal";
	public const string SCENE_WALK = "Map1_WonGyuWalk";
	public const string SCENE_WATCHING = "Map1_WonGyuWatching";
	public const string SCENE_REST = "Map1_WonGyuRest";
	public const string SCENE_MUTATION_EASY = "Map1_JunWook_MutationEasy01";
	public const string SCENE_MUTATION_NORMAL = "Map1_JunWook_MutationNormal01";
	public const string SCENE_MUTATION_MEDIUM = "Map1_WonGyu_MutationMedium01";
	public const string SCENE_MUTATION_HARD = "Map1_WonGyu_MutationHard01";
	public const string SCENE_MUTATION_EXTREME = "Map1_WonGyu_MutationExtreme01";
	#endregion
	#region[TIME STOP ITEM]
	public const int PLAYER_CAM_DEFAULT = 0;
	public const int PLAYER_CAM_TIMESTOP = 1;
	public const int PLAYER_CAM_CHASE = -3;
	#endregion

}
