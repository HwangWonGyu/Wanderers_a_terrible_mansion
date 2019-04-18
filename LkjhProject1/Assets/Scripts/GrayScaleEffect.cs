using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GrayScaleEffect : MonoBehaviour
{
	public float intensity;
	public Material material;

	void Awake()
	{
		material = new Material(Shader.Find("ShaderTest"));
	}

	// Update is called once per frame
	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (intensity == GlobalData.PLAYER_CAM_DEFAULT)
		{
			Graphics.Blit(source, destination);
			return;
		}
		else
		{
			material.SetFloat("_bwBlend", intensity);
			Graphics.Blit(source, destination, material);
			return;
		}
	}
}