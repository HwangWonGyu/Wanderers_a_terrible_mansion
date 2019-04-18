using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour {
    private AudioSource audioSource;
    private AudioSource footStepAudioSource;
    [Header("ITEM Clip")]
    [SerializeField]
    private AudioClip playerRunClip;
    [SerializeField]
    private AudioClip playerWalkClip;
    [SerializeField]
    private AudioClip playerLowStaminaClip;
	// Use this for initialization
	void Start ()
    {
        audioSource = GetComponent<AudioSource>();
        footStepAudioSource=transform.Find("VisibleTarget").GetComponent<AudioSource>();
        VolumSet();
        SoundManager.GetInstance.VolumeChanged += VolumSet;
        PlayerAudioSetInitialize();
    }
    
    public void PlayerAudioSetInitialize()
    {
        StartCoroutine(PlayerAudioSetInitializeCoroutine());
        
    }
    IEnumerator PlayerAudioSetInitializeCoroutine()
    {
        yield return null;
        SetPlayerFootStepAudiosourceMute(true);
        footStepAudioSource.clip = playerWalkClip;
        audioSource.clip = null;
        footStepAudioSource.Play();
        yield return null;
    }
    public void PlayplayerRun()
    {
        
        footStepAudioSource.clip = playerRunClip;
        footStepAudioSource.Play();
        audioSource.clip = playerLowStaminaClip;
        audioSource.Play();

    }
    public void PlayPlayerWalk()
    {
        footStepAudioSource.clip = playerWalkClip;
        footStepAudioSource.Play();
        audioSource.clip = null;
    }
    public void PlayPlayerSit()
    {
        footStepAudioSource.clip = null;
        footStepAudioSource.Stop();
        audioSource.clip = null;
    }
    public void SetPlayerFootStepAudiosourceMute(bool value)
    {
        footStepAudioSource.mute = value;
        audioSource.mute = value;
        
    }
    public void VolumSet()
    {
        audioSource.volume = SoundManager.GetInstance.EffectVolume;
        footStepAudioSource.volume = SoundManager.GetInstance.EffectVolume;
        
    }
    private void OnDestroy()
    {
        if(SoundManager.GetInstance!=null)
            SoundManager.GetInstance.VolumeChanged -= VolumSet;
    }




}
