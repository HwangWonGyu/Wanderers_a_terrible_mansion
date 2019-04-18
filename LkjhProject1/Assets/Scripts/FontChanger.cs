using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FontChanger : MonoBehaviour {
    public Font changerFont;

    public void ChangeFont()
    {
        Text[] texts = GameObject.FindObjectsOfType<Text>();
        foreach (Text t in texts)
        {
            t.font = changerFont;
        }
    }
	
}
