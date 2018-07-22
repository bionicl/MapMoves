using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterButton : MonoBehaviour {

	public FilterTypes filterType;
	public int colorId;
	Image image;
	Button button;
	public Color disabledColor;

	public bool isOn = true;

	void Awake() {
		image = GetComponent<Image>();
		button = GetComponent<Button>();
	}

	void Start() {
		RefreshButton(true);
	}

	public void ButtonClicked() {
		isOn = !isOn;
		RefreshButton();
	}

	void RefreshButton(bool isOnStart = false) {
		if (!isOnStart) {
			RenderMap.instance.ChangeFilter(filterType, isOn);
		}
		if (isOn)
			image.color = ReadJson.colors[colorId];
		else
			image.color = disabledColor;
	}
}
