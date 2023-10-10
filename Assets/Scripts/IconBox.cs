using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconBox : MonoBehaviour {

	public Color unselectedColor;
	public Color selectedColor;
	public Image box;
	public Image icon;

	// Cache
	[HideInInspector] public string categoryId;

	public void SetupIcon(PlacesDataJson.PlaceCategory placeType) {
		icon.sprite = placeType.smallIcon;
		Color tempColor = placeType.placeTypeCategory.ColorConverted;
		tempColor.a = 0.75f;
		box.color = tempColor;
		categoryId = placeType.id;
	}

	public void IconClicked() {
		RightListUI.instance.IconClicked(categoryId);
	}

	public void MarkAsSelected() {
		Color tempColor = box.color;
		tempColor.a = 1;
		box.color = tempColor;
	}

	public void MarkAsDeselected() {
		Color tempColor = box.color;
		tempColor.a = 0.5f;
		box.color = tempColor;
	}
}
