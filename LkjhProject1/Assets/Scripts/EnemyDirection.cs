using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyDirection : MonoBehaviour {
	public LayerMask targetMask;
	private Transform target;
	private Animator animator;
	public float distance;
	public float delay;
	public bool isTakeStun;
	public Transform findTarget;
	public AudioClip catchClip;
	[Header("Timer")]
	public float isTakeStunDuration;
	public float waitingTime;
	public float stunTime;

	private AudioSource audioSource;
	private float timer;
	private bool isDetect;
	private PlayerController playerController;
	private bool IsStun;
	// Use this for initialization
	void Start () {
		target = GameObject.FindGameObjectWithTag("Player").transform;
		playerController = target.GetComponentInParent<PlayerController>();
		animator = GetComponentInChildren<Animator>();
		audioSource = GetComponent<AudioSource>();

		GameManager.GetInstance.OnGameReset += EnemyReset;
		SoundManager.GetInstance.VolumeChanged += VolumeSet;
		playerController.OnPlayerDead += OnplayerDead;
		playerController.OnPlayerItemEat += Stun;

		VolumeSet();
		isTakeStun = false;
		isDetect = false;
		IsStun = false;
		timer = 0;
		StartCoroutine(TakeStun(delay));
		StartCoroutine(FindVisibleTarget());
	}
	private void OnplayerDead()
	{
		audioSource.mute = true;
	}
	IEnumerator FindVisibleTarget()
	{
		while (true)
		{
			if (isTakeStun)
			{
				Debug.DrawRay(findTarget.position, findTarget.up * 10f, Color.red);
				if (Physics.Raycast(findTarget.position, findTarget.up, 10f, targetMask))
				{
					playerController.TakeStun(stunTime);
					isDetect = true;
				}
			}
			yield return null;
		}
			
	}

	IEnumerator TakeStun(float delay)
	{
		while (true)
		{
			
				animator.SetBool("IsStun", true);
				animator.SetBool("CatcherOn", false);
				animator.SetBool("CatcherOff", false);
				isTakeStun = true;
				while (timer < isTakeStunDuration)
				{
					if(!IsStun)
						timer += Time.deltaTime;
					if (isDetect)
						break;
					yield return null;
				}
			
			//				yield return new WaitForSeconds(isStunTimer);
			if (isDetect)
			{
				timer = 0;
				animator.SetBool("PlayerCatch", true);
				isTakeStun = false;
				animator.SetBool("IsStun", false);
				animator.SetBool("CatcherOn", false);
				animator.SetBool("CatcherOff", false);
				audioSource.PlayOneShot(catchClip);
				while (timer < stunTime)
				{
					if (!IsStun)
						timer += Time.deltaTime;
					
					yield return null;
				}
				timer = 0;
				animator.SetBool("PlayerPut", true);
				animator.SetBool("PlayerCatch", false);
				while (timer < 1f)
				{
					if (!IsStun)
						timer += Time.deltaTime;

					yield return null;
				}
				animator.SetBool("PlayerPut", false);
			}
			
			isTakeStun = false;
			timer = 0;
			isDetect = false;
			animator.SetBool("CatcherOn", true);
			animator.SetBool("IsStun", false);
			animator.SetBool("CatcherOff", false);
			while (timer < waitingTime)
			{
				if (!IsStun)
					timer += Time.deltaTime;

				yield return null;
			}
			timer = 0;
			animator.SetBool("CatcherOff", true);
			animator.SetBool("IsStun", false);
			animator.SetBool("CatcherOn", false);
			while (timer < 1.4f)
			{
				if (!IsStun)
					timer += Time.deltaTime;

				yield return null;
			}

		}
	}
	void Stun()
	{

		StartCoroutine(EnemyStun());

	}
	IEnumerator EnemyStun()
	{
		bool originTakeStun = isTakeStun;
		
		IsStun = true;
		isTakeStun = false;
		animator.speed = 0;
		
		yield return new WaitForSeconds(6f);
		
		animator.speed = 1;
		
		IsStun = false;
		isTakeStun = originTakeStun;
	}
	private void EnemyReset()
	{
		StopAllCoroutines();
		animator.speed = 1;
		isTakeStun = false;
		isDetect = false;
		IsStun = false;
		timer = 0;
		StartCoroutine(TakeStun(delay));
		StartCoroutine(FindVisibleTarget());
		audioSource.mute = false;

	}
	private void VolumeSet()
	{ 
		audioSource.volume = SoundManager.GetInstance.EffectVolume;
	}
	private void OnDestroy()
	{
		if (SoundManager.GetInstance != null)
		{
			SoundManager.GetInstance.VolumeChanged -= VolumeSet;
		}
		
		if (playerController != null)
			playerController.OnPlayerDead -= OnplayerDead;

	}


}
