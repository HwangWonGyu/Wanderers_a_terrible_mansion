using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearcherControll : MonoBehaviour
{
	[Header("ViewVariable")]
	public float viewRadius;
	[Range(0, 360)]
	public float viewAngle;
	public LayerMask targetMask;
	public LayerMask obstacleMask;
	public Transform player;
	public Transform viewTransform;
	[Header("Repeat")]
	public float repeatTime;
	public bool IsScan;
	private Animator animator;
	[Header("AudioClip")]
	public AudioClip chaseClip;
	public AudioClip vaseClip;
	private AudioSource audioSource;
	private bool IsStun;
	private Coroutine coroutine;
	private PlayerController playerController;
	public float stunTime;
	// Use this for initialization
	private void Awake()
	{
		animator = GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();
			
		IsScan = false;
		IsStun = false;
	}
	void Start()
	{
		playerController = GameObject.Find("Player").GetComponent<PlayerController>();
		playerController.OnPlayerDead += OnplayerDead;
		playerController.OnPlayerItemEat += Stun;
		GameManager.GetInstance.OnGameReset += ResetEnemy;
		SoundManager.GetInstance.VolumeChanged += VolumeSet;
		AdsTestWonGyu.GetInstance.ShowresultFinished += ResetEnemy;
		StartCoroutine(PlayScanAnimationrepeatedly(repeatTime));
		VolumeSet();
	}
	private void OnplayerDead()
	{
		audioSource.mute = true;
	}
	public void ResetEnemy()
	{
		animator.speed = 1;
		audioSource.mute = false;
		IsScan = false;
		IsStun = false;
	}
	IEnumerator PlayScanAnimationrepeatedly(float delay)
	{
		while (true)
		{
			yield return new WaitForSeconds(delay);
			
			animator.SetBool("Scan", true);
			audioSource.PlayOneShot(vaseClip);
		}
	}
	public void Stun()
	{
		StartCoroutine(EnemyStun());
	}
	IEnumerator EnemyStun()
	{
		IsStun = true;
		animator.speed = 0;
		audioSource.mute = true;
		yield return new WaitForSeconds(stunTime);
		audioSource.mute = false;
		animator.speed = 1;
		IsStun = false;
		 
	}
	public void AnimOff()
	{
		animator.SetBool("Scan", false);
	}
	public void PlayChaseClip()
	{
		
		audioSource.PlayOneShot(chaseClip);
		coroutine = StartCoroutine(FindTarget());
	}
	public void IsScanOn()
	{
		
		IsScan = true;
	}
	public void IsScanOff()
	{
		
		//animator.SetBool("Scan", false);
		IsScan = false;
		StopCoroutine(coroutine);
	}
	IEnumerator FindTarget()
	{
		while (true)
		{
			
			if (IsScan)
				FindVisibleTargets();
			yield return null;
			//yield return new WaitForSeconds(delay);

		}
	}
	void FindVisibleTargets()
	{
		if (playerController.isDead)
			return;
		if (IsStun)
			return;
		player = null;
		Collider[] targetsInViewRadius = Physics.OverlapSphere(viewTransform.position, viewRadius, targetMask);
		for (int i = 0; i < targetsInViewRadius.Length; ++i)
		{
			Transform target = targetsInViewRadius[i].transform;
			Vector3 dirToTarget = (target.position - viewTransform.position).normalized; // target의 방향 찾는다

			if (Vector3.Angle(viewTransform.forward, dirToTarget) < viewAngle / 2)
			{
				float dstToTarget = Vector3.Distance(viewTransform.position, target.position);
				if (!Physics.Raycast(viewTransform.position, dirToTarget, dstToTarget, obstacleMask))
				{
					player = target;
				}
			}
		}
		if (player != null)
		{
			if (playerController.playerState != GlobalData.PlayerState.Sit)
			{
				
				playerController.OnPlayerDeathWithScan();
			}
		}
	}
	private void VolumeSet()
	{
		audioSource.volume = SoundManager.GetInstance.EffectVolume;
	}
	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal)
			angleInDegrees += viewTransform.eulerAngles.y;

		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}
	private void OnDestroy()
	{
		if (SoundManager.GetInstance != null)
		{
			SoundManager.GetInstance.VolumeChanged -= VolumeSet;
		}
		if(AdsTestWonGyu.GetInstance !=null)
			AdsTestWonGyu.GetInstance.ShowresultFinished -= ResetEnemy;
		if (playerController != null)
			playerController.OnPlayerDead -= OnplayerDead;
		if (GameManager.GetInstance != null)
			GameManager.GetInstance.OnGameReset -= ResetEnemy;

	}
}
