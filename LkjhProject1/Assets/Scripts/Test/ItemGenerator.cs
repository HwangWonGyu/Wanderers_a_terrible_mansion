using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class ItemGenerator : MonoBehaviour {

	public GameObject itemPrefab;
	
	
	public List<GameObject> itemList;
	public int itemNumber;
	
	

	public float yPos=0.5f;
	public float itemInterval=5.0f;
	public Transform itemParent;

	public void MakeItem()
	{
		
		for (int i = 0; i < itemNumber; ++i)
		{
			for (int j = 0; j < itemNumber; ++j)
			{
				Vector3 makePos = transform.position + new Vector3(i * itemInterval, yPos, j* itemInterval);
				NavMeshHit hit;
				if (NavMesh.SamplePosition(makePos, out hit, 1.0f, NavMesh.AllAreas))
				{
					
					GameObject go= Instantiate(itemPrefab, makePos, Quaternion.identity, itemParent);
					itemList.Add(go);
					
				}
			}
		}
	}
	


	public void DeleteItem()
	{
		for (int i = 0; i < itemList.Count; ++i)
		{
			DestroyImmediate(itemList[i].gameObject);
		}
		itemList.Clear();

	}


}
