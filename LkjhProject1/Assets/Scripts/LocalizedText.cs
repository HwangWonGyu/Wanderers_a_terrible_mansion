using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LocalizedText : MonoBehaviour {
	
	public GlobalData.LocalizationTag locTag;

	private Text uiText;
	private TextMesh textMesh;
	// Use this for initialization
	void Start () {
		uiText = GetComponent<Text>();
		textMesh = GetComponent<TextMesh>();
		if (TextData.GetInstance != null)
			SetText();

	}
	private void OnEnable()
	{
		if (TextData.GetInstance != null)
			TextData.GetInstance.textsOnScene.Add(this);
	}
	private void OnDisable()
	{
		if(TextData.GetInstance !=null)
			TextData.GetInstance.textsOnScene.Remove(this);
	}
	private string GetText()
	{
		string result;
		switch (locTag)
		{
			case GlobalData.LocalizationTag.gameStart:
				result = TextData.GetInstance.currentLanguagePack.gameStart;
				break;
			case GlobalData.LocalizationTag.exitButton:
				result = TextData.GetInstance.currentLanguagePack.exitButton;
				break;
			case GlobalData.LocalizationTag.optionButton:
				result = TextData.GetInstance.currentLanguagePack.optionButton;
				break;
			case GlobalData.LocalizationTag.RankingButton:
				result = TextData.GetInstance.currentLanguagePack.RankingButton;
				break;
			case GlobalData.LocalizationTag.ShopButton:
				result = TextData.GetInstance.currentLanguagePack.ShopButton;
				break;
			case GlobalData.LocalizationTag.tutorialButton:
				result = TextData.GetInstance.currentLanguagePack.tutorialButton;
				break;
			case GlobalData.LocalizationTag.horizontalSensitivity:
				result = TextData.GetInstance.currentLanguagePack.horizontalSensitivity;
				break;
			case GlobalData.LocalizationTag.verticalThreshold:
				result = TextData.GetInstance.currentLanguagePack.verticalThreshold;
				break;
			case GlobalData.LocalizationTag.bgm:
				result = TextData.GetInstance.currentLanguagePack.bgm;
				break;
			case GlobalData.LocalizationTag.effect:
				result = TextData.GetInstance.currentLanguagePack.effect;
				break;
			case GlobalData.LocalizationTag.skipButton:
				result = TextData.GetInstance.currentLanguagePack.skipButton;
				break;
			case GlobalData.LocalizationTag.reStart:
				result = TextData.GetInstance.currentLanguagePack.reStart;
				break;
			case GlobalData.LocalizationTag.GoToMain:
				result = TextData.GetInstance.currentLanguagePack.GoToMain;
				break;
			case GlobalData.LocalizationTag.continues:
				result = TextData.GetInstance.currentLanguagePack.continues;
				break;
			case GlobalData.LocalizationTag.removeAds:
				result = TextData.GetInstance.currentLanguagePack.removeAds;
				break;
			case GlobalData.LocalizationTag.removeAdsDescription:
				result = TextData.GetInstance.currentLanguagePack.removeAdsDescription;
				break;
			case GlobalData.LocalizationTag.logInError:
				result = TextData.GetInstance.currentLanguagePack.logInError;
				break;
			case GlobalData.LocalizationTag.yes:
				result = TextData.GetInstance.currentLanguagePack.yes;
				break;
			case GlobalData.LocalizationTag.timeAttack:
				result = TextData.GetInstance.currentLanguagePack.timeAttack;
				break;
			case GlobalData.LocalizationTag.SwirlMansion:
				result = TextData.GetInstance.currentLanguagePack.SwirlMansion;
				break;
			case GlobalData.LocalizationTag.thelastsupper:
				result = TextData.GetInstance.currentLanguagePack.thelastsupper;
				break;
			case GlobalData.LocalizationTag.deerHunter:
				result = TextData.GetInstance.currentLanguagePack.deerHunter;
				break;
			case GlobalData.LocalizationTag.exhibitionA:
				result = TextData.GetInstance.currentLanguagePack.exhibitionA;
				break;
			case GlobalData.LocalizationTag.exhibitionB:
				result = TextData.GetInstance.currentLanguagePack.exhibitionB;
				break;
			case GlobalData.LocalizationTag.silence:
				result = TextData.GetInstance.currentLanguagePack.silence;
				break;
			case GlobalData.LocalizationTag.silenceDescription:
				result = TextData.GetInstance.currentLanguagePack.silenceDescription;
				break;
			case GlobalData.LocalizationTag.timeAttackDescription:
				result = TextData.GetInstance.currentLanguagePack.timeAttackDescription;
				break;
			case GlobalData.LocalizationTag.swirlMansionDescription:
				result = TextData.GetInstance.currentLanguagePack.swirlMansionDescription;
				break;
			case GlobalData.LocalizationTag.meal:
				result = TextData.GetInstance.currentLanguagePack.meal;
				break;
			case GlobalData.LocalizationTag.stroll:
				result = TextData.GetInstance.currentLanguagePack.stroll;
				break;
			case GlobalData.LocalizationTag.watch:
				result = TextData.GetInstance.currentLanguagePack.watch;
				break;
			case GlobalData.LocalizationTag.postdinner:
				result = TextData.GetInstance.currentLanguagePack.postdinner;
				break;
			case GlobalData.LocalizationTag.mutationDescription:
				result = TextData.GetInstance.currentLanguagePack.mutationDescription;
				break;
			case GlobalData.LocalizationTag.mutation:
				result = TextData.GetInstance.currentLanguagePack.mutation;
				break;
			case GlobalData.LocalizationTag.extremeLock:
				result = TextData.GetInstance.currentLanguagePack.extremeLock;
				break;
			case GlobalData.LocalizationTag.exitDescription:
				result = TextData.GetInstance.currentLanguagePack.exitDescription;
				break;
			case GlobalData.LocalizationTag.no:
				result = TextData.GetInstance.currentLanguagePack.no;
				break;
			default:
				result = string.Empty;
				break;
		}
		return result;
	}
	public void SetText()
	{
		if (uiText)
			uiText.text = GetText();
		if (textMesh)
			textMesh.text = GetText();
	}
	
	
}
