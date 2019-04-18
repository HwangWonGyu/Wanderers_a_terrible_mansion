using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour {
	public float viewRadius;
	public float hearRadius;
	[Range(0,360)]
	public float viewAngle;

	public LayerMask targetMask;
	public LayerMask obstacleMask;
    public Transform chaseTarget;

    public Transform player; // Editor 때문에 public 으로 해둠 Private 로 바꿀것
	protected Vector3 hear_targetPos;
	public float timer = 5f;
	private float timerCount;
	private Vector3 nullpos = new Vector3(-100, -100, -100);
	public GlobalData.EnemyState enemyState;
	public GlobalData.EnemyType enemyType;
	public event System.Action OnStateChange;

	public Transform viewTransform;
	protected IEnumerator FindTargetsWithDelay(float delay)
	{
		hear_targetPos = nullpos;
		while (true)
		{
			yield return new WaitForSeconds(delay);
			if (enemyState != GlobalData.EnemyState.Wait)
            {
                FindVisibleTargets();
                if (enemyType != GlobalData.EnemyType.Type3)
                    FindAudibleTargets();
            }
        }
	}
	void FindAudibleTargets()
	{

		//hear_target = null;
		Collider[] targetsInHearRadius = Physics.OverlapSphere(viewTransform.position, hearRadius, targetMask);
		for (int i = 0; i < targetsInHearRadius.Length; ++i)
		{
			Transform target = targetsInHearRadius[i].transform;

			if (target.GetComponentInParent<PlayerController>().soundedState)
				hear_targetPos = target.position;
		}
		if (enemyState == GlobalData.EnemyState.Patrol)
		{
			if (hear_targetPos != nullpos)
			{
				EnemyStateChange(GlobalData.EnemyState.ChaseWithHear);
				StartCoroutine(ChaseWithHear(10.0f));
			}
		}
		
	}
	IEnumerator ChaseWithHear(float delay)
	{
		//yield return new WaitForSeconds(delay);
	
			while (Vector3.SqrMagnitude(viewTransform.position - hear_targetPos) > 3.0)
			{
				yield return null;
			}

        if (enemyState == GlobalData.EnemyState.ChaseWithHear)
        {
            EnemyStateChange(GlobalData.EnemyState.Patrol);
            hear_targetPos = nullpos;
        }
			
		
	}

	void FindVisibleTargets()  // enemy State 를 Chase 로 바꾸는 역할과 Enemy State를 다시 Patrol로 바꾸는 역할
	{
		player = null;
		Collider[] targetsInViewRadius = Physics.OverlapSphere(viewTransform.position, viewRadius, targetMask); // Radius 반경안에 있는 모든 Target 찾는다

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
		if (enemyState == GlobalData.EnemyState.Chase)  
		{
			if (player != null) 
			{
				timerCount = Time.time;
			}
			else //적이 Count동안 안보인다면 패트롤로
			{
				if (Time.time - timerCount >= timer)
				{
					if (enemyType == GlobalData.EnemyType.Type3)
					{
						EnemyStateChange(GlobalData.EnemyState.Idle);
					}
					else
						EnemyStateChange(GlobalData.EnemyState.Patrol);
				}

			}
		}
		else
		{
			if (player != null)
			{
				timerCount = Time.time;
                
				EnemyStateChange(GlobalData.EnemyState.Chase);

			}
		}
		

	}
    public void EnemyStateChange(GlobalData.EnemyState targetState)
    {
        if (enemyState == GlobalData.EnemyState.Chase)
        {
            if (targetState != GlobalData.EnemyState.Chase) //현재 Chase 상태인데 다른상태로 바꾼다면 ChaseCount 감소
			{
                GameManager.GetInstance.ChaseCountDown();
			}
        }
        else if (enemyState != GlobalData.EnemyState.Chase)
        {
            if (targetState == GlobalData.EnemyState.Chase)  // 현재 Chase 상태가 아닌데 Chase 로 바꿀시 ChaseCount 증가
                GameManager.GetInstance.ChaseCountUp();
        }
		enemyState = targetState;
		if(OnStateChange!=null)
			OnStateChange();
	}

	public Vector3 DirFromAngle(float angleInDegrees,bool angleIsGlobal)
	{
		if (!angleIsGlobal)
			angleInDegrees += viewTransform.eulerAngles.y;
		
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}

}
