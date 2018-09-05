using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckUIScale : MonoBehaviour {

	CanvasScaler canvas;
	public bool retinaEnabled = true;

	private void Awake() {
		canvas = GetComponent<CanvasScaler>();
	}

	// Use this for initialization
	void Start() {
#if (UNITY_EDITOR)
		return;
#endif
		if (Screen.dpi >= 190 && retinaEnabled) {
			canvas.scaleFactor = 2.6f;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
