using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Tutorials : MonoBehaviour {
	public Image scriptBackGround;
	public Image animationImage;
	public Text text;
	public bool IsTyping = false;
	GlobalData.Language language;
	public float typeDelay = 0.1f;
	
	public dialogue[] dialogues;
	public bool scriptSkip = false;
	private void OnEnable()
	{ 
		switch (Application.systemLanguage)
		{
			case SystemLanguage.English:
				language = GlobalData.Language.English;
				break;
			case SystemLanguage.Korean:
				language = GlobalData.Language.Korean;
				break;
			default:
				language = GlobalData.Language.English;
				break;
		}
		StartCoroutine(Type());
	}
	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && IsTyping)
		{
			
				scriptSkip = true;
		}
	}

	IEnumerator Type()
	{
		
		scriptBackGround.enabled = true;
		IsTyping = true; //타이핑 가능한지
		int i = 0;
		while (i < dialogues.Length)
		{

			if (IsTyping)
			{
				for (int j = 0; j <= dialogues[i].scripts[(int)language].Length; ++j)
				{
					if (scriptSkip)
					{
						text.text = dialogues[i].scripts[(int)language];
						scriptSkip = false;
						break;
					}
					text.text = dialogues[i].scripts[(int)language].Substring(0, j); //한글자씩 text에 추가.
					yield return new WaitForSeconds(typeDelay);
				}
				IsTyping = false; //다추가했다면 타이핑 불가능으로
				animationImage.gameObject.SetActive(true);
			}
			else
			{
				if (Input.GetMouseButtonDown(0)) // Script가 다 완료되었을때 다음 Script로
				{
					
					
						i++;
						IsTyping = true;
						SoundManager.GetInstance.PlayOneShotItemEatClip();
						animationImage.gameObject.SetActive(false);
					
				}
			}
			yield return null;

		}
		gameObject.SetActive(false);

	}
	[System.Serializable]
	public struct dialogue
	{
		[TextArea(1, 4)]
		public string[] scripts;
	}
}
