using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class DisplayExit : MonoBehaviour {
	private GameObject player;
	private Transform goal;
	private RawImage image;
	private void Awake()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}
	private void Start()
	{
		image = GetComponent<RawImage>();
	}
	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name != "TitleScene")
		{
			player = GameObject.FindGameObjectWithTag("Player");
			goal = GameObject.Find("Goal").transform;
		}
	}
	
	void Update ()
	{
		if (player != null)
		{
			Vector2 newvector=new Vector2 (goal.position.x - player.transform.position.x, goal.position.z-player.transform.position.z);
			image.rectTransform.localPosition = newvector;
			//image.rectTransform.locaSetPositionAndRotation(newvector,Quaternion.identity);
//			transform.position = newvector;

		}
	}
}
