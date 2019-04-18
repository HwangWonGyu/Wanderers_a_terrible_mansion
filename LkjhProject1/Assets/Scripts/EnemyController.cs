using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyController : FieldOfView {

    public Transform patrolTarget;
	public Transform patrolTargetParent;
	public float walkSpeed;
	public float runSpeed;

    private Vector3 originPos;
    private Quaternion originRot;
    private Transform originPatrolTarget;

	private NavMeshAgent navMeshAgent;
	private Material materialOrigin;
	private Animator animator;
	private Coroutine playAnimation;
	private bool resetComplete=true;
    AudioSource[] enemyAudioSources;
    //private bool playAnim=false;
    public Material materialChange;
	public float stunTime;
	public Renderer eyeRenderer;
	bool IsStun;
	/// <summary> AI가 다음 타겟 근처에 도착하면 멈춰야하는 거리 </summary>
	public float stoppingDistance;

	/// <summary> 워커가 랜덤으로 패트롤하게 할지 결정하는 변수 </summary>
	public bool isWalkerRandomPatrol;

	/// <summary> 대기자가 아직 변신 중인지 검사하는 변수 </summary>
	public bool isType3TransformAnimating;

	void Start ()
	{
		IsStun = false;
		navMeshAgent = GetComponent<NavMeshAgent>();
        SoundManager.GetInstance.VolumeChanged += EnemyVolumSet;
		chaseTarget = GameObject.FindGameObjectWithTag("PlayerObject").transform;
        chaseTarget.GetComponent<PlayerController>().OnPlayerDead += OnPlayerDeath;
		chaseTarget.GetComponentInChildren<PlayerController>().OnPlayerItemEat += Stun;
		AdsTestWonGyu.GetInstance.ShowresultFinished += EnemyShowResultFinished;
		
		if (SceneManager.GetActiveScene().name != GlobalData.SCENE_TITLE)
			GameManager.GetInstance.OnPlayerClear += OnPlayerDeath;

		OnStateChange += OnEnemyStateChange;
		//materialOrigin = GetComponent<Renderer>().material;
		// -> 러너 모델링 Hierarchy상 구조에 따른 코드 변경 2018.11.01 원규
		materialOrigin = /*eyeRenderer.material*/eyeRenderer.sharedMaterial;
		//2018.11.21 원규
		//http://rapapa.net/?p=2472
		//Script에서 Material을 사용할 때 Render.material 호출은 Material을 Copy하여 생성하는 것이므로 Draw Call이 추가 발생하게 된다.
		//대신 Render.sharedMaterial을 사용하여 Batching이 발생하도록 공유하는 형태로 사용하는 것이 좋다.
		//따라서 나중에 바꿔서 테스트해볼것

		originPos = transform.position;
        originRot = transform.localRotation;
		animator = GetComponentInChildren<Animator>();
        enemyAudioSources = GetComponentsInChildren<AudioSource>();
        EnemyVolumSet();
		if (enemyType == GlobalData.EnemyType.Type3)
		{
			enemyState = GlobalData.EnemyState.Idle;
			viewTransform = transform.Find("FindTargetPivot").transform;
			if (navMeshAgent.isOnNavMesh)
				navMeshAgent.isStopped = true;
			GetComponent<BoxCollider>().enabled = false;
			navMeshAgent.enabled = false;
		}
		else
		{
			enemyState = GlobalData.EnemyState.Patrol;
			originPatrolTarget = patrolTarget;
			patrolTargetParent = patrolTarget.transform.parent;
			viewTransform = transform;
			if (enemyType == GlobalData.EnemyType.Worker ||
				enemyType == GlobalData.EnemyType.GhostCart)
			{
				// 2018.12.03 원규
				// 모든 워커가 다 랜덤 루트로 움직이는게 아니라 우리가 원하는 워커만 랜덤 루트로 움직이게 변경
				if (isWalkerRandomPatrol)
					patrolTarget = ReturnRandomPatrol();
				else
				{
					if (patrolTarget.GetComponent<NextTarget>().nextTarget.Length == 1)
						patrolTarget = patrolTarget.GetComponent<NextTarget>().nextTarget[0];
					else
					{
						patrolTarget = patrolTarget.GetComponent<NextTarget>().
							nextTarget[Random.Range(0, patrolTarget.GetComponent<NextTarget>().nextTarget.Length)];
					}
				}
				// 모든 워커가 다 랜덤 루트로 움직이는게 아니라 우리가 원하는 워커만 랜덤 루트로 움직이게 변경
			}

			if (navMeshAgent.isOnNavMesh)
				SetDestination(patrolTarget);
			else
			{
				// 네브메쉬 위에 안올라가있어서 SetDestination 안되는중!
			}
			
		}
		

		StartCoroutine(FindTargetsWithDelay(.1f));
		
		StartCoroutine(EnemyAction(0.01f));
		
		
	}
	private void EnemyShowResultFinished()
	{
		EnemyStateChange(GlobalData.EnemyState.Reset);
	}
	

	IEnumerator EnemyAction(float delay)
	{
		
		while (true)
		{
            yield return new WaitForSeconds(delay);
            switch (enemyState)
			{
				case GlobalData.EnemyState.Patrol: // Walker,Runner 만 해당
					if (Vector3.SqrMagnitude(patrolTarget.position - transform.position) < stoppingDistance)
					{
						if(enemyType == GlobalData.EnemyType.Runner ||
							enemyType == GlobalData.EnemyType.GhostRunner)
							patrolTarget = patrolTarget.GetComponent<NextTarget>().nextTarget[0];
						else if (enemyType == GlobalData.EnemyType.Worker ||
								enemyType == GlobalData.EnemyType.GhostCart)
						{
							// 2018.12.03 원규
							// 모든 워커가 다 랜덤 루트로 움직이는게 아니라 우리가 원하는 워커만 랜덤 루트로 움직이게 변경
							if(isWalkerRandomPatrol)
								patrolTarget = ReturnRandomPatrol();
							else
							{
								if(patrolTarget.GetComponent<NextTarget>().nextTarget.Length == 1)
									patrolTarget = patrolTarget.GetComponent<NextTarget>().nextTarget[0];
								else
								{
									patrolTarget = patrolTarget.GetComponent<NextTarget>().
										nextTarget[Random.Range(0, patrolTarget.GetComponent<NextTarget>().nextTarget.Length)];
								}
							}
							// 모든 워커가 다 랜덤 루트로 움직이는게 아니라 우리가 원하는 워커만 랜덤 루트로 움직이게 변경
						}
						if (navMeshAgent.isOnNavMesh)
							SetDestination(patrolTarget);

					}
					break;
				case GlobalData.EnemyState.ChaseWithHear: //Walker,Runner 만 해당
					SetDestination(hear_targetPos);
					break;
				case GlobalData.EnemyState.Chase: // 모두 해당
					SetDestination(chaseTarget);
					break;
				case GlobalData.EnemyState.Idle: // Type3 만 들어오는 State
					if (Vector3.SqrMagnitude(originPos - transform.position) < stoppingDistance)
					{
						if (resetComplete==false)
						{
							StopCoroutine(playAnimation);
							navMeshAgent.Warp(originPos);
							transform.localRotation = originRot;
							navMeshAgent.isStopped = true;
							
							resetComplete = true;
							StartCoroutine(EnemyType3RewindAnimation());
						}
						
					}
					else
						navMeshAgent.SetDestination(originPos);
					break;
			}
			
			
		}
	}
	Transform ReturnRandomPatrol()
	{
		int randnumber = Random.Range(0, patrolTargetParent.childCount);
		return patrolTargetParent.GetChild(randnumber);
	}
	void Stun()
	{
		
		StartCoroutine(EnemyStun());
		
	}
	IEnumerator EnemyStun()
	{
		IsStun = true;
		if(navMeshAgent.enabled)
			navMeshAgent.isStopped = true;
		animator.speed = 0;
		EnemyMute(true);
		yield return new WaitForSeconds(stunTime);
		if (enemyType == GlobalData.EnemyType.Type3) // 대기자는 Idle일경우는 Stop을 False로 하면안된다 chase
		{
			if (!resetComplete)
			{
				if (!isType3TransformAnimating)
				{
					navMeshAgent.isStopped = false;
					GetComponent<BoxCollider>().enabled = true;
					
				}
			}
		}	
		else
			navMeshAgent.isStopped = false;
		animator.speed = 1;
		EnemyMute(false);
		IsStun = false;
	}

	void SetDestination(Transform _target)
	{
		if(navMeshAgent.isOnNavMesh)
			navMeshAgent.SetDestination(_target.position);
	}
	void SetDestination(Vector3 _target)
	{
		if (navMeshAgent.isOnNavMesh)
			navMeshAgent.SetDestination(_target);
	}
	void OnEnemyStateChange()
	{
		switch (enemyState)
		{
			case GlobalData.EnemyState.Patrol:
				if (enemyType == GlobalData.EnemyType.Worker ||
					enemyType == GlobalData.EnemyType.GhostCart)
				{
					animator.SetBool("Walk", true);
					animator.SetBool("Run", false);
					navMeshAgent.speed = walkSpeed;
				}
				if(navMeshAgent.isOnNavMesh)
					SetDestination(patrolTarget);
				break;
			case GlobalData.EnemyState.Chase:
				if(!IsStun)
					SoundManager.GetInstance.PlayOneShotPlayerDetectClip();
				if (enemyType == GlobalData.EnemyType.Type3)
				{
					playAnimation = StartCoroutine(EnemyType3Animation());
					navMeshAgent.speed = runSpeed;
				}
				else if (enemyType == GlobalData.EnemyType.Worker ||
						enemyType == GlobalData.EnemyType.GhostCart)	
				{
					animator.SetBool("Walk", false);
					animator.SetBool("Run", true);
					navMeshAgent.speed = runSpeed;
				}
				SetDestination(chaseTarget);
				break;
			case GlobalData.EnemyState.ChaseWithHear:
				SetDestination(hear_targetPos);
				break;
			case GlobalData.EnemyState.Reset:
				ResetEnemy();
				break;
			case GlobalData.EnemyState.Idle:
				if(enemyType== GlobalData.EnemyType.Type3)
					navMeshAgent.speed = walkSpeed;
				break;
		}
		// Switch 문에서 밖으로 뺀이유는 Chase 일때는 머테리얼을 바꾸고 아닐경우 다시 처음 머테리얼로 되돌리기위함.
		if (enemyState == GlobalData.EnemyState.Chase)
		{
			if (materialChange != null)
				//GetComponent<Renderer>().material = materialChange;
				// -> 러너 모델링 Hierarchy상 구조에 따른 코드 변경 2018.11.01 원규
				eyeRenderer./*material*/sharedMaterial = materialChange;
				//2018.11.27 원규
				//http://rapapa.net/?p=2472
				//Script에서 Material을 사용할 때 Render.material 호출은 Material을 Copy하여 생성하는 것이므로 Draw Call이 추가 발생하게 된다.
				//대신 Render.sharedMaterial을 사용하여 Batching이 발생하도록 공유하는 형태로 사용하는 것이 좋다.
				//따라서 나중에 바꿔서 테스트해볼것
		}
		else
			//GetComponent<Renderer>().material = materialOrigin;
			// -> 러너 모델링 Hierarchy상 구조에 따른 코드 변경 2018.11.01 원규
			eyeRenderer./*material*/sharedMaterial = materialOrigin;
			//2018.11.27 원규
			//http://rapapa.net/?p=2472
			//Script에서 Material을 사용할 때 Render.material 호출은 Material을 Copy하여 생성하는 것이므로 Draw Call이 추가 발생하게 된다.
			//대신 Render.sharedMaterial을 사용하여 Batching이 발생하도록 공유하는 형태로 사용하는 것이 좋다.
			//따라서 나중에 바꿔서 테스트해볼것
	}
	void OnPlayerDeath()
    {
        EnemyMute(true);
        if(navMeshAgent.isOnNavMesh)
            navMeshAgent.isStopped = true;
        EnemyStateChange(GlobalData.EnemyState.Wait);
    }

    IEnumerator EnemyType3Animation()
	{
		
		animator.SetBool("Detect", true);
		resetComplete = false;

		//yield return new WaitForSeconds(4.667f);

		//if(!IsStun)
		//	navMeshAgent.isStopped = false;
		//print("대기자 isStopped = false");

		isType3TransformAnimating = true;
		float animationTimer = 4.667f;
		float animationTimerStartTime = Time.time;
		float animationStopTime = 0f;
		while (animationTimer > Time.time - animationTimerStartTime + animationStopTime)
		{
			if (IsStun)
			{
				animationStopTime = Time.time - animationTimerStartTime;
				yield return new WaitForSeconds(stunTime);
				animationTimerStartTime = Time.time;
			}
			else
				yield return null;
		}
		navMeshAgent.enabled = true;
		navMeshAgent.isStopped = false;
		GetComponent<BoxCollider>().enabled = true;
		
		//print("대기자 플레이어 죽이는 판정 켬");
		isType3TransformAnimating = false;
	}
	IEnumerator EnemyType3RewindAnimation()
	{
		viewRadius = 0;
		GetComponent<BoxCollider>().enabled = false;
		navMeshAgent.enabled = false;
		//print("대기자 플레이어 죽이는 판정 끔");
		
		animator.SetBool("Detect", false);
		yield return new WaitForSeconds(4.667f);
		viewRadius = 5f;
    }


	void ResetEnemy()
    {
		
        transform.localRotation = originRot;
        navMeshAgent.Warp(originPos);
		if (enemyType != GlobalData.EnemyType.Type3)
		{
			patrolTarget = originPatrolTarget;
			EnemyStateChange(GlobalData.EnemyState.Patrol);
			if(navMeshAgent.isOnNavMesh)
			{
				navMeshAgent.isStopped = false;
			}
		}
		else
		{
			EnemyStateChange(GlobalData.EnemyState.Idle);
			if(navMeshAgent.enabled)
				navMeshAgent.isStopped = true;
        }
		animator.speed = 1;
		EnemyMute(false);

    }
    public void EnemyVolumSet()
    {
        
        foreach (AudioSource audioSource in enemyAudioSources)
        {
            audioSource.volume = SoundManager.GetInstance.EffectVolume;
        }
    }
	public bool IsEnemyStun()
	{
		return IsStun;
	}
    private void EnemyMute(bool value)
    {
        foreach (AudioSource audioSource in enemyAudioSources)
        {
            audioSource.mute = value;
        }
    }
    private void OnDestroy()
    {
		if (SoundManager.GetInstance != null)
		{
			SoundManager.GetInstance.VolumeChanged -= EnemyVolumSet;
		}
		if(AdsTestWonGyu.GetInstance !=null)
			AdsTestWonGyu.GetInstance.ShowresultFinished -= EnemyShowResultFinished;
	}





}
