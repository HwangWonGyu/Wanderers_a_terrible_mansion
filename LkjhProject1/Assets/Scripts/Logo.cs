using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Logo : MonoBehaviour {

    Camera cam;
    public SpriteRenderer logo;

	void Start () {
        cam = GetComponent<Camera>();
        StartCoroutine(FadeOut());
	}

    IEnumerator FadeOut()
    {
        Color fadeColor = new Color(1, 1, 1, 0);
        float speed = 0;

        while (cam.backgroundColor != Color.black)
        {
            cam.backgroundColor = Color.Lerp(Color.white, Color.black, speed * 0.5f);
            //fadeColor.a += Time.deltaTime * 0.5f;
            //logo.color = fadeColor;
            speed += Time.deltaTime;

            yield return null;
        }
        //while (fadeColor.a < 1)
        //{
        //    fadeColor.a += Time.deltaTime * 0.5f;
        //    logo.color = fadeColor;
        //    yield return null;

        //}
        yield return new WaitForSeconds(1f);
        //while (fadeColor.a > 0)
        //{
        //    fadeColor.a -= Time.deltaTime * 0.5f;
        //    logo.color = fadeColor;
        //    yield return null;

        //}
        SceneManager.LoadScene("Opening");
    }

}
