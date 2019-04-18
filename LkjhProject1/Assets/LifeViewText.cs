using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LifeViewText : MonoBehaviour {
	Text lifetext;
	private void Awake()
	{
		lifetext = GetComponent<Text>();
	}
	private void OnEnable()
	{
		lifetext.text =GameManager.GetInstance.lives.ToString()+" / "+GameManager.GetInstance.Originlife.ToString();
	}
}
