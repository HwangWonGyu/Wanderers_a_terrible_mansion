using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
public class StringBundle
{
	public string gameStart;
	public string optionButton;
	public string tutorialButton;
	public string exitButton;
	public string RankingButton;
	public string ShopButton;
	public string horizontalSensitivity;
	public string verticalThreshold;
	public string bgm;
	public string effect;
	public string skipButton;
	public string reStart;
	public string GoToMain;
	public string continues;
	public string removeAds;
	public string removeAdsDescription;
	public string logInError;
	public string yes;
	public string timeAttack;
	public string SwirlMansion;
	public string thelastsupper;
	public string deerHunter;
	public string exhibitionA;
	public string exhibitionB;
	public string silence;
	public string silenceDescription;
	public string timeAttackDescription;
	public string swirlMansionDescription;
	public string meal;
	public string stroll;
	public string watch;
	public string postdinner;
	public string mutationDescription;
	public string mutation;
	public string extremeLock;
	public string exitDescription;
	public string no;

}
public class TextData : Singleton<TextData> {
	Dictionary<string, Hashtable> languagePacks = new Dictionary<string, Hashtable>();
	public StringBundle currentLanguagePack;
	public List<LocalizedText> textsOnScene = new List<LocalizedText>();

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

		GetLanguagePacks();
		currentLanguagePack = SetLanguage(Application.systemLanguage);
		ApplyLanguage();
	}
	private void GetLanguagePacks()
	{
		TextAsset xmlData = (TextAsset)Resources.Load("Language");
		XmlDocument document = new XmlDocument();
		document.LoadXml(xmlData.text);

		XmlNodeList nodes = document.SelectSingleNode("Languages").ChildNodes;
		foreach (XmlNode node in nodes)
		{
			Hashtable languagePack = new Hashtable();
			foreach (XmlElement element in node.ChildNodes)
			{
				languagePack.Add(element.GetAttribute("name"), element.InnerText);
			}
			languagePacks.Add(node.Name, languagePack);
		}
	}

	private StringBundle SetLanguage(SystemLanguage language)
	{
		StringBundle result = new StringBundle();
		string languageString;
		switch (language)
		{
			case  SystemLanguage.English:
				languageString = "English";
				break;
			case SystemLanguage.Korean:
				languageString = "Korean";
				break;
			default:
				languageString = "English";
				break;

		}
		result.gameStart = GetString("gameStart", languageString);
		result.optionButton = GetString("optionButton", languageString);
		result.exitButton = GetString("exitButton", languageString);
		result.tutorialButton = GetString("tutorialButton", languageString);
		result.RankingButton = GetString("RankingButton", languageString);
		result.ShopButton = GetString("ShopButton", languageString);
		result.horizontalSensitivity = GetString("horizontalSensitivity", languageString);
		result.verticalThreshold = GetString("verticalThreshold", languageString);
		result.bgm = GetString("bgm", languageString);
		result.effect = GetString("effect", languageString);
		result.skipButton = GetString("skipButton",languageString);
		result.reStart = GetString("reStart", languageString);
		result.GoToMain = GetString("GoToMain", languageString);
		result.continues = GetString("continues", languageString);
		result.removeAds = GetString("removeAds", languageString);
		result.removeAdsDescription = GetString("removeAdsDescription", languageString);
		result.logInError = GetString("logInError", languageString);
		result.yes = GetString("yes", languageString);
		result.timeAttack = GetString("timeAttack", languageString);
		result.SwirlMansion = GetString("SwirlMansion", languageString);
		result.thelastsupper = GetString("thelastsupper", languageString);
		result.deerHunter = GetString("deerHunter", languageString);
		result.exhibitionA = GetString("exhibitionA", languageString);
		result.exhibitionB = GetString("exhibitionB", languageString);
		result.silence = GetString("silence", languageString);
		result.silenceDescription = GetString("silenceDescription", languageString);
		result.timeAttackDescription = GetString("timeAttackDescription", languageString);
		result.swirlMansionDescription = GetString("swirlMansionDescription", languageString);
		result.meal = GetString("meal", languageString);
		result.stroll = GetString("stroll", languageString);
		result.watch = GetString("watch", languageString);
		result.postdinner = GetString("postdinner", languageString);
		result.mutationDescription = GetString("mutationDescription", languageString);
		result.mutation = GetString("mutation", languageString);
		result.extremeLock = GetString("extremeLock", languageString);
		result.exitDescription = GetString("exitDescription", languageString);
		result.no = GetString("no", languageString);
		return result;

	}
	private string GetString(string name, string language)
	{
		try { return (string)languagePacks[language][name]; }
		catch { return string.Empty; }
	}
	private void ApplyLanguage()
	{
		for (int i = 0; i < textsOnScene.Count; i++)
		{
			textsOnScene[i].SetText();
		}
	}

}
