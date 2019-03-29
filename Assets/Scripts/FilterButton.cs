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
	public Image icon;
	public Text text;
	public Image checkIcon;

	public bool isOn = true;
	bool unifiedColor;

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

	public void ChangeButtonColor(bool unifiedColor) {
		this.unifiedColor = unifiedColor;
		RefreshButton(true);
	}

	void RefreshButton(bool isOnStart = false) {
		if (!isOnStart) {
			RenderMap.instance.ChangeFilter(filterType, isOn);
		}
		Color colorToSet = Color.white;
		if (isOn) {
			checkIcon.gameObject.SetActive(true);
			if (unifiedColor)
				colorToSet = ReadJson.colors[8];
			else {
				if (filterType == FilterTypes.place)
					colorToSet = ReadJson.PlaceColor;
				else
					colorToSet = ReadJson.colors[colorId];
			}
		} else {
			colorToSet = disabledColor;
			checkIcon.gameObject.SetActive(false);
		}
		checkIcon.color = colorToSet;
		icon.color = colorToSet;
	}


}
