using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckUIScale : MonoBehaviour {

	CanvasScaler canvas;
	public bool retinaEnabled = true;
	public static bool isRetina = false;

	// Use this for initialization
	void Awake() {
		canvas = GetComponent<CanvasScaler>();
#if (UNITY_EDITOR)
		return;
#endif
		if (Screen.dpi >= 190 && retinaEnabled) {
			isRetina = true;
			canvas.scaleFactor = 2.6f;
		}
	}
}
