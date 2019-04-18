using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SoundManager : Singleton<SoundManager> {
    private AudioSource bgmaudioSource;
    private AudioSource effectaudioSource;
    private AudioSource bgmRainaudioSource;
	[Header("BGM Clips")]
    [SerializeField]
    private AudioClip bgmChaseClip;
    [SerializeField]
    private AudioClip bgmNomalClip;
    [Header("Effect Clips")]
    [SerializeField]
    private AudioClip itemEatClip;
	[SerializeField]
	private AudioClip stunItemEatClip;
	[SerializeField]
    private AudioClip playerDetectClip;
    [SerializeField]
    private AudioClip uiButtonClickClip;
    [SerializeField]
    private AudioClip thunderClip;
	[SerializeField]
	private AudioClip itemAllEatClip;
	[SerializeField]
	private AudioClip itemStunClip;
	[SerializeField]
	private AudioClip potionItemEatClip;
	[SerializeField]
	private AudioClip activePotionItemClip;
	[Header("Volume")]
    [SerializeField]
    private float bgmSoundDownValue=0.3f;
    [SerializeField]
    private float bgmRainDownValue = 0.15f;
    [SerializeField]
    private float effectSoundDownValue = 0.3f;
	
	

    private Coroutine chaseBGM;
    private float effectVolume;
    private float bgmVolume;
    private float bgmRainVolume;
    public float EffectVolume
    {
        get
        {
            return effectVolume;
        }

        set
        {
            effectVolume = value;
        }
    }
    public float BgmVolume
    {
        get
        {
            return bgmVolume;
        }

        set
        {
            bgmVolume = value;
        }
    }
    public event Action VolumeChanged;
    private void Awake()
    {
      
        if (sInstance == null)
        {
            sInstance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != GlobalData.SCENE_TITLE)
        {
            SetMuteBGM(false);
            
            StartCoroutine(InitializeSoundSetting());
        }
        if (bgmRainaudioSource != null)
        {
            bgmRainaudioSource.mute = false;
            
        }
    }
	private void Start()
    {
        bgmaudioSource = GetComponent<AudioSource>();
        effectaudioSource = transform.Find("Effect").GetComponent<AudioSource>();
        bgmRainaudioSource = transform.Find("BgmRain").GetComponent<AudioSource>();
        
		StartCoroutine(InitializeSoundSetting());
    }

    public void PlayOneShotThunderClip()
    {
        if (thunderClip != null)
        {
            effectaudioSource.PlayOneShot(thunderClip, EffectVolume);
        }
    }
    public void PlayOneShotItemEatClip()
    {
        if(itemEatClip !=null)
            effectaudioSource.PlayOneShot(itemEatClip, EffectVolume* effectSoundDownValue);
    }
	public void PlayOneShotStunItemEatClip()
	{
		if (stunItemEatClip != null)
			effectaudioSource.PlayOneShot(stunItemEatClip, EffectVolume * effectSoundDownValue);
	}
	public void PlayOneShotPlayerDetectClip()
    {
        if (playerDetectClip != null)
            effectaudioSource.PlayOneShot(playerDetectClip, EffectVolume);
    }
    public void PlayOneShotUIBtnClick()
    {
        if (uiButtonClickClip != null)
            effectaudioSource.PlayOneShot(uiButtonClickClip, EffectVolume);
    }
	public void PlayOneShotAllItemEatClip()
	{
		if (itemAllEatClip != null)
			effectaudioSource.PlayOneShot(itemAllEatClip, EffectVolume * effectSoundDownValue);
	}
	public void PlayItemStunEatClip()
	{
		if (itemStunClip != null)
		{
			StartCoroutine(PlayItemStunDelay(6f));
		}
			//effectaudioSource.Play(itemStunClip, EffectVolume * effectSoundDownValue); 
	}
	public void PlayOneShotItemPotionEatClip()
	{
		if (potionItemEatClip != null)
			effectaudioSource.PlayOneShot(potionItemEatClip);
	}
	public void PlayOneShotItemPotionActiveClip()
	{
		if (activePotionItemClip != null)
			effectaudioSource.PlayOneShot(activePotionItemClip);
	}
	IEnumerator PlayItemStunDelay(float delay)
	{
		effectaudioSource.volume = EffectVolume * 0.5f;
		effectaudioSource.clip = itemStunClip;
		effectaudioSource.loop = true;
		effectaudioSource.Play();
		yield return new WaitForSeconds(delay);
		effectaudioSource.Stop();
		effectaudioSource.loop = false;
		effectaudioSource.clip = null;
		effectaudioSource.volume = EffectVolume;
	}

	public void PlayBGM()
    {
        bgmRainaudioSource.volume = bgmRainVolume;
        bgmaudioSource.volume = bgmVolume;
        bgmaudioSource.clip = bgmNomalClip;
        bgmaudioSource.loop = true;
        bgmaudioSource.Play();
    }
    public void PlayChaseBGM()
    {
        if (chaseBGM != null)
        {
            StopCoroutine(chaseBGM);
            
        }
        chaseBGM=StartCoroutine(PlayChaseBGMCoroutine());
    }

    IEnumerator PlayChaseBGMCoroutine()
    {
        bgmaudioSource.volume = bgmVolume;
        bgmaudioSource.clip = bgmChaseClip;
        bgmaudioSource.loop = true;
        bgmaudioSource.Play();
       
        while (GameManager.GetInstance.ChaseCount > 0)
        {
            yield return null;
        }
        while (bgmaudioSource.volume > 0)
        {
            bgmaudioSource.volume -= Time.deltaTime*0.1f;
            yield return null;
        }

        PlayBGM();
        chaseBGM = null;
    }
    IEnumerator InitializeSoundSetting()
    {
        while (!SaveLoadDataManager.GetInstance.isLoadingComplete)
            yield return null;
        SetVolume();
        
    }
    public void SetVolume()
    {
        EffectVolume = SaveLoadDataManager.GetInstance.data.EffectVolume;
        bgmVolume = SaveLoadDataManager.GetInstance.data.BgmVolume* bgmSoundDownValue;
        bgmRainVolume = SaveLoadDataManager.GetInstance.data.BgmVolume * bgmRainDownValue;
        bgmRainaudioSource.volume = bgmRainVolume;
        bgmaudioSource.volume = BgmVolume;
        effectaudioSource.volume = EffectVolume;
        if (VolumeChanged != null)
            VolumeChanged();
        
    }
    public void SetMuteBGM(bool value)
    {
        bgmaudioSource.mute = value;
        bgmRainaudioSource.mute = value;
    }
   






}
