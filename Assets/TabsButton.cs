using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabsButton : MonoBehaviour {

	[HideInInspector]
	public Button button;
	public Image image;
	public Text text;

	public Sprite unSelectedSprite;
	public Sprite selectedSprite;

	void Awake() {
		button = GetComponent<Button>();
	}

	public void Enable() {
		image.sprite = selectedSprite;
	}

	public void Disable() {
		image.sprite = unSelectedSprite;
	}
}
