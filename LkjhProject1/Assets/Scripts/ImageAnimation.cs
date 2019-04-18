using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ImageAnimation : MonoBehaviour {
	private Image image;
	public Sprite[] sprites;
	public float animateSpeed;
	
	private void OnEnable()
	{
		image = GetComponent<Image>();
		StartCoroutine(Animation());
	}
	IEnumerator Animation()
	{
		while (true)
		{
			for (int i = 0; i < sprites.Length; ++i)
			{
				image.sprite = sprites[i];
				yield return new WaitForSeconds(animateSpeed);
			}
		}
	}
}
