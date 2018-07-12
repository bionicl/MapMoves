using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabItem : MonoBehaviour {

	public Image icon;
	public Text text;
	public Color selectedColor;
	public Color deselectedColor;

	void Awake() {
		Deselect();
	}

	public void Select() {
		icon.color = selectedColor;
		text.color = selectedColor;
	}

	public void Deselect() {
		icon.color = deselectedColor;
		text.color = deselectedColor;
	}
}
