using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabsButton : MonoBehaviour {

	[HideInInspector]
	public Button button;
	[HideInInspector]
	public Image image;
	public Text text;

	void Awake() {
		button = GetComponent<Button>();
		image = GetComponent<Image>();
	}

	public void Enable() {
		text.color = Color.white;
		image.color = GlobalVariables.inst.accentColor;
	}

	public void Disable() {
		text.color = GlobalVariables.inst.disabledColor;
		image.color = Color.white;
	}
}
