using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Thunder : MonoBehaviour {
    private Light thunder;
    [SerializeField]
    private float thunderSpeed;
    [SerializeField]
    private float thunderIntensity;
	[SerializeField]
	private float thunderInterval;
    private GameObject player;
	void Start () {
		
		thunder = GetComponent<Light>();
        player = GameObject.Find("Player");
        StartCoroutine(ThunderGenerate(thunderInterval));
	}

    IEnumerator ThunderGenerate(float interval)
    {
		while (true)
		{
            thunder.intensity = thunderIntensity;
            while (thunder.intensity > 0)
            {
                thunder.intensity -= Time.deltaTime* thunderSpeed;
                yield return null;
            }
			if(SceneManager.GetActiveScene().name == GlobalData.SCENE_TITLE)
			{
				SoundManager.GetInstance.PlayOneShotThunderClip();
			}
			else
			{
				if (!player.GetComponent<PlayerController>().isDead)
					SoundManager.GetInstance.PlayOneShotThunderClip();
			}
            yield return new WaitForSeconds(interval);
        }
    }
	
	
}
