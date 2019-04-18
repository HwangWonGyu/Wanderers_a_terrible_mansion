using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class Opening : MonoBehaviour {
    #region [Public Variable]
    #region [Audio]
    public AudioSource audioSource;
    public AudioSource bgmaudioSource;
	public AudioSource effectSource;
    public AudioClip owl;
    public AudioClip walk;
    public AudioClip doorClose;
    public AudioClip rain;
	public AudioClip btnClick;
	#endregion
	#endregion
	GlobalData.Language language;
    public GameObject panelYesOrNo;
    public GameObject nameFrame;
	public GameObject animationImage;
	public Image scriptBackGround;
    public Image backGround;
    public Image illust;
    public Image houseImage;
	
    public Text text;
    public Image fadePanel;
    [Header("Dialogue")]
    public Dialogue[] dialogues;
    [Header("Setting")]
    public float startDelay = 1f;
    public float typeDelay = 0.1f;
    public float fadeSpeed = 1f;
    public float scrollSpeed=100f;
    public float bgmvolumeDown = 0.5f;
    public bool IsTyping=false;
    public bool scriptSkip = false;
    
    #region [Private Variable]
    
    #endregion
    void Start()
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
        StartCoroutine(OpeningStart());
        StartCoroutine(PlayRandomOneShot(owl));

	}
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsTyping)
        {
            if(!EventSystem.current.IsPointerOverGameObject())
                scriptSkip = true;
        }
    }
    IEnumerator OpeningStart()
    {
        Color fadeColor = new Color(1, 1, 1, 0);
        audioSource.volume = 0f;
        bgmaudioSource.volume = 0f;
        while (fadeColor.a < 1)
        {
            
            fadeColor.a += Time.deltaTime * fadeSpeed;
            audioSource.volume += Time.deltaTime * fadeSpeed;
            bgmaudioSource.volume += Time.deltaTime * fadeSpeed*bgmvolumeDown;
            backGround.color = fadeColor;
            yield return null;
        }
        
        StartCoroutine(Type());
    }
   
    IEnumerator Type()
    {
        yield return new WaitForSeconds(startDelay);
        scriptBackGround.enabled = true;
        IsTyping = true; //타이핑 가능한지
        int i = 0;
        while (i < dialogues.Length)
        {

            if (IsTyping)
            {
                if (dialogues[i].sprite == null)
                {   illust.enabled = false;
                    nameFrame.SetActive(false);
                }
                else
                {
                    illust.sprite = dialogues[i].sprite;
                    if (dialogues[i].name.Length>0)
                    {
                        nameFrame.GetComponentInChildren<Text>().text = dialogues[i].name[(int)language];
                        nameFrame.SetActive(true);
                    }
                    else
                        nameFrame.SetActive(false);
                    illust.enabled = true;

                }
                
                for (int j = 0; j <= dialogues[i].script[(int)language].Length; ++j)
                {
                    
                    if (scriptSkip)
                    {
                        text.text = dialogues[i].script[(int)language];
                        scriptSkip = false;
                        break;
                    }
                    text.text = dialogues[i].script[(int)language].Substring(0, j); //한글자씩 text에 추가.
                    yield return new WaitForSeconds(typeDelay);
                }
                IsTyping = false; //다추가했다면 타이핑 불가능으로
				animationImage.SetActive(true);

			}
            else
            {
                if (Input.GetMouseButtonDown(0)) // Script가 다 완료되었을때 다음 Script로
                {
                    if (!EventSystem.current.IsPointerOverGameObject())
                    {
                        if (!string.IsNullOrEmpty(dialogues[i].Method))
                        {
                            yield return StartCoroutine(dialogues[i].Method);

                        }
                        i++;
                        IsTyping = true;
						animationImage.SetActive(false);
					}
                }
            }
            yield return null;

        }

    }
    IEnumerator PlayOneShotWalkClip()
    {
        audioSource.PlayOneShot(walk);
        
        yield return new WaitForSeconds(1.5f);
        Color fadeColor = new Color(1, 1, 1, 0);
        illust.enabled = true;
        while (fadeColor.a < 1)
        {
            fadeColor.a += Time.deltaTime * fadeSpeed;
            illust.color = fadeColor;
            yield return null;

        }
            
    }
    IEnumerator PlayRainSoundAndFadeOut()
    {
        illust.enabled = false;
        nameFrame.SetActive(false);
        text.text = null;
        audioSource.clip = rain;
        audioSource.Play();
        Color fadecolor = new Color(1, 1, 1, 1);
        audioSource.volume = 0;
        bgmaudioSource.volume = 0;
        while (fadecolor.a >0)
        {
            fadecolor.a -= Time.deltaTime * fadeSpeed;
            houseImage.color = fadecolor;
            audioSource.volume += Time.deltaTime * fadeSpeed*0.7f;
            bgmaudioSource.volume += Time.deltaTime * fadeSpeed* bgmvolumeDown;

            yield return null;
        }
    }
    IEnumerator PlayWalkSoundLongTime()
    {
        audioSource.clip = walk;
        audioSource.volume = 1f;
        audioSource.Play();
        yield return new WaitForSeconds(2.0f);
        Color fadeColor = new Color(1, 1, 1, 1);

        while (audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime * fadeSpeed;
            fadeColor.a -= Time.deltaTime * fadeSpeed;
            illust.color = fadeColor;
            yield return null; 
        }

    }
    IEnumerator PlayOneShotDoorClose()
    {
        audioSource.PlayOneShot(doorClose);
        yield return new WaitForSeconds(3.5f);
        audioSource.PlayOneShot(walk);
        
        yield return new WaitForSeconds(3f);
        
    }
    IEnumerator PlayRandomOneShot(AudioClip audioClip)
    {
        while (true)
        { int rand = Random.Range(11, 16);
            yield return new WaitForSeconds(rand);
            audioSource.PlayOneShot(audioClip);
        }

    }
    IEnumerator FadeOut()
    {
        Color fadeColor = new Color(1, 1, 1, 1);
        text.text = null;
        while (fadeColor.a > 0)
        {
            fadeColor.a -= Time.deltaTime * fadeSpeed;
            audioSource.volume -= Time.deltaTime * fadeSpeed;
            bgmaudioSource.volume -= Time.deltaTime * fadeSpeed* bgmvolumeDown;

            backGround.color = fadeColor;
            scriptBackGround.color = fadeColor;
            text.color = fadeColor;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        backGround.enabled = false;
        yield return StartCoroutine(FadeIn());
    }
    IEnumerator FadeIn()
    {
        Color fadeColor = new Color(1, 1, 1, 0);
        houseImage.enabled = true;
        audioSource.clip = null;
        
        scriptBackGround.gameObject.SetActive(false);
        
        while (fadeColor.a < 1)
        {
            fadeColor.a += Time.deltaTime * fadeSpeed;
            audioSource.volume += Time.deltaTime * fadeSpeed;
            bgmaudioSource.volume += Time.deltaTime * fadeSpeed* bgmvolumeDown;
            houseImage.color = fadeColor;
            scriptBackGround.color = fadeColor;
            text.color = fadeColor;
            yield return null;
        }
        yield return StartCoroutine(HouseImageScroll());
        scriptBackGround.gameObject.SetActive(true);
    }
    IEnumerator HouseImageScroll()
    {
        while (houseImage.rectTransform.localPosition.y < 500)
        {
            houseImage.rectTransform.localPosition += Vector3.up * Time.deltaTime* scrollSpeed;
            yield return null;
        }
        yield return new WaitForSeconds(2f);
    }
    IEnumerator StartGameScene()
    {
        Color fadeColor = new Color(1, 1, 1, 1);
        
        while (fadeColor.a > 0)
        {
            fadeColor.a -= Time.deltaTime * fadeSpeed;
            audioSource.volume -= Time.deltaTime * fadeSpeed;
            bgmaudioSource.volume -= Time.deltaTime * fadeSpeed * bgmvolumeDown;
            scriptBackGround.color = fadeColor;
            text.color = fadeColor;
            yield return null;
        }
        SceneManager.LoadScene("TitleScene");
        
    }
    
    public void SkipBtnClicked()
    {
        panelYesOrNo.SetActive(true);
		effectSource.PlayOneShot(btnClick);


	}
    public void YesBtnClicked()
    {
		effectSource.PlayOneShot(btnClick);
        StartCoroutine(LoadScene());
	}
	
    IEnumerator LoadScene()
    {
        Color fadeColor = new Color(0, 0, 0, 0);
		fadePanel.gameObject.SetActive(true);
        fadePanel.color = fadeColor;
        while (fadeColor.a < 1)
        {
            fadeColor.a += Time.deltaTime * fadeSpeed;
            fadePanel.color = fadeColor;
            yield return null;
        }
        SceneManager.LoadScene(GlobalData.SCENE_TITLE);
    }
    public void NoBtnClicked()
    {
        panelYesOrNo.SetActive(false);
		effectSource.PlayOneShot(btnClick);
	}
    [System.Serializable]
    public struct Dialogue
    {
        public Sprite sprite;
		[TextArea(1, 2)]
		public string[] script;//=new string[]();
        public string Method;
        public string[] name;
    }
}
