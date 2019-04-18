using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class NavMeshAgentTypeChange : MonoBehaviour {
	private NavMeshAgent agent;
	private int originType;
	private int changeType=0;
	EnemyController controller;
	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent>();
		
		controller = GetComponent<EnemyController>();
		controller.OnStateChange += EnemyStateChange; 
		originType = agent.agentTypeID;
		changeType = 0;
	}
	void EnemyStateChange()
	{
		if (controller.enemyState == GlobalData.EnemyState.Chase)
			agent.agentTypeID = changeType;
		else
			agent.agentTypeID = originType;
	}
	private void OnDestroy()
	{
		controller.OnStateChange -= EnemyStateChange;
	}

}
